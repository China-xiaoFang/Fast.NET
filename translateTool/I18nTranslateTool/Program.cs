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

using I18nTranslateTool.Modes;
using I18nTranslateTool.Utils;

namespace I18nTranslateTool;

internal class Program
{
    static void Main(string[] args)
    {
        // 更改颜色
        Console.ForegroundColor = ConsoleColor.DarkGray;
        // 开源协议输出
        Console.WriteLine(@"
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
");

        // 更改颜色
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        // 信息输出
        Console.WriteLine(@"
    Welcome to the Fast.NET vue-i18n internationalization tool!（欢迎使用 Fast.NET vue-i18n 国际化工具！）
");

        // 当前启动时间输出
        Console.WriteLine(@$"
    Current time（当前时间）: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}

");

        // 更改颜色
        Console.ForegroundColor = ConsoleColor.Yellow;

        // 获取前端项目的绝对路径
        Console.WriteLine(@"
Please enter the folder path of the Vue project (the root directory is sufficient), for example: F:\Fast.NET\frontend-template
请输入Vue项目的文件夹路径（根目录即可），比如：F:\Fast.NET\frontend-template
");

        var projectPath = ConsoleUtils.GetUserInput<string>(wh => !string.IsNullOrEmpty(wh), Console.ReadLine);

        // 获取前端项目的文件夹名称
        var projectName = projectPath.Split(Path.DirectorySeparatorChar).Last();

        // 组装对应的翻译信息存放地址
        var translateFilePath = $"{Directory.GetCurrentDirectory()}\\Files\\{projectName}";

        // 更改颜色
        Console.ForegroundColor = ConsoleColor.Green;

        // 获取前端项目的绝对路径
        Console.WriteLine(@$"
After successful execution, please search for the corresponding file information in the [{translateFilePath}] directory.
执行成功后，请在 【{translateFilePath}】 目录下查找对应的文件信息
");

        // 更改颜色
        Console.ForegroundColor = ConsoleColor.Yellow;

        // 模式选择
        Console.WriteLine(@"
Mode selection（模式选择）：
1.Extract translation copy to Excel.
  提取翻译文案到 Excel 文件。
2.Automatically translate Excel files (can be translated manually).
  自动翻译 Excel 文件（可手动翻译）。
3.Update translation copy to project.
  更新翻译文案到项目。

Please select the corresponding function/mode.（请选择对应的功能/模式。）
");

        // 还原颜色
        Console.ResetColor();

        var modeSelect = ConsoleUtils.GetUserInput<int>(wh => wh is >= 1 and <= 3, Console.ReadLine);

        // 根据对应的模式执行对应的代码
        switch (modeSelect)
        {
            // 提取翻译文案到 Excel 文件
            case 1:
                ExtractContentToExcel.Run(projectPath, projectName, translateFilePath);
                break;
            // 自动翻译 Excel 文件（可手动翻译）
            case 2:
                AutoTranslateExcel.Run(projectPath, projectName, translateFilePath);
                break;
            // 更新翻译文案到项目
            case 3:
                UpdateTranslateToProject.Run(projectPath, projectName, translateFilePath);
                break;
        }

        // 更改颜色
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        // 信息输出
        Console.WriteLine(@"
execution succeed!（执行成功！）
");

        // 还原颜色
        Console.ResetColor();

        // 阻塞程序退出
        Console.WriteLine("Press any key to exit...（按任意键退出...）");
        // 等待用户按下任意键
        Console.ReadKey();
    }
}