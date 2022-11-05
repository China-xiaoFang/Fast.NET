using Furion.FriendlyException;

namespace Fast.Core;

/// <summary>
/// 异常Code枚举
/// </summary>
[ErrorCodeType]
public enum ErrorCode
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
    TenantSysError,

    /// <summary>
    /// 系统配置错误，请检查系统配置！
    /// </summary>
    [ErrorCodeItemMetadata("系统配置错误，请检查系统配置！")]
    ConfigError,

    /// <summary>
    /// 演示环境，禁止操作！
    /// </summary>
    [ErrorCodeItemMetadata("演示环境，禁止操作！")]
    DemoEnvNoOperate,

    /// <summary>
    /// 系统内部错误，请联系管理员处理！
    /// </summary>
    [ErrorCodeItemMetadata("系统内部错误，请联系管理员处理！")]
    SystemError,

    /// <summary>
    /// 已存在同名租户信息！
    /// </summary>
    [ErrorCodeItemMetadata("已存在同名租户信息！")]
    TenantRepeatError,

    /// <summary>
    /// 已存在同主机租户信息！
    /// </summary>
    [ErrorCodeItemMetadata("已存在同主机租户信息！")]
    TenantWebUrlRepeatError,

    /// <summary>
    /// 租户信息不存在！
    /// </summary>
    [ErrorCodeItemMetadata("租户信息不存在！")]
    TenantNotExistError,

    /// <summary>
    /// 租户数据库信息不存在！
    /// </summary>
    [ErrorCodeItemMetadata("租户数据库信息不存在！")]
    TenantDbNotExistError,

    /// <summary>
    /// 租户数据库已存在！
    /// </summary>
    [ErrorCodeItemMetadata("租户数据库已存在！")]
    TenantDataBaseRepeatError,

    /// <summary>
    /// 接口请求过于频繁，请稍后再试！
    /// </summary>
    [ErrorCodeItemMetadata("接口请求过于频繁，请稍后再试！")]
    ApiLimitError,
}