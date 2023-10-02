using System.Reflection;
using Fast.Core.DynamicApiController.Formatters;
using Fast.Core.DynamicApiController.Providers;
using Fast.DynamicApplication.Conventions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.DynamicApplication.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> 动态API应用拓展类
/// </summary>
public static class DynamicApplicationIServiceCollectionExtension
{
    /// <summary>
    /// 添加动态接口控制器服务
    /// </summary>
    /// <param name="mvcBuilder"><see cref="IMvcBuilder"/>Mvc构建器</param>
    /// <returns><see cref="IMvcBuilder"/>Mvc构建器</returns>
    public static IMvcBuilder AddDynamicApiControllers(this IMvcBuilder mvcBuilder)
    {
        mvcBuilder.Services.AddDynamicApiControllers();

        return mvcBuilder;
    }

    /// <summary>
    /// 添加动态接口控制器服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddDynamicApiControllers(this IServiceCollection services)
    {
        var partManager =
            services.FirstOrDefault(s => s.ServiceType == typeof(ApplicationPartManager))?.ImplementationInstance as
                ApplicationPartManager ?? throw new InvalidOperationException(
                $"`{nameof(AddDynamicApiControllers)}` must be invoked after `{nameof(MvcServiceCollectionExtensions.AddControllers)}`.");

        // 获取入口程序集
        var entryAssembly = Assembly.GetEntryAssembly();

        // 获取入口程序集所引用的所有程序集
        var referencedAssemblies = entryAssembly?.GetReferencedAssemblies();

        // 加载引用的程序集
        var assemblies = referencedAssemblies.Select(Assembly.Load).ToList();

        // 将入口程序集也放入集合
        assemblies.Add(entryAssembly);

        // 解决项目类型为 <Project Sdk="Microsoft.NET.Sdk"> 不能加载 API 问题，默认支持 <Project Sdk="Microsoft.NET.Sdk.Web">
        foreach (var assembly in assemblies)
        {
            if (partManager.ApplicationParts.Any(u => u.Name != assembly.GetName().Name))
            {
                partManager.ApplicationParts.Add(new AssemblyPart(assembly));
            }
        }

        // 添加控制器特性提供器
        partManager.FeatureProviders.Add(new DynamicApplicationFeatureProvider());

        // 配置 Mvc 选项
        services.Configure<MvcOptions>(options =>
        {
            // 添加应用模型转换器
            options.Conventions.Add(new DynamicApiControllerApplicationModelConvention(services));

            // 添加 text/plain 请求 Body 参数支持
            options.InputFormatters.Add(new TextPlainMediaTypeFormatter());
        });

        return services;
    }
}