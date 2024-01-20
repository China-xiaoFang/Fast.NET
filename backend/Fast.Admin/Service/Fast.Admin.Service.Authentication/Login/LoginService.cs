using System.Text.RegularExpressions;
using Fast.Admin.Core.Constants;
using Fast.Admin.Core.Entity.Log.Entities;
using Fast.Admin.Core.Entity.System.Account;
using Fast.Admin.Core.Entity.System.Tenant;
using Fast.Admin.Core.Enum.Common;
using Fast.Admin.Core.Enum.Login;
using Fast.Admin.Core.EventSubscriber.Sources;
using Fast.Admin.Core.EventSubscriber.SysLogSql;
using Fast.Admin.Core.Services;
using Fast.Admin.Entity.Tenant.Organization;
using Fast.Admin.Service.Authentication.Login.Dto;
using Fast.Cache;
using Fast.DependencyInjection;
using Fast.EventBus;
using Fast.IaaS;
using Fast.JwtBearer.Services;
using Fast.NET.Core;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Admin.Service.Authentication.Login;

/// <summary>
/// <see cref="LoginService"/> 登录服务
/// </summary>
public class LoginService : ILoginService, ITransientDependency
{
    private readonly ICache _cache;
    private readonly ISqlSugarClient _repository;
    private readonly IJwtBearerCryptoService _jwtBearerCryptoService;
    private readonly IEventPublisher _eventPublisher;

    public LoginService(ICache cache, ISqlSugarClient repository, IJwtBearerCryptoService jwtBearerCryptoService,
        IEventPublisher eventPublisher)
    {
        _cache = cache;
        _repository = repository;
        _jwtBearerCryptoService = jwtBearerCryptoService;
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<LoginOutput> Login(LoginInput input)
    {
        SysAccountModel account;

        // 判断登录方式
        switch (input.LoginMethod)
        {
            case LoginMethodEnum.Account:
                account = await _repository.Queryable<SysAccountModel>().Where(wh => wh.Account == input.Account).FirstAsync();
                break;
            case LoginMethodEnum.Mobile:
                // 判断是否为一个有效的邮箱地址
                var mobileRegex = new Regex(RegexConst.Mobile);
                if (!mobileRegex.IsMatch(input.Account))
                {
                    throw new UserFriendlyException("不是一个有效的手机号码！");
                }

                account = await _repository.Queryable<SysAccountModel>().Where(wh => wh.Mobile == input.Account).FirstAsync();
                break;
            case LoginMethodEnum.Email:
                // 判断是否为一个有效的邮箱地址
                var emailRegex = new Regex(RegexConst.EmailAddress);
                if (!emailRegex.IsMatch(input.Account))
                {
                    throw new UserFriendlyException("不是一个有效的邮箱地址！");
                }

                account = await _repository.Queryable<SysAccountModel>().Where(wh => wh.Email == input.Account).FirstAsync();
                break;
            default:
                throw new UserFriendlyException("不是一个有效的登录方式！");
        }

        // 判断是否存在账号信息
        if (account == null)
        {
            throw input.LoginMethod switch
            {
                LoginMethodEnum.Account => new UserFriendlyException("账号不存在！"),
                LoginMethodEnum.Mobile => new UserFriendlyException("手机号码不存在！"),
                LoginMethodEnum.Email => new UserFriendlyException("邮箱地址不存在！"),
                _ => new UserFriendlyException("账号不存在！")
            };
        }

        // 验证账号状态
        if (account.Status == CommonStatusEnum.Disable)
        {
            throw new UserFriendlyException("账号已经被停用！");
        }

        var dateTime = DateTime.Now;

        /*
         * 连续错误5次，冻结1分钟
         * 连续错误10次，冻结5分钟
         * 连续错误15次，冻结30分钟
         * 连续错误20次，冻结60分钟
         * 连续错误25次，冻结120分钟
         * 连续错误30次，冻结账号
         * 登录成功后消除缓存
         */

        var errorPasswordCacheKey = CacheConst.GetCacheKey(CacheConst.InputErrorPassword, account.Id);

        if (account.Password != CryptoUtil.MD5Encrypt(input.Password))
        {
            // 记录密码错误次数
            // 记录密码错误次数
            var errorPasswordDto = await _cache.GetAsync<InputErrorPasswordDto>(errorPasswordCacheKey) ??
                                   new InputErrorPasswordDto {Count = 0, FreezeMinutes = 0, ThawingTime = null};

            // 错误次数+1
            errorPasswordDto.Count++;

            // 判断是否为5的倍数
            if (errorPasswordDto.Count == 30)
            {
                // 错误30次，直接冻结账号
                account.Status = CommonStatusEnum.Disable;
                await _repository.Updateable(account).ExecuteCommandAsync();
                await _cache.SetAsync(errorPasswordCacheKey, errorPasswordDto);
                throw new UserFriendlyException("密码连续输入错误30次，账号已被停用，请联系管理员！");
            }

            // 判断是否存在解冻时间
            if (errorPasswordDto.ThawingTime != null && errorPasswordDto.ThawingTime.Value > dateTime)
            {
                var thawingMinutes = Math.Floor((errorPasswordDto.ThawingTime.Value - dateTime).TotalMinutes);
                if (thawingMinutes == 0)
                {
                    thawingMinutes = 1;
                }

                throw new UserFriendlyException(
                    $"密码连续输入错误{errorPasswordDto.Count - 1}次，已被冻结{errorPasswordDto.FreezeMinutes}分钟，请{thawingMinutes}分钟后再重试！");
            }

            if (errorPasswordDto.Count % 5 != 0)
            {
                errorPasswordDto.ThawingTime = null;
                await _cache.SetAsync(errorPasswordCacheKey, errorPasswordDto);
                throw new UserFriendlyException("密码不正确！");
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
            errorPasswordDto.ThawingTime = dateTime.AddMinutes(addMinutes);
            await _cache.SetAsync(errorPasswordCacheKey, errorPasswordDto);
            throw new UserFriendlyException($"密码连续输入错误{errorPasswordDto.Count}次，已被冻结{addMinutes}分钟，请{addMinutes}分钟后再重试！");
        }

        // 删除记录的错误密码次数
        await _cache.DelAsync(errorPasswordCacheKey);

        var result = new LoginOutput {AccountId = account.Id};

        // 查询当前账号的所有租户信息
        result.TenantList = await _repository.Queryable<SysTenantAccountModel>()
            .LeftJoin<SysTenantModel>((t1, t2) => t1.TenantId == t2.Id)
            .Where((t1, t2) => t1.AccountId == account.Id && t1.Status == CommonStatusEnum.Enable)
            .Select<LoginOutput.LoginTenantDto>("[t1].*, [t2].[ChName]").ToListAsync();

        if (result.TenantList.Count == 0)
        {
            throw new UserFriendlyException("没有任何的租户信息！");
        }

        // 判断是否只有一个租户，如果是则，自动登录
        if (result.TenantList.Count == 1)
        {
            result.IsAutoLogin = true;
            var tenant = result.TenantList.First();
            await TenantLogin(new TenantLoginInput
            {
                AccountId = account.Id, TenantAccountId = tenant.Id, Password = input.Password
            });

            result.TenantList = null;

            return result;
        }

        result.IsAutoLogin = false;

        // 设置选择租户时间限制缓存，这里默认1分钟，如果在1分钟内没有选择登录，则需要重新登录
        var selectTenantLoginCacheKey = CacheConst.GetCacheKey(CacheConst.SelectTenantLogin, account.Id);

        // 设置缓存
        await _cache.SetAsync(selectTenantLoginCacheKey, dateTime, 65);

        return result;
    }

    /// <summary>
    /// 租户登录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    /// <exception cref="UnauthorizedAccessException"></exception>
    public async Task TenantLogin(TenantLoginInput input)
    {
        var account = await _repository.Queryable<SysAccountModel>().Where(wh => wh.Id == input.AccountId).FirstAsync();

        if (account == null)
        {
            throw new UserFriendlyException("账号信息不存在！");
        }

        // 验证账号状态
        if (account.Status == CommonStatusEnum.Disable)
        {
            throw new UserFriendlyException("账号已经被停用！");
        }

        if (account.Password != CryptoUtil.MD5Encrypt(input.Password))
        {
            throw new UserFriendlyException("密码不正确！");
        }

        // 查询对应的租户信息
        var sysTenantAccount = await _repository.Queryable<SysTenantAccountModel>().Where(wh => wh.Id == input.TenantAccountId)
            .FirstAsync();

        if (sysTenantAccount == null)
        {
            throw new UserFriendlyException("租户账号信息不存在！");
        }

        // 查询租户信息
        var tenant = await _repository.Queryable<SysTenantModel>().Where(wh => wh.Id == sysTenantAccount.TenantId).FirstAsync();

        if (tenant == null)
        {
            throw new UserFriendlyException("租户信息不存在！");
        }

        // TODO：租户授权时间判断

        var _sqlSugarEntityService = FastContext.GetService<ISqlSugarEntityService>();
        var adminConnectionConfig = await _sqlSugarEntityService.GetAdminCoreSqlSugarClient(sysTenantAccount.TenantId);
        var db = new SqlSugarClient(adminConnectionConfig);

        // 查询租户用户信息
        var tenUser = await db.Queryable<TenUserModel>().Where(wh => wh.Id == sysTenantAccount.UserId).FirstAsync();

        if (tenUser == null)
        {
            throw new UserFriendlyException("租户用户信息不存在！");
        }

        // 判断租户用户状态是否已经被禁用
        if (tenUser.Status != CommonStatusEnum.Enable)
        {
            throw new UserFriendlyException("租户用户账号非正常状态！");
        }

        try
        {
            var httpContext = FastContext.HttpContext;

            var dateTime = DateTime.Now;

            // 获取设备信息
            var userAgentInfo = httpContext.RequestUserAgentInfo();

            // 获取Ip信息
            var ip = httpContext.RemoteIpv4();

            // 获取万网Ip信息
            var wanNetIpInfo = await httpContext.RemoteIpv4InfoAsync(ip);

            // 更新最后登录时间
            sysTenantAccount.LastLoginDevice = userAgentInfo.Device;
            sysTenantAccount.LastLoginOS = userAgentInfo.OS;
            sysTenantAccount.LastLoginBrowser = userAgentInfo.Browser;
            sysTenantAccount.LastLoginProvince = wanNetIpInfo.Province;
            sysTenantAccount.LastLoginCity = wanNetIpInfo.City;
            sysTenantAccount.LastLoginIp = ip;
            sysTenantAccount.LastLoginTime = dateTime;

            tenUser.LastLoginDevice = userAgentInfo.Device;
            tenUser.LastLoginOS = userAgentInfo.OS;
            tenUser.LastLoginBrowser = userAgentInfo.Browser;
            tenUser.LastLoginProvince = wanNetIpInfo.Province;
            tenUser.LastLoginCity = wanNetIpInfo.City;
            tenUser.LastLoginIp = ip;
            tenUser.LastLoginTime = dateTime;

            // 更新数据
            _repository.Updateable(sysTenantAccount);
            db.Updateable(tenUser);

            // 添加访问日志
            var sysLogVisModel = new SysLogVisModel
            {
                Id = YitIdHelper.NextId(),
                Account = account.Account,
                JobNumber = sysTenantAccount.JobNumber,
                VisitType = VisitTypeEnum.Login,
                VisTime = dateTime,
                TenantId = tenant.Id
            };
            sysLogVisModel.RecordCreate(httpContext);

            // 获取日志库连接配置
            var logConnectionConfig = await _sqlSugarEntityService.GetLogSqlSugarClient();

            // 事件总线执行日志
            await _eventPublisher.PublishAsync(new SqlSugarChannelEventSource(SysLogSqlEventSubscriberEnum.AddVisLog,
                logConnectionConfig, sysLogVisModel));

            sysTenantAccount.SysAccount = account;
            sysTenantAccount.SysTenant = tenant;

            // 生成 Token 令牌，默认20分钟
            var accessToken = _jwtBearerCryptoService.GenerateToken(new Dictionary<string, object>
            {
                {nameof(ClaimConst.TenantNo), sysTenantAccount.SysTenant.TenantNo},
                {nameof(ClaimConst.JobNumber), sysTenantAccount.JobNumber},
                {nameof(sysTenantAccount.LastLoginIp), sysTenantAccount.LastLoginIp},
                {nameof(sysTenantAccount.LastLoginTime), sysTenantAccount.LastLoginTime},
            });

            // 生成刷新Token，有效期24小时
            var refreshToken = _jwtBearerCryptoService.GenerateRefreshToken(accessToken);

            // 设置Token令牌
            httpContext.Response.Headers["access-token"] = accessToken;

            // 设置刷新Token令牌
            httpContext.Response.Headers["x-access-token"] = refreshToken;

            // 设置Swagger自动登录
            httpContext.SignInToSwagger(accessToken);
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException($"401 登录鉴权失败：{ex.Message}");
        }
    }
}