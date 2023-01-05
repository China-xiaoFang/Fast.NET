using System.ComponentModel;
using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.AdminFactory.ModelFactory.Tenant;
using Fast.Core.AdminFactory.ServiceFactory.Tenant;
using Fast.Core.Cache.Options;
using Fast.Core.Options;
using Fast.Core.SqlSugar.Options;
using Furion.FriendlyException;

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
            if (App.User != null)
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

                // 获取请求头中的站点Url
                var headersWebUrl = OriginUrl;
                if (!headersWebUrl.IsEmpty())
                {
                    var tenantInfo = App.GetService<ISysTenantService>().GetAllTenantInfo(wh => wh.WebUrl.Contains(headersWebUrl))
                        .Result;
                    if (tenantInfo is {Count: > 0})
                    {
                        return tenantInfo[0].Id;
                    }
                }
            }

            return 0L;
        }
    }

    /// <summary>
    /// 其他租户Id
    /// 根据业务自定义设置的租户Id
    /// 请谨慎使用
    /// </summary>
    public static long OtherTenantId { get; set; }

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
    /// 是否超级管理员
    /// </summary>
    public static bool IsSuperAdmin
    {
        get
        {
            if (App.User == null)
                return false;
            return App.User.FindFirst(ClaimConst.IsSuperAdmin)?.Value == AdminTypeEnum.SuperAdmin.GetHashCode().ParseToString();
        }
    }

    /// <summary>
    /// 是否系统管理员
    /// </summary>
    public static bool IsSystemAdmin
    {
        get
        {
            if (App.User == null)
                return false;
            return App.User.FindFirst(ClaimConst.IsSuperAdmin)?.Value == AdminTypeEnum.SystemAdmin.GetHashCode().ParseToString();
        }
    }

    /// <summary>
    /// 是否租户管理员
    /// </summary>
    public static bool IsTenantAdmin
    {
        get
        {
            if (App.User == null)
                return false;
            return App.User.FindFirst(ClaimConst.IsSuperAdmin)?.Value == AdminTypeEnum.TenantAdmin.GetHashCode().ParseToString();
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
        if (!TenantId.IsNullOrZero())
        {
            return TenantId;
        }

        if (!OtherTenantId.IsNullOrZero())
        {
            return OtherTenantId;
        }

        if (isThrow)
            throw Oops.Oh(ErrorCode.TenantSysError);

        return 0;
    }

    /// <summary>
    /// 租户库Db Info
    /// </summary>
    public static SysTenantDataBaseModel TenantDbInfo { get; set; }

    /// <summary>
    /// 数据库配置
    /// </summary>
    public static ConnectionStringsOptions ConnectionStringsOptions { get; set; }

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

/// <summary>
/// 环境枚举
/// </summary>
public enum EnvironmentEnum
{
    /// <summary>
    /// 生产
    /// </summary>
    [Description("生产")]
    Production = 1,

    /// <summary>
    /// 预生产
    /// </summary>
    [Description("预生产")]
    PreProduction = 2,

    /// <summary>
    /// 演示
    /// </summary>
    [Description("演示")]
    Demonstration = 3,

    /// <summary>
    /// 测试
    /// </summary>
    [Description("测试")]
    Test = 4,

    /// <summary>
    /// 开发
    /// </summary>
    [Description("开发")]
    Development = 5,
}

/// <summary>
/// Http请求前缀枚举
/// </summary>
public enum HttpRequestPrefixEnum
{
    /// <summary>
    /// 登录
    /// </summary>
    login,

    /// <summary>
    /// 查询
    /// </summary>
    list,

    /// <summary>
    /// 分页
    /// </summary>
    page,

    /// <summary>
    /// 添加
    /// </summary>
    add,

    /// <summary>
    /// 编辑
    /// </summary>
    edit,

    /// <summary>
    /// 删除
    /// </summary>
    delete,

    /// <summary>
    /// 导出
    /// </summary>
    export,

    /// <summary>
    /// 导入
    /// </summary>
    import,
}