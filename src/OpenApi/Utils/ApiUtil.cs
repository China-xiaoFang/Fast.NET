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
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace Fast.OpenApi;

/// <summary>
/// <see cref="OpenApiUtil"/> OpenApi Api工具类
/// </summary>
public static partial class OpenApiUtil
{
    /// <summary>
    /// 处理请求行为
    /// </summary>
    /// <param name="requestAction"><see cref="HttpRequestActionEnum"/> 请求行为</param>
    /// <returns></returns>
    internal static string DisposeRequestAction(HttpRequestActionEnum requestAction)
    {
        switch (requestAction)
        {
            case HttpRequestActionEnum.Auth:
                return "auth";
            case HttpRequestActionEnum.Paged:
            case HttpRequestActionEnum.Query:
                return "query";
            case HttpRequestActionEnum.Add:
                return "add";
            case HttpRequestActionEnum.Edit:
                return "edit";
            case HttpRequestActionEnum.Delete:
                return "delete";
            case HttpRequestActionEnum.Download:
                return "download";
            case HttpRequestActionEnum.Upload:
                return "upload";
            case HttpRequestActionEnum.Export:
                return "export";
            case HttpRequestActionEnum.Import:
                return "import";
            case HttpRequestActionEnum.None:
            case HttpRequestActionEnum.Submit:
            case HttpRequestActionEnum.Notify:
            case HttpRequestActionEnum.Callback:
            case HttpRequestActionEnum.Other:
            default:
                return "other";
        }
    }

    /// <summary>
    /// 写入 OpenApi 文档 Api 文件
    /// </summary>
    /// <param name="rootDir"><see cref="string"/> 根目录</param>
    /// <param name="hasWeb"><see cref="bool"/> 是否为Web端</param>
    /// <param name="apiDescriptionGroupCollectionProvider"><see cref="IApiDescriptionGroupCollectionProvider"/> 接口描述提供程序</param>
    /// <param name="openApiDocument"><see cref="OpenApiDocumentDto"/> 文档Dto</param>
    /// <param name="dtoSchemas"><see cref="List{ComponentSchemaDto}"/> Dto声明</param>
    /// <param name="enumSchemas"><see cref="List{ComponentSchemaDto}"/> 枚举声明</param>
    /// <param name="scriptLanguage"><see cref="ScriptLanguageEnum"/> 脚本语言</param>
    /// <returns></returns>
    internal static async Task WriteOpenApiDocumentApiFile(string rootDir, bool hasWeb,
        IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider, OpenApiDocumentDto openApiDocument,
        List<ComponentSchemaDto> dtoSchemas, List<ComponentSchemaDto> enumSchemas, ScriptLanguageEnum scriptLanguage)
    {
        try
        {
            // 获取所有Tag
            var tagList = openApiDocument.Paths.Where(wh => wh.Value.Tag != null)
                .Select(sl => sl.Value.Tag)
                .Distinct()
                .Select(sl =>
                {
                    var split = sl.Split("/", StringSplitOptions.RemoveEmptyEntries);
                    if (split.Length == 1)
                        return split[0];

                    return split[0]
                           + string.Concat(split.Skip(1)
                               .Select(s => char.ToUpperInvariant(s[0]) + s[1..]));
                })
                .ToList();

            // 获取所有接口描述
            var apiDescriptions = apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items.SelectMany(sl => sl.Items)
                .ToList();

            foreach (var tag in tagList)
            {
                // 处理 xxx/xxx 这种tag
                var tagName = tag;
                var tagSplit = tag.Split("/", StringSplitOptions.RemoveEmptyEntries);
                if (tagSplit.Length > 1)
                {
                    tagName = tagSplit[0]
                              + string.Concat(tagSplit.Skip(1)
                                  .Select(s => char.ToUpperInvariant(s[0]) + s[1..]));
                }

                // 创建 api 文件夹
                var apiFileDir = Path.Combine(rootDir, tagName);
                Directory.CreateDirectory(apiFileDir);

                // 获取当前 tag 下所有的接口
                var curPaths = openApiDocument.Paths.Where(wh => wh.Value.Tag == tag)
                    .ToList();

                // 模块描述
                var tagDescription = openApiDocument.Tags?.SingleOrDefault(s => s.Name == tag)
                    ?.Description;

                var contentSb = new StringBuilder();
                // 引用声明
                var refSchemas = new HashSet<string>();

                for (var i = 0; i < curPaths.Count; i++)
                {
                    var apiInfo = curPaths[i].Value;
                    var apiName = curPaths[i].Key;
                    // 判断是否为路由格式的接口，如果是则不生成接口文档
                    if (Regex.IsMatch(apiName, @"\/\{[a-zA-Z0-9_]+\}"))
                    {
                        continue;
                    }

                    var apiFuncName = apiName.Split("/")
                        .LastOrDefault();

                    // 获取接口描述
                    var apiDescription = apiDescriptions.Single(s => $"/{s.RelativePath}" == apiName);

                    // 获取请求特性
                    var apiInfoAttribute = apiDescription.ActionDescriptor.EndpointMetadata.OfType<ApiInfoAttribute>()
                        .FirstOrDefault();

                    // 获取请求类型
                    var apiActionEnum = apiInfoAttribute?.Action ?? HttpRequestActionEnum.Other;
                    var apiAction = DisposeRequestAction(apiActionEnum);

                    // 响应数据类型
                    var responseType = DisposeSchemaRefKey(apiInfo.Method.Responses?.Code200?.Content?.Json?.Schema?.Ref,
                        refSchemas);

                    var methodInfo = apiInfo.Method;

                    // 获取接口名称（注释）
                    var apiSummary = apiInfoAttribute?.Name ?? methodInfo.Summary;

                    contentSb.Append($"""
                                        /**
                                         * {apiSummary}
                                         */
                                        {apiFuncName}(
                                      """);

                    // 请求参数
                    var requestParam = "";
                    var requestParamSb = new StringBuilder();

                    // 请求体类型
                    var requestDataType = "";

                    // 判断是否为 FormData 文件上传
                    if (methodInfo?.RequestBody?.Content?.FormData != null)
                    {
                        requestDataType = "FormData";
                    }
                    // 判断是否为引用类型
                    else if (methodInfo?.RequestBody?.Content?.Json?.Schema?.Ref != null)
                    {
                        requestDataType = DisposeSchemaRefKey(methodInfo.RequestBody.Content.Json.Schema.Ref, refSchemas);
                    }
                    // 判断是否为数组引用类型
                    else if (methodInfo?.RequestBody?.Content?.Json?.Schema?.Items?.Ref != null)
                    {
                        requestDataType = DisposeSchemaRefKey(methodInfo.RequestBody.Content.Json.Schema.Items.Ref, refSchemas);
                    }

                    if (methodInfo?.Parameters != null)
                    {
                        for (var j = 0; j < methodInfo.Parameters.Count; j++)
                        {
                            var parameter = methodInfo.Parameters[j];

                            switch (scriptLanguage)
                            {
                                case ScriptLanguageEnum.JavaScript:
                                    requestParam += $"{parameter.Name}, ";
                                    break;
                                case ScriptLanguageEnum.TypeScript:
                                    // 判断是否为引用类型
                                    if (parameter?.Schema?.Ref != null)
                                    {
                                        var schemaRefKey = DisposeSchemaRefKey(parameter.Schema.Ref, refSchemas);
                                        requestParam += $"{parameter.Name}: {schemaRefKey}, ";
                                    }
                                    else
                                    {
                                        var baseType = DisposeBaseType(parameter?.Schema?.Type);
                                        requestParam += $"{parameter.Name}: {baseType}, ";
                                    }

                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(scriptLanguage), scriptLanguage, null);
                            }

                            requestParamSb.Append($"        {parameter.Name},");

                            if (j + 1 != methodInfo.Parameters.Count)
                            {
                                requestParamSb.Append(Environment.NewLine);
                            }
                        }

                        // 处理可能存在Url参数和Body参数的情况
                        if (string.IsNullOrWhiteSpace(requestDataType))
                        {
                            requestParam = requestParam.TrimEnd(' ')
                                .TrimEnd(',');
                        }

                        contentSb.Append(requestParam);
                    }

                    if (!string.IsNullOrWhiteSpace(requestDataType))
                    {
                        switch (scriptLanguage)
                        {
                            case ScriptLanguageEnum.JavaScript:
                                // 判断是否为 FormData 文件上传
                                if (!hasWeb && methodInfo?.RequestBody?.Content?.FormData != null)
                                {
                                    // 移动端使用的是 filePath 参数，默认 String 类型
                                    contentSb.Append("filePath");
                                }
                                else
                                {
                                    contentSb.Append("data");
                                }

                                break;
                            case ScriptLanguageEnum.TypeScript:
                                // 判断是否为 FormData 文件上传
                                if (!hasWeb && methodInfo?.RequestBody?.Content?.FormData != null)
                                {
                                    // 移动端使用的是 filePath 参数，默认 String 类型
                                    contentSb.Append("filePath: string");
                                }
                                else
                                {
                                    contentSb.Append($"data: {requestDataType}");
                                }

                                // 判断是否为数组
                                if (methodInfo?.RequestBody?.Content?.Json?.Schema?.Type == "array")
                                {
                                    contentSb.Append("[]");
                                }

                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(scriptLanguage), scriptLanguage, null);
                        }
                    }

                    contentSb.Append("""
                                     ) {
                                         return axiosUtil.request
                                     """);

                    if (!string.IsNullOrWhiteSpace(responseType) && scriptLanguage == ScriptLanguageEnum.TypeScript)
                    {
                        contentSb.Append($"<{responseType}>");
                    }

                    contentSb.Append($$"""
                                       ({
                                             url: "{{apiName}}",
                                       """);
                    contentSb.Append(Environment.NewLine);

                    // 判断是否为 FormData 文件上传
                    if (!hasWeb && methodInfo?.RequestBody?.Content?.FormData != null)
                    {
                        // 移动端默认使用 upload
                        contentSb.Append("""
                                               method: "upload",
                                         """);
                    }
                    else
                    {
                        contentSb.Append($$"""
                                                 method: "{{apiDescription.HttpMethod.ToLowerInvariant()}}",
                                           """);
                    }

                    contentSb.Append(Environment.NewLine);

                    if (requestParamSb.Length > 0)
                    {
                        contentSb.Append($$"""
                                                 params: {
                                           {{requestParamSb}}
                                                 },
                                           """);
                        contentSb.Append(Environment.NewLine);
                    }

                    if (!string.IsNullOrWhiteSpace(requestDataType))
                    {
                        // 判断是否为 FormData 文件上传
                        if (!hasWeb && methodInfo?.RequestBody?.Content?.FormData != null)
                        {
                            // 移动端使用的是 filePath 参数，默认 String 类型
                            contentSb.Append("""
                                                   name: "file",
                                             """);
                            contentSb.Append(Environment.NewLine);
                            contentSb.Append("      filePath,");
                            contentSb.Append(Environment.NewLine);
                        }
                        else
                        {
                            contentSb.Append("      data,");
                            contentSb.Append(Environment.NewLine);
                        }
                    }

                    // 处理下载和导出
                    if (apiActionEnum is HttpRequestActionEnum.Download or HttpRequestActionEnum.Export)
                    {
                        contentSb.Append("""
                                               responseType: "blob",
                                               autoDownloadFile: true,
                                         """);
                        contentSb.Append(Environment.NewLine);
                    }

                    // 处理文件上传重复请求
                    if (apiActionEnum is HttpRequestActionEnum.Upload)
                    {
                        contentSb.Append("""
                                               cancelDuplicateRequest: false,
                                         """);
                        contentSb.Append(Environment.NewLine);
                    }

                    contentSb.Append($$"""
                                             requestType: "{{apiAction}}",
                                           });
                                         },
                                       """);

                    if (i + 1 != curPaths.Count)
                    {
                        contentSb.Append(Environment.NewLine);
                    }
                }

                switch (scriptLanguage)
                {
                    case ScriptLanguageEnum.JavaScript:
                        // 写入文件
                        await File.WriteAllTextAsync(Path.Combine(apiFileDir, "index.js"), $$"""
                              import { axiosUtil } from "@fast-china/axios";

                              /**
                               * {{tagDescription}}Api
                               */
                              export const {{tagName}}Api = {
                              {{contentSb}}
                              };

                              """.Replace("\r\n", "\n"));
                        break;
                    case ScriptLanguageEnum.TypeScript:
                        // 生成 import
                        var (schemaImport, newRefSchemas) = GenerateSchemaImport(hasWeb, "models", refSchemas, enumSchemas);
                        if (newRefSchemas.Count > 0)
                        {
                            // 创建 model 文件
                            var importFileDir = Path.Combine(apiFileDir, "models");
                            Directory.CreateDirectory(importFileDir);

                            foreach (var dtoSchema in dtoSchemas.Where(wh => newRefSchemas.Contains(wh.Name))
                                         .ToList())
                            {
                                // 写入 import 文件
                                await WriteOpenApiDocumentSchemaFile(hasWeb, importFileDir, openApiDocument, dtoSchema,
                                    dtoSchemas, enumSchemas, ScriptLanguageEnum.TypeScript);
                            }
                        }

                        if (schemaImport?.Length > 0)
                        {
                            // 写入文件
                            await File.WriteAllTextAsync(Path.Combine(apiFileDir, "index.ts"), $$"""
                                  import { axiosUtil } from "@fast-china/axios";
                                  {{schemaImport}}
                                  /**
                                   * {{tagDescription}}Api
                                   */
                                  export const {{tag}}Api = {
                                  {{contentSb}}
                                  };

                                  """.Replace("\r\n", "\n"));
                        }
                        else
                        {
                            // 写入文件
                            await File.WriteAllTextAsync(Path.Combine(apiFileDir, "index.ts"), $$"""
                                  import { axiosUtil } from "@fast-china/axios";

                                  /**
                                   * {{tagDescription}}Api
                                   */
                                  export const {{tagName}}Api = {
                                  {{contentSb}}
                                  };

                                  """.Replace("\r\n", "\n"));
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(scriptLanguage), scriptLanguage, null);
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
            logSb.Append($"写入 {openApiDocument.Url} {scriptLanguage.ToString()} Api文件失败...");
            logSb.Append(Environment.NewLine);
            logSb.Append("      ");
            logSb.Append($"{ex}");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            Console.WriteLine(logSb.ToString());
        }
    }
}