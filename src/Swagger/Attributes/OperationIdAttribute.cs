// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

namespace Fast.Swagger;

/// <summary>
/// 配置规范化文档 OperationId 问题
/// </summary>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.Method)]
public sealed class OperationIdAttribute : Attribute
{
    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    /// <param name="operationId">operation 的唯一标识</param>
    public OperationIdAttribute(string operationId)
    {
        OperationId = operationId;
    }

    /// <summary>
    /// 自定义 OperationId
    /// </summary>
    public string OperationId { get; set; }
}
