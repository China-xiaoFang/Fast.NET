using Fast.Iaas.Attributes;
using Fast.Iaas.BaseModel;
using SqlSugar;

namespace Fast.Admin.Model.Model.Tenant.Organization.User;

/// <summary>
/// 租户员工表Model类
/// </summary>
[SugarTable("Ten_Emp", "租户员工表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenEmpModel : BaseEntity
{
    /// <summary>
    /// 系统用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "职位Id", IsNullable = false)]
    public long SysUserId { get; set; }

    /// <summary>
    /// 工号
    /// </summary>
    [SugarColumn(ColumnDescription = "工号", ColumnDataType = "Nvarchar(20)", IsNullable = false,
        UniqueGroupNameList = new[] {nameof(JobNum)})]
    public string JobNum { get; set; }

    /// <summary>
    /// 机构Id
    /// </summary>
    [SugarColumn(ColumnDescription = "机构Id", IsNullable = false)]
    public long OrgId { get; set; }

    /// <summary>
    /// 职位Id
    /// </summary>
    [SugarColumn(ColumnDescription = "职位Id", IsNullable = false)]
    public long PositionId { get; set; }

    /// <summary>
    /// 主管系统用户Id
    /// </summary>
    [SugarColumn(ColumnDescription = "职位Id", IsNullable = true)]
    public long? LeaderSysUserId { get; set; }

    /// <summary>
    /// 职级Id
    /// </summary>
    [SugarColumn(ColumnDescription = "职级Id", IsNullable = true)]
    public long? RankId { get; set; }

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
    public string FamilyPhone { get; set; }

    /// <summary>
    /// 办公电话
    /// </summary>
    [SugarColumn(ColumnDescription = "办公电话", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string OfficePhone { get; set; }

    /// <summary>
    /// 紧急联系人
    /// </summary>
    [SugarColumn(ColumnDescription = "紧急联系人", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string EmergencyContact { get; set; }

    /// <summary>
    /// 紧急联系电话
    /// </summary>
    [SugarColumn(ColumnDescription = "紧急联系电话", ColumnDataType = "Nvarchar(20)", IsNullable = true)]
    public string EmergencyPhone { get; set; }

    /// <summary>
    /// 紧急联系地址
    /// </summary>
    [SugarColumn(ColumnDescription = "紧急联系地址", ColumnDataType = "Nvarchar(200)", IsNullable = true)]
    public string EmergencyAddress { get; set; }
}