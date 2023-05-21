namespace Fast.Admin.Service.Cache.Dto;

/// <summary>
/// 编辑缓存Value输入
/// </summary>
public class EditCacheValueInput
{
    /// <summary>
    /// 缓存Key
    /// </summary>
    [Required(ErrorMessage = "缓存Key不能为空")]
    public string Key { get; set; }

    /// <summary>
    /// 缓存Value
    /// </summary>
    [Required(ErrorMessage = "缓存Value不能为空")]
    public string Value { get; set; }
}

/// <summary>
/// 删除缓存Value输入
/// </summary>
public class DeleteCacheValueInput
{
    /// <summary>
    /// 缓存Key
    /// </summary>
    [Required(ErrorMessage = "缓存Key不能为空")]
    public string Key { get; set; }
}