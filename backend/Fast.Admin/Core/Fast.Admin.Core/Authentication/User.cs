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
using Fast.Admin.Core.Enums;
using Fast.JwtBearer.Services;

namespace Fast.Admin.Core.Authentication;

/// <summary>
/// <see cref="User"/> 授权用户信息
/// <remarks>作用域注册，保证当前请求管道中是唯一的，并且只会加载一次</remarks>
/// </summary>
public class User : IUser, IScopedDependency
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
    public virtual long UserId { get; private set; }

    /// <summary>
    /// 用户账号
    /// </summary>
    public virtual string UserAccount { get; private set; }

    /// <summary>
    /// 用户工号
    /// </summary>
    public virtual string UserJobNo { get; private set; }

    /// <summary>
    /// 用户名称
    /// </summary>
    public virtual string UserName { get; private set; }

    /// <summary>
    /// 部门Id
    /// </summary>
    public long DepartmentId { get; set; }

    /// <summary>
    /// 部门名称
    /// </summary>
    public string DepartmentName { get; set; }

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    public virtual bool IsSuperAdmin { get; private set; }

    /// <summary>
    /// 是否系统管理员
    /// </summary>
    public virtual bool IsSystemAdmin { get; private set; }

    /// <summary>
    /// 是否租户管理员
    /// </summary>
    public virtual bool IsTenantAdmin { get; private set; }

    /// <summary>
    /// 最后登录设备
    /// </summary>
    public string LastLoginDevice { get; internal set; }

    /// <summary>
    /// 最后登录操作系统（版本）
    /// </summary>
    public string LastLoginOS { get; internal set; }

    /// <summary>
    /// 最后登录浏览器（版本）
    /// </summary>
    public string LastLoginBrowser { get; internal set; }

    /// <summary>
    /// 最后登录省份
    /// </summary>
    public string LastLoginProvince { get; internal set; }

    /// <summary>
    /// 最后登录城市
    /// </summary>
    public string LastLoginCity { get; internal set; }

    /// <summary>
    /// 最后登录Ip
    /// </summary>
    public string LastLoginIp { get; internal set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LastLoginTime { get; internal set; }

    /// <summary>
    /// App 运行环境
    /// </summary>
    public AppEnvironmentEnum AppEnvironment { get; }

    /// <summary>
    /// 身份标识
    /// </summary>
    private const string IdTag = "IdTag";

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
            UserJobNo = _httpContext.Request.Query[ClaimConst.UserJobNo].ToString().Base64ToString();
        }
        else
        {
            AppEnvironment = System.Enum.Parse<AppEnvironmentEnum>(_httpContext.Request.Headers[ClaimConst.AppOrigin]);
            TenantNo = _httpContext.Request.Headers[ClaimConst.TenantNo].ToString().Base64ToString();
            UserJobNo = _httpContext.Request.Headers[ClaimConst.UserJobNo].ToString().Base64ToString();
        }

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
    /// <param name="authUserInfo"><see cref="AuthUserInfo"/> 授权用户信息</param>
    public void SetAuthUser(AuthUserInfo authUserInfo)
    {
        // 判断授权用户信息是否为空
        if (authUserInfo == null)
        {
            throw new UnauthorizedAccessException("授权用户信息不存在！");
        }

        TenantId = authUserInfo.TenantId;
        TenantNo = authUserInfo.TenantNo;
        UserId = authUserInfo.UserId;
        UserAccount = authUserInfo.UserAccount;
        UserJobNo = authUserInfo.UserJobNo;
        UserName = authUserInfo.UserName;
        DepartmentId = authUserInfo.DepartmentId;
        DepartmentName = authUserInfo.DepartmentName;
        IsSuperAdmin = authUserInfo.IsSuperAdmin;
        IsSystemAdmin = authUserInfo.IsSystemAdmin;
        IsTenantAdmin = authUserInfo.IsTenantAdmin;
        LastLoginDevice = authUserInfo.LastLoginDevice;
        LastLoginOS = authUserInfo.LastLoginOS;
        LastLoginBrowser = authUserInfo.LastLoginBrowser;
        LastLoginProvince = authUserInfo.LastLoginProvince;
        LastLoginCity = authUserInfo.LastLoginCity;
        LastLoginIp = authUserInfo.LastLoginIp;
        LastLoginTime = authUserInfo.LastLoginTime;
    }

    /// <summary>
    /// 统一登录
    /// </summary>
    /// <param name="authUserInfo"><see cref="AuthUserInfo"/> 授权用户信息</param>
    /// <returns></returns>
    public async Task Login(AuthUserInfo authUserInfo)
    {
        // 判断授权用户信息是否为空
        if (authUserInfo == null)
        {
            throw new UnauthorizedAccessException("授权用户信息不存在！");
        }

        try
        {
            // TODO:这里缺少菜单权限和按钮权限的查询

            // 生成最后登录时间
            authUserInfo.LastLoginTime = DateTime.Now;

            // 获取设备信息
            var userAgentInfo = _httpContext.RequestUserAgentInfo();
            authUserInfo.LastLoginDevice = userAgentInfo.Device;
            authUserInfo.LastLoginOS = userAgentInfo.OS;
            authUserInfo.LastLoginBrowser = userAgentInfo.Browser;

            // 获取Ip信息
            var ip = _httpContext.RemoteIpv4();
            authUserInfo.LastLoginIp = ip;

            // 获取万网Ip信息
            var wanNetIpInfo = await _httpContext.RemoteIpv4InfoAsync(ip);
            authUserInfo.LastLoginProvince = wanNetIpInfo.Province;
            authUserInfo.LastLoginCity = wanNetIpInfo.City;

            // 刷新登录信息
            await Refresh(authUserInfo);

            // 生成Token令牌，默认20分钟
            var accessToken = _jwtBearerCryptoService.GenerateToken(new Dictionary<string, object>
            {
                {nameof(ClaimConst.TenantNo), authUserInfo.TenantNo},
                {nameof(ClaimConst.UserJobNo), authUserInfo.UserJobNo},
                {nameof(authUserInfo.LastLoginIp), authUserInfo.LastLoginIp},
                {nameof(authUserInfo.LastLoginTime), authUserInfo.LastLoginTime},
            });

            // 生成刷新Token，有效期24小时
            var refreshToken = _jwtBearerCryptoService.GenerateRefreshToken(accessToken);

            // 设置Token令牌
            _httpContext.Response.Headers["access-token"] = accessToken;

            // 设置刷新Token令牌
            _httpContext.Response.Headers["x-access-token"] = refreshToken;

            // 设置Swagger自动登录
            _httpContext.SignInToSwagger(accessToken);
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException($"401 登录鉴权失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 刷新登录信息
    /// </summary>
    /// <param name="authUserInfo"></param>
    /// <returns></returns>
    public async Task Refresh(AuthUserInfo authUserInfo)
    {
        // 设置授权用户信息
        SetAuthUser(authUserInfo);

        // 获取缓存Key
        var cacheKey = $"{CacheConst.GetCacheKey(CacheConst.AuthUserInfo, authUserInfo.TenantNo)}{authUserInfo.UserJobNo}";

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

            // 从 Token 中读取 TenantNo 和 UserJobNo
            var tenantNo = jwtSecurityToken.Payload[nameof(ClaimConst.TenantNo)].ToString();
            var userJobNo = jwtSecurityToken.Payload[nameof(ClaimConst.UserJobNo)].ToString();

            if (!tenantNo.IsEmpty() && !userJobNo.IsEmpty())
            {
                // 获取缓存Key
                var tokenCacheKey = $"{CacheConst.GetCacheKey(CacheConst.ExpiredToken, tenantNo)}{userJobNo}:{token[^10..]}";
                // 将当前 Token 放入过期缓存中，建议设置缓存过期时间为 Token 过期时间 - 当前时间
                await _cache.SetAsync(tokenCacheKey, token, jwtSecurityToken.ValidTo - DateTimeOffset.UtcNow);

                // 退出登录需要传入 刷新Token
                var refreshToken = _jwtBearerCryptoService.GetJwtBearerToken(_httpContext, "X-Authorization");

                if (!refreshToken.IsEmpty())
                {
                    var jwtSecurityRefreshToken = _jwtBearerCryptoService.SecurityReadJwtToken(refreshToken);

                    // 获取缓存Key
                    var refreshTokenCacheKey =
                        $"{CacheConst.GetCacheKey(CacheConst.ExpiredToken, tenantNo)}{userJobNo}:{refreshToken[^10..]}";
                    // 将当前 Token 放入过期缓存中，建议设置缓存过期时间为 Token 过期时间 - 当前时间
                    await _cache.SetAsync(refreshTokenCacheKey, refreshToken,
                        jwtSecurityRefreshToken.ValidTo - DateTimeOffset.UtcNow);
                }

                // 获取缓存Key
                var userCacheKey = $"{CacheConst.GetCacheKey(CacheConst.AuthUserInfo, tenantNo)}{userJobNo}";
            }
        }
    }
}