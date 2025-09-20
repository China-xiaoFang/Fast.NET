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
using Microsoft.AspNetCore.Mvc.ApiExplorer;

// ReSharper disable once CheckNamespace
namespace Fast.OpenApi;

/// <summary>
/// <see cref="OpenApiUtil"/> OpenApi 工具类
/// </summary>
public static partial class OpenApiUtil
{
    /// <summary>
    /// 生成 OpenApi 文档资源
    /// </summary>
    /// <param name="address"><see cref="string"/> 文档地址；<see href="http://127.0.0.1:38080"/></param>
    /// <param name="apiDescriptionGroupCollectionProvider"><see cref="IApiDescriptionGroupCollectionProvider"/> 接口描述提供程序</param>
    /// <param name="groupList"><see cref="List{T}"/> 分组集合；默认使用 ["All Groups"]</param>
    /// <returns></returns>
    public static async Task GenerateOpenApi(string address,
        IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider, List<string> groupList = null)
    {
        try
        {
            {
                var logSb = new StringBuilder();
                logSb.Append("\u001b[40m\u001b[1m\u001b[32m");
                logSb.Append("info");
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                logSb.Append(": ");
                logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                logSb.Append(Environment.NewLine);
                logSb.Append("\u001b[40m\u001b[90m");
                logSb.Append("      ");
                logSb.Append("开始生成 Open Api 文件...");
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                Console.WriteLine(logSb.ToString());
            }

            if (groupList == null)
            {
                // 为空则使用全部分组
                groupList = ["All Groups"];
            }
            // 增加默认分组
            else if (groupList.All(a => a != "Default"))
            {
                groupList.Add("Default");
            }

            // 根目录
            var rootDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fast.OpenApi");
            if (Directory.Exists(rootDir))
            {
                // 每次确保都是最新的
                Directory.Delete(rootDir, true);
            }

            Directory.CreateDirectory(rootDir);

            foreach (var group in groupList)
            {
                // 获取主机和端口信息
                var uri = new Uri(address);

                // 获取文档地址
                var url = $"{address.TrimEnd('/')}/swagger/{group}/swagger.json";

                // 获取文档信息
                var openApiDocument = await GetOpenApiDocument(url);
                if (openApiDocument == null)
                    continue;

                // JavaScript
                await GenerateOpenApi(apiDescriptionGroupCollectionProvider, openApiDocument, rootDir, group, uri, true,
                    ScriptLanguageEnum.JavaScript);
                await GenerateOpenApi(apiDescriptionGroupCollectionProvider, openApiDocument, rootDir, group, uri, false,
                    ScriptLanguageEnum.JavaScript);

                // TypeScript
                await GenerateOpenApi(apiDescriptionGroupCollectionProvider, openApiDocument, rootDir, group, uri, true,
                    ScriptLanguageEnum.TypeScript);
                await GenerateOpenApi(apiDescriptionGroupCollectionProvider, openApiDocument, rootDir, group, uri, false,
                    ScriptLanguageEnum.TypeScript);
            }

            {
                var logSb = new StringBuilder();
                logSb.Append("\u001b[40m\u001b[1m\u001b[32m");
                logSb.Append("info");
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                logSb.Append(": ");
                logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
                logSb.Append(Environment.NewLine);
                logSb.Append("\u001b[40m\u001b[90m");
                logSb.Append("      ");
                logSb.Append("生成 Open Api 文件成功。");
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
                Console.WriteLine(logSb.ToString());
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
            logSb.Append("生成 Open Api 文件失败...");
            logSb.Append(Environment.NewLine);
            logSb.Append("      ");
            logSb.Append($"{ex}");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            Console.WriteLine(logSb.ToString());
            throw;
        }
    }

    /// <summary>
    /// 生成 OpenApi 文档资源
    /// </summary>
    /// <param name="apiDescriptionGroupCollectionProvider"><see cref="IApiDescriptionGroupCollectionProvider"/> 接口描述提供程序</param>
    /// <param name="openApiDocument"><see cref="OpenApiDocumentDto"/> 文档信息</param>
    /// <param name="rootDir"><see cref="string"/> 根目录</param>
    /// <param name="group"><see cref="string"/> 分组</param>
    /// <param name="uri"><see cref="Uri"/> 地址</param>
    /// <param name="hasWeb"><see cref="bool"/> 是否为Web端</param>
    /// <param name="scriptLanguage"><see cref="ScriptLanguageEnum"/> 脚本语言</param>
    /// <returns></returns>
    internal static async Task GenerateOpenApi(IApiDescriptionGroupCollectionProvider apiDescriptionGroupCollectionProvider,
        OpenApiDocumentDto openApiDocument, string rootDir, string group, Uri uri, bool hasWeb, ScriptLanguageEnum scriptLanguage)
    {
        // 判断是否存在路由
        if (openApiDocument.Paths.Count == 0)
            return;

        // 当前文档地址
        var curRootDir = Path.Combine(rootDir,
            $"{uri.Host}_{uri.Port}_{(hasWeb ? "Web" : "Mobile")}_{scriptLanguage.ToString()}");
        Directory.CreateDirectory(curRootDir);

        // 枚举文件
        var enumRootDir = Path.Combine(curRootDir, "enums");
        Directory.CreateDirectory(enumRootDir);

        // Api文件
        //var apiRootDir = Path.Combine(curRootDir, "services", group);
        var apiRootDir = Path.Combine(curRootDir, "services");
        Directory.CreateDirectory(apiRootDir);

        // 写入枚举
        var enumSchemas = await WriteOpenApiDocumentEnumFile(enumRootDir, openApiDocument, scriptLanguage);

        // 生成Dto
        var dtoSchemas = await GenerateOpenApiDocumentSchemaFile(openApiDocument, scriptLanguage);

        // 写入Api
        await WriteOpenApiDocumentApiFile(apiRootDir, hasWeb, apiDescriptionGroupCollectionProvider, openApiDocument, dtoSchemas,
            enumSchemas, scriptLanguage);
    }
}