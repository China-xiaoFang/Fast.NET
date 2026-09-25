// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Fast.Logging;

/// <summary>
/// 应用启动时自动注册中间件
/// </summary>
internal sealed class LoggingStartupFilter : IStartupFilter
{
    /// <inheritdoc />
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> action)
    {
        return app =>
        {
            // 存储根服务
            Penetrates.RootServices = app.ApplicationServices;

            // 调用启动层的 Startup
            action(app);
        };
    }
}
