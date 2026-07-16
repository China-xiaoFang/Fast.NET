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

using System.Net;
using System.Runtime.CompilerServices;
using Fast.Consul.Internal;
using Fast.Consul.Registers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

[assembly: InternalsVisibleTo("Fast.IaaS")]

namespace Fast.Consul.Filters;

/// <summary>
/// <see cref="ConsulStartupFilter"/> 应用启动时自动注册中间件
/// </summary>
internal class ConsulStartupFilter : IStartupFilter
{
    /// <summary>
    /// 配置中间件
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> action)
    {
        return app =>
        {
            if (Penetrates.ConsulSettings.Enable == true)
            {
                // 注册心跳响应
                app.Map(Penetrates.ConsulSettings.HealthCheck, options =>
                {
                    options.Run(async context =>
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.OK;
                        await context.Response.WriteAsync("OK");
                    });
                });

                // 获取 IHostApplicationLifetime 实例
                var hostApplicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

                // IServerAddressesFeature 仅在应用启动完成后可用；同时必须观察注册任务的异常。
                hostApplicationLifetime.ApplicationStarted.Register(() =>
                {
                    var registerTask = app.ApplicationServices.GetService<IConsulRegister>()
                        ?.ConsulRegisterAsync();
                    if (registerTask == null)
                        return;

                    _ = registerTask.ContinueWith(
                        task => { Console.Error.WriteLine($"[Fast.Consul] Service registration failed: {task.Exception}"); },
                        CancellationToken.None,
                        TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously,
                        TaskScheduler.Default);
                });
            }

            // 无论 Consul 是否启用，都必须继续配置应用管道。
            action(app);
        };
    }
}