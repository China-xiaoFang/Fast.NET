﻿// Apache开源许可证
//
// 版权所有 © 2018-Now 小方
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

using System.Globalization;
using Fast.IaaS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Fast.NET.Core.Filters;

/// <summary>
/// <see cref="CoreStartupFilter"/> 应用启动时自动注册中间件
/// </summary>
internal class CoreStartupFilter : IStartupFilter
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
            // 存储应用程序构建起
            FastContext.WebApplication = app;

            // 存储根服务
            FastContext.RootServices = app.ApplicationServices;

            // 解决 IIS 或者 Nginx 反向代理获取不到真实客户端IP的问题
            app.UseForwardedHeaders();

            // 环境名
            var envName = FastContext.WebHostEnvironment?.EnvironmentName ?? "Unknown";

            // 设置响应报文头信息
            app.Use(async (context, next) =>
            {
                // 处理 WebSocket 请求
                if (context.IsWebSocketRequest())
                {
                    await next.Invoke();
                }
                else
                {
                    // 输出当前环境标识
                    context.Response.Headers.TryAdd(nameof(Fast) + "-Environment", envName);

                    // 默认输出信息
                    context.Response.Headers.TryAdd(nameof(Fast) + "-Site-Url", "https://fastdotnet.com");
                    context.Response.Headers.TryAdd(nameof(Fast) + "-Repository-Url", "https://gitee.com/Net-18K/Fast.NET");

                    // 输出当前请求时间
                    context.Response.Headers.TryAdd(nameof(Fast) + "-Request-Time",
                        DateTimeOffset.Now.ToString("dddd, zzz, yyyy-MM-dd HH:mm:ss.fffffff", new CultureInfo("en-US")));

                    // 执行下一个中间件
                    await next.Invoke();

                    // 解决刷新 Token 时间和 Token 时间相近问题
                    if (!context.Response.HasStarted && context.Response.StatusCode == StatusCodes.Status401Unauthorized &&
                        context.Response.Headers.ContainsKey("access-token") &&
                        context.Response.Headers.ContainsKey("x-access-token"))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    }

                    // 释放所有未托管的服务提供器
                    IaaSContext.DisposeUnmanagedObjects();
                }
            });

            // 调用启动层的 Startup
            action(app);
        };
    }
}