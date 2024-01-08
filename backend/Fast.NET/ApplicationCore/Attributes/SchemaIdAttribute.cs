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

// ReSharper disable once CheckNamespace
namespace Fast.Swagger;

/// <summary>
/// <see cref="SchemaIdAttribute"/> 解决规范化文档 SchemaId 冲突问题
/// </summary>
[SuppressSniffer, AttributeUsage(AttributeTargets.Class)]
public sealed class SchemaIdAttribute : Attribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="schemaId">自定义 SchemaId，只能是字母开头，只运行下划线_连接</param>
    public SchemaIdAttribute(string schemaId)
    {
        SchemaId = schemaId;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="schemaId">自定义 SchemaId</param>
    /// <param name="replace">默认在头部叠加，设置 true 之后，将直接使用 <see cref="SchemaId"/></param>
    public SchemaIdAttribute(string schemaId, bool replace)
    {
        SchemaId = schemaId;
        Replace = replace;
    }

    /// <summary>
    /// 自定义 SchemaId
    /// </summary>
    public string SchemaId { get; set; }

    /// <summary>
    /// 完全覆盖
    /// </summary>
    /// <remarks>默认在头部叠加，设置 true 之后，将直接使用 <see cref="SchemaId"/></remarks>
    public bool Replace { get; set; }
}