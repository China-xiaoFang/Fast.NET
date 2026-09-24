// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

namespace Fast.Logging;

/// <summary>
/// 控制台颜色结构
/// </summary>
internal readonly struct ConsoleColors
{
    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    /// <param name="foreground">控制台前景色</param>
    /// <param name="background">控制台背景色</param>
    public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
    {
        Foreground = foreground;
        Background = background;
    }

    /// <summary>
    /// 前景色
    /// </summary>
    public ConsoleColor? Foreground { get; }

    /// <summary>
    /// 背景色
    /// </summary>
    public ConsoleColor? Background { get; }
}
