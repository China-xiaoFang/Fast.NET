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

using System.Text;

// ReSharper disable once CheckNamespace
namespace Fast.OpenApi;

/// <summary>
/// <see cref="OpenApiUtil"/> OpenApi Dto工具类
/// </summary>
public static partial class OpenApiUtil
{
    /// <summary>
    /// 处理声明引用key
    /// </summary>
    /// <param name="refKey"></param>
    /// <param name="refSchemas"></param>
    /// <returns></returns>
    internal static string DisposeSchemaRefKey(string refKey, HashSet<string> refSchemas = null)
    {
        // 获取 $ref 最后一个/后的Name
        refKey = refKey?.Split("/")
            .LastOrDefault();

        if (string.IsNullOrWhiteSpace(refKey))
            return null;

        // 判断是否存在导入声明类型
        if (Penetrates.OpenApiSettings.ImportTypeMappings.Any(a => refKey.StartsWith(a.Name)))
        {
            // 导入声明类型
            foreach (var typeMapping in Penetrates.OpenApiSettings.ImportTypeMappings.Where(wh => refKey.StartsWith(wh.Name)))
            {
                // 截取字符串
                refKey = refKey[typeMapping.Name.Length..];

                // 判断是否为基础类型
                var baseTypeMapping = Penetrates.OpenApiSettings.BaseTypeMappings.FirstOrDefault(f => f.Key == refKey);
                if (baseTypeMapping.Value != null)
                    refKey = baseTypeMapping.Value;
                else if (!Penetrates.OpenApiSettings.ImportTypeMappings.Any(a => refKey.StartsWith(a.Name)))
                    // 最后一个不是基类则添加引用
                    refSchemas?.Add(refKey);

                // 填充字符串
                refKey = string.Format(typeMapping.MappingName, refKey);

                // 判断是否存在引用声明
                if (typeMapping.RefSchema?.Count > 0)
                    refSchemas?.UnionWith(typeMapping.RefSchema);
            }
        }
        else
        {
            // 不存在，直接添加引用
            refSchemas?.Add(refKey);
        }

        return refKey;
    }

    /// <summary>
    /// 处理基础类型
    /// </summary>
    /// <param name="refKey"></param>
    /// <returns></returns>
    internal static string DisposeBaseType(string refKey)
    {
        var baseTypeMapping = Penetrates.OpenApiSettings.BaseTypeMappings.FirstOrDefault(f => f.Key == refKey);
        return baseTypeMapping.Value ?? "any";
    }

    /// <summary>
    /// 生成声明导入
    /// </summary>
    /// <param name="hasWeb"><see cref="bool"/> 是否Web端</param>
    /// <param name="dirName"><see cref="string"/> 文件夹名称</param>
    /// <param name="refSchemas"><see cref="List{String}"/>引用声明</param>
    /// <param name="enumSchemas"><see cref="List{ComponentSchemaDto}"/> 枚举声明</param>
    /// <returns></returns>
    internal static (StringBuilder importSb, HashSet<string> refSchemas) GenerateSchemaImport(bool hasWeb, string dirName,
        HashSet<string> refSchemas, List<ComponentSchemaDto> enumSchemas)
    {
        if (refSchemas == null || refSchemas.Count == 0)
            return (null, []);

        var schemaImport = new StringBuilder();
        var newRefSchemas = new HashSet<string>();

        // 导入声明映射
        var schemaMapping = Penetrates.OpenApiSettings.ImportSchemaMappings.Where(wh => refSchemas.Contains(wh.Name))
            .ToList();
        if (schemaMapping.Count != 0)
        {
            var schemaMappingGroup = schemaMapping.GroupBy(gb => hasWeb ? gb.WebImportPath : gb.MobileImportPath)
                .ToList();
            foreach (var item in schemaMappingGroup)
            {
                schemaImport.AppendLine($$"""
                                          import { {{string.Join(", ", item.Select(sl => sl.Name))}} } from "{{item.Key}}";
                                          """);
            }
        }

        foreach (var refSchema in refSchemas)
        {
            // 判断是否为导入声明映射
            if (schemaMapping.Any(a => a.Name == refSchema))
                continue;

            // 判断是否为枚举
            var enumSchema = enumSchemas.SingleOrDefault(s => s.Name == refSchema);
            if (enumSchema != null)
            {
                schemaImport.AppendLine(enumSchema.ImportPath);
                continue;
            }

            schemaImport.AppendLine($$"""
                                      import { {{refSchema}} } from "./{{dirName}}/{{refSchema}}";
                                      """);
            newRefSchemas.Add(refSchema);
        }

        return (schemaImport, newRefSchemas);
    }

    /// <summary>
    /// 生成 OpenApi 文档声明文件
    /// </summary>
    /// <param name="openApiDocument"><see cref="OpenApiDocumentDto"/> 文档Dto</param>
    /// <param name="scriptLanguage"><see cref="ScriptLanguageEnum"/> 脚本语言</param>
    /// <returns></returns>
    internal static async Task<List<ComponentSchemaDto>> GenerateOpenApiDocumentSchemaFile(OpenApiDocumentDto openApiDocument,
        ScriptLanguageEnum scriptLanguage)
    {
        if (openApiDocument.Components.Schemas == null)
            return null;

        var result = new List<ComponentSchemaDto>();

        // JavaScript 没有类型声明
        if (scriptLanguage == ScriptLanguageEnum.JavaScript)
            return result;

        try
        {
            // 获取文档Dto声明
            var dtoSchemas = openApiDocument.Components.Schemas.Where(wh => wh.Value.Enum == null)
                .ToList();

            foreach (var dtoSchema in dtoSchemas)
            {
                if (string.IsNullOrWhiteSpace(dtoSchema.Key))
                    continue;

                // 判断是否已经生成
                if (result.Any(a => a.Name == dtoSchema.Key))
                    continue;

                // 判断是否为忽略声明
                if (Penetrates.OpenApiSettings.IgnoreSchemas.Contains(dtoSchema.Key))
                    continue;

                // 判断是否为导入声明映射Name
                if (Penetrates.OpenApiSettings.ImportSchemaMappings.Any(a => a.Name.StartsWith(dtoSchema.Key)))
                    continue;

                // 判断是否为导入类型映射Name
                if (Penetrates.OpenApiSettings.ImportTypeMappings.Any(a => a.Name.StartsWith(dtoSchema.Key)))
                    continue;

                // 判断是否为基类
                if (Penetrates.OpenApiSettings.BaseTypeMappings.Any(a => a.Key == dtoSchema.Key))
                    continue;

                var schemaDto = new ComponentSchemaDto {Name = dtoSchema.Key, Content = new StringBuilder(), RefSchemas = []};

                // 获取声明描述
                var schemaDescription = dtoSchema.Value.Description?.Replace("\r\n", "\r\n * ");

                schemaDto.Content.Append($"""
                                          /**
                                           * {schemaDescription}
                                           */
                                          export interface {dtoSchema.Key}
                                          """);

                // 判断是否存在分页
                var hasPaged =
                    Penetrates.OpenApiSettings.PagedSchemaProperties.All(a => dtoSchema.Value.Properties.ContainsKey(a));
                if (hasPaged)
                {
                    schemaDto.Content.Append(" extends PagedInput ");
                    schemaDto.RefSchemas.Add("PagedInput");
                }

                schemaDto.Content.Append(" {");

                // 属性
                foreach (var property in dtoSchema.Value.Properties)
                {
                    // 判断是否为分页属性
                    if (Penetrates.OpenApiSettings.PagedSchemaProperties.Contains(property.Key))
                        continue;

                    // 获取属性描述
                    var propertyDescription = property.Value.Description?.Replace("\r\n\r\n", "\r\n")
                        .Replace("\r\n", "\r\n	 * ");

                    schemaDto.Content.Append(Environment.NewLine);
                    schemaDto.Content.Append($"""
                                                /**
                                                 * {propertyDescription}
                                                 */
                                                {(property.Value.ReadOnly ? "readonly " : "")}{property.Key}?: 
                                              """);

                    // 判断是否为引用属性
                    if (property.Value.Ref != null)
                    {
                        var propertyRefKey = DisposeSchemaRefKey(property.Value.Ref, schemaDto.RefSchemas);
                        schemaDto.Content.Append($"{propertyRefKey};");
                    }
                    // 判断是否为数组
                    else if (property.Value.Type == "array")
                    {
                        var propertyRefKey =
                            // 判断是否为引用类型数组
                            property.Value.Items.Ref != null
                                ? DisposeSchemaRefKey(property.Value.Items.Ref, schemaDto.RefSchemas)
                                : DisposeBaseType(property.Value.Items.Type);
                        schemaDto.Content.Append($"Array<{propertyRefKey}>;");
                    }
                    else
                    {
                        var propertyRefKey = DisposeBaseType(property.Value.Format ?? property.Value.Type);
                        schemaDto.Content.Append($"{propertyRefKey};");
                    }
                }

                schemaDto.Content.Append(Environment.NewLine);
                schemaDto.Content.AppendLine("}");
                schemaDto.Content.Append(Environment.NewLine);

                result.Add(schemaDto);
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
            logSb.Append($"生成 {openApiDocument.Url} {scriptLanguage.ToString()} 声明文件失败...");
            logSb.Append(Environment.NewLine);
            logSb.Append("      ");
            logSb.Append($"{ex}");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            Console.WriteLine(logSb.ToString());
        }

        await Task.CompletedTask;

        return result;
    }

    /// <summary>
    /// 写入 OpenApi 文档声明文件
    /// </summary>
    /// <param name="hasWeb"><see cref="bool"/> 是否Web端</param>
    /// <param name="rootDir"><see cref="string"/> 根目录</param>
    /// <param name="openApiDocument"><see cref="OpenApiDocumentDto"/> 文档Dto</param>
    /// <param name="schemaDto"><see cref="ComponentSchemaDto"/> 声明</param>
    /// <param name="dtoSchemas"><see cref="List{ComponentSchemaDto}"/> Dto声明</param>
    /// <param name="enumSchemas"><see cref="List{ComponentSchemaDto}"/> 枚举声明</param>
    /// <param name="scriptLanguage"><see cref="ScriptLanguageEnum"/> 脚本语言</param>
    /// <returns></returns>
    internal static async Task WriteOpenApiDocumentSchemaFile(bool hasWeb, string rootDir, OpenApiDocumentDto openApiDocument,
        ComponentSchemaDto schemaDto, List<ComponentSchemaDto> dtoSchemas, List<ComponentSchemaDto> enumSchemas,
        ScriptLanguageEnum scriptLanguage)
    {
        // JavaScript 没有类型声明
        if (scriptLanguage == ScriptLanguageEnum.JavaScript)
            return;

        try
        {
            // 排除递归的问题
            var refSchemas = schemaDto.RefSchemas.Where(wh => wh != schemaDto.Name)
                .ToList();

            var schemaImport = new StringBuilder();

            // 导入声明映射
            var schemaMapping = Penetrates.OpenApiSettings.ImportSchemaMappings.Where(wh => refSchemas.Contains(wh.Name))
                .ToList();
            if (schemaMapping.Count != 0)
            {
                var schemaMappingGroup = schemaMapping.GroupBy(gb => hasWeb ? gb.WebImportPath : gb.MobileImportPath)
                    .ToList();
                foreach (var item in schemaMappingGroup)
                {
                    schemaImport.AppendLine($$"""
                                              import { {{string.Join(", ", item.Select(sl => sl.Name))}} } from "{{item.Key}}";
                                              """);
                }
            }

            foreach (var refSchema in refSchemas)
            {
                // 判断是否为导入声明映射
                if (schemaMapping.Any(a => a.Name == refSchema))
                    continue;

                // 判断是否为枚举
                var enumSchema = enumSchemas.SingleOrDefault(s => s.Name == refSchema);
                if (enumSchema != null)
                {
                    schemaImport.AppendLine(enumSchema.ImportPath);
                    continue;
                }

                schemaImport.AppendLine($$"""
                                          import { {{refSchema}} } from "./{{refSchema}}";
                                          """);
            }

            if (schemaImport.Length > 0)
                // 写入文件
                await File.WriteAllTextAsync(Path.Combine(rootDir, $"{schemaDto.Name}.ts"), $"""
                     {schemaImport}
                     {schemaDto.Content}
                     """);
            else
                // 写入文件
                await File.WriteAllTextAsync(Path.Combine(rootDir, $"{schemaDto.Name}.ts"), $"{schemaDto.Content}");


            // 处理引用文件
            foreach (var refSchema in refSchemas)
            {
                // 判断是否为导入声明映射
                if (schemaMapping.Any(a => a.Name == refSchema))
                    continue;

                // 判断是否为枚举声明
                if (enumSchemas.Any(a => a.Name == refSchema))
                    continue;

                // 从声明集合中查找
                var childrenSchemaDto = dtoSchemas.Single(s => s.Name == refSchema);
                await WriteOpenApiDocumentSchemaFile(hasWeb, rootDir, openApiDocument, childrenSchemaDto, dtoSchemas, enumSchemas,
                    scriptLanguage);
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
            logSb.Append($"写入 {openApiDocument.Url} {scriptLanguage.ToString()} 声明文件失败...");
            logSb.Append(Environment.NewLine);
            logSb.Append("      ");
            logSb.Append($"{ex}");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            Console.WriteLine(logSb.ToString());
        }
    }
}