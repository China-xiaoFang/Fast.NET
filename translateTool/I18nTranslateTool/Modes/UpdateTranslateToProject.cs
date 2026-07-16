// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
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

using System.Text;
using System.Text.Json;
using Fast.IaaS;
using MiniExcelLibs;

namespace I18nTranslateTool.Modes;

/// <summary>
/// <see cref="UpdateTranslateToProject"/> 更新翻译文案到项目
/// </summary>
internal static class UpdateTranslateToProject
{
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="projectPath"><see cref="string"/> 项目路径</param>
    /// <param name="projectName"><see cref="string"/> 项目名称</param>
    /// <param name="translateFilePath"><see cref="string"/> 翻译文件存放位置</param>
    internal static void Run(string projectPath, string projectName, string translateFilePath)
    {
        // 组装前端项目文件夹 src\lang 的路径
        var langPath = Path.Combine(projectPath, "src", "lang");

        // 获取 lang 文件夹中的语言，只获取文件夹名称
        var langList = Directory.GetDirectories(langPath, "*", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileName)
            .Where(wh => wh != "common")
            .ToList();

        // 获取翻译文件目录下最后一次更新的 xlsx 文件，这里使用文件名称排序的方式
        var excelFile = Directory.GetFiles(translateFilePath, "*.xlsx", SearchOption.TopDirectoryOnly)
            .Select(sl => new FileInfo(sl)).MaxBy(ob => ob.Name);
        if (excelFile == null)
            throw new FileNotFoundException($"目录“{translateFilePath}”中没有可用的 xlsx 翻译文件。");

        // 组装读取Excel的字典
        var excelDictionary = new List<IDictionary<string, string>>();

        // 读取 Excel文件
        foreach (IDictionary<string, object> row in MiniExcel.Query(excelFile.FullName, useHeaderRow: true))
        {
            // 先组装默认数据
            var excelRow = new Dictionary<string, string>
            {
                {"页面文件路径", GetRowValue(row, "页面文件路径")},
                {"页面文件路由", GetRowValue(row, "页面文件路由")},
                {"页面文件引用相关组件", GetRowValue(row, "页面文件引用相关组件")},
                {"翻译文件路径（参数化）", GetRowValue(row, "翻译文件路径（参数化）")},
                {"翻译使用前缀", GetRowValue(row, "翻译使用前缀")},
            };

            // 循环语言包
            foreach (var langItem in langList)
            {
                excelRow.Add(langItem, GetRowValue(row, langItem));
            }

            excelDictionary.Add(excelRow);
        }

        if (excelDictionary.Count == 0)
            throw new InvalidDataException($"翻译文件“{excelFile.FullName}”不包含任何数据行。");

        var autoLoadList = new List<(string routePath, List<string> refComponentPathList)>();

        // 循环语言包，写入对应的语言文件
        foreach (var langItem in langList)
        {
            // 组装语言包的文件夹路径
            var langItemPath = Path.Combine(langPath, langItem);

            // 删除语言包中的所有文件，包括文件夹;
            if (Directory.Exists(langItemPath))
                Directory.Delete(langItemPath, true);

            // 这里会删除语言包本身的文件夹，所以删除完成后立即创建一个
            Directory.CreateDirectory(langItemPath);

            // 使用 "翻译文件路径（参数化）" 进行分组
            foreach (var fileItem in excelDictionary.GroupBy(gb => gb["翻译文件路径（参数化）"]))
            {
                if (string.IsNullOrWhiteSpace(fileItem.Key))
                    throw new InvalidDataException("Excel 中存在空的“翻译文件路径（参数化）”。");

                var firstFileItem = fileItem.First();
                // 判断路由地址是否存在
                var routePath = firstFileItem["页面文件路由"];

                if (!string.IsNullOrEmpty(routePath))
                {
                    // 判断是否已经添加了
                    if (autoLoadList.All(a => a.routePath != routePath))
                    {
                        var refComponentPathStr = firstFileItem["页面文件引用相关组件"];

                        if (!string.IsNullOrEmpty(refComponentPathStr))
                        {
                            autoLoadList.Add((routePath,
                                refComponentPathStr.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()));
                        }
                    }
                }

                // 组装对应的语言包详情文件路径
                var fileItemPath = string.Format(System.Globalization.CultureInfo.InvariantCulture, fileItem.Key, langItem);

                // 如果翻译文件路径不存在，则创建
                FileUtil.TryCreateDirectory(fileItemPath);

                // 获取当前文件的所有翻译数据
                var langObjectList = fileItem.Select(sl => (sl["zh-CN"], sl[langItem])).ToList();

                var langContent = new StringBuilder();

                langContent.AppendLine(@$"// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
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

/**
 * 前缀：{firstFileItem["翻译使用前缀"]}
 * 使用方式：
 * i18n.global.t(""{firstFileItem["翻译使用前缀"]}.Fast.NET"")
 * t(""{firstFileItem["翻译使用前缀"]}.Fast.NET"")
 * $t(""{firstFileItem["翻译使用前缀"]}.Fast.NET"")
 */

export default {{");

                // 循环文件翻译内容
                foreach (var langObjectItem in langObjectList)
                {
                    // 使用 JSON 编码生成合法的 TypeScript 字符串，避免引号、换行等内容破坏文件语法。
                    var keyLiteral = JsonSerializer.Serialize(langObjectItem.Item1);
                    var valueLiteral = JsonSerializer.Serialize(langObjectItem.Item2);
                    langContent.AppendLine($"    [{keyLiteral}]: {valueLiteral},");
                }

                // 写入文件尾部
                langContent.AppendLine(@"};");

                // 写入文件
                File.WriteAllText(fileItemPath, langContent.ToString(), Encoding.UTF8);

                // 更改颜色
                Console.ForegroundColor = ConsoleColor.DarkGray;

                // 消息提示
                Console.WriteLine(fileItemPath);
            }
        }

        // 生成语言包按需加载关系的文件
        var autoLoadContent = new StringBuilder();

        autoLoadContent.AppendLine(@"// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
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

/**
 * 自动生成的语言包按需加载映射表
 * 使用 ${lang} 代替当前语言
 * key为页面路由的地址，value为页语言包文件相对路径
 * 访问时按需自动加载映射表的语言包（若存在）
 */

export default {");

        foreach (var autoLoadItem in autoLoadList)
        {
            // 这里需要递归查找对应引用组件相关的关系，比如A组件引用了B，B组件引用了C，则A需要加载 B，C 的语言包，不能只加载 B 的语言包
            var curRefComponentPathList = FindRefComponentRoutePathList(autoLoadItem.routePath, autoLoadList);
            var valueStr = string.Join(", ", curRefComponentPathList
                .Distinct(StringComparer.Ordinal)
                .Where(path => path.EndsWith(".vue", StringComparison.OrdinalIgnoreCase)
                               || path.EndsWith(".ts", StringComparison.OrdinalIgnoreCase)
                               || path.EndsWith(".tsx", StringComparison.OrdinalIgnoreCase))
                .Select(path => $"\"./${{lang}}/{NormalizePath(Path.ChangeExtension(path, ".ts"))}\""));

            // 格式： ["/"]: ["./${lang}/..."]
            autoLoadContent.AppendLine(@$"    [""{autoLoadItem.routePath}""]: [{valueStr}],");
        }

        // 写入文件尾部
        autoLoadContent.AppendLine(@"};");

        // 写入文件
        var autoLoadFilePath = Path.Combine(langPath, "autoLoad.ts");
        File.WriteAllText(autoLoadFilePath, autoLoadContent.ToString(), Encoding.UTF8);

        // 消息提示
        Console.WriteLine(autoLoadFilePath);
    }

    /// <summary>
    /// 递归查找引用组件路由信息集合
    /// </summary>
    /// <param name="routePath"></param>
    /// <param name="refComponentPathList"></param>
    /// <returns></returns>
    private static List<string> FindRefComponentRoutePathList(string routePath,
        List<(string routePath, List<string> refComponentPathList)> refComponentPathList)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        var visitedRoutes = new HashSet<string>(StringComparer.Ordinal);

        // 组件引用可能成环（A -> B -> A），必须记录已访问路由，否则递归会无限循环。
        void Visit(string currentRoute)
        {
            if (!visitedRoutes.Add(currentRoute))
                return;

            var current = refComponentPathList.FirstOrDefault(item =>
                string.Equals(item.routePath, currentRoute, StringComparison.Ordinal));
            if (current.routePath == null)
                return;

            foreach (var componentPath in current.refComponentPathList)
            {
                var normalizedComponentPath = NormalizePath(componentPath);
                result.Add(normalizedComponentPath);
                Visit(ToRoutePath(normalizedComponentPath));
            }
        }

        Visit(routePath);
        return result.ToList();
    }

    private static string ToRoutePath(string componentPath)
    {
        var routePath = NormalizePath(componentPath);
        if (routePath.StartsWith("views/", StringComparison.Ordinal))
            routePath = routePath["views".Length..];

        routePath = Path.ChangeExtension(routePath, null);
        if (routePath.EndsWith("/index", StringComparison.OrdinalIgnoreCase))
            routePath = routePath[..^"/index".Length];

        return routePath;
    }

    private static string GetRowValue(IDictionary<string, object> row, string key)
    {
        return row.TryGetValue(key, out var value) ? value?.ToString() ?? string.Empty : string.Empty;
    }

    private static string NormalizePath(string path)
    {
        return path.Replace('\\', '/');
    }
}
