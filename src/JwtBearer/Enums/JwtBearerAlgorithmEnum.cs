// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.ComponentModel;

namespace Fast.JwtBearer;

/// <summary>
/// JwtBearer 加密算法
/// </summary>
[FastEnum("JwtBearer 加密算法")]
public enum JwtBearerAlgorithmEnum : byte
{
    /// <summary>
    /// HS256 <para>默认的</para>
    /// </summary>
    [Description("HS256")]
    HS256 = 0,

    /// <summary>
    /// HS384
    /// </summary>
    [Description("HS384")]
    HS384 = 1,

    /// <summary>
    /// HS512
    /// </summary>
    [Description("HS512")]
    HS512 = 2,

    /// <summary>
    /// PS256
    /// </summary>
    [Description("PS256")]
    PS256 = 3,

    /// <summary>
    /// PS384
    /// </summary>
    [Description("PS384")]
    PS384 = 4,

    /// <summary>
    /// PS512
    /// </summary>
    [Description("PS512")]
    PS512 = 5,

    /// <summary>
    /// ES256
    /// </summary>
    [Description("ES256")]
    ES256 = 6,

    /// <summary>
    /// ES256K
    /// </summary>
    [Description("ES256K")]
    ES256K = 7,

    /// <summary>
    /// ES384
    /// </summary>
    [Description("ES384")]
    ES384 = 8,

    /// <summary>
    /// ES512
    /// </summary>
    [Description("ES512")]
    ES512 = 9,

    /// <summary>
    /// EdDSA
    /// </summary>
    [Description("EdDSA")]
    EdDSA = 10
}
