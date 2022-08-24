namespace Fast.NET.Core.AdminFactory.EnumFactory;

/// <summary>
/// 异常Code枚举
/// </summary>
[ErrorCodeType]
public enum ErrorCodeEnum
{
    [ErrorCodeItemMetadata("系统内部错误，请联系管理员处理！")]
    SystemError,

    [ErrorCodeItemMetadata("SqlSugar config error, please Check whether the Model inherits the interface!")]
    SugarModelError,
    
    [ErrorCodeItemMetadata("租户数据库配置异常！")]
    TenantDbError,
    
    [ErrorCodeItemMetadata("数据库Type 配置异常！")]
    DbTypeError,
}