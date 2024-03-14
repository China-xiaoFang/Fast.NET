// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using System.Globalization;
using System.Text.Json;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

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
        if (string.IsNullOrEmpty(path))
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
            CommentHandling = JsonCommentHandling.Skip,
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
                    var newPath = string.IsNullOrEmpty(currentPath) ? property.Name : $"{currentPath}:{property.Name}";
                    VisitJsonElement(property.Value, dictionary, newPath);
                }
            }
                break;
            // 数组
            case JsonValueKind.Array:
            {
                for (var index = 0; index < root.GetArrayLength(); index++)
                {
                    var newPath = string.IsNullOrEmpty(currentPath) ? $"{index}" : $"{currentPath}:{index}";
                    VisitJsonElement(root[index], dictionary, newPath);
                }
            }
                break;
            // 字符串
            case JsonValueKind.String:
            {
                // 去除转义字符
                var unescapedValue = root.GetString()?.Replace("\\", "");
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