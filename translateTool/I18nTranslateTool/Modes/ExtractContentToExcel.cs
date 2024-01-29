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
using System.Text.RegularExpressions;
using Fast.IaaS;
using MiniExcelLibs;

namespace I18nTranslateTool.Modes;

/// <summary>
/// <see cref="ExtractContentToExcel"/> 提取翻译文案到 Excel 文件
/// </summary>
internal class ExtractContentToExcel
{
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="projectPath"><see cref="string"/> 项目路径</param>
    /// <param name="projectName"><see cref="string"/> 项目名称</param>
    /// <param name="translateFilePath"><see cref="string"/> 翻译文件存放位置</param>
    internal static void Run(string projectPath, string projectName, string translateFilePath)
    {
        var srcPath = $"{projectPath}\\src";

        // 组装前端项目文件夹 src\lang 的路径
        var langPath = $"{projectPath}\\src\\lang";

        // 获取前端项目文件夹中 src 文件夹的所有文件信息
        var srcDirectoryList = Directory.GetFiles(srcPath, "*", SearchOption.AllDirectories)
            // 只获取 .vue .ts .tsx 的文件
            .Where(wh => wh.EndsWith(".vue", StringComparison.OrdinalIgnoreCase) ||
                         wh.EndsWith(".ts", StringComparison.OrdinalIgnoreCase) ||
                         wh.EndsWith(".tsx", StringComparison.OrdinalIgnoreCase))
            // 这里剔除前端所在的翻译文件夹路径
            .Where(wh => !wh.StartsWith(langPath));

        // 获取 lang 文件夹中的语言，只获取文件夹名称，并且默认剔除 zh-CN，因为项目默认的就是 zh-CN
        var langList = Directory.GetDirectories(langPath, "*", SearchOption.TopDirectoryOnly)
            .Select(sl => sl.Split(Path.DirectorySeparatorChar).Last()).Where(wh => wh != "zh-CN").ToList();

        // 匹配 $t 和 t 对应的文案（包含单引号）
        var i18nRegex = new Regex(@"(\$t\(['""](.+?)['""]\))|(t\(['""](.+?)['""]\))");

        // 匹配TypeScript默认导出的对象
        var tsDefaultExportRegex = new Regex(@"export\s+default\s*{([\s\S]+?)}");

        // 匹配翻译文案内容，模板为 ["Fast.NET"]: "Hello World！"
        var tsObjectRegex = new Regex(@"\[""(.*?)""\]: ""(.*?)""");

        /*
         * 匹配页面引用地址
         * import { detailForm } from "./detailForm.vue";
         * import { detailForm } from "@/views/login/index.vue";
         * import detailForm from "./detailForm.vue";
         * import detailForm from "@/views/login/index.vue";
         */
        var importFromRegex = new Regex(@"import\s+(?:\{(?:[\s\S]+?)\}|(\w+))\s+from\s+[""']([^""']+?\.vue)[""']");

        /*
         * sourceFilePath：源文件路径
         * translateFilePath：翻译文件路径带参数化的
         * prefix：vue中使用的i18n前缀
         * keyList：需要翻译的Key集合
         * langDictionary：语言包字典
         */
        var translateList =
            new List<(string sourceFilePath, string translateFilePath, string prefix, List<string> keyList,
                IDictionary<string, IDictionary<string, string>> langDictionary, string componentPath, List<string>
                refComponentList)>();

        // 更改颜色
        Console.ForegroundColor = ConsoleColor.DarkGray;

        // 循环所有文件信息
        foreach (var fileItem in srcDirectoryList)
        {
            // 获取翻译文件的相对路径
            var targetPath = Path.GetRelativePath(srcPath, fileItem);
            // 去掉文件后缀
            targetPath = targetPath.Substring(0, targetPath.LastIndexOf(".", StringComparison.Ordinal));

            // 获取在vue中使用的前缀，掉文件最后的index，因为index在vue中默认就是根目录
            var prefix = targetPath.EndsWith("index")
                ? targetPath.Substring(0, targetPath.LastIndexOf("\\", StringComparison.Ordinal))
                : targetPath;
            // 替换 \ 为 .
            prefix = prefix.Replace("\\", ".");

            // 输出当前执行的文件信息
            Console.WriteLine(@$"{fileItem}  {prefix}");

            // 引用组件的文件路径
            var refComponentList = new List<string>();

            // 组件名称
            var componentPath = "";

            // 当前文件需要翻译的Key
            var keyList = new List<string>();

            // 读取文件内容
            var fileContent = File.ReadAllText(fileItem, Encoding.UTF8);

            // 只有路径是 views 的路径才会执行
            if (fileItem.StartsWith($"{srcPath}\\views"))
            {
                // 页面路由地址
                componentPath = Path.GetRelativePath($"{srcPath}\\views", fileItem);

                // 默认引用自己
                refComponentList.Add($"views\\{componentPath}");

                // 判断是否已 .vue 结尾，如果是则剔除
                if (componentPath.EndsWith(".vue"))
                {
                    componentPath = componentPath[..^".vue".Length];
                }

                // 判断是否已 /index 结尾，如果是则剔除
                if (componentPath.EndsWith("\\index"))
                {
                    componentPath = componentPath[..^"\\index".Length];
                }

                // 拼接前缀
                componentPath = $"\\{componentPath}";

                // 查找对应的页面引用地址
                var importFromMatch = importFromRegex.Match(fileContent);

                // 匹配页面组件引用路径
                while (importFromMatch.Success)
                {
                    // 获取组件路径
                    var curComponentPath = importFromMatch.Groups[2].Value;

                    // 获取下一个匹配项
                    importFromMatch = importFromMatch.NextMatch();

                    // 判断是否已 "@/" 开头，如果是，则删除
                    if (curComponentPath.StartsWith("@/"))
                    {
                        curComponentPath = curComponentPath[2..];
                        // 这里的根目录就是 src 目录，组装
                        curComponentPath = Path.Combine(srcPath, curComponentPath);
                    }
                    else
                    {
                        // 不是 @/ 开头，那就是相对路径，则根目录就是当前文件所存在的目录
                        curComponentPath = Path.GetFullPath(curComponentPath, Path.GetDirectoryName(fileItem));
                    }

                    // 这里只获取相对路径
                    var refComponentTargetPath = Path.GetRelativePath(srcPath, curComponentPath);
                    refComponentList.Add(refComponentTargetPath);
                }
            }

            var match = i18nRegex.Match(fileContent);

            // 匹配翻译文案
            while (match.Success)
            {
                // 获取翻译文案
                var text = string.IsNullOrEmpty(match.Groups[2].Value) ? match.Groups[4].Value : match.Groups[2].Value;

                // 获取下一个匹配项
                match = match.NextMatch();

                if (string.IsNullOrEmpty(text))
                {
                    continue;
                }

                // 这里必须采用严格模式，也就是当前文件中不能使用别的文件的翻译文本
                if (text.StartsWith(prefix))
                {
                    // 获取翻译Key
                    var key = text[(prefix.Length + 1)..];

                    // 判断当前Key是否已经存在于集合中
                    if (!keyList.Contains(key))
                    {
                        // 输出Key
                        Console.WriteLine($"Key：{key}");
                        keyList.Add(key);
                    }
                }
            }

            // 这里只缓存有 key 的文件信息
            if (keyList.Any())
            {
                var curTranslateFilePath = $"{langPath}\\{{0}}\\{targetPath}.ts";

                var langDictionary = new Dictionary<string, IDictionary<string, string>>();

                // 循环待翻译的语言包
                if (langList.Any())
                {
                    // 循环语言包
                    foreach (var langItem in langList)
                    {
                        // 当前语言包的文件路径
                        var curLangFilePath = string.Format(curTranslateFilePath, langItem);

                        // 判断文件是否存在
                        if (File.Exists(curLangFilePath))
                        {
                            // 读取对应的翻译文件内容
                            var langContent = File.ReadAllText(curLangFilePath, Encoding.UTF8);

                            var tsMatch = tsDefaultExportRegex.Match(langContent);

                            if (tsMatch.Success)
                            {
                                var text = tsMatch.Groups[1].Value;
                                if (!string.IsNullOrEmpty(text))
                                {
                                    var langObjectDictionary = new Dictionary<string, string>();

                                    // 使用正则表达式，手动解析
                                    var objectMatch = tsObjectRegex.Matches(text);

                                    foreach (Match matchItem in objectMatch)
                                    {
                                        langObjectDictionary.Add(matchItem.Groups[1].Value, matchItem.Groups[2].Value);
                                    }

                                    langDictionary.Add(langItem, langObjectDictionary);
                                }
                            }
                        }
                    }
                }

                // 放入集合中
                translateList.Add((fileItem, curTranslateFilePath, prefix, keyList, langDictionary, componentPath,
                    refComponentList));
            }
        }

        // 将 translateList 写入 Excel 文件中

        // 组装写入Excel的字典
        var excelDictionary = new List<IDictionary<string, string>>();

        // 循环 translateList
        foreach (var item in translateList)
        {
            // 循环需要翻译的Key
            foreach (var keyItem in item.keyList)
            {
                // 先组装默认数据
                var excelRow = new Dictionary<string, string>
                {
                    {"页面文件路径", item.sourceFilePath},
                    {"页面文件路由", item.componentPath.Replace("\\", "/")},
                    {"页面文件引用相关组件", string.Join(",", item.refComponentList.Select(sl => sl.Replace("\\", "/")))},
                    {"翻译文件路径（参数化）", item.translateFilePath},
                    {"翻译使用前缀", item.prefix},
                    {"zh-CN", keyItem},
                };

                // 循环语言包
                foreach (var langItem in langList)
                {
                    // 判断当前语言包是否存在已经翻译的值
                    if (item.langDictionary.TryGetValue(langItem, out var langDic))
                    {
                        if (langDic.TryGetValue(keyItem, out var langKeyValue))
                        {
                            excelRow.Add(langItem, langKeyValue);
                        }
                        else
                        {
                            excelRow.Add(langItem, "");
                        }
                    }
                    else
                    {
                        excelRow.Add(langItem, "");
                    }
                }

                excelDictionary.Add(excelRow);
            }
        }

        var filePath = $"{translateFilePath}\\{projectName}-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

        // 如果翻译文件目录不存在，则创建
        FileUtil.TryCreateDirectory(filePath);

        // 将翻译信息，写入Excel文件
        MiniExcel.SaveAs(filePath, excelDictionary);
    }
}