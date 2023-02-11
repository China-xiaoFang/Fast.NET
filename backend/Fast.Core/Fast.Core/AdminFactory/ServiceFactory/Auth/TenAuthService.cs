using System.Text.RegularExpressions;
using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.AdminFactory.ModelFactory.Tenant;
using Fast.Core.AdminFactory.ServiceFactory.Auth.Dto;
using Fast.Core.Const;
using Fast.Core.Internal.EventSubscriber;
using Fast.Core.Operation.Config;
using Fast.Core.Util.Http;
using Furion.DataEncryption;
using Furion.DependencyInjection;
using Furion.EventBus;
using Furion.FriendlyException;
using Microsoft.AspNetCore.Http;

namespace Fast.Core.AdminFactory.ServiceFactory.Auth;

/// <summary>
/// 租户授权服务
/// </summary>
public class TenAuthService : ITenAuthService, ITransient
{
    private readonly ISqlSugarRepository<TenUserModel> _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEventPublisher _eventPublisher;

    public TenAuthService(ISqlSugarRepository<TenUserModel> repository, IHttpContextAccessor httpContextAccessor,
        IEventPublisher eventPublisher)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Web登录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task WebLogin(WebLoginInput input)
    {
        TenUserModel userInfo;

        // 判断登录方式
        switch (input.LoginMethod)
        {
            case LoginMethodEnum.Account:
                userInfo = await _repository.FirstOrDefaultAsync(f =>
                    f.Account == input.Account && f.Status != CommonStatusEnum.Delete);

                break;
            case LoginMethodEnum.Email:
                // 判断是否为邮箱地址
                var emailRegex = new Regex(CommonConst.RegexStr.EmailAddress);
                if (!emailRegex.IsMatch(input.Account))
                {
                    throw Oops.Bah(ErrorCode.EmailAddressInvalid);
                }

                userInfo = await _repository.FirstOrDefaultAsync(f =>
                    f.Email == input.Account && f.Status != CommonStatusEnum.Delete);
                break;
            case LoginMethodEnum.Phone:
                userInfo = await _repository.FirstOrDefaultAsync(f =>
                    f.Phone == input.Account && f.Status != CommonStatusEnum.Delete);
                break;
            default:
                throw Oops.Bah(ErrorCode.LoginMethodInvalid);
        }

        // 判断是否查询到了用户信息
        if (userInfo == null)
        {
            throw input.LoginMethod switch
            {
                LoginMethodEnum.Account => Oops.Bah("账号不存在！"),
                LoginMethodEnum.Email => Oops.Bah("邮箱地址不存在！"),
                LoginMethodEnum.Phone => Oops.Bah("手机号码不存在！"),
                _ => Oops.Bah("账号不存在！")
            };
        }

        // 验证账号状态
        if (userInfo.Status == CommonStatusEnum.Disable)
        {
            Oops.Bah("账号已经被停用！");
        }

        // 判断密码是否正确
        if (userInfo.Password != MD5Encryption.Encrypt(input.Password))
        {
            // TODO：这里增加密码次数判断
            /*
             * 连续错误5次，冻结1分钟
             * 连续错误10次，冻结5分钟
             * 连续错误15次，冻结30分钟
             * 连续错误20次，冻结60分钟
             * 连续错误25次，冻结120分钟
             * 连续错误30次，冻结账号
             * 登录成功后消除缓存
             */
            Oops.Bah("密码不正确！");
        }

        // 获取Token过期时间
        var tokenExpiredTimeInfo =
            await ConfigOperation.GetConfigAsync(ConfigConst.Tenant.TokenExpiredTime, SysConfigTypeEnum.Tenant);
        var tokenExpiredTime = tokenExpiredTimeInfo.Value.ParseToInt();

        // 生成Token令牌
        var accessToken = JWTEncryption.Encrypt(
            new Dictionary<string, object>
            {
                {ClaimConst.UserId, userInfo.Id},
                {ClaimConst.Account, userInfo.Account},
                {ClaimConst.Name, userInfo.Name},
                {ClaimConst.AdminType, userInfo.AdminType},
                {ClaimConst.TenantId, GlobalContext.TenantId}
            }, tokenExpiredTime);

        // 设置Swagger自动登录
        _httpContextAccessor.HttpContext.SigninToSwagger(accessToken);

        // 设置Token令牌
        _httpContextAccessor.HttpContext!.Response.Headers[ClaimConst.RefreshToken] = accessToken;

        // 更新最后登录时间和Ip
        userInfo.LastLoginIp = HttpNewUtil.Ip;
        userInfo.LastLoginTime = DateTime.Now;
        await _eventPublisher.PublishAsync(new FastChannelEventSource("Update:UserLoginInfo", GlobalContext.TenantId, userInfo));
    }
}