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

using System.ComponentModel.DataAnnotations;
using Fast.Runtime;

// ReSharper disable once CheckNamespace
namespace Fast.NET.Core;

/// <summary>
/// <see cref="CorsAccessorSettingsOptions"/> 跨域配置选项
/// </summary>
[SuppressSniffer]
public sealed class CorsAccessorSettingsOptions : IPostConfigure
{
    /// <summary>
    /// 策略名称
    /// </summary>
    [Required]
    public string PolicyName { get; set; }

    /// <summary>
    /// 允许来源域名，没有配置则允许所有来源
    /// </summary>
    public string[] WithOrigins { get; set; }

    /// <summary>
    /// 请求表头，没有配置则允许所有表头
    /// </summary>
    public string[] WithHeaders { get; set; }

    /// <summary>
    /// 设置客户端可获取的响应标头
    /// </summary>
    public string[] WithExposedHeaders { get; set; }

    /// <summary>
    /// 设置跨域允许请求谓词，没有配置则允许所有
    /// </summary>
    public string[] WithMethods { get; set; }

    /// <summary>
    /// 是否允许跨域请求中的凭据
    /// </summary>
    public bool? AllowCredentials { get; set; }

    /// <summary>
    /// 设置预检过期时间
    /// </summary>
    public int? SetPreflightMaxAge { get; set; }

    /// <summary>
    /// 修正前端无法获取 Token 问题
    /// </summary>
    public bool? FixedClientToken { get; set; }

    /// <summary>
    /// 启用 SignalR 跨域支持
    /// </summary>
    public bool? SignalRSupport { get; set; }

    /// <summary>
    /// 后期配置
    /// </summary>
    public void PostConfigure()
    {
        PolicyName ??= "App.Cors.Policy";
        WithOrigins ??= Array.Empty<string>();
        AllowCredentials ??= true;
        FixedClientToken ??= true;
        SignalRSupport ??= false;
        // 默认支持前端非代理情况，使用Axios的情况
        WithExposedHeaders ??= new[] {"Content-Disposition"};
    }
}