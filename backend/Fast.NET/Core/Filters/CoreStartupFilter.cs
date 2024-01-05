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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Fast.NET.Core.Filters;

/// <summary>
/// <see cref="CoreStartupFilter"/> 应用启动时自动注册中间件
/// </summary>
public class CoreStartupFilter : IStartupFilter
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
            // 存储根服务
            InternalContext.RootServices = app.ApplicationServices;

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
                    context.Response.Headers["Fast-Environment"] = envName;

                    // 默认输出信息
                    context.Response.Headers["Fast-Site-Url"] = "https://fastdotnet.com";
                    context.Response.Headers["Fast-Repository-Url"] = "https://gitee.com/Net-18K/Fast.NET";

                    // 输出当前请求时间
                    context.Response.Headers["Fast-Request-Time"] =
                        DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff zzz dddd");

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
                    FastContext.DisposeUnmanagedObjects();
                }
            });

            // 调用启动层的 Startup
            action(app);
        };
    }
}