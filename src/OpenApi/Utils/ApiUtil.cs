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
/// OpenAPI API 工具类
/// </summary>
public static partial class OpenApiUtil
{
    /// <summary>
    /// 处理请求行为
    /// </summary>
    /// <param name="requestAction">请求行为</param>
    /// <returns>处理请求行为</returns>
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
            case HttpRequestActionEnum.Submit:
                return "submit";
            case HttpRequestActionEnum.Upload:
                return "upload";
            case HttpRequestActionEnum.Download:
                return "download";
            case HttpRequestActionEnum.Export:
                return "export";
            case HttpRequestActionEnum.Import:
                return "import";
            case HttpRequestActionEnum.Callback:
                return "callback";
            case HttpRequestActionEnum.None:
            case HttpRequestActionEnum.Notify:
            case HttpRequestActionEnum.Other:
            default:
                return "other";
        }
    }

    /// <summary>
    /// 写入 OpenAPI 文档 API 文件
    /// </summary>
    /// <param name="rootDir">根目录</param>
    /// <param name="hasWeb">是否为 Web 端</param>
    /// <param name="apiDescriptionGroupCollectionProvider">接口描述提供程序</param>
    /// <param name="openApiDocument">OpenAPI 文档</param>
    /// <param name="dtoSchemas">DTO 声明</param>
    /// <param name="enumSchemas">枚举声明</param>
    /// <param name="scriptLanguage">脚本语言</param>
    /// <returns>表示异步写入 OpenAPI 文档 API 文件的任务</returns>
    internal static async Task WriteOpenApiDocumentApiFile(string rootDir, bool hasWeb,
        IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider, OpenApiDocumentDto openApiDocument,
        List<ComponentSchemaDto> dtoSchemas, List<ComponentSchemaDto> enumSchemas, ScriptLanguageEnum scriptLanguage)
    {
        try
        {
            // 获取所有 Tag
            var tagList = openApiDocument.Paths.Where(wh => wh.Value.Tag != null)
                .Select(sl => sl.Value.Tag)
                .Distinct()
                .ToList();

            // 获取所有接口描述
            var apiDescriptions = apiDescriptionGroupCollectionProvider.ApiDescriptionGroups.Items.SelectMany(sl => sl.Items)
                .ToList();

            foreach (var tag in tagList)
            {
                // 处理 xxx/xxx 这种 tag
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
                // 仅在当前模块包含上传接口时生成 Axios 上传进度类型导入。
                var hasUpload = false;

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
                    var responseType = DisposeSchemaType(apiInfo.Method.Responses?.Code200?.Content?.Json?.Schema, refSchemas);

                    var methodInfo = apiInfo.Method;
                    var isFormData = methodInfo?.RequestBody?.Content?.FormData != null;
                    var isMobileUpload = !hasWeb && isFormData;
                    if (isFormData)
                    {
                        hasUpload = true;
                    }

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

                    if (isFormData)
                    {
                        requestDataType = "FormData";
                    }
                    else if (methodInfo?.RequestBody?.Content?.Json?.Schema != null)
                    {
                        requestDataType = DisposeSchemaType(methodInfo.RequestBody.Content.Json.Schema, refSchemas);
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
                                    var parameterType = DisposeSchemaType(parameter?.Schema, refSchemas) ?? "unknown";
                                    requestParam += $"{parameter.Name}: {parameterType}, ";

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

                        // 处理可能存在 URL 参数和 Body 参数的情况
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
                                if (isMobileUpload)
                                {
                                    // 移动端使用 filePath 参数，并按字符串处理
                                    contentSb.Append("filePath");
                                }
                                else
                                {
                                    contentSb.Append("data");
                                }

                                break;
                            case ScriptLanguageEnum.TypeScript:
                                if (isMobileUpload)
                                {
                                    // 移动端使用 filePath 参数，并按字符串处理
                                    contentSb.Append("filePath: string");
                                }
                                else
                                {
                                    contentSb.Append($"data: {requestDataType}");
                                }

                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(scriptLanguage), scriptLanguage, null);
                        }

                        if (isFormData)
                        {
                            contentSb.Append(scriptLanguage == ScriptLanguageEnum.TypeScript
                                ? ", onUploadProgress?: (progressEvent: AxiosProgressEvent) => void"
                                : ", onUploadProgress");
                        }
                    }

                    contentSb.Append(scriptLanguage == ScriptLanguageEnum.TypeScript
                        ? $$"""
                            ): Promise<{{(string.IsNullOrWhiteSpace(responseType) ? "unknown" : responseType)}}> {
                                return axiosUtil.request
                            """
                        : """
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

                    if (isMobileUpload)
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
                        if (isMobileUpload)
                        {
                            // 移动端使用 filePath 参数，并按字符串处理
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

                        if (isFormData)
                        {
                            contentSb.Append("      onUploadProgress,");
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
                        await File.WriteAllTextAsync(Path.Combine(apiFileDir, "index.js"), FormatScriptContent($$"""
                              import { axiosUtil } from "@fast-china/axios";

                              /**
                               * {{tagDescription}}Api
                               */
                              export const {{tagName}}Api = {
                              {{contentSb}}
                              };

                              """));
                        break;
                    case ScriptLanguageEnum.TypeScript:
                        // 生成 import
                        var (externalImports, schemaImports, newRefSchemas) = GenerateSchemaImport(hasWeb, "models", refSchemas,
                            enumSchemas);
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

                        var imports = new List<string> {"import { axiosUtil } from \"@fast-china/axios\";"};
                        if (hasUpload)
                        {
                            imports.Add("import type { AxiosProgressEvent } from \"axios\";");
                        }

                        imports.AddRange(externalImports);
                        imports.AddRange(schemaImports);
                        await File.WriteAllTextAsync(Path.Combine(apiFileDir, "index.ts"), FormatScriptContent($$"""
                              {{string.Join(Environment.NewLine, imports)}}

                              /**
                               * {{tagDescription}}Api
                               */
                              export const {{tagName}}Api = {
                              {{contentSb}}
                              };

                              """));

                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(scriptLanguage), scriptLanguage, null);
                }
            }
        }
        catch (Exception ex)
        {
            var useColor = !Console.IsOutputRedirected;
            var logSb = new StringBuilder();
            if (useColor)
                logSb.Append("\u001b[41m\u001b[30m");
            logSb.Append("fail");
            if (useColor)
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            logSb.Append(": ");
            logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
            logSb.Append(Environment.NewLine);
            if (useColor)
                logSb.Append("\u001b[41m\u001b[30m");
            logSb.Append("      ");
            logSb.Append($"写入 {openApiDocument.Url} {scriptLanguage.ToString()} Api文件失败...");
            logSb.Append(Environment.NewLine);
            logSb.Append("      ");
            logSb.Append($"{ex}");
            if (useColor)
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            Console.WriteLine(logSb.ToString());
        }
    }

    /// <summary>
    /// 格式化生成的脚本内容
    /// </summary>
    /// <param name="content">脚本内容</param>
    /// <returns>使用 Tab 缩进和 LF 换行符的脚本内容</returns>
    internal static string FormatScriptContent(string content)
    {
        var normalizedContent = content.Replace("\r\n", "\n")
            .Replace('\r', '\n');
        return Regex.Replace(normalizedContent, @"(?m)^(?: {2})+", match => new string('\t', match.Value.Length / 2));
    }
}
