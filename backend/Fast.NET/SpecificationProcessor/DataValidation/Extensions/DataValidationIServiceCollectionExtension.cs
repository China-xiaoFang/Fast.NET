// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Fast.SpecificationProcessor.DataValidation.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.SpecificationProcessor.DataValidation.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> 数据验证拓展类
/// </summary>
public static class DataValidationIServiceCollectionExtension
{
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