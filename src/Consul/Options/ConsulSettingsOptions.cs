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

namespace Fast.Consul.Options;

/// <summary>
/// <see cref="ConsulSettingsOptions"/> Consul配置选项
/// </summary>
public sealed class ConsulSettingsOptions : IPostConfigure
{
    /// <summary>
    /// Consul 是否启用
    /// </summary>
    [Required]
    public bool? Enable { get; set; }

    /// <summary>
    /// Consul 客户端地址
    /// </summary>
    [Required]
    public string Address { get; set; }

    /// <summary>
    /// 对外注册的服务地址；为空时使用服务器实际监听地址。
    /// </summary>
    /// <remarks>容器或反向代理场景应显式配置可被 Consul 访问的绝对地址。</remarks>
    public string ServiceAddress { get; set; }

    /// <summary>
    /// Consul 健康检查地址
    /// </summary>
    [Required]
    public string HealthCheck { get; set; }

    /// <summary>
    /// Consul 服务启动后多久注册，单位秒
    /// </summary>
    public int? DeregisterCriticalServiceAfter { get; set; }

    /// <summary>
    /// Consul 健康检查时间间隔，单位秒
    /// </summary>
    public int? HealthCheckInterval { get; set; }

    /// <summary>
    /// Consul 健康检查超时时间，单位秒
    /// </summary>
    public int? HealthCheckTimeout { get; set; }

    /// <summary>
    /// 后期配置
    /// </summary>
    public void PostConfigure()
    {
        Enable ??= true;
        Address ??= "http://127.0.0.1:8500";
        HealthCheck ??= "/healthCheck";
        DeregisterCriticalServiceAfter ??= 60;
        HealthCheckInterval ??= 10;
        HealthCheckTimeout ??= 5;

        if (!Uri.TryCreate(Address, UriKind.Absolute, out var consulUri) || consulUri.Scheme is not ("http" or "https"))
            throw new InvalidOperationException("ConsulSettings:Address 必须是有效的 HTTP/HTTPS 绝对地址。");

        if (!string.IsNullOrWhiteSpace(ServiceAddress)
            && (!Uri.TryCreate(ServiceAddress, UriKind.Absolute, out var serviceUri)
                || serviceUri.Scheme is not ("http" or "https")))
            throw new InvalidOperationException("ConsulSettings:ServiceAddress 必须是有效的 HTTP/HTTPS 绝对地址。");

        if (string.IsNullOrWhiteSpace(HealthCheck))
            throw new InvalidOperationException("ConsulSettings:HealthCheck 不能为空。");
        if (!HealthCheck.StartsWith('/'))
            HealthCheck = "/" + HealthCheck;

        if (DeregisterCriticalServiceAfter <= 0 || HealthCheckInterval <= 0 || HealthCheckTimeout <= 0)
            throw new InvalidOperationException("Consul 健康检查和服务摘除时间必须大于 0 秒。");
    }
}