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

using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

// ReSharper disable once CheckNamespace
namespace Fast.OpenApi;

/// <summary>
/// <see cref="OpenApiUtil"/> OpenApi 请求工具类
/// </summary>
public static partial class OpenApiUtil
{
    /// <summary>
    /// 获取 OpenApi 文档信息
    /// </summary>
    /// <param name="documentUrl"><see cref="string"/> 文档地址</param>
    /// <returns></returns>
    internal static async Task<OpenApiDocumentDto> GetOpenApiDocument(string documentUrl)
    {
        try
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(documentUrl);
            response.EnsureSuccessStatusCode();
            var jsonContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OpenApiDocumentDto>(jsonContent, new JsonSerializerOptions
            {
                // 忽略只有在 .NET 6 才会存在的循环引用问题
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                // 解决 JSON 乱码问题
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                // 允许尾随逗号
                AllowTrailingCommas = true,
                // 忽略注释
                ReadCommentHandling = JsonCommentHandling.Skip,
                // 允许数字带引号
                NumberHandling = JsonNumberHandling.AllowReadingFromString,
                // 默认不区分大小写匹配
                PropertyNameCaseInsensitive = true
            });

            result.Url = documentUrl;

            return result;
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            // 404，Swagger 文档不存在
            return null;
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
            logSb.Append($"读取 {documentUrl} 文档信息失败...");
            logSb.Append(Environment.NewLine);
            logSb.Append("      ");
            logSb.Append($"{ex}");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            Console.WriteLine(logSb.ToString());
        }

        return null;
    }
}