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

using System.Globalization;
using System.Text.Json;

// ReSharper disable once CheckNamespace
namespace Fast.NET.Core;

/// <summary>
/// <see cref="JsonUtils"/> Json 工具类
/// </summary>
[SuppressSniffer]
public static class JsonUtils
{
    /// <summary>
    /// 读取 Json 文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static IDictionary<string, string> ReadJsonFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentNullException(path);
        }

        // 读取文件内容
        using var streamReader = new StreamReader(path);
        var json = streamReader.ReadToEnd();

        // 解析 Json 字符串
        return ParseJson(json);
    }

    /// <summary>
    /// 解析 Json 字符串
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static IDictionary<string, string> ParseJson(string json)
    {
        var options = new JsonDocumentOptions
        {
            // 允许尾随逗号
            AllowTrailingCommas = true,
            // 忽略注释
            CommentHandling = JsonCommentHandling.Skip
        };

        var dictionary = new Dictionary<string, string>();

        using var doc = JsonDocument.Parse(json, options);
        var root = doc.RootElement;

        VisitJsonElement(root, dictionary);

        return dictionary;
    }

    /// <summary>
    /// 处理
    /// </summary>
    /// <param name="root"></param>
    /// <param name="dictionary"></param>
    /// <param name="currentPath"></param>
    private static void VisitJsonElement(JsonElement root, IDictionary<string, string> dictionary, string currentPath = "")
    {
        // 判断是否为对象
        switch (root.ValueKind)
        {
            case JsonValueKind.Undefined:
                break;
            case JsonValueKind.Null:
            {
                dictionary.TryAdd(currentPath, null);
            }
                break;
            // 对象
            case JsonValueKind.Object:
            {
                foreach (var property in root.EnumerateObject())
                {
                    var newPath = string.IsNullOrWhiteSpace(currentPath) ? property.Name : $"{currentPath}:{property.Name}";
                    VisitJsonElement(property.Value, dictionary, newPath);
                }
            }
                break;
            // 数组
            case JsonValueKind.Array:
            {
                for (var index = 0; index < root.GetArrayLength(); index++)
                {
                    var newPath = string.IsNullOrWhiteSpace(currentPath) ? $"{index}" : $"{currentPath}:{index}";
                    VisitJsonElement(root[index], dictionary, newPath);
                }
            }
                break;
            // 字符串
            case JsonValueKind.String:
            {
                // 去除转义字符
                var unescapedValue = root.GetString()
                    ?.Replace("\\", "");
                dictionary.TryAdd(currentPath, unescapedValue);
            }
                break;
            case JsonValueKind.Number:
            {
                if (root.TryGetInt32(out var intVal))
                {
                    dictionary.TryAdd(currentPath, intVal.ToString());
                }
                else if (root.TryGetDouble(out var doubleVal))
                {
                    dictionary.TryAdd(currentPath, doubleVal.ToString(CultureInfo.InvariantCulture));
                }
                else if (root.TryGetInt64(out var longVal))
                {
                    dictionary.TryAdd(currentPath, longVal.ToString());
                }
                else
                {
                    dictionary.TryAdd(currentPath, root.GetRawText());
                }
            }
                break;
            case JsonValueKind.True:
            {
                dictionary.TryAdd(currentPath, true.ToString());
            }
                break;
            case JsonValueKind.False:
            {
                dictionary.TryAdd(currentPath, false.ToString());
            }
                break;
        }
    }
}