// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

namespace Fast.OpenApi;

/// <summary>
/// OpenAPI 文档路由请求方法参数 DTO
/// </summary>
public class OpenApiDocumentPathMethodParameterDto
{
    /// <summary>
    /// 参数名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 参数来源位置，例如 query、path、header 或 cookie
    /// </summary>
    public string In { get; set; }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 声明
    /// </summary>
    public OpenApiDocumentSchemaPropertyDto Schema { get; set; }
}
