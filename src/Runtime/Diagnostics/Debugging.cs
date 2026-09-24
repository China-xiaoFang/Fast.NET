// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.Diagnostics;


// ReSharper disable once CheckNamespace
namespace System;

/// <summary>
/// 向事件管理器中输出事件信息
/// </summary>
[SuppressSniffer]
public static class Debugging
{
    /// <summary>
    /// 输出一行事件信息
    /// </summary>
    /// <param name="level">信息级别 跟踪 信息 警告 错误 文件 提示 搜索 时钟</param>
    /// <param name="message">要记录或返回的消息</param>
    public static void WriteLine(int level, string message)
    {
        // 获取信息级别对应的 emoji
        string category = GetLevelEmoji(level);

        Debug.WriteLine(message, category);
    }

    /// <summary>
    /// 输出一行事件信息
    /// </summary>
    /// <param name="level">信息级别 跟踪 信息 警告 错误 文件 提示 搜索 时钟</param>
    /// <param name="message">要记录或返回的消息</param>
    /// <param name="args">格式化消息时使用的参数</param>
    public static void WriteLine(int level, string message, params object[] args)
    {
        WriteLine(level, string.Format(message, args));
    }

    /// <summary>
    /// 输出跟踪级别事件信息
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    public static void Trace(string message)
    {
        WriteLine(1, message);
    }

    /// <summary>
    /// 输出跟踪级别事件信息
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    /// <param name="args">格式化消息时使用的参数</param>
    public static void Trace(string message, params object[] args)
    {
        WriteLine(1, message, args);
    }

    /// <summary>
    /// 输出信息级别事件信息
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    public static void Info(string message)
    {
        WriteLine(2, message);
    }

    /// <summary>
    /// 输出信息级别事件信息
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    /// <param name="args">格式化消息时使用的参数</param>
    public static void Info(string message, params object[] args)
    {
        WriteLine(2, message, args);
    }

    /// <summary>
    /// 输出警告级别事件信息
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    public static void Warn(string message)
    {
        WriteLine(3, message);
    }

    /// <summary>
    /// 输出警告级别事件信息
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    /// <param name="args">格式化消息时使用的参数</param>
    public static void Warn(string message, params object[] args)
    {
        WriteLine(3, message, args);
    }

    /// <summary>
    /// 输出错误级别事件信息
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    public static void Error(string message)
    {
        WriteLine(4, message);
    }

    /// <summary>
    /// 输出错误级别事件信息
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    /// <param name="args">格式化消息时使用的参数</param>
    public static void Error(string message, params object[] args)
    {
        WriteLine(4, message, args);
    }

    /// <summary>
    /// 输出文件级别事件信息
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    public static void File(string message)
    {
        WriteLine(5, message);
    }

    /// <summary>
    /// 输出文件级别事件信息
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    /// <param name="args">格式化消息时使用的参数</param>
    public static void File(string message, params object[] args)
    {
        WriteLine(5, message, args);
    }

    /// <summary>
    /// 输出提示级别事件信息
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    public static void Tip(string message)
    {
        WriteLine(6, message);
    }

    /// <summary>
    /// 输出提示级别事件信息
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    /// <param name="args">格式化消息时使用的参数</param>
    public static void Tip(string message, params object[] args)
    {
        WriteLine(6, message, args);
    }

    /// <summary>
    /// 输出搜索级别事件信息
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    public static void Search(string message)
    {
        WriteLine(7, message);
    }

    /// <summary>
    /// 输出搜索级别事件信息
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    /// <param name="args">格式化消息时使用的参数</param>
    public static void Search(string message, params object[] args)
    {
        WriteLine(7, message, args);
    }

    /// <summary>
    /// 输出时钟级别事件信息
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    public static void Clock(string message)
    {
        WriteLine(8, message);
    }

    /// <summary>
    /// 输出时钟级别事件信息
    /// </summary>
    /// <param name="message">要记录或返回的消息</param>
    /// <param name="args">格式化消息时使用的参数</param>
    public static void Clock(string message, params object[] args)
    {
        WriteLine(8, message, args);
    }

    /// <summary>
    /// 获取信息级别对应的 emoji
    /// </summary>
    /// <param name="level">日志级别</param>
    /// <returns>获取到的信息级别对应的 emoji</returns>
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
