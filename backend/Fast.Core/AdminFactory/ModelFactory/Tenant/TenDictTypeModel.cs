using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.SqlSugar.Tenant.AttributeFilter;
using Fast.SqlSugar.Tenant.Internal.Enum;

namespace Fast.Core.AdminFactory.ModelFactory.Tenant;

/// <summary>
/// 租户字典类型表Model类
/// </summary>
[SugarTable("Ten_Dict_Type", "系统字典类型表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenDictTypeModel : SysDictTypeModel
{
    /// <summary>
    /// 数据集合
    /// </summary>
    [Navigate(NavigateType.OneToMany, nameof(TenDictDataModel.TypeId))]
    public new List<TenDictDataModel> DataList { get; set; }
}