// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

namespace Fast.UnifyResult;

/// <summary>
/// 异常元数据
/// </summary>
[SuppressSniffer]
public sealed class ExceptionMetadata
{
    /// <summary>
    /// 状态码
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// 错误码
    /// </summary>
    public object ErrorCode { get; set; }

    /// <summary>
    /// 原始错误码（未被覆盖的 <see cref="ErrorCode"/>）
    /// </summary>
    public object OriginErrorCode { get; set; }

    /// <summary>
    /// 错误对象（信息）
    /// </summary>
    public object Errors { get; set; }

    /// <summary>
    /// 额外数据
    /// </summary>
    public object Data { get; set; }
}
