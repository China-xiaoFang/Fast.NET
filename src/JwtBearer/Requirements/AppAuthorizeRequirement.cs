// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using Microsoft.AspNetCore.Authorization;

namespace Fast.JwtBearer;

/// <summary>
/// 策略对应的需求
/// </summary>
[SuppressSniffer]
public sealed class AppAuthorizeRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    /// <param name="policies">授权策略名称集合</param>
    public AppAuthorizeRequirement(params string[] policies)
    {
        Policies = policies;
    }

    /// <summary>
    /// 策略
    /// </summary>
    public string[] Policies { get; private set; }
}
