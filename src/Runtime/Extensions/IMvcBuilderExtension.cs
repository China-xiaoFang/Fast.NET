// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Runtime;

/// <summary>
/// 为 <see cref="IMvcBuilder"/> 提供扩展方法
/// </summary>
[SuppressSniffer]
public static class IMvcBuilderExtension
{
    /// <summary>
    /// 注册 Mvc 过滤器
    /// </summary>
    /// <param name="builder">要配置的应用构建器</param>
    /// <param name="configure">额外的 MVC 配置操作</param>
    /// <typeparam name="TFilter">要注册的 MVC 过滤器类型</typeparam>
    /// <returns>返回当前 MVC 构建器，便于链式调用</returns>
    public static IMvcBuilder AddMvcFilter<TFilter>(this IMvcBuilder builder, Action<MvcOptions> configure = null)
        where TFilter : IFilterMetadata
    {
        builder.Services.AddMvcFilter<TFilter>(configure);

        return builder;
    }
}
