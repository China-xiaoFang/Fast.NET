// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using System.Text;

namespace Fast.OpenApi;

/// <summary>
/// OpenAPI 文档组件声明 DTO
/// </summary>
internal sealed class ComponentSchemaDto
{
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 导入路径
    /// </summary>
    /// <remarks>枚举专用</remarks>
    public string ImportPath { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    /// <remarks>DTO 专用</remarks>
    public StringBuilder Content { get; set; }

    /// <summary>
    /// 引用声明
    /// </summary>
    /// <remarks>DTO 专用</remarks>
    public HashSet<string> RefSchemas { get; set; }
}
