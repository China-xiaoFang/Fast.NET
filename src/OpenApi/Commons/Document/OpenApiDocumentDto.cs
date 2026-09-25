// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using System.Text.Json.Serialization;

namespace Fast.OpenApi;

/// <summary>
/// OpenAPI 文档 DTO
/// </summary>
public class OpenApiDocumentDto
{
    /// <summary>
    /// 地址
    /// </summary>
    /// <remarks><see href="http://127.0.0.1:38080/swagger/All Groups/swagger.json"/></remarks>
    public string Url { get; set; }

    /// <summary>
    /// OpenAPI 版本
    /// </summary>
    [JsonPropertyName("openapi")]
    public string OpenApi { get; set; }

    /// <summary>
    /// 文档信息
    /// </summary>
    public OpenApiDocumentInfoDto Info { get; set; }

    /// <summary>
    /// 路由
    /// </summary>
    public IDictionary<string, OpenApiDocumentPathDto> Paths { get; set; }

    /// <summary>
    /// 组件
    /// </summary>
    public OpenApiDocumentComponentDto Components { get; set; }

    /// <summary>
    /// 模块
    /// </summary>
    public List<OpenApiDocumentTagDto> Tags { get; set; }
}
