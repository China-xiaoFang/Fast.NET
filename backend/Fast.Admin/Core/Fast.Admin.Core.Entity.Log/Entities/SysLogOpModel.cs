// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
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

using Fast.Admin.Core.Enum.Common;
using Fast.Admin.Core.Enum.Db;
using Fast.Admin.Core.Enum.Http;

namespace Fast.Admin.Core.Entity.Log.Entities;

/// <summary>
/// 系统操作日志表Model类
/// </summary>
[SugarTable("Sys_Log_Op_{year}{month}{day}", "系统操作日志表")]
[SplitTable(SplitType.Month)]
[SugarDbType(FastDbTypeEnum.SysCoreLog)]
public class SysLogOpModel : BaseSnowflakeRecordEntity, IBaseTEntity
{
    /// <summary>
    /// 操作人账号
    /// </summary>
    [SugarColumn(ColumnDescription = "操作人账号", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string Account { get; set; }

    /// <summary>
    /// 操作人工号
    /// </summary>
    [SugarColumn(ColumnDescription = "操作人工号", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string UserJobNo { get; set; }

    /// <summary>
    /// 是否执行成功
    /// </summary>
    [SugarColumn(ColumnDescription = "是否执行成功", ColumnDataType = "tinyint", IsNullable = true)]
    public YesOrNotEnum Success { get; set; }

    /// <summary>
    /// 操作行为
    /// </summary>
    [SugarColumn(ColumnDescription = "操作行为", ColumnDataType = "tinyint", IsNullable = true)]
    public HttpRequestActionEnum? OperationAction { get; set; }

    /// <summary>
    /// 操作名称
    /// </summary>
    [SugarColumn(ColumnDescription = "操作名称", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string OperationName { get; set; }

    /// <summary>
    /// 类名
    /// </summary>
    [SugarColumn(ColumnDescription = "类名", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string ClassName { get; set; }

    /// <summary>
    /// 方法名
    /// </summary>
    [SugarColumn(ColumnDescription = "方法名", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string MethodName { get; set; }

    /// <summary>
    /// 请求地址
    /// </summary>
    [SugarColumn(ColumnDescription = "请求地址", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string Url { get; set; }

    /// <summary>
    /// 请求方式
    /// </summary>
    [SugarColumn(ColumnDescription = "请求方式", ColumnDataType = "tinyint", IsNullable = true)]
    public HttpRequestMethodEnum? ReqMethod { get; set; }

    /// <summary>
    /// 请求参数
    /// </summary>
    [SugarColumn(ColumnDescription = "请求参数", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string Param { get; set; }

    /// <summary>
    /// 返回结果
    /// </summary>
    [SugarColumn(ColumnDescription = "返回结果", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string Result { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(ColumnDescription = "地址", ColumnDataType = "Nvarchar(MAX)", IsNullable = true)]
    public string Location { get; set; }

    /// <summary>
    /// 耗时（毫秒）
    /// </summary>
    [SugarColumn(ColumnDescription = "耗时（毫秒）", IsNullable = true)]
    public long? ElapsedTime { get; set; }

    /// <summary>
    /// 操作时间
    /// </summary>
    [SplitField]
    [SugarColumn(ColumnDescription = "操作时间", ColumnDataType = "datetimeoffset", IsNullable = false)]
    public DateTime OpTime { get; set; }

    /// <summary>
    /// 租户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "租户Id", IsNullable = true, CreateTableFieldSort = 997)]
    public long TenantId { get; set; }
}