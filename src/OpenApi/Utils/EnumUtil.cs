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

using System.ComponentModel;
using System.Reflection;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Fast.OpenApi;

/// <summary>
/// <see cref="OpenApiUtil"/> OpenApi 枚举工具类
/// </summary>
public static partial class OpenApiUtil
{
    /// <summary>
    /// 写入 OpenApi 文档枚举文件
    /// </summary>
    /// <param name="rootDir"><see cref="string"/> 根目录</param>
    /// <param name="openApiDocument"><see cref="OpenApiDocumentDto"/> 文档Dto</param>
    /// <param name="scriptLanguage"><see cref="ScriptLanguageEnum"/> 脚本语言</param>
    /// <returns></returns>
    internal static async Task<List<ComponentSchemaDto>> WriteOpenApiDocumentEnumFile(string rootDir,
        OpenApiDocumentDto openApiDocument, ScriptLanguageEnum scriptLanguage)
    {
        if (openApiDocument.Components.Schemas == null)
            return null;

        var result = new List<ComponentSchemaDto>();

        try
        {
            // 获取所有有效枚举类型
            var enumTypeList = MAppContext.EffectiveTypes.Where(wh => wh.IsEnum)
                .ToList();
            // 获取文档枚举声明
            var enumSchemas = openApiDocument.Components.Schemas.Where(wh => wh.Value.Enum != null)
                .ToList();

            foreach (var enumSchema in enumSchemas)
            {
                // 根据名称查找枚举类型
                var enumType = enumTypeList.FirstOrDefault(f => f.Name == enumSchema.Key);
                if (enumType == null)
                    continue;

                // 判断是否忽略
                if (Penetrates.OpenApiSettings.IgnoreSchemas.Any(a => a == enumType.Name))
                    continue;

                // 判断是否已经生成
                if (result.Any(a => a.Name == enumType.Name))
                    continue;

                // 通过 Type.GetTypeCode() 获取底层类型的 TypeCode，判断基础类型
                var typeCode = Type.GetTypeCode(enumType);
                var hasLong = typeCode == TypeCode.Int64;

                var enumNames = Enum.GetNames(enumType);
                var enumValues = Enum.GetValues(enumType);
                var enumDetail = new StringBuilder();

                for (var i = 0; i < enumValues.Length; i++)
                {
                    var enumName = enumNames[i];
                    // 获取枚举值
                    var enumValue = Convert.ToInt64(enumValues.GetValue(i))
                        .ToString();
                    if (string.IsNullOrWhiteSpace(enumValue))
                        continue;

                    // 判断是否为 long 类型枚举
                    if (hasLong)
                        enumValue = @$"""{enumValue}""";

                    // 获取枚举描述，如果为空则默认使用Name
                    var enumDescription = enumType.GetField(enumName)
                                              ?.GetCustomAttribute<DescriptionAttribute>(false)
                                              ?.Description
                                          ?? enumName;

                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                    switch (scriptLanguage)
                    {
                        case ScriptLanguageEnum.JavaScript:
                            enumDetail.Append($"""
                                                 /**
                                                  * {enumDescription}
                                                  */
                                                 {enumName}: {enumValue},
                                               """);
                            break;
                        case ScriptLanguageEnum.TypeScript:
                            enumDetail.Append($"""
                                                 /**
                                                  * {enumDescription}
                                                  */
                                                 {enumName} = {enumValue},
                                               """);
                            break;
                    }

                    // 拼接换行符
                    if (i + 1 != enumValues.Length)
                        enumDetail.Append(Environment.NewLine);
                }

                // 获取声明描述
                var schemaDescription = enumSchema.Value.Description?.Replace("\r\n", "\r\n * ")
                                        ?? enumType.GetCustomAttribute<FastEnumAttribute>()
                                            ?.ChName;

                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                switch (scriptLanguage)
                {
                    case ScriptLanguageEnum.JavaScript:
                        // 写入文件
                        await File.WriteAllTextAsync(Path.Combine(rootDir, $"{enumType.Name}.js"), $$"""
                              /**
                               * {{schemaDescription}}
                               */
                              export const {{enumType.Name}} = {
                              {{enumDetail}}
                              };
                              """.Replace("\r\n", "\n"));

                        result.Add(new ComponentSchemaDto
                        {
                            Name = enumType.Name,
                            ImportPath = $$"""import { {{enumType.Name}} } from "@/api/enums/{{enumType.Name}}.js";"""
                        });
                        break;
                    case ScriptLanguageEnum.TypeScript:
                        // 写入文件
                        await File.WriteAllTextAsync(Path.Combine(rootDir, $"{enumType.Name}.ts"), $$"""
                              /**
                               * {{schemaDescription}}
                               */
                              export enum {{enumType.Name}} {
                              {{enumDetail}}
                              }

                              """.Replace("\r\n", "\n"));

                        result.Add(new ComponentSchemaDto
                        {
                            Name = enumType.Name,
                            ImportPath = $$"""import { {{enumType.Name}} } from "@/api/enums/{{enumType.Name}}";"""
                        });
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            var logSb = new StringBuilder();
            logSb.Append("\u001b[41m\u001b[30m");
            logSb.Append("fail");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            logSb.Append(": ");
            logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
            logSb.Append(Environment.NewLine);
            logSb.Append("\u001b[41m\u001b[30m");
            logSb.Append("      ");
            logSb.Append($"写入 {openApiDocument.Url} {scriptLanguage.ToString()} 枚举文件失败...");
            logSb.Append(Environment.NewLine);
            logSb.Append("      ");
            logSb.Append($"{ex}");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            Console.WriteLine(logSb.ToString());
        }

        return result;
    }
}