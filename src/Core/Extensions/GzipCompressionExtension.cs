// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.NET.Core;

/// <summary>
/// 提供 Gzip 压缩扩展方法
/// </summary>
[SuppressSniffer]
public static class GzipCompressionExtension
{
    /// <summary>
    /// 添加 Gzip 压缩
    /// </summary>
    /// <param name="services">要添加服务的服务集合</param>
    /// <returns>返回 <paramref name="services"/>，便于链式调用</returns>
    public static IServiceCollection AddGzipCompression(this IServiceCollection services)
    {
        Debugging.Info("Registering for the Gzip compression service......");

        services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
        services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
            {
                "text/html; charset=utf-8", "application/xhtml+xml", "application/atom+xml", "image/svg+xml"
            });
        });

        return services;
    }

    /// <summary>
    /// 启用 Gzip 压缩
    /// </summary>
    /// <param name="app">要配置的应用管道</param>
    /// <returns>返回 <paramref name="app"/>，便于链式调用</returns>
    public static IApplicationBuilder UseGzipCompression(this IApplicationBuilder app)
    {
        // 启用 Gzip 压缩
        app.UseResponseCompression();

        return app;
    }
}
