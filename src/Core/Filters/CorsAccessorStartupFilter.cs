// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Fast.NET.Core;

/// <summary>
/// 应用启动时自动注册中间件
/// </summary>
internal sealed class CorsAccessorStartupFilter : IStartupFilter
{
    /// <inheritdoc />
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> action)
    {
        return app =>
        {
            // 获取选项
            CorsAccessorSettingsOptions corsAccessorSettings = app.ApplicationServices
                .GetService<IOptions<CorsAccessorSettingsOptions>>()
                .Value;

            // 判断是否启用 SignalR 跨域支持
            if (corsAccessorSettings.SignalRSupport == false)
            {
                // 配置跨域中间件
                app.UseCors(corsAccessorSettings.PolicyName);
            }
            else
            {
                // 配置跨域中间件
                app.UseCors(builder => { CorsAccessorExtension.SetCorsPolicy(builder, corsAccessorSettings, true); });
            }

            // 调用启动层的 Startup
            action(app);
        };
    }
}
