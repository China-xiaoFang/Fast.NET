// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using Microsoft.OpenApi;

namespace Fast.Swagger;

/// <summary>
/// 安全定义需求子项
/// </summary>
[SuppressSniffer]
public sealed class SwaggerOpenApiSecurityRequirementItem
{
    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    public SwaggerOpenApiSecurityRequirementItem()
    {
        Accesses = Array.Empty<string>();
    }

    /// <summary>
    /// 安全 Schema
    /// </summary>
    public OpenApiSecurityScheme Scheme { get; set; }

    /// <summary>
    /// 权限
    /// </summary>
    public string[] Accesses { get; set; }
}
