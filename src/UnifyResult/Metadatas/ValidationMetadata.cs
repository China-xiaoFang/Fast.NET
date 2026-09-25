// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Fast.UnifyResult;

/// <summary>
/// 验证信息元数据
/// </summary>
[SuppressSniffer]
public sealed class ValidationMetadata
{
    /// <summary>
    /// 验证结果
    /// </summary>
    /// <remarks>返回字典或字符串类型</remarks>
    public object ValidationResult { get; set; }

    /// <summary>
    /// 异常消息
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 验证状态
    /// </summary>
    public ModelStateDictionary ModelState { get; set; }

    /// <summary>
    /// 错误码
    /// </summary>
    public object ErrorCode { get; set; }

    /// <summary>
    /// 原始错误码（未被覆盖的 <see cref="ErrorCode"/>）
    /// </summary>
    public object OriginErrorCode { get; set; }

    /// <summary>
    /// 状态码
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// 首个错误属性
    /// </summary>
    public string FirstErrorProperty { get; set; }

    /// <summary>
    /// 首个错误消息
    /// </summary>
    public string FirstErrorMessage { get; set; }

    /// <summary>
    /// 额外数据
    /// </summary>
    public object Data { get; set; }
}
