using Fast.UnifyResult.Filters;
using Fast.UnifyResult.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Fast.UnifyResult.Extensions;

/// <summary>
/// 规范化服务拓展类
/// </summary>
public static class UnifyResultIServiceCollectionExtension
{
    /// <summary>
    /// 添加规范化结果服务
    /// </summary>
    /// <typeparam name="TUnifyResultProvider"></typeparam>
    /// <param name="mvcBuilder"></param>
    /// <returns></returns>
    public static IMvcBuilder AddUnifyResult<TUnifyResultProvider>(this IMvcBuilder mvcBuilder)
        where TUnifyResultProvider : class, IUnifyResultProvider
    {
        mvcBuilder.Services.AddUnifyResult<TUnifyResultProvider>();

        return mvcBuilder;
    }

    /// <summary>
    /// 添加规范化结果服务
    /// </summary>
    /// <typeparam name="TUnifyResultProvider"></typeparam>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddUnifyResult<TUnifyResultProvider>(this IServiceCollection services)
        where TUnifyResultProvider : class, IUnifyResultProvider
    {
        var providerType = typeof(TUnifyResultProvider);

        // 添加规范化提供器
        services.TryAddSingleton(providerType, providerType);

        // 添加成功规范化结果筛选器
        services.Configure<MvcOptions>(options => { options.Filters.Add<SucceededUnifyResultFilter>(); });

        return services;
    }
}