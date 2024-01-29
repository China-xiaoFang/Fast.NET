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

using Fast.IaaS;
using Fast.SqlSugar.Handlers;

namespace Fast.SqlSugar.Attributes;

/// <summary>
/// <see cref="SugarDbTypeAttribute"/> SqlSugar DB类型
/// <remarks>放入 Class 头部，支持传入 Object，然后在 <see cref="ISqlSugarEntityHandler"/> 自行解析</remarks>
/// </summary>
[SuppressSniffer, AttributeUsage(AttributeTargets.Class)]
public class SugarDbTypeAttribute : Attribute
{
    /// <summary>
    /// <see cref="object"/> Entity 的DB类型
    /// <remarks>如果为Null，则代表的默认库</remarks>
    /// </summary>
    public object Type { get; set; }

    /// <summary>
    /// <see cref="SugarDbTypeAttribute"/> SqlSugar DB类型
    /// <remarks>放入 Class 头部，支持传入 Object，然后在 <see cref="ISqlSugarEntityHandler"/> 自行解析</remarks>
    /// </summary>
    public SugarDbTypeAttribute()
    {
        Type = null;
    }

    /// <summary>
    /// <see cref="SugarDbTypeAttribute"/> SqlSugar DB类型
    /// <remarks>放入 Class 头部，支持传入 Object，然后在 <see cref="ISqlSugarEntityHandler"/> 自行解析</remarks>
    /// </summary>
    /// <param name="type"><see cref="object"/> Entity 的DB类型</param>
    public SugarDbTypeAttribute(object type)
    {
        Type = type;
    }
}