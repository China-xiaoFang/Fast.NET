// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fast.OpenApi;

/// <summary>
/// OpenAPI 文档声明属性 DTO
/// </summary>
public class OpenApiDocumentSchemaPropertyDto
{
    /// <summary>
    /// 类型
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 格式
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// 可为空
    /// </summary>
    public bool Nullable { get; set; }

    /// <summary>
    /// 只读
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 声明项
    /// </summary>
    public OpenApiDocumentSchemaPropertyDto Items { get; set; }

    /// <summary>
    /// 对象属性定义。
    /// </summary>
    public IDictionary<string, OpenApiDocumentSchemaPropertyDto> Properties { get; set; }

    /// <summary>
    /// 必填属性名称。
    /// </summary>
    public HashSet<string> Required { get; set; }

    /// <summary>
    /// 附加属性定义。
    /// </summary>
    /// <remarks>OpenAPI 允许该值为布尔值或架构对象，因此保留原始 JSON 结构。</remarks>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public JsonElement AdditionalProperties { get; set; }

    /// <summary>
    /// 全部匹配的组合架构。
    /// </summary>
    public List<OpenApiDocumentSchemaPropertyDto> AllOf { get; set; }

    /// <summary>
    /// 任一匹配的组合架构。
    /// </summary>
    public List<OpenApiDocumentSchemaPropertyDto> AnyOf { get; set; }

    /// <summary>
    /// 唯一匹配的组合架构。
    /// </summary>
    public List<OpenApiDocumentSchemaPropertyDto> OneOf { get; set; }

    /// <summary>
    /// 引用
    /// </summary>
    [JsonPropertyName("$ref")]
    public string Ref { get; set; }
}
