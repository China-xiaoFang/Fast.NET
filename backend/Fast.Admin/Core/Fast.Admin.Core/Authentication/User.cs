// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Fast.Admin.Core.Constants;
using Fast.Admin.Core.Entity.System.Account;
using Fast.Admin.Core.Enum.System;
using Fast.JwtBearer.Services;

namespace Fast.Admin.Core.Authentication;

/// <summary>
/// <see cref="User"/> 授权用户信息
/// <remarks>作用域注册，保证当前请求管道中是唯一的，并且只会加载一次</remarks>
/// </summary>
public sealed class User : IUser, IScopedDependency
{
    /// <summary>
    /// 租户Id
    /// </summary>
    public long TenantId { get; private set; }

    /// <summary>
    /// 租户编号
    /// </summary>
    public string TenantNo { get; private set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    public long UserId { get; private set; }

    /// <summary>
    /// 用户账号
    /// </summary>
    public string Account { get; private set; }

    /// <summary>
    /// 用户工号
    /// </summary>
    public string JobNumber { get; private set; }

    /// <summary>
    /// 用户名称
    /// </summary>
    public string UserName { get; private set; }

    /// <summary>
    /// 部门Id
    /// </summary>
    public long DepartmentId { get; private set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DepartmentName { get; private set; }

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    public bool IsSuperAdmin { get; private set; }

    /// <summary>
    /// 是否系统管理员
    /// </summary>
    public bool IsSystemAdmin { get; private set; }

    /// <summary>
    /// 最后登录设备
    /// </summary>
    public string LastLoginDevice { get; private set; }

    /// <summary>
    /// 最后登录操作系统（版本）
    /// </summary>
    public string LastLoginOS { get; private set; }

    /// <summary>
    /// 最后登录浏览器（版本）
    /// </summary>
    public string LastLoginBrowser { get; private set; }

    /// <summary>
    /// 最后登录省份
    /// </summary>
    public string LastLoginProvince { get; private set; }

    /// <summary>
    /// 最后登录城市
    /// </summary>
    public string LastLoginCity { get; private set; }

    /// <summary>
    /// 最后登录Ip
    /// </summary>
    public string LastLoginIp { get; private set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LastLoginTime { get; private set; }

    /// <summary>
    /// App 运行环境
    /// </summary>
    public AppEnvironmentEnum AppEnvironment { get; private set; }

    private readonly ICache _cache;

    private readonly IJwtBearerCryptoService _jwtBearerCryptoService;

    private readonly HttpContext _httpContext;

    public User(ICache cache, IJwtBearerCryptoService jwtBearerCryptoService, IHttpContextAccessor httpContextAccessor)
    {
        _cache = cache;
        _jwtBearerCryptoService = jwtBearerCryptoService;
        _httpContext = httpContextAccessor.HttpContext;

        // 判断是否为空，如果是，则不是正常请求
        if (_httpContext == null)
            return;

        // 判断是否为 WebSocket 请求
        if (_httpContext.IsWebSocketRequest())
        {
            // WebSocket 请求，从Url地址后面获取标识
            AppEnvironment = System.Enum.Parse<AppEnvironmentEnum>(_httpContext.Request.Query[ClaimConst.AppOrigin]);
            TenantNo = _httpContext.Request.Query[ClaimConst.TenantNo].ToString().Base64ToString();
            JobNumber = _httpContext.Request.Query[ClaimConst.JobNumber].ToString().Base64ToString();
        }
        else
        {
            AppEnvironment = System.Enum.Parse<AppEnvironmentEnum>(_httpContext.Request.Headers[ClaimConst.AppOrigin]);
            TenantNo = _httpContext.Request.Headers[ClaimConst.TenantNo].ToString().Base64ToString();
            JobNumber = _httpContext.Request.Headers[ClaimConst.JobNumber].ToString().Base64ToString();
        }

        // 获取缓存Key
        var cacheKey = $"{CacheConst.GetCacheKey(CacheConst.AuthUserInfo, TenantNo)}{JobNumber}";

        // 获取缓存信息
        var authUserInfo = _cache.Get<SysTenantAccountModel>(cacheKey);

        // 设置授权用户
        SetAuthUser(authUserInfo);

        // App 来源判断
        // TODO：这里需要根据App授权信息，判断是否存在访问的权限
        switch (AppEnvironment)
        {
            case AppEnvironmentEnum.Web:
            case AppEnvironmentEnum.Pc:
            case AppEnvironmentEnum.WeChatMiniProgram:
            case AppEnvironmentEnum.AndroidApp:
            case AppEnvironmentEnum.IOSApp:
            case AppEnvironmentEnum.Other:
                break;
            default:
                throw new UnauthorizedAccessException("未知的客户端环境！");
        }
    }

    /// <summary>
    /// 设置授权用户
    /// </summary>
    /// <param name="authUserInfo"><see cref="SysTenantAccountModel"/> 租户系统账户信息</param>
    public void SetAuthUser(SysTenantAccountModel authUserInfo)
    {
        // 判断授权用户信息是否为空
        if (authUserInfo == null)
        {
            throw new UnauthorizedAccessException("授权用户信息不存在！");
        }

        TenantId = authUserInfo.TenantId;
        TenantNo = authUserInfo.SysTenant.TenantNo;
        UserId = authUserInfo.UserId;
        Account = authUserInfo.SysAccount.Account;
        JobNumber = authUserInfo.JobNumber;
        UserName = authUserInfo.SysAccount.UserName;
        DepartmentId = authUserInfo.DepartmentId;
        DepartmentName = authUserInfo.DepartmentName;
        IsSuperAdmin = authUserInfo.AdminType == AdminTypeEnum.SuperAdmin;
        IsSystemAdmin = authUserInfo.AdminType == AdminTypeEnum.SystemAdmin;
        LastLoginDevice = authUserInfo.LastLoginDevice;
        LastLoginOS = authUserInfo.LastLoginOS;
        LastLoginBrowser = authUserInfo.LastLoginBrowser;
        LastLoginProvince = authUserInfo.LastLoginProvince;
        LastLoginCity = authUserInfo.LastLoginCity;
        LastLoginIp = authUserInfo.LastLoginIp;
        LastLoginTime = authUserInfo.LastLoginTime;
    }

    /// <summary>
    /// 刷新登录信息
    /// </summary>
    /// <param name="authUserInfo"><see cref="SysTenantAccountModel"/> 租户系统账户信息</param>
    /// <returns></returns>
    public async Task Refresh(SysTenantAccountModel authUserInfo)
    {
        // 设置授权用户信息
        SetAuthUser(authUserInfo);

        // 获取缓存Key
        var cacheKey =
            $"{CacheConst.GetCacheKey(CacheConst.AuthUserInfo, authUserInfo.SysTenant.TenantNo)}{authUserInfo.JobNumber}";

        // 设置授权信息缓存
        await _cache.SetAsync(cacheKey, authUserInfo);
    }

    /// <summary>
    /// 统一退出登录
    /// </summary>
    /// <returns></returns>
    public async Task Logout()
    {
        /*
         * 首先确定，退出登录有两种情况，
         *  1.正常情况，点击退出登录，这个时候的Token是存在的，且没有过期的。
         *  2.401的情况下，系统调用退出登录的接口，这个时候虽然存在Token，但是Token肯定是过期的。
         */

        // 这里直接从请求头中获取Token
        var token = _jwtBearerCryptoService.GetJwtBearerToken(_httpContext);

        if (!token.IsEmpty())
        {
            // 读取 Token 信息
            var jwtSecurityToken = _jwtBearerCryptoService.SecurityReadJwtToken(token);

            // 从 Token 中读取 TenantNo 和 JobNumber
            var tenantNo = jwtSecurityToken.Payload[nameof(ClaimConst.TenantNo)].ToString();
            var JobNumber = jwtSecurityToken.Payload[nameof(ClaimConst.JobNumber)].ToString();

            if (!tenantNo.IsEmpty() && !JobNumber.IsEmpty())
            {
                // 获取缓存Key
                var tokenCacheKey = $"{CacheConst.GetCacheKey(CacheConst.ExpiredToken, tenantNo)}{JobNumber}:{token[^10..]}";
                // 将当前 Token 放入过期缓存中，建议设置缓存过期时间为 Token 过期时间 - 当前时间
                await _cache.SetAsync(tokenCacheKey, token, jwtSecurityToken.ValidTo - DateTimeOffset.UtcNow);

                // 退出登录需要传入 刷新Token
                var refreshToken = _jwtBearerCryptoService.GetJwtBearerToken(_httpContext, "X-Authorization");

                if (!refreshToken.IsEmpty())
                {
                    var jwtSecurityRefreshToken = _jwtBearerCryptoService.SecurityReadJwtToken(refreshToken);

                    // 获取缓存Key
                    var refreshTokenCacheKey =
                        $"{CacheConst.GetCacheKey(CacheConst.ExpiredToken, tenantNo)}{JobNumber}:{refreshToken[^10..]}";
                    // 将当前 Token 放入过期缓存中，建议设置缓存过期时间为 Token 过期时间 - 当前时间
                    await _cache.SetAsync(refreshTokenCacheKey, refreshToken,
                        jwtSecurityRefreshToken.ValidTo - DateTimeOffset.UtcNow);
                }

                // 获取缓存Key
                var userCacheKey = $"{CacheConst.GetCacheKey(CacheConst.AuthUserInfo, tenantNo)}{JobNumber}";
            }
        }
    }
}