// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

namespace Fast.OpenApi;

/// <summary>
/// OpenAPI 文档路由请求方法 DTO
/// </summary>
public class OpenApiDocumentPathMethodDto
{
    /// <summary>
    /// 模块
    /// </summary>
    /// <remarks>这里一般默认获取第一个即可</remarks>
    public List<string> Tags { get; set; }

    /// <summary>
    /// 接口名称
    /// </summary>
    public string Summary { get; set; }

    /// <summary>
    /// 操作Id
    /// </summary>
    public string OperationId { get; set; }

    /// <summary>
    /// URL 参数
    /// </summary>
    public List<OpenApiDocumentPathMethodParameterDto> Parameters { get; set; }

    /// <summary>
    /// Body 参数
    /// </summary>
    public OpenApiDocumentPathMethodRequestBodyDto RequestBody { get; set; }

    /// <summary>
    /// 响应
    /// </summary>
    public OpenApiDocumentPathMethodResponseDto Responses { get; set; }
}
