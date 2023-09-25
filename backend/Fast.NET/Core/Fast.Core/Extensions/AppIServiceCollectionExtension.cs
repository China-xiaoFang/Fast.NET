using Fast.Core.DataValidation.Extensions;
using Fast.Core.DynamicApiController.Extensions;
using Fast.Core.FriendlyException.Extensions;
using Fast.Core.UnifyResult.Extensions;
using Fast.Core.UnifyResult.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> Fast.NET 应用服务集合拓展类
/// </summary>
public static class AppIServiceCollectionExtension
{
    /// <summary>
    /// 服务注入基础配置（带规范化文档）
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IMvcBuilder AddInject(this IMvcBuilder builder)
    {
        builder.Services.AddInject();

        return builder;
    }

    /// <summary>
    /// 服务注入基础配置（带规范化文档）
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddInject(this IServiceCollection services)
    {
        // TODO：规范化文档集成
        // 添加规范化文档服务
        //services.AddSpecificationDocuments();

        // 添加动态接口控制器服务
        services.AddDynamicApiControllers();

        // 添加全局数据验证
        services.AddDataValidation();

        // 添加友好异常服务
        services.AddFriendlyException();

        return services;
    }

    /// <summary>
    /// 注入基础配置和规范化结果
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IMvcBuilder AddInjectWithUnifyResult(this IMvcBuilder builder)
    {
        builder.Services.AddInjectWithUnifyResult();

        return builder;
    }

    /// <summary>
    /// 注入基础配置和规范化结果
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddInjectWithUnifyResult(this IServiceCollection services)
    {
        services.AddInject().AddUnifyResult();

        return services;
    }

    /// <summary>
    /// 注入基础配置和规范化结果
    /// </summary>
    /// <typeparam name="TUnifyResultProvider"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IMvcBuilder AddInjectWithUnifyResult<TUnifyResultProvider>(this IMvcBuilder builder)
        where TUnifyResultProvider : class, IUnifyResultProvider
    {
        builder.Services.AddInjectWithUnifyResult<TUnifyResultProvider>();

        return builder;
    }

    /// <summary>
    /// 注入基础配置和规范化结果
    /// </summary>
    /// <typeparam name="TUnifyResultProvider"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddInjectWithUnifyResult<TUnifyResultProvider>(this IServiceCollection services)
        where TUnifyResultProvider : class, IUnifyResultProvider
    {
        services.AddInject().AddUnifyResult<TUnifyResultProvider>();

        return services;
    }
}