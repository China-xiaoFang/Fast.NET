namespace Fast.SDK.MiniExcel.Dto;

/// <summary>
/// MiniExcel Row Info
/// </summary>
/// <typeparam name="T"></typeparam>
public class MiniExcelRowInfoDto<T>
{
    /// <summary>
    /// 行编号
    /// </summary>
    public int RowNumber { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public T Value { get; set; }
}