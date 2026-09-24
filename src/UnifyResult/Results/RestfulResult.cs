// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

namespace Fast.UnifyResult;

/// <summary>
/// RESTful 风格返回格式
/// </summary>
/// <typeparam name="T">该类型处理的值类型</typeparam>
[SuppressSniffer]
public class RestfulResult<T>
{
    /// <summary>
    /// 执行成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 状态码
    /// </summary>
    public int? Code { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public object Message { get; set; }

    /// <summary>
    /// 数据
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public long Timestamp { get; set; }
}
