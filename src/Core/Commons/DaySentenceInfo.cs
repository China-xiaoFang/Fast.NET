// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.Text.Json.Serialization;

namespace Fast.NET.Core;

/// <summary>
/// 每日一句信息
/// </summary>
[SuppressSniffer]
public class DaySentenceInfo
{
    /// <summary>
    /// 图片 2
    /// </summary>
    public string Picture2 { get; set; }

    /// <summary>
    /// 说明
    /// </summary>
    public string Caption { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// 英文内容
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// 分享图片
    /// </summary>
    [JsonPropertyName("fenxiang_img")]
    public string ShareImg { get; set; }

    /// <summary>
    /// 时间
    /// </summary>
    [JsonPropertyName("dateline")]
    public DateTime DateTime { get; set; }
}
