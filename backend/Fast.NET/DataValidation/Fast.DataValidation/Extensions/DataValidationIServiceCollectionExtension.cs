using Fast.DataValidation.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.DataValidation.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> 数据验证拓展类
/// </summary>
public static class DataValidationIServiceCollectionExtension
{
    /// <summary>
    /// 添加全局数据验证
    /// </summary>
    /// <param name="mvcBuilder"><see cref="IMvcBuilder"/></param>
    /// <returns><see cref="IMvcBuilder"/></returns>
    public static IMvcBuilder AddDataValidation(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.Services.AddDataValidation();

        return mvcBuilder;
    }

    /// <summary>
    /// 添加全局数据验证
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddDataValidation(this IServiceCollection services)
    {
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
}