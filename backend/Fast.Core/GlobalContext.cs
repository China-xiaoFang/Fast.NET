using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.Cache.Options;
using Fast.Core.Const;
using Fast.Core.Internal.Options;
using Fast.SqlSugar.Tenant;

namespace Fast.Core;

/// <summary>
/// 系统通用上下文
/// </summary>
public class GlobalContext
{
    /// <summary>
    /// 当前租户Id
    /// </summary>
    public static long TenantId
    {
        get
        {
            if (App.User is {Identity.IsAuthenticated: true})
            {
                // 获取Token中的
                return (App.User?.FindFirst(ClaimConst.TenantId)?.Value).ParseToLong();
            }

            if (App.HttpContext != null)
            {
                // 获取请求头中Base64加密的租户Id
                var headersTenantId = App.HttpContext.Request.Headers[ClaimConst.TenantId].ParseToString().Base64ToString()
                    .ParseToLong();
                if (!headersTenantId.IsNullOrZero())
                {
                    return headersTenantId;
                }

                //// 获取请求头中的站点Url
                //var headersWebUrl = OriginUrl;
                //if (!headersWebUrl.IsEmpty())
                //{
                //    var tenantInfo = App.GetService<ISysTenantService>().GetAllTenantInfo(wh => wh.WebUrl.Contains(headersWebUrl))
                //        .Result;
                //    if (tenantInfo is {Count: > 0})
                //    {
                //        return tenantInfo[0].Id;
                //    }
                //}
            }

            return 0L;
        }
    }

    /// <summary>
    /// 其他租户Id
    /// 根据业务自定义设置的租户Id
    /// 请谨慎使用
    /// </summary>
    public static long CustomTenantId
    {
        get => SugarContext.CustomTenantId;
        set => SugarContext.CustomTenantId = value;
    }

    /// <summary>
    /// 当前用户Id
    /// </summary>
    public static long UserId => (App.User?.FindFirst(ClaimConst.UserId)?.Value).ParseToLong();

    /// <summary>
    /// 当前用户账号
    /// </summary>
    public static string UserAccount => (App.User?.FindFirst(ClaimConst.Account)?.Value).ParseToString();

    /// <summary>
    /// 当前用户名称
    /// </summary>
    public static string UserName => (App.User?.FindFirst(ClaimConst.Name)?.Value).ParseToString();

    /// <summary>
    /// 请求来源Url
    /// </summary>
    public static string OriginUrl => App.HttpContext.Request.Headers[ClaimConst.Origin].ParseToString();

    /// <summary>
    /// 请求客户端UUID，唯一标识，不安全
    /// </summary>
    public static string UUID => App.HttpContext.Request.Headers[ClaimConst.UUID].ParseToString();

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    public static bool IsSuperAdmin
    {
        get
        {
            if (App.User is {Identity.IsAuthenticated: true})
                return false;
            return App.User.FindFirst(ClaimConst.AdminType)?.Value == AdminTypeEnum.SuperAdmin.GetHashCode().ParseToString();
        }
    }

    /// <summary>
    /// 是否系统管理员
    /// </summary>
    public static bool IsSystemAdmin
    {
        get
        {
            if (App.User is {Identity.IsAuthenticated: true})
                return false;
            return App.User.FindFirst(ClaimConst.AdminType)?.Value == AdminTypeEnum.SystemAdmin.GetHashCode().ParseToString();
        }
    }

    /// <summary>
    /// 是否租户管理员
    /// </summary>
    public static bool IsTenantAdmin
    {
        get
        {
            if (App.User is {Identity.IsAuthenticated: true})
                return false;
            return App.User.FindFirst(ClaimConst.AdminType)?.Value == AdminTypeEnum.TenantAdmin.GetHashCode().ParseToString();
        }
    }

    ///// <summary>
    ///// 当前用户信息
    ///// </summary>
    //public static SysUserModel UserModelInfo =>
    //    GetService<ISqlSugarRepository<SysUserModel>>().FirstOrDefault(f => f.Id == UserId);

    ///// <summary>
    ///// 检测用户是否存在
    ///// </summary>
    ///// <param name="userId"></param>
    ///// <returns></returns>
    //public static async Task<SysUserModel> CheckUserAsync(long userId)
    //{
    //    var user = await GetService<ISqlSugarRepository<SysUserModel>>().FirstOrDefaultAsync(u => u.Id == userId);
    //    return user ?? throw Oops.Bah(ErrorCodeEnum.D1002);
    //}

    ///// <summary>
    ///// 获取用户员工信息
    ///// </summary>
    ///// <param name="userId"></param>
    ///// <returns></returns>
    //public static async Task<SysEmpModel> GetUserEmpInfo(long userId)
    //{
    //    var emp = await GetService<ISqlSugarRepository<SysEmpModel>>().FirstOrDefaultAsync(u => u.Id == userId);
    //    return emp ?? throw Oops.Bah(ErrorCodeEnum.D1002);
    //}

    /// <summary>
    /// 获取租户Id，
    /// 复杂业务请用此方法
    /// </summary>
    /// <param name="isThrow">是否抛出错误</param>
    /// <returns></returns>
    public static long GetTenantId(bool isThrow = true)
    {
        return SugarContext.GetTenantId(isThrow);
    }

    /// <summary>
    /// 缓存配置
    /// </summary>
    public static CacheOptions CacheOptions { get; set; }

    /// <summary>
    /// 系统配置
    /// </summary>
    public static SystemSettingsOptions SystemSettingsOptions { get; set; }

    /// <summary>
    /// 版权信息
    /// </summary>
    public static CopyrightInfoOptions CopyrightInfoOptions { get; set; }

    /// <summary>
    /// 上传文件配置
    /// </summary>
    public static UploadFileOptions UploadFileOptions { get; set; }

    /// <summary>
    /// 服务配置集合
    /// </summary>
    public static ServiceCollectionOptions ServiceCollectionOptions { get; set; }
}