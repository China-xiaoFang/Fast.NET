﻿// Apache开源许可证
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

using System.Net;
using Fast.Consul.Internal;
using Fast.Consul.Registers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fast.Consul.Filters;

/// <summary>
/// <see cref="ConsulStartupFilter"/> 应用启动时自动注册中间件
/// </summary>
public class ConsulStartupFilter : IStartupFilter
{
    /// <summary>
    /// 排序
    /// </summary>
#pragma warning disable CA1822
    public int Order => 69099;
#pragma warning restore CA1822

    /// <summary>
    /// 配置中间件
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> action)
    {
        return app =>
        {
            // 判断是否启用
            if (!Penetrates.ConsulSettings.Enable!.Value)
                return;

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

            // 订阅 ApplicationStarted 事件
            hostApplicationLifetime?.ApplicationStarted.Register(() =>
            {
                // 在应用程序完全启动后执行自定义逻辑
                // 注册 Consul 服务
                app.ApplicationServices.GetService<IConsulRegister>()?.ConsulRegisterAsync();
            });

            // 调用启动层的 Startup
            action(app);
        };
    }
}