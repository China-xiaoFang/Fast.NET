// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fast.UnifyResult;

/// <summary>
/// 为 <see cref="IServiceCollection"/> 提供统一返回扩展方法
/// </summary>
[SuppressSniffer]
public static class IServiceCollectionExtension
{
    /// <summary>
    /// 数据验证服务
    /// </summary>
    /// <param name="services">要添加服务的服务集合</param>
    /// <returns>返回 <paramref name="services"/>，便于链式调用</returns>
    public static IServiceCollection AddDataValidation(this IServiceCollection services)
    {
        Debugging.Info("Registering data validation......");

        // 启用了全局验证，则默认关闭原生 ModelStateInvalidFilter 验证
        services.Configure<ApiBehaviorOptions>(options =>
        {
            // 是否禁用映射异常
            options.SuppressMapClientErrors = false;
            // 是否禁用模型验证过滤器
            options.SuppressModelStateInvalidFilter = true;
        });

        // 添加全局数据验证
        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add<DataValidationFilter>();

            // 关闭空引用对象验证
            options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
        });

        return services;
    }

    /// <summary>
    /// 友好异常服务
    /// </summary>
    /// <param name="services">要添加服务的服务集合</param>
    /// <returns>返回 <paramref name="services"/>，便于链式调用</returns>
    public static IServiceCollection AddFriendlyException(this IServiceCollection services)
    {
        Debugging.Info("Registering friendly exception......");

        // 查找全局异常处理实现类
        Type globalExceptionHandler =
            MAppContext.EffectiveTypes.FirstOrDefault(f => typeof(IGlobalExceptionHandler).IsAssignableFrom(f) && !f.IsInterface);

        if (globalExceptionHandler != null)
        {
            // 注册全局异常处理实现类
            services.AddSingleton(typeof(IGlobalExceptionHandler), globalExceptionHandler);
        }

        services.Configure<MvcOptions>(options => { options.Filters.Add<FriendlyExceptionFilter>(); });

        return services;
    }

    /// <summary>
    /// 添加统一返回服务
    /// </summary>
    /// <param name="services">要添加服务的服务集合</param>
    /// <returns>返回 <paramref name="services"/>，便于链式调用</returns>
    public static IServiceCollection AddUnifyResult(this IServiceCollection services)
    {
        Debugging.Info("Registering unify result......");

        // 是否启用规范化结果
        UnifyContext.EnabledUnifyHandler = true;

        // 数据验证
        services.AddDataValidation();

        // 友好异常
        services.AddFriendlyException();

        // 查找规范化响应数据提供器实现类
        Type unifyResponseProvider =
            MAppContext.EffectiveTypes.FirstOrDefault(f => typeof(IUnifyResponseProvider).IsAssignableFrom(f) && !f.IsInterface);

        if (unifyResponseProvider != null)
        {
            // 注册规范化响应数据提供器实现类
            services.AddSingleton(typeof(IUnifyResponseProvider), unifyResponseProvider);
        }

        Type unifyResultProvider =
            MAppContext.EffectiveTypes.FirstOrDefault(f => typeof(IUnifyResultProvider).IsAssignableFrom(f) && !f.IsInterface);

        if (unifyResultProvider != null)
        {
            // 注册规范化提供器
            services.AddSingleton(typeof(IUnifyResultProvider), unifyResultProvider);
        }
        else
        {
            // 添加规范化提供器
            services.TryAddSingleton<IUnifyResultProvider, RestfulResultProvider>();
        }

        // 添加成功规范化结果筛选器
        services.Configure<MvcOptions>(options => { options.Filters.Add<SucceededUnifyResultFilter>(); });

        // 注册 UnifyResult Startup 过滤器
        services.AddTransient(typeof(IStartupFilter), typeof(UnifyResultStartupFilter));

        return services;
    }
}
