// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using Fast.Runtime;

// ReSharper disable once CheckNamespace
namespace Fast.JwtBearer;

/// <summary>
/// <see cref="JWTSettingsOptions"/> Jwt 配置
/// </summary>
[SuppressSniffer]
public sealed class JWTSettingsOptions : IPostConfigure
{
    /// <summary>
    /// 验证签发方密钥
    /// </summary>
    /// <remarks>默认false</remarks>
    public bool? ValidateIssuerSigningKey { get; set; }

    /// <summary>
    /// 签发方密钥
    /// </summary>
    public string IssuerSigningKey { get; set; }

    /// <summary>
    /// 验证签发方
    /// </summary>
    /// <remarks>默认false</remarks>
    public bool? ValidateIssuer { get; set; }

    /// <summary>
    /// 签发方
    /// </summary>
    public string ValidIssuer { get; set; }

    /// <summary>
    /// 验证签收方
    /// </summary>
    /// <remarks>默认false</remarks>
    public bool? ValidateAudience { get; set; }

    /// <summary>
    /// 签收方
    /// </summary>
    public string ValidAudience { get; set; }

    /// <summary>
    /// 验证生存期
    /// </summary>
    /// <remarks>默认false</remarks>
    public bool? ValidateLifetime { get; set; }

    /// <summary>
    /// 验证 AccessToken
    /// </summary>
    /// <remarks>
    /// <para>默认false</para>
    /// <para>需调用 <see cref="JwtBearerUtil.SetExpiredToken"/> 才会验证</para>
    /// </remarks>
    public bool? ValidateAccessToken { get; set; }

    /// <summary>
    /// 过期时间容错值，解决服务器端时间不同步问题（秒）
    /// </summary>
    /// <remarks>默认5秒</remarks>
    public long? ClockSkew { get; set; }

    /// <summary>
    /// Token 过期时间（分钟）
    /// </summary>
    /// <remarks>默认20分钟</remarks>
    public long? TokenExpiredTime { get; set; }

    /// <summary>
    /// 刷新Token 过期时间（分钟）
    /// </summary>
    /// <remarks>默认1440分钟(24小时)</remarks>
    public long? RefreshTokenExpireTime { get; set; }

    /// <summary>
    /// 加密算法
    /// </summary>
    /// <remarks>默认HS256</remarks>
    public JwtBearerAlgorithmEnum? Algorithm { get; set; }

    /// <summary>
    /// 启用
    /// </summary>
    /// <remarks>默认true</remarks>
    public bool? Enable { get; set; }

    /// <summary>
    /// 后期配置
    /// </summary>
    public void PostConfigure()
    {
        ValidateIssuerSigningKey ??= false;
        IssuerSigningKey ??= "Ax5/zZ8NB~[%^SIJsL)1o2Mw4jm6eg73";
        ValidateIssuer ??= false;
        ValidIssuer ??= "Fast.NET.API";
        ValidateAudience ??= false;
        ValidAudience ??= "Fast.NET.Client";
        ValidateLifetime ??= false;
        ValidateAccessToken ??= false;
        ClockSkew ??= 5;
        TokenExpiredTime ??= 20;
        RefreshTokenExpireTime ??= 1440;
        Algorithm ??= JwtBearerAlgorithmEnum.HS256;
        Enable ??= true;
    }
}