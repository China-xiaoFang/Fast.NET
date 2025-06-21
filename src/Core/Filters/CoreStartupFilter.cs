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

using System.Globalization;
using Fast.Runtime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

// ReSharper disable once CheckNamespace
namespace Fast.NET.Core;

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
                    // 输出当前程序集版本号
                    context.Response.Headers.TryAdd(nameof(Fast) + "-Version", $"v{MAppContext.AssemblyVersion}");

                    // 输出当前环境标识
                    context.Response.Headers.TryAdd(nameof(Fast) + "-Environment", envName);

                    // 输出当前请求时间
                    context.Response.Headers.TryAdd(nameof(Fast) + "-Request-Time",
                        DateTimeOffset.Now.ToString("dddd, zzz, yyyy-MM-dd HH:mm:ss.fffffff", new CultureInfo("en-US")));

                    // 默认输出信息
                    context.Response.Headers.TryAdd(nameof(Fast) + "-Site-Url", "https://fastdotnet.com");
                    context.Response.Headers.TryAdd(nameof(Fast) + "-Repository-Url",
                        "https://gitee.com/China-xiaoFang/Fast.NET");

                    // 执行下一个中间件
                    await next.Invoke();

                    // 解决刷新 Token 时间和 Token 时间相近问题
                    if (!context.Response.HasStarted
                        && context.Response.StatusCode == StatusCodes.Status401Unauthorized
                        && context.Response.Headers.ContainsKey("access-token")
                        && context.Response.Headers.ContainsKey("x-access-token"))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    }

                    // 释放所有未托管的服务提供器
                    MAppContext.DisposeUnmanagedObjects();
                }
            });

            // 调用启动层的 Startup
            action(app);
        };
    }
}