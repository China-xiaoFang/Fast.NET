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

using System.ComponentModel.DataAnnotations;
using Fast.IaaS;

namespace Fast.CorsAccessor.Options;

/// <summary>
/// 跨域配置选项
/// </summary>
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