// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fast.OpenApi;

/// <summary>
/// OpenAPI 文档组件声明 DTO
/// </summary>
public class OpenApiDocumentComponentSchemaDto
{
    /// <summary>
    /// 枚举
    /// </summary>
    public List<long> Enum { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 格式
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// 可为空。
    /// </summary>
    public bool Nullable { get; set; }

    /// <summary>
    /// 声明项。
    /// </summary>
    public OpenApiDocumentSchemaPropertyDto Items { get; set; }

    /// <summary>
    /// 架构属性定义
    /// </summary>
    public IDictionary<string, OpenApiDocumentSchemaPropertyDto> Properties { get; set; }

    /// <summary>
    /// 必填属性名称。
    /// </summary>
    public HashSet<string> Required { get; set; }

    /// <summary>
    /// 附加属性
    /// </summary>
    [JsonIgnore]
    public bool AdditionalProperties
    {
        get => _additionalProperties;
        set
        {
            _additionalProperties = value;
            _additionalPropertiesSchema = JsonSerializer.SerializeToElement(value);
        }
    }

    /// <summary>
    /// 附加属性的原始定义。
    /// </summary>
    /// <remarks>用于同时兼容 OpenAPI 中的布尔值和架构对象。</remarks>
    [JsonPropertyName("additionalProperties")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public JsonElement AdditionalPropertiesSchema
    {
        get
        {
            if (_additionalPropertiesSchema.ValueKind != JsonValueKind.Undefined)
                return _additionalPropertiesSchema;

            return JsonSerializer.SerializeToElement(AdditionalProperties);
        }
        set
        {
            _additionalPropertiesSchema = value;
            _additionalProperties = value.ValueKind is JsonValueKind.True or JsonValueKind.Object;
        }
    }

    /// <summary>
    /// 引用。
    /// </summary>
    [JsonPropertyName("$ref")]
    public string Ref { get; set; }

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
    /// 描述
    /// </summary>
    public string Description { get; set; }

    private bool _additionalProperties;

    private JsonElement _additionalPropertiesSchema;
}
