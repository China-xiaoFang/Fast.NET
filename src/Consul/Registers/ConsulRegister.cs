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

using System.Reflection;
using Consul;
using Fast.Consul.Options;
using Fast.IaaS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Options;

namespace Fast.Consul.Registers;

/// <summary>
/// <see cref="ConsulRegister"/> Consul 服务注册
/// </summary>
internal class ConsulRegister : IConsulRegister
{
    private readonly IServer _server;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly ConsulSettingsOptions _consulSettingsOptions;

    public ConsulRegister(IServer server, IWebHostEnvironment webHostEnvironment,
        IOptionsMonitor<ConsulSettingsOptions> consulSettingsOptions)
    {
        _server = server;
        _webHostEnvironment = webHostEnvironment;
        _consulSettingsOptions = consulSettingsOptions.CurrentValue;
    }

    /// <summary>
    /// 服务注册
    /// </summary>
    /// <returns></returns>
    public async Task ConsulRegisterAsync()
    {
        using var client = new ConsulClient(options =>
        {
            // Consul 客户端地址
            options.Address = new Uri(_consulSettingsOptions.Address);
        });

        // 获取当前程序启动的地址
        var startupAddress = string.IsNullOrWhiteSpace(_consulSettingsOptions.ServiceAddress)
            ? _server.Features.Get<IServerAddressesFeature>()
                ?.Addresses.FirstOrDefault()
            : _consulSettingsOptions.ServiceAddress;
        if (!Uri.TryCreate(startupAddress, UriKind.Absolute, out var startupUri))
            throw new InvalidOperationException("无法从服务器功能中获取有效的应用监听地址，Consul 服务注册已终止。");

        if (startupUri.Host is "0.0.0.0" or "::" or "[::]")
            throw new InvalidOperationException("应用监听的是通配地址，无法直接注册到 Consul；请配置 ConsulSettings:ServiceAddress。");

        // TODO：后续可以考虑读取根目录父级文件夹的名称做版本区分

        // 获取当前入口程序集的版本号
        var version = Assembly.GetEntryAssembly()
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        var versionArr = version?.Split('.');

        // TODO：这里考虑是否要放开副修订版本号，类似 v1.1.1.1 最后的 .1 是否算一个单独的版本，还是说算入 v1.1.1 版本

        // 处理存在副修订版本号的情况
        if (versionArr?.Length >= 4)
        {
            version = $"{versionArr[0]}.{versionArr[1]}.{versionArr[2]}";
        }

        var registration = new AgentServiceRegistration
        {
            // 唯一ID
            ID = GuidUtil.GetGuid(),
            // 服务名，
            Name = _webHostEnvironment.ApplicationName + $"{(string.IsNullOrEmpty(version) ? null : $"_v{version}")}",
            // 服务绑定IP
            Address = startupUri.Host,
            // 服务绑定端口
            Port = startupUri.Port,
            // Tag 标签
            Check = new AgentServiceCheck
            {
                // 服务启动后多久注册
                DeregisterCriticalServiceAfter =
                    TimeSpan.FromSeconds(_consulSettingsOptions.DeregisterCriticalServiceAfter!.Value),
                // 健康检查时间间隔
                Interval = TimeSpan.FromSeconds(_consulSettingsOptions.HealthCheckInterval!.Value),
                // 健康检查地址
                HTTP = $"{startupUri.AbsoluteUri.TrimEnd('/')}{_consulSettingsOptions.HealthCheck}",
                // 健康检查超时时间
                Timeout = TimeSpan.FromSeconds(_consulSettingsOptions.HealthCheckTimeout!.Value)
            }
        };

        await client.Agent.ServiceRegister(registration)
            .ConfigureAwait(false);
    }
}