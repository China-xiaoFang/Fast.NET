// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using Microsoft.Extensions.Configuration;

namespace Fast.Runtime;

/// <summary>
/// 为 <see cref="IConfiguration"/> 提供扩展方法
/// </summary>
[SuppressSniffer]
public static class IConfigurationExtension
{
    /// <summary>
    /// 加载后期配置
    /// </summary>
    /// <param name="type">目标类型</param>
    /// <typeparam name="T">要执行后期配置的选项类型</typeparam>
    /// <returns>加载后的后期配置</returns>
    public static T LoadPostConfigure<T>(this T type) where T : IPostConfigure
    {
        // 空值判断
        type ??= Activator.CreateInstance<T>();

        // 加载后期配置
        type.PostConfigure();

        return type;
    }
}
