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

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Fast.UnifyResult;

/// <summary>
/// <see cref="IServiceCollection"/> 统一返回 拓展类
/// </summary>
[SuppressSniffer]
public static class IServiceCollectionExtension
{
    /// <summary>
    /// 数据验证服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
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
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddFriendlyException(this IServiceCollection services)
    {
        Debugging.Info("Registering friendly exception......");

        // 查找全局异常处理实现类
        var globalExceptionHandler =
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
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
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
        var unifyResponseProvider =
            MAppContext.EffectiveTypes.FirstOrDefault(f => typeof(IUnifyResponseProvider).IsAssignableFrom(f) && !f.IsInterface);

        if (unifyResponseProvider != null)
        {
            // 注册规范化响应数据提供器实现类
            services.AddSingleton(typeof(IUnifyResponseProvider), unifyResponseProvider);
        }

        var unifyResultProvider =
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