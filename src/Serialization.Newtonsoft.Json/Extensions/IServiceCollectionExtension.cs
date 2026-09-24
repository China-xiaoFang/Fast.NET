// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Serialization;

/// <summary>
/// 为 <see cref="IServiceCollection"/> 提供 Newtonsoft.Json 扩展方法
/// </summary>
public static class IServiceCollectionExtension
{
    /// <summary>
    /// 添加 Newtonsoft.Json 序列化服务
    /// </summary>
    /// <param name="services">要添加服务的服务集合</param>
    /// <param name="configureOptions">JSON 配置操作</param>
    /// <returns>返回 <paramref name="services"/>，便于链式调用</returns>
    public static IServiceCollection AddSerialization(this IServiceCollection services,
        Action<MvcNewtonsoftJsonOptions> configureOptions = null)
    {
        // 非 Web 环境配置
        var jsonOptions = new MvcNewtonsoftJsonOptions();
        JsonContext.JsonOptionsAction.Invoke(jsonOptions);
        configureOptions?.Invoke(jsonOptions);
        JsonContext.SerializerOptions = jsonOptions.SerializerSettings;

        services.Configure(JsonContext.JsonOptionsAction);

        return services;
    }
}
