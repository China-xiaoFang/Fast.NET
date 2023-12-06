// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
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

using Fast.IaaS;
using Fast.JwtBearer.Enums;

namespace Fast.JwtBearer.Options;

/// <summary>
/// <see cref="JWTSettingsOptions"/> Jwt 配置
/// </summary>
public sealed class JWTSettingsOptions : IPostConfigure
{
    /// <summary>
    /// 验证签发方密钥
    /// <remarks>默认false</remarks>
    /// </summary>
    public bool? ValidateIssuerSigningKey { get; set; }

    /// <summary>
    /// 签发方密钥
    /// </summary>
    public string IssuerSigningKey { get; set; }

    /// <summary>
    /// 验证签发方
    /// <remarks>默认false</remarks>
    /// </summary>
    public bool? ValidateIssuer { get; set; }

    /// <summary>
    /// 签发方
    /// </summary>
    public string ValidIssuer { get; set; }

    /// <summary>
    /// 验证签收方
    /// <remarks>默认false</remarks>
    /// </summary>
    public bool? ValidateAudience { get; set; }

    /// <summary>
    /// 签收方
    /// </summary>
    public string ValidAudience { get; set; }

    /// <summary>
    /// 验证生存期
    /// <remarks>默认false</remarks>
    /// </summary>
    public bool? ValidateLifetime { get; set; }

    /// <summary>
    /// 过期时间容错值，解决服务器端时间不同步问题（秒）
    /// <remarks>默认5秒</remarks>
    /// </summary>
    public long? ClockSkew { get; set; }

    /// <summary>
    /// Token 过期时间（分钟）
    /// <remarks>默认20分钟</remarks>
    /// </summary>
    public long? TokenExpiredTime { get; set; }

    /// <summary>
    /// 刷新Token 过期时间（分钟）
    /// <remarks>默认1440分钟(24小时)</remarks>
    /// </summary>
    public long? RefreshTokenExpireTime { get; set; }

    /// <summary>
    /// 加密算法
    /// <remarks>默认HS256</remarks>
    /// </summary>
    public JwtBearerAlgorithmEnum? Algorithm { get; set; }

    /// <summary>
    /// 后期配置
    /// </summary>
    public void PostConfigure()
    {
        ValidateIssuerSigningKey ??= false;
        ValidateIssuer ??= false;
        ValidateAudience ??= false;
        ValidateLifetime ??= false;
        ClockSkew ??= 5;
        TokenExpiredTime ??= 20;
        RefreshTokenExpireTime ??= 1440;
        Algorithm ??= JwtBearerAlgorithmEnum.HS256;
    }
}