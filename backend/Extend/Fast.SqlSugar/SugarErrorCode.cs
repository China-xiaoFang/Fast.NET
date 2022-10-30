namespace Fast.SqlSugar;

/// <summary>
/// 异常Code枚举
/// </summary>
[ErrorCodeType]
public enum SugarErrorCode
{
    /// <summary>
    /// SqlSugar配置错误，请检查 Model 是否继承了接口!
    /// </summary>
    [ErrorCodeItemMetadata("SqlSugar配置错误，请检查 Model 是否继承了接口!")]
    SugarModelError,

    /// <summary>
    /// 租户数据库配置异常！
    /// </summary>
    [ErrorCodeItemMetadata("租户数据库配置异常！")]
    TenantDbError,

    /// <summary>
    /// 数据库Type 配置异常！
    /// </summary>
    [ErrorCodeItemMetadata("数据库Type 配置异常！")]
    DbTypeError,

    /// <summary>
    /// 租户系统异常！
    /// </summary>
    [ErrorCodeItemMetadata("租户系统异常！")]
    TenantSysError
}