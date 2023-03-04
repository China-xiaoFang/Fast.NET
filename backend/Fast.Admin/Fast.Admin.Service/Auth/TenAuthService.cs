using System.Text.RegularExpressions;
using Fast.Admin.Service.Auth.Dto;
using Fast.Core.AdminFactory.ModelFactory.Tenant;
using Fast.Core.Internal.EventSubscriber;
using Furion.DataEncryption;
using Furion.EventBus;
using Microsoft.AspNetCore.Http;

namespace Fast.Admin.Service.Auth;

/// <summary>
/// 租户授权服务
/// </summary>
public class TenAuthService : ITenAuthService, ITransient
{
    private readonly ISqlSugarRepository<TenUserModel> _tenRepository;
    private readonly ICache _cache;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEventPublisher _eventPublisher;

    public TenAuthService(ISqlSugarRepository<TenUserModel> tenRepository, ICache cache, IHttpContextAccessor httpContextAccessor,
        IEventPublisher eventPublisher)
    {
        _tenRepository = tenRepository;
        _cache = cache;
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
                userInfo = await _tenRepository.FirstOrDefaultAsync(f =>
                    f.Account == input.Account && f.Status != CommonStatusEnum.Delete);

                break;
            case LoginMethodEnum.Email:
                // 判断是否为邮箱地址
                var emailRegex = new Regex(CommonConst.RegexStr.EmailAddress);
                if (!emailRegex.IsMatch(input.Account))
                {
                    throw Oops.Bah("不是一个有效的邮箱地址！");
                }

                userInfo = await _tenRepository.FirstOrDefaultAsync(f =>
                    f.Email == input.Account && f.Status != CommonStatusEnum.Delete);
                break;
            case LoginMethodEnum.Phone:
                userInfo = await _tenRepository.FirstOrDefaultAsync(f =>
                    f.Phone == input.Account && f.Status != CommonStatusEnum.Delete);
                break;
            default:
                throw Oops.Bah("不是一个有效的登录方式！");
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
            throw Oops.Bah("账号已经被停用！");
        }

        /*
         * 连续错误5次，冻结1分钟
         * 连续错误10次，冻结5分钟
         * 连续错误15次，冻结30分钟
         * 连续错误20次，冻结60分钟
         * 连续错误25次，冻结120分钟
         * 连续错误30次，冻结账号
         * 登录成功后消除缓存
         */
        var errorPasswordCacheKey = $"{CacheConst.InputErrorPassword}{userInfo.Id}";
        // 判断密码是否正确
        if (userInfo.Password != MD5Encryption.Encrypt(input.Password))
        {
            // 记录密码错误次数
            var errorPasswordDto = await _cache.GetAsync<InputErrorPasswordDto>(errorPasswordCacheKey) ??
                                   new InputErrorPasswordDto {Count = 0, FreezeMinutes = 0, ThawingTime = null};

            // 错误次数+1
            errorPasswordDto.Count++;

            // 判断是否为5的倍数
            if (errorPasswordDto.Count == 30)
            {
                // 错误30次，直接冻结账号
                userInfo.Status = CommonStatusEnum.Disable;
                await _tenRepository.UpdateAsync(userInfo);
                await _cache.SetAsync(errorPasswordCacheKey, errorPasswordDto);
                throw Oops.Bah("密码连续输入错误30次，账号已被停用，请联系管理员！");
            }

            var time = DateTime.Now;

            // 判断是否存在解冻时间
            if (errorPasswordDto.ThawingTime != null && errorPasswordDto.ThawingTime.Value > time)
            {
                throw Oops.Bah(
                    $"密码连续输入错误{errorPasswordDto.Count}次，已被冻结{errorPasswordDto.FreezeMinutes}分钟，请{(errorPasswordDto.ThawingTime.Value - time).TotalMinutes.ParseToInt() + 1}分钟后再重试！");
            }

            if (errorPasswordDto.Count % 5 != 0)
            {
                errorPasswordDto.ThawingTime = null;
                await _cache.SetAsync(errorPasswordCacheKey, errorPasswordDto);
                throw Oops.Bah("密码不正确！");
            }

            // 解冻分钟数
            var addMinutes = errorPasswordDto.Count switch
            {
                5 => 1,
                10 => 5,
                15 => 30,
                20 => 60,
                25 => 120,
                _ => 0
            };
            errorPasswordDto.FreezeMinutes = addMinutes;
            // 解冻时间
            errorPasswordDto.ThawingTime = DateTime.Now.AddMinutes(addMinutes);
            await _cache.SetAsync(errorPasswordCacheKey, errorPasswordDto);
            throw Oops.Bah($"密码连续输入错误{errorPasswordDto.Count}次，已被冻结{addMinutes}分钟，请{addMinutes}分钟后再重试！");
        }

        // 删除记录的错误密码次数
        await _cache.DelAsync(errorPasswordCacheKey);

        // 生成Token令牌
        var accessToken = JWTEncryption.Encrypt(
            new Dictionary<string, object>
            {
                {ClaimConst.UserId, userInfo.Id},
                {ClaimConst.Account, userInfo.Account},
                {ClaimConst.Name, userInfo.Name},
                {ClaimConst.AdminType, userInfo.AdminType},
                {ClaimConst.TenantId, GlobalContext.TenantId}
            }, (await ConfigOperation.Tenant.GetConfigAsync(ConfigConst.Tenant.TokenExpiredTime)).Value.ParseToInt());

        // 获取刷新Token
        // ReSharper disable once RedundantArgumentDefaultValue
        var refreshToken = JWTEncryption.GenerateRefreshToken(accessToken,
            (await ConfigOperation.Tenant.GetConfigAsync(ConfigConst.Tenant.RefreshTokenExpiredTime)).Value.ParseToInt());

        // 设置Token令牌
        _httpContextAccessor.HttpContext!.Response.Headers[ClaimConst.AccessToken] = accessToken;

        // 设置刷新Token令牌
        _httpContextAccessor.HttpContext!.Response.Headers[ClaimConst.RefreshToken] = refreshToken;

        // 更新最后登录时间和Ip
        userInfo.LastLoginIp = HttpNewUtil.Ip;
        userInfo.LastLoginTime = DateTime.Now;
        await _eventPublisher.PublishAsync(new FastChannelEventSource("Update:UserLoginInfo", GlobalContext.TenantId, userInfo));
    }
}