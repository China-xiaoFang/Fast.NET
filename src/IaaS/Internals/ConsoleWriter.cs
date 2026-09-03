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

using System;
using System.IO;

namespace Fast.IaaS;

/// <summary>
/// 控制台同步写入回调使用的颜色与文本输出上下文
/// </summary>
/// <remarks>仅在当前写入回调内使用，输出重定向时忽略颜色设置</remarks>
internal sealed class ConsoleWriter
{
    private readonly TextWriter _writer;
    private readonly bool _useColor;
    private readonly ConsoleColor _originalForegroundColor;
    private readonly ConsoleColor _originalBackgroundColor;

    internal ConsoleWriter(TextWriter writer)
    {
        _writer = writer;
        _useColor = !Console.IsOutputRedirected;
        _originalForegroundColor = Console.ForegroundColor;
        _originalBackgroundColor = Console.BackgroundColor;
    }

    /// <summary>
    /// 当前控制台前景色，输出重定向时忽略设置
    /// </summary>
    public ConsoleColor ForegroundColor
    {
        get => Console.ForegroundColor;
        set
        {
            if (_useColor)
                Console.ForegroundColor = value;
        }
    }

    /// <summary>
    /// 当前控制台背景色，输出重定向时忽略设置
    /// </summary>
    public ConsoleColor BackgroundColor
    {
        get => Console.BackgroundColor;
        set
        {
            if (_useColor)
                Console.BackgroundColor = value;
        }
    }

    /// <summary>
    /// 写入文本而不追加换行
    /// </summary>
    /// <param name="value">要写入的文本，为 <see langword="null"/> 时不输出内容</param>
    public void Write(string value)
    {
        _writer.Write(value);
    }

    /// <summary>
    /// 写入对象的文本表示而不追加换行
    /// </summary>
    /// <param name="value">要写入的对象，为 <see langword="null"/> 时不输出内容</param>
    public void Write(object value)
    {
        _writer.Write(value);
    }

    /// <summary>
    /// 写入换行
    /// </summary>
    public void WriteLine()
    {
        _writer.WriteLine();
    }

    /// <summary>
    /// 写入文本并追加换行
    /// </summary>
    /// <param name="value">要写入的文本，为 <see langword="null"/> 时仅输出换行</param>
    public void WriteLine(string value)
    {
        _writer.WriteLine(value);
    }

    /// <summary>
    /// 写入对象的文本并追加换行
    /// </summary>
    /// <param name="value">要写入的对象，为 <see langword="null"/> 时仅输出换行</param>
    public void WriteLine(object value)
    {
        _writer.WriteLine(value);
    }

    /// <summary>
    /// 恢复进入当前写入回调时的前景色和背景色
    /// </summary>
    public void ResetColor()
    {
        if (_useColor)
        {
            Console.ForegroundColor = _originalForegroundColor;
            Console.BackgroundColor = _originalBackgroundColor;
        }
    }
}
