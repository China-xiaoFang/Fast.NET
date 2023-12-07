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

namespace Fast.Logging.Templates;

/// <summary>
/// 模板静态类
/// </summary>
public static class TP
{
    /// <summary>
    /// 模板正则表达式对象
    /// </summary>
    private static readonly Lazy<Regex> _lazyRegex = new(() => new(@"^##(?<prop>.*)?##[:：]?\s*(?<content>[\s\S]*)"));

    /// <summary>
    /// 生成规范日志模板
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="description">描述</param>
    /// <param name="items">列表项，如果以 ##xxx## 开头，自动生成 xxx: 属性</param>
    /// <returns><see cref="string"/></returns>
    public static string Wrapper(string title, string description, params string[] items)
    {
        // 处理不同编码问题
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"┏━━━━━━━━━━━  {title} ━━━━━━━━━━━").AppendLine();

        // 添加描述
        if (!string.IsNullOrWhiteSpace(description))
        {
            stringBuilder.Append($"┣ {description}").AppendLine().Append("┣ ").AppendLine();
        }

        // 添加项
        if (items != null && items.Length > 0)
        {
            var propMaxLength = items.Where(u => _lazyRegex.Value.IsMatch(u)).DefaultIfEmpty(string.Empty)
                .Max(u => _lazyRegex.Value.Match(u).Groups["prop"].Value.Length);

            // 控制项名称对齐空白占位数
            propMaxLength += (propMaxLength >= 5 ? 10 : 5);

            // 遍历每一项并进行正则表达式匹配
            for (var i = 0; i < items.Length; i++)
            {
                var item = items[i];

                // 判断是否匹配 ##xxx##
                if (_lazyRegex.Value.IsMatch(item))
                {
                    var match = _lazyRegex.Value.Match(item);
                    var prop = match.Groups["prop"].Value;
                    var content = match.Groups["content"].Value;

                    var propTitle = $"{prop}：";
                    stringBuilder.Append($"┣ {PadRight(propTitle, propMaxLength)}{content}").AppendLine();
                }
                else
                {
                    stringBuilder.Append($"┣ {item}").AppendLine();
                }
            }
        }

        stringBuilder.Append($"┗━━━━━━━━━━━  {title} ━━━━━━━━━━━").AppendLine();
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 矩形包裹
    /// </summary>
    /// <param name="lines">多行消息</param>
    /// <param name="align">对齐方式，-1/左对齐；0/居中对其；1/右对齐</param>
    /// <param name="pad">间隙</param>
    /// <returns><see cref="string"/></returns>
    public static string WrapperRectangle(string[] lines, int align = 0, int pad = 20)
    {
        // 处理不同编码问题
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // 计算矩形框的宽度，取所有字符串中最长的长度，再乘以 2
        var width = lines.Max(GetLength) + pad;

        // 创建一个 StringBuilder 对象
        var stringBuilder = new StringBuilder();

        // 在 StringBuilder 对象中添加矩形框的上边框
        stringBuilder.AppendLine("+" + new string('-', width - 2) + "+");

        // 遍历每个字符串，并添加到 StringBuilder 对象中
        var row = 0;
        foreach (var line in lines)
        {
            // 当前字符串的长度
            var len = GetLength(line);
            var padding = align switch
            {
                -1 => 2,
                0 => (width - len - 2) / 2,
                1 => (width - len - 2) - 2,
                _ => 2
            };

            // 在 StringBuilder 对象中添加当前字符串前的空格，使得当前字符串在矩形框中居中显示
            stringBuilder.Append("|" + new string(' ', padding));

            // 在 StringBuilder 对象中添加当前字符串
            stringBuilder.Append(line);

            // 在 StringBuilder 对象中添加当前字符串后的空格，使得矩形框的宽度保持不变
            stringBuilder.Append(new string(' ', width - len - 2 - padding) + "|");

            // 在 StringBuilder 对象中添加换行符
            stringBuilder.AppendLine();

            // 更新当前行数
            row++;
        }

        // 在 StringBuilder 对象中添加矩形框的下边框
        stringBuilder.Append("+" + new string('-', width - 2) + "+");

        // 返回包含矩形框的所有字符串的 StringBuilder 对象的字符串表示形式
        return stringBuilder.ToString();
    }

    /// <summary>
    /// 等宽文字对齐
    /// </summary>
    /// <param name="str"></param>
    /// <param name="totalByteCount"></param>
    /// <returns></returns>
    private static string PadRight(string str, int totalByteCount)
    {
        var coding = Encoding.GetEncoding("gbk");
        var dcount = 0;

        foreach (var character in str.ToCharArray())
        {
            if (coding.GetByteCount(character.ToString()) == 2)
                dcount++;
        }

        var w = str.PadRight(totalByteCount - dcount);
        return w;
    }

    /// <summary>
    /// 获取字符串长度
    /// </summary>
    /// <param name="str">字符串</param>
    /// <returns>字符串长度</returns>
    public static int GetLength(string str)
    {
        var coding = Encoding.GetEncoding("gbk");
        return coding.GetByteCount(str);
    }
}