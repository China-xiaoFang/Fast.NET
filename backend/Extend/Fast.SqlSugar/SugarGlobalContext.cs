namespace Fast.SqlSugar;

/// <summary>
/// Sugar通用上下文
/// </summary>
public class SugarGlobalContext
{
    /// <summary>
    /// 当前租户Id
    /// </summary>
    public static long TenantId
    {
        get
        {
            if (User != null)
            {
                // 获取Token中的
                return (User?.FindFirst(ClaimConst.TenantId)?.Value).ParseToLong();
            }

            if (HttpContext != null)
            {
                // 获取请求头中Base64加密的租户Id
                var headersTenantId = HttpContext.Request.Headers["TenantId"].ParseToString().Base64ToString().ParseToLong();
                if (!headersTenantId.IsNullOrZero())
                {
                    return headersTenantId;
                }

                // 获取请求头中的站点Url
                var headersWebUrl = HttpContext.Request.Headers["Fast-NET-Origin"].ParseToString();
                if (!headersWebUrl.IsEmpty())
                {
                    var _db = ((SqlSugarClient) GetService<ISqlSugarClient>()).GetConnection(ConnectionStringsOptions
                        .DefaultServiceIp);
                    var tenantInfo = _db.Queryable<dynamic>().AS(SugarFieldConst.TenantTableName)
                        .Where($"{SugarFieldConst.TenantTableSiteUrlFieldName} LIKE @Sys_SiteUrl",
                            new {Sys_SiteUrl = $"%{headersWebUrl}%"}).Select(SugarFieldConst.TenantTableTenantIdFieldName)
                        .First();
                    if (tenantInfo != null)
                    {
                        return tenantInfo.TenantId;
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
    public static long UserId => (User?.FindFirst(ClaimConst.UserId)?.Value).ParseToLong();

    /// <summary>
    /// 当前用户账号
    /// </summary>
    public static string UserAccount => (User?.FindFirst(ClaimConst.Account)?.Value).ParseToString();

    /// <summary>
    /// 当前用户名称
    /// </summary>
    public static string UserName => (User?.FindFirst(ClaimConst.Name)?.Value).ParseToString();

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    public static bool IsSuperAdmin
    {
        get
        {
            if (User == null)
                return false;
            return User.FindFirst(ClaimConst.IsSuperAdmin)?.Value == AdminTypeEnum.SuperAdmin.GetHashCode().ParseToString();
        }
    }

    /// <summary>
    /// 是否系统管理员
    /// </summary>
    public static bool IsSystemAdmin
    {
        get
        {
            if (User == null)
                return false;
            return User.FindFirst(ClaimConst.IsSuperAdmin)?.Value == AdminTypeEnum.SystemAdmin.GetHashCode().ParseToString();
        }
    }

    /// <summary>
    /// 是否租户管理员
    /// </summary>
    public static bool IsTenantAdmin
    {
        get
        {
            if (User == null)
                return false;
            return User.FindFirst(ClaimConst.IsSuperAdmin)?.Value == AdminTypeEnum.TenantAdmin.GetHashCode().ParseToString();
        }
    }

    /// <summary>
    /// 租户库Db Info
    /// </summary>
    public static SysTenantDataBaseModel TenantDbInfo { get; set; }

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
            throw Oops.Oh(SugarErrorCode.TenantSysError);

        return 0;
    }

    /// <summary>
    /// 数据库配置
    /// </summary>
    public static ConnectionStringsOptions ConnectionStringsOptions => GetConfig<ConnectionStringsOptions>("ConnectionStrings");
}