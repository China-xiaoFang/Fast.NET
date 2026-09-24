// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

namespace Fast.Swagger;

/// <summary>
/// 解决规范化文档 SchemaId 冲突问题
/// </summary>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.Class)]
public sealed class SchemaIdAttribute : Attribute
{
    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    /// <param name="schemaId">schema 的唯一标识</param>
    public SchemaIdAttribute(string schemaId)
    {
        SchemaId = schemaId;
    }

    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    /// <param name="schemaId">schema 的唯一标识</param>
    /// <param name="replace">替换默认 SchemaId 的自定义值</param>
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
    /// <remarks>默认在头部叠加，设置 <see langword="true"/> 之后，将直接使用 <see cref="SchemaId"/></remarks>
    public bool Replace { get; set; }
}
