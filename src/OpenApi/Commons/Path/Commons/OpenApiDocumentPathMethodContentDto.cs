// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using System.Text.Json.Serialization;

namespace Fast.OpenApi;

/// <summary>
/// OpenAPI 文档路由内容 DTO
/// </summary>
public class OpenApiDocumentPathMethodContentDto
{
    /// <summary>
    /// JSON 格式
    /// </summary>
    [JsonPropertyName("application/json")]
    public OpenApiDocumentPathMethodContentSchemaDto Json { get; set; }

    /// <summary>
    /// 表单格式
    /// </summary>
    [JsonPropertyName("multipart/form-data")]
    public OpenApiDocumentPathMethodContentSchemaDto FormData { get; set; }
}
