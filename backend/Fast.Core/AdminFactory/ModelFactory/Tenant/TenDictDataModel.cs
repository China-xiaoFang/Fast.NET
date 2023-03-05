using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.SqlSugar.Tenant.AttributeFilter;
using Fast.SqlSugar.Tenant.Internal.Enum;

namespace Fast.Core.AdminFactory.ModelFactory.Tenant;

/// <summary>
/// 租户字典数据表Model类
/// </summary>
[SugarTable("Ten_Dict_Data", "租户字典数据表")]
[SugarDbType(SugarDbTypeEnum.Tenant)]
public class TenDictDataModel : SysDictDataModel
{
}