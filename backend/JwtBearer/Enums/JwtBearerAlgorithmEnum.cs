// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using System.ComponentModel;

namespace Fast.JwtBearer.Enums;

/// <summary>
/// <see cref="JwtBearerAlgorithmEnum"/> JwtBearer 加密算法
/// </summary>
public enum JwtBearerAlgorithmEnum
{
    /// <summary>
    /// HS256，默认的
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
    EdDSA = 10,
}