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

using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>
/// <see cref="Debugging"/> 向事件管理器中输出事件信息
/// </summary>
[SuppressSniffer]
public static class Debugging
{
    /// <summary>
    /// 输出一行事件信息
    /// </summary>
    /// <param name="level">
    /// <para>信息级别</para>
    /// <list type="number">
    /// <item>
    /// <description>跟踪</description>
    /// </item>
    /// <item>
    /// <description>信息</description>
    /// </item>
    /// <item>
    /// <description>警告</description>
    /// </item>
    /// <item>
    /// <description>错误</description>
    /// </item>
    /// <item>
    /// <description>文件</description>
    /// </item>
    /// <item>
    /// <description>提示</description>
    /// </item>
    /// <item>
    /// <description>搜索</description>
    /// </item>
    /// <item>
    /// <description>时钟</description>
    /// </item>
    /// </list>
    /// </param>
    /// <param name="message">事件信息</param>
    public static void WriteLine(int level, string message)
    {
        // 获取信息级别对应的 emoji
        var category = GetLevelEmoji(level);

        Debug.WriteLine(message, category);
    }

    /// <summary>
    /// 输出一行事件信息
    /// </summary>
    /// <param name="level">
    /// <para>信息级别</para>
    /// <list type="number">
    /// <item>
    /// <description>跟踪</description>
    /// </item>
    /// <item>
    /// <description>信息</description>
    /// </item>
    /// <item>
    /// <description>警告</description>
    /// </item>
    /// <item>
    /// <description>错误</description>
    /// </item>
    /// <item>
    /// <description>文件</description>
    /// </item>
    /// <item>
    /// <description>提示</description>
    /// </item>
    /// <item>
    /// <description>搜索</description>
    /// </item>
    /// <item>
    /// <description>时钟</description>
    /// </item>
    /// </list>
    /// </param>
    /// <param name="message">事件信息</param>
    /// <param name="args">格式化参数</param>
    public static void WriteLine(int level, string message, params object[] args)
    {
        WriteLine(level, string.Format(message, args));
    }

    /// <summary>
    /// 输出跟踪级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    public static void Trace(string message)
    {
        WriteLine(1, message);
    }

    /// <summary>
    /// 输出跟踪级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    /// <param name="args">格式化参数</param>
    public static void Trace(string message, params object[] args)
    {
        WriteLine(1, message, args);
    }

    /// <summary>
    /// 输出信息级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    public static void Info(string message)
    {
        WriteLine(2, message);
    }

    /// <summary>
    /// 输出信息级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    /// <param name="args">格式化参数</param>
    public static void Info(string message, params object[] args)
    {
        WriteLine(2, message, args);
    }

    /// <summary>
    /// 输出警告级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    public static void Warn(string message)
    {
        WriteLine(3, message);
    }

    /// <summary>
    /// 输出警告级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    /// <param name="args">格式化参数</param>
    public static void Warn(string message, params object[] args)
    {
        WriteLine(3, message, args);
    }

    /// <summary>
    /// 输出错误级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    public static void Error(string message)
    {
        WriteLine(4, message);
    }

    /// <summary>
    /// 输出错误级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    /// <param name="args">格式化参数</param>
    public static void Error(string message, params object[] args)
    {
        WriteLine(4, message, args);
    }

    /// <summary>
    /// 输出文件级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    public static void File(string message)
    {
        WriteLine(5, message);
    }

    /// <summary>
    /// 输出文件级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    /// <param name="args">格式化参数</param>
    public static void File(string message, params object[] args)
    {
        WriteLine(5, message, args);
    }

    /// <summary>
    /// 输出提示级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    public static void Tip(string message)
    {
        WriteLine(6, message);
    }

    /// <summary>
    /// 输出提示级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    /// <param name="args">格式化参数</param>
    public static void Tip(string message, params object[] args)
    {
        WriteLine(6, message, args);
    }

    /// <summary>
    /// 输出搜索级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    public static void Search(string message)
    {
        WriteLine(7, message);
    }

    /// <summary>
    /// 输出搜索级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    /// <param name="args">格式化参数</param>
    public static void Search(string message, params object[] args)
    {
        WriteLine(7, message, args);
    }

    /// <summary>
    /// 输出时钟级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    public static void Clock(string message)
    {
        WriteLine(8, message);
    }

    /// <summary>
    /// 输出时钟级别事件信息
    /// </summary>
    /// <param name="message">事件信息</param>
    /// <param name="args">格式化参数</param>
    public static void Clock(string message, params object[] args)
    {
        WriteLine(8, message, args);
    }

    /// <summary>
    /// 获取信息级别对应的 emoji
    /// </summary>
    /// <param name="level">
    /// <para>信息级别</para>
    /// <list type="number">
    /// <item>
    /// <description>跟踪</description>
    /// </item>
    /// <item>
    /// <description>信息</description>
    /// </item>
    /// <item>
    /// <description>警告</description>
    /// </item>
    /// <item>
    /// <description>错误</description>
    /// </item>
    /// <item>
    /// <description>文件</description>
    /// </item>
    /// <item>
    /// <description>提示</description>
    /// </item>
    /// <item>
    /// <description>搜索</description>
    /// </item>
    /// <item>
    /// <description>时钟</description>
    /// </item>
    /// </list>
    /// </param>
    /// <returns><see cref="string"/></returns>
    public static string GetLevelEmoji(int level)
    {
        return level switch
        {
            1 => "🛠️",
            2 => "ℹ️",
            3 => "⚠️",
            4 => "❌",
            5 => "📄",
            6 => "💡",
            7 => "🔍",
            8 => "⏱️",
            _ => string.Empty
        };
    }
}