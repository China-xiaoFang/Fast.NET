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

#if NET10_0_OR_GREATER
using System.Text.Json.Nodes;
using Microsoft.OpenApi;
#else
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
#endif
using System.Text.RegularExpressions;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fast.Swagger;

/// <summary>
/// 修正 规范化文档 Enum 提示
/// </summary>
internal class EnumSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// 中文正则表达式
    /// </summary>
    private const string CHINESE_PATTERN = @"[\u4e00-\u9fa5]";

#if NET10_0_OR_GREATER
    /// <summary>
    /// 实现过滤器方法
    /// </summary>
    /// <param name="schema"></param>
    /// <param name="context"></param>
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        var type = context.Type;

        // 排除其他程序集的枚举
        if (type.IsEnum && MAppContext.Assemblies.Contains(type.Assembly) && schema is OpenApiSchema model)
        {
            model.Enum?.Clear();

            var enumValues = Enum.GetValues(type);

            // 从配置文件中读取全局配置
            var convertToNumber = Penetrates.SwaggerSettings.EnumToNumber!.Value;

            // 包含中文情况
            if (Enum.GetNames(type)
                .Any(v => Regex.IsMatch(v, CHINESE_PATTERN)))
            {
                convertToNumber = true;
            }

            // 获取枚举实际值类型
            var enumValueType = type.GetField("value__")
                ?.FieldType;

            foreach (var value in enumValues)
            {
                var numValue = value.ChangeType(enumValueType);

                // OpenApi v2: 使用 JsonNode 替代 OpenApiString/OpenApiAnyFactory
                model.Enum?.Add(!convertToNumber ? JsonValue.Create(value.ToString()) : JsonNode.Parse($"{numValue}"));
            }

            if (!convertToNumber)
            {
                model.Type = JsonSchemaType.String;
                model.Format = null;
            }
        }
    }
#else
    /// <summary>
    /// 实现过滤器方法
    /// </summary>
    /// <param name="model"></param>
    /// <param name="context"></param>
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        var type = context.Type;

        // 排除其他程序集的枚举
        if (type.IsEnum && MAppContext.Assemblies.Contains(type.Assembly))
        {
            model.Enum.Clear();

            var enumValues = Enum.GetValues(type);

            // 从配置文件中读取全局配置
            var convertToNumber = Penetrates.SwaggerSettings.EnumToNumber!.Value;

            // 包含中文情况
            if (Enum.GetNames(type)
                .Any(v => Regex.IsMatch(v, CHINESE_PATTERN)))
            {
                convertToNumber = true;
            }

            // 获取枚举实际值类型
            var enumValueType = type.GetField("value__")
                ?.FieldType;

            foreach (var value in enumValues)
            {
                var numValue = value.ChangeType(enumValueType);

                model.Enum.Add(!convertToNumber
                    ? new OpenApiString(value.ToString())
                    : OpenApiAnyFactory.CreateFromJson($"{numValue}"));
            }

            if (!convertToNumber)
            {
                model.Type = "string";
                model.Format = null;
            }
        }
    }
#endif
}