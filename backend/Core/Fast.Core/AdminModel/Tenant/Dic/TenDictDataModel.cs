using Fast.Core.AdminModel.Sys.Dic;
using Fast.Iaas.Attributes;

namespace Fast.Core.AdminModel.Tenant.Dic;

/// <summary>
/// 租户字典数据表Model类
/// </summary>
[SugarTable("Ten_Dict_Data", "租户字典数据表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenDictDataModel : SysDictDataModel
{
}