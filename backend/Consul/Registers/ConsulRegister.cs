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
        var client = new ConsulClient(options =>
        {
            // Consul 客户端地址
            options.Address = new Uri(_consulSettingsOptions.Address);
        });

        // 获取当前程序启动的地址
        var startupUri = new Uri(_server.Features?.Get<IServerAddressesFeature>()?.Addresses?.First());

        // TODO：后续可以考虑读取根目录父级文件夹的名称做版本区分

        // 获取当前入口程序集的版本号
        var version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
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
                Timeout = TimeSpan.FromSeconds(_consulSettingsOptions.HealthCheckTimeout!.Value),
            }
        };

        await client.Agent.ServiceRegister(registration);
    }
}