using Fast.Admin.Model.BaseModel;
using Fast.Admin.Model.Enum;
using Fast.SDK.Common.EnumFactory;
using Fast.SqlSugar.Tenant.AttributeFilter;
using Fast.SqlSugar.Tenant.Internal.Enum;
using SqlSugar;

namespace Fast.Admin.Model.Model.Sys.Log;

/// <summary>
/// 系统操作日志表Model类
/// </summary>
[SugarTable("Sys_Log_Op_{year}{month}{day}", "系统操作日志表")]
[SplitTable(SplitType.Month)]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class SysLogOpModel : BaseLogEntity
{
    /// <summary>
    /// 主键Id
    /// </summary>
    [SugarColumn(ColumnDescription = "Id主键", IsPrimaryKey = true)]
    public new long Id { get; set; }

    /// <summary>
    /// 操作人账号
    /// </summary>
    [SugarColumn(ColumnDescription = "操作人账号", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string Account { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    [SugarColumn(ColumnDescription = "姓名", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string UserName { get; set; }

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
    [SugarColumn(ColumnDescription = "操作名称", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string OperationName { get; set; }

    /// <summary>
    /// 类名
    /// </summary>
    [SugarColumn(ColumnDescription = "类名", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string ClassName { get; set; }

    /// <summary>
    /// 方法名
    /// </summary>
    [SugarColumn(ColumnDescription = "方法名", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string MethodName { get; set; }

    /// <summary>
    /// 请求地址
    /// </summary>
    [SugarColumn(ColumnDescription = "请求地址", ColumnDataType = "Nvarchar(100)", IsNullable = true)]
    public string Url { get; set; }

    /// <summary>
    /// 请求方式（GET POST PUT DELETE)
    /// </summary>
    [SugarColumn(ColumnDescription = "请求方式（GET POST PUT DELETE)", ColumnDataType = "Nvarchar(10)", IsNullable = true)]
    public string ReqMethod { get; set; }

    /// <summary>
    /// 请求参数
    /// </summary>
    [SugarColumn(ColumnDescription = "请求参数", ColumnDataType = "Nvarchar(max)", IsNullable = true)]
    public string Param { get; set; }

    /// <summary>
    /// 返回结果
    /// </summary>
    [SugarColumn(ColumnDescription = "返回结果", ColumnDataType = "Nvarchar(max)", IsNullable = true)]
    public string Result { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    [SugarColumn(ColumnDescription = "地址", ColumnDataType = "Nvarchar(500)", IsNullable = true)]
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
}