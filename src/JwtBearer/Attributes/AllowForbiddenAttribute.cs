// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

namespace Fast.JwtBearer;

/// <summary>
/// 跳过 Fast 自身的权限检查，不跳过身份、角色、声明或第三方授权要求。
/// </summary>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowForbiddenAttribute : Attribute
{
    /// <inheritdoc />
    public override string ToString()
    {
        return "AllowForbidden";
    }
}
