using Fast.Authentication.Infrastructures;
using Fast.Cache;
using Fast.IaaS.Extensions;
using Fast.NET.Core.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Fast.Authentication.Realize;

/// <summary>
/// 授权用户信息
/// 作用域注册，保证当前请求管道中是唯一的，并且只会加载一次
/// </summary>
public class User : AuthUserInfo, IUser
{
    /// <summary>
    /// 当前租户Id
    /// </summary>
    public override long TenantId { get; }

    /// <summary>
    /// App 运行环境
    /// </summary>
    public AppEnvironmentEnum AppEnvironment { get; }

    /// <summary>
    /// 身份标识
    /// </summary>
    private const string IdTag = "IdTag";

    /// <summary>
    /// 缓存Key
    /// </summary>
    private const string CacheKey = "Auth:UserInfo:";

    private readonly ICache _cache;

    private readonly HttpContext _httpContext;

    public User(ICache cache, IHttpContextAccessor httpContextAccessor)
    {
        _cache = cache;

        _httpContext = httpContextAccessor.HttpContext;

        // 判断是否为空，如果是，则不是正常请求
        if (_httpContext == null)
            return;

        // 获取并且检测来源
        AppEnvironment = (AppEnvironmentEnum) _httpContext.Request.Headers["Fast-App-Origin"].ParseToInt();

        switch (AppEnvironment)
        {
            case AppEnvironmentEnum.PC:
            case AppEnvironmentEnum.Windows:
            case AppEnvironmentEnum.App:
            case AppEnvironmentEnum.H5:
            case AppEnvironmentEnum.WeChatMiniProgram:
                break;
            default:
                throw new Exception("未知的客户端环境！");
            //throw Oops.Bah("未知的客户端环境！").StatusCode(StatusCodes.Status401Unauthorized);
        }

        // 尝试用请求头中获取Base64加密的租户ID，仅限于租户ID
        var tenantID = _httpContext.Request.Headers["TenantID"].ToString().Base64ToString().ParseToLong();

        if (!tenantID.IsNullOrZero())
        {
            TenantId = tenantID;
        }

        // 判断是否不需要登录
        if (_httpContext.GetEndpoint()?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
            // ReSharper disable once RedundantJumpStatement
            return;

        //// 由于生命周期的问题，这里的Token不能直接获取
        //var token = Furion.DataEncryption.JWTEncryption.GetJwtBearerToken(_httpContext as DefaultHttpContext);

        //if (token.IsEmpty())
        //{
        //    throw new Exception("鉴权失败！");
        //    //throw Oops.Bah("鉴权失败!").StatusCode(StatusCodes.Status401Unauthorized);
        //}

        //var jwtTokenInfo = JWTEncryption.ReadJwtToken(token);

        //if (jwtTokenInfo == null)
        //{
        //    throw new Exception("鉴权失败！");
        //    //throw Oops.Bah("鉴权失败!").StatusCode(StatusCodes.Status401Unauthorized);
        //}

        //// 获取 JWT 中的 MD5 字符串
        //var md5Str = jwtTokenInfo.Claims.FirstOrDefault(f => f.Type == IdTag)?.Value;

        //if (md5Str.IsEmpty())
        //{
        //    throw new Exception("获取用户信息鉴权失败！");
        //    //throw Oops.Bah("获取用户信息鉴权失败！").StatusCode(StatusCodes.Status401Unauthorized);
        //}

        //// 从缓存中读取用户信息，这里使用模糊匹配
        //var authDicInfo = _cache.Get<Dictionary<string, dynamic>>($"{CacheKey}*:{md5Str}");

        //if (authDicInfo == null || authDicInfo.Count == 0)
        //{
        //    throw new Exception("换取用户信息鉴权失败！");
        //    //throw Oops.Bah("换取用户信息鉴权失败！").StatusCode(StatusCodes.Status401Unauthorized);
        //}

        //// 绑定赋值
        //TenantId = authDicInfo[nameof(TenantId)];
        //UserId = authDicInfo[nameof(UserId)];
        //UserAccount = authDicInfo[nameof(UserAccount)];
        //UserJobNum = authDicInfo[nameof(UserJobNum)];
        //UserName = authDicInfo[nameof(UserName)];
        //IsSuperAdmin = authDicInfo[nameof(IsSuperAdmin)];
        //IsSystemAdmin = authDicInfo[nameof(IsSystemAdmin)];
        //IsTenantAdmin = authDicInfo[nameof(IsTenantAdmin)];
    }

    /// <summary>
    /// 统一登录
    /// </summary>
    /// <param name="authUserInfo"></param>
    /// <returns></returns>
    public async Task LoginAsync(AuthUserInfo authUserInfo)
    {
        if (authUserInfo == null)
        {
            throw new Exception("授权用户信息不能为空！");
            //throw Oops.Bah("授权用户信息不能为空！");
        }

        //try
        //{
        //    var cacheIdTag = MD5Encryption.Encrypt($"{authUserInfo.TenantId}_{authUserInfo.UserId}");

        //    // 设置授权信息缓存
        //    await _cache.SetAsync($"{CacheKey}{authUserInfo.TenantId}:{cacheIdTag}", authUserInfo);

        //    // TODO:这里的时间改成从数据库/缓存读取

        //    // 生成Token令牌，30分钟
        //    var accessToken = JWTEncryption.Encrypt(new Dictionary<string, object> {{IdTag, cacheIdTag}}, 30);

        //    // 获取刷新Token，30天
        //    // ReSharper disable once RedundantArgumentDefaultValue
        //    var refreshToken = JWTEncryption.GenerateRefreshToken(accessToken, 60 * 24 * 30);

        //    // 设置Token令牌
        //    _httpContext!.Response.Headers["access-token"] = accessToken;

        //    // 设置刷新Token令牌
        //    _httpContext!.Response.Headers["x-access-token"] = refreshToken;

        //    // 设置Swagger自动登录
        //    App.HttpContext.SigninToSwagger(accessToken);
        //}
        //catch (Exception ex)
        //{
        //    throw Oops.Bah($"403 登录鉴权失败：{ex.Message}").StatusCode(StatusCodes.Status403Forbidden);
        //}
    }

    /// <summary>
    /// 统一退出登录
    /// </summary>
    /// <returns></returns>
    public async Task LogoutAsync()
    {
        /*
         * 首先确定，退出登录有两种情况：
         *  1.正常情况下，手动点击退出登录，这个时候是存在Token的，Token也是没有过期的。
         *  2.401的情况下，系统调用退出登录的接口，这个时候虽然是存在Token的，但是Token肯定是过期的
         */

        //// 这里直接从请求头中获取 Token
        //var token = JWTEncryption.GetJwtBearerToken(_httpContext as DefaultHttpContext);

        //// 只有 Token 不等于空才进行后面的操作
        //if (!token.IsEmpty())
        //{
        //    // 读取 JWT Token 信息
        //    var jwtTokenInfo = JWTEncryption.ReadJwtToken(token);

        //    if (jwtTokenInfo != null)
        //    {
        //        // 获取 JWT 中的 MD5 字符串
        //        var md5Str = jwtTokenInfo.Claims.FirstOrDefault(f => f.Type == IdTag)?.Value;

        //        if (!md5Str.IsEmpty())
        //        {
        //            // 清除授权用户信息
        //            await _cache.DelAsync($"{CacheKey}*:{md5Str}");
        //        }
        //    }
        //}
    }
}