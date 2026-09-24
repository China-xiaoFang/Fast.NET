// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

namespace Fast.Runtime;

/// <summary>
/// 用户代理信息
/// </summary>
[SuppressSniffer]
public class UserAgentInfo
{
    /// <summary>
    /// 设备
    /// </summary>
    public string Device { get; set; }

    /// <summary>
    /// 操作系统（版本）
    /// </summary>
    public string OS { get; set; }

    /// <summary>
    /// 浏览器（版本）
    /// </summary>
    public string Browser { get; set; }
}
