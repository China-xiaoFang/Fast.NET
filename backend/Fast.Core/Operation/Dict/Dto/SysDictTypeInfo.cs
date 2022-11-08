namespace Fast.Core.Operation.Dict.Dto;

/// <summary>
/// 系统字典类型
/// </summary>
public class SysDictTypeInfo
{
    /// <summary>
    /// 编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 中文名称
    /// </summary>
    public string ChName { get; set; }

    /// <summary>
    /// 英文名称
    /// </summary>
    public string EnName { get; set; }

    /// <summary>
    /// 数据集合
    /// </summary>
    public List<SysDictDataInfo> DataList { get; set; }
}