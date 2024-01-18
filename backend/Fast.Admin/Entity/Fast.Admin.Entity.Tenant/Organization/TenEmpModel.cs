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

using Fast.Admin.Core.Enum.Db;

namespace Fast.Admin.Entity.Tenant.Organization;

/// <summary>
/// <see cref="TenEmpModel"/> 租户员工档案表Model类
/// </summary>
[SugarTable("Ten_Emp", "租户员工档案表")]
[SugarDbType(FastDbTypeEnum.SysAdminCore)]
public class TenEmpModel : BaseEntity
{
    /// <summary>
    /// 用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "用户Id", IsNullable = false)]
    public long UserId { get; set; }

    /// <summary>
    /// 入职日期
    /// </summary>
    [SugarColumn(ColumnDescription = "入职日期", ColumnDataType = "datetimeoffset", IsNullable = false)]
    public DateTime EntryDate { get; set; }

    /// <summary>
    /// 离职日期
    /// </summary>
    [SugarColumn(ColumnDescription = "离职日期", ColumnDataType = "datetimeoffset", IsNullable = true)]
    public DateTime ResignationDate { get; set; }

    /// <summary>
    /// 民族
    /// </summary>
    [SugarColumn(ColumnDescription = "民族", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string Nation { get; set; }

    /// <summary>
    /// 籍贯
    /// </summary>
    [SugarColumn(ColumnDescription = "籍贯", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string NativePlace { get; set; }

    /// <summary>
    /// 家庭地址
    /// </summary>
    [SugarColumn(ColumnDescription = "家庭地址", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string FamilyAddress { get; set; }

    /// <summary>
    /// 通信地址
    /// </summary>
    [SugarColumn(ColumnDescription = "通信地址", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string MailingAddress { get; set; }

    /// <summary>
    /// 证件类型
    /// </summary>
    [SugarColumn(ColumnDescription = "证件类型", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string IdType { get; set; }

    /// <summary>
    /// 证件号码
    /// </summary>
    [SugarColumn(ColumnDescription = "证件号码", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string IdNumber { get; set; }

    /// <summary>
    /// 文件程度
    /// </summary>
    [SugarColumn(ColumnDescription = "文件程度", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string EducationLevel { get; set; }

    /// <summary>
    /// 政治面貌
    /// </summary>
    [SugarColumn(ColumnDescription = "政治面貌", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string PoliticalStatus { get; set; }

    /// <summary>
    /// 毕业学院
    /// </summary>
    [SugarColumn(ColumnDescription = "毕业学院", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string GraduationCollege { get; set; }

    /// <summary>
    /// 学历
    /// </summary>
    [SugarColumn(ColumnDescription = "学历", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string AcademicQualifications { get; set; }

    /// <summary>
    /// 学制
    /// </summary>
    [SugarColumn(ColumnDescription = "学制", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string AcademicSystem { get; set; }

    /// <summary>
    /// 学位
    /// </summary>
    [SugarColumn(ColumnDescription = "学位", ColumnDataType = "Nvarchar(50)", IsNullable = true)]
    public string Degree { get; set; }

    /// <summary>
    /// 家庭电话
    /// </summary>
    [SugarColumn(ColumnDescription = "家庭电话", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string FamilyMobile { get; set; }

    /// <summary>
    /// 办公电话
    /// </summary>
    [SugarColumn(ColumnDescription = "办公电话", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string OfficeMobile { get; set; }

    /// <summary>
    /// 紧急联系人
    /// </summary>
    [SugarColumn(ColumnDescription = "紧急联系人", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string EmergencyContact { get; set; }

    /// <summary>
    /// 紧急联系电话
    /// </summary>
    [SugarColumn(ColumnDescription = "紧急联系电话", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string EmergencyMobile { get; set; }

    /// <summary>
    /// 紧急联系地址
    /// </summary>
    [SugarColumn(ColumnDescription = "紧急联系地址", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string EmergencyAddress { get; set; }
}