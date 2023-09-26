using Fast.Core.Extensions;
using Fast.IaaS.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Fast.Core.Filters;

/// <summary>
/// 应用启动时自动注册中间件
/// </summary>
/// <remarks>
/// </remarks>
public class StartupFilter : IStartupFilter
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
            InternalApp.RootServices = app.ApplicationServices;

            // 环境名
            var envName = App.WebHostEnvironment?.EnvironmentName ?? "Unknown";

            // 设置响应报文头信息
            app.Use(async (context, next) =>
            {
                // 处理 WebSocket 请求
                if (context.IsWebSocketRequest())
                    await next.Invoke();
                else
                {
                    // 输出当前环境标识
                    context.Response.Headers["environment"] = envName;

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
                    App.DisposeUnmanagedObjects();
                }
            });

            // 调用默认中间件
            app.UseApp();

            // 调用启动层的 Startup
            action(app);
        };
    }
}