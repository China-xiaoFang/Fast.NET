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

namespace I18nTranslateTool.Utils;

/// <summary>
/// <see cref="ConsoleUtils"/> 控制台工具类
/// </summary>
internal class ConsoleUtils
{
    /// <summary>
    /// 获取用户输入
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="isValidInput"></param>
    /// <param name="func"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal static TResult GetUserInput<TResult>(Func<TResult, bool> isValidInput, Func<string> func)
    {
        if (isValidInput == null)
        {
            throw new ArgumentNullException(nameof(isValidInput));
        }

        if (func == null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        var resultType = typeof(TResult);

        var result = default(TResult);

        // 还原颜色
        Console.ResetColor();

        while (!isValidInput(result))
        {
            var inputStr = func();
            try
            {
                // 把输入的字符串尝试转为传入的类型
                var input = (TResult) Convert.ChangeType(inputStr, resultType);

                if (isValidInput(input))
                {
                    result = input;
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                // 更改颜色
                Console.ForegroundColor = ConsoleColor.Red;

                // 错误输出
                Console.WriteLine("Invalid input, please re-enter!（无效的输入，请重新输入！）");

                // 还原颜色
                Console.ResetColor();
            }
        }

        return result;
    }
}