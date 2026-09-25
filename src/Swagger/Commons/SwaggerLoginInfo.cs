// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

namespace Fast.Swagger;

/// <summary>
/// Swagger 文档授权登录配置信息
/// </summary>
[SuppressSniffer]
public sealed class SwaggerLoginInfo
{
    /// <summary>
    /// 是否启用授权控制
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// 检查登录地址
    /// </summary>
    public string CheckUrl { get; set; }

    /// <summary>
    /// 提交登录地址
    /// </summary>
    public string SubmitUrl { get; set; }
}
