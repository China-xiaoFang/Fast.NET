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
using Fast.IaaS;
using MiniExcelLibs;

namespace I18nTranslateTool.Modes;

/// <summary>
/// <see cref="UpdateTranslateToProject"/> 更新翻译文案到项目
/// </summary>
internal class UpdateTranslateToProject
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
        var langPath = $"{projectPath}\\src\\lang";

        // 获取 lang 文件夹中的语言，只获取文件夹名称
        var langList = Directory.GetDirectories(langPath, "*", SearchOption.TopDirectoryOnly)
            .Select(sl => sl.Split(Path.DirectorySeparatorChar).Last()).ToList();

        // 获取翻译文件目录下最后一次更新的 xlsx 文件，这里使用文件名称排序的方式
        var excelFile = Directory.GetFiles(translateFilePath, "*.xlsx", SearchOption.TopDirectoryOnly)
            .Select(sl => new FileInfo(sl)).MaxBy(ob => ob.Name);

        // 组装读取Excel的字典
        var excelDictionary = new List<IDictionary<string, string>>();

        // 读取 Excel文件
        foreach (IDictionary<string, object> row in MiniExcel.Query(excelFile!.FullName, useHeaderRow: true))
        {
            // 先组装默认数据
            var excelRow = new Dictionary<string, string>
            {
                {"页面文件路径", row["页面文件路径"]?.ToString() ?? ""},
                {"页面文件路由", row["页面文件路由"]?.ToString() ?? ""},
                {"页面文件引用相关组件", row["页面文件引用相关组件"]?.ToString() ?? ""},
                {"翻译文件路径（参数化）", row["翻译文件路径（参数化）"]?.ToString() ?? ""},
                {"翻译使用前缀", row["翻译使用前缀"]?.ToString() ?? ""},
            };

            // 循环语言包
            foreach (var langItem in langList)
            {
                excelRow.Add(langItem, row[langItem]?.ToString() ?? "");
            }

            excelDictionary.Add(excelRow);
        }

        var autoLoadList = new List<(string routePath, List<string> refComponentPathList)>();

        // 循环语言包，写入对应的语言文件
        foreach (var langItem in langList)
        {
            // 组装语言包的文件夹路径
            var langItemPath = $"{langPath}\\{langItem}";

            // 删除语言包中的所有文件，包括文件夹;
            Directory.Delete(langItemPath, true);

            // 这里会删除语言包本身的文件夹，所以删除完成后立即创建一个
            Directory.CreateDirectory(langItemPath);

            // 使用 "翻译文件路径（参数化）" 进行分组
            foreach (var fileItem in excelDictionary.GroupBy(gb => gb["翻译文件路径（参数化）"]))
            {
                // 判断路由地址是否存在
                var routePath = fileItem.First()["页面文件路由"];

                if (!string.IsNullOrEmpty(routePath))
                {
                    // 判断是否已经添加了
                    if (autoLoadList.All(a => a.routePath != routePath))
                    {
                        var refComponentPathStr = fileItem.First()["页面文件引用相关组件"];

                        if (!string.IsNullOrEmpty(refComponentPathStr))
                        {
                            autoLoadList.Add((routePath,
                                refComponentPathStr.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()));
                        }
                    }
                }

                // 组装对应的语言包详情文件路径
                var fileItemPath = string.Format(fileItem.Key, langItem);

                // 如果翻译文件路径不存在，则创建
                FileUtil.TryCreateDirectory(fileItemPath);

                // 获取当前文件的所有翻译数据
                var langObjectList = excelDictionary.Where(wh => wh["翻译文件路径（参数化）"] == fileItem.Key)
                    .Select(sl => (sl["zh-CN"], sl[langItem])).ToList();

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
 * 前缀：{(fileItem.First()["翻译使用前缀"])}
 * 使用方式：
 * i18n.global.t(""{(fileItem.First()["翻译使用前缀"])}.Fast.NET"")
 * t(""{(fileItem.First()["翻译使用前缀"])}.Fast.NET"")
 * $t(""{(fileItem.First()["翻译使用前缀"])}.Fast.NET"")
 */

export default {{");

                // 循环文件翻译内容
                foreach (var langObjectItem in langObjectList)
                {
                    langContent.AppendLine(@$"    [""{langObjectItem.Item1}""]: ""{langObjectItem.Item2}"",");
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
            var valueStr = "";
            // 去重
            foreach (var refComponentPathItem in curRefComponentPathList.Distinct())
            {
                if (refComponentPathItem.EndsWith(".vue"))
                {
                    valueStr += $"\"./${{lang}}/{refComponentPathItem[..^3]}ts\", ";
                }

                if (refComponentPathItem.EndsWith(".ts"))
                {
                    valueStr += $"\"./${{lang}}/{refComponentPathItem[..^2]}ts\", ";
                }

                if (refComponentPathItem.EndsWith(".tsx"))
                {
                    valueStr += $"\"./${{lang}}/{refComponentPathItem[..^2]}ts\", ";
                }
            }

            // 去掉多余的 , 
            if (!string.IsNullOrEmpty(valueStr))
            {
                valueStr = valueStr[..^2];
            }

            // 格式： ["/"]: ["./${lang}/..."]
            autoLoadContent.AppendLine(@$"    [""{autoLoadItem.routePath}""]: [{valueStr}],");
        }

        // 写入文件尾部
        autoLoadContent.AppendLine(@"};");

        // 写入文件
        File.WriteAllText($"{langPath}\\autoLoad.ts", autoLoadContent.ToString(), Encoding.UTF8);

        // 消息提示
        Console.WriteLine($"{langPath}\\autoLoad.ts");
    }

    /// <summary>
    /// 递归查找引用组件路由信息集合
    /// </summary>
    /// <param name="routePath"></param>
    /// <param name="refComponentPathList"></param>
    /// <returns></returns>
    static List<string> FindRefComponentRoutePathList(string routePath,
        List<(string routePath, List<string> refComponentPathList)> refComponentPathList)
    {
        var result = new List<string>();

        // 查找当前路由引用的组件
        var curRefComponentPathList = refComponentPathList.FirstOrDefault(f => f.routePath == routePath);

        // 循环查找当前引用组件的关系
        if (curRefComponentPathList.routePath != null)
        {
            var index = 0;
            // 循环当前所有引用组件
            foreach (var refComponentPathItem in curRefComponentPathList.refComponentPathList)
            {
                // 这里的 refComponentPathItem 为 views/**/*.vue
                var curRoutePath = refComponentPathItem;

                // 判断是否已 views 开头，如果是则剔除
                if (curRoutePath.StartsWith("views"))
                {
                    curRoutePath = curRoutePath[5..];
                }

                // 判断是否已 .vue 结尾，如果是则剔除
                if (curRoutePath.EndsWith(".vue"))
                {
                    curRoutePath = curRoutePath[..^".vue".Length];
                }

                // 判断是否已 /index 结尾，如果是则剔除
                if (curRoutePath.EndsWith("/index"))
                {
                    curRoutePath = curRoutePath[..^"/index".Length];
                }

                var curFindInfo1 = refComponentPathList.FirstOrDefault(f => f.routePath == curRoutePath);
                if (curFindInfo1.routePath != null && curFindInfo1.refComponentPathList.Count > 0)
                {
                    result.AddRange(curFindInfo1.refComponentPathList);
                }

                if (index > 0)
                {
                    // 查找当前组件对应的引用
                    foreach (var findItem in FindRefComponentRoutePathList(curRoutePath, refComponentPathList))
                    {
                        // 这里的 findItem 为 views/**/*.vue
                        var curFindItem = findItem;

                        // 判断是否已 views 开头，如果是则剔除
                        if (curFindItem.StartsWith("views"))
                        {
                            curFindItem = curFindItem[5..];
                        }

                        // 判断是否已 .vue 结尾，如果是则剔除
                        if (curFindItem.EndsWith(".vue"))
                        {
                            curFindItem = curFindItem[..^".vue".Length];
                        }

                        // 判断是否已 /index 结尾，如果是则剔除
                        if (curFindItem.EndsWith("/index"))
                        {
                            curFindItem = curFindItem[..^"/index".Length];
                        }

                        var curFindInfo2 = refComponentPathList.FirstOrDefault(f => f.routePath == curFindItem);
                        if (curFindInfo2.routePath != null && curFindInfo2.refComponentPathList.Count > 0)
                        {
                            result.AddRange(curFindInfo2.refComponentPathList);
                        }
                    }
                }

                index++;
            }
        }

        return result;
    }
}