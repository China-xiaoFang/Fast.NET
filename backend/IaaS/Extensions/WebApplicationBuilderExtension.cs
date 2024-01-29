// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="WebApplicationBuilder"/> 拓展类
/// </summary>
[SuppressSniffer]
public static class WebApplicationBuilderExtension
{
    /// <summary>
    /// 添加管道启动服务注册
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddHostInjection(this WebApplicationBuilder builder)
    {
        IWebHostBuilder webHostBuilder = builder.WebHost;

        var iHostStartupType = typeof(IHostInjection);

        // 查找所有继承了 IHostInjection 的类型
        var iHostStartupTypes = IaaSContext.EffectiveTypes.Where(wh =>
            iHostStartupType.IsAssignableFrom(wh) && wh.IsClass && !wh.IsInterface && !wh.IsAbstract).Select(sl =>
        {
            var hostInjection = Activator.CreateInstance(sl) as IHostInjection;

            return new {Type = hostInjection, Order = hostInjection?.Order ?? -1};
        }).OrderByDescending(ob => ob.Order).Select(sl => sl.Type);

        foreach (var hostInjection in iHostStartupTypes)
        {
            hostInjection?.Configure(webHostBuilder);
        }

        return builder;
    }

    /// <summary>
    /// 添加 MVC 控制器
    /// <remarks>
    /// <para>不包括对视图的支持</para>
    /// <para>自带框架内部的一些注入</para>
    /// </remarks>
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddControllers(this WebApplicationBuilder builder)
    {
        // 添加控制器
        builder.Services.AddControllers();

        // 添加控制器之后的服务注册
        builder.AddControllersInjection();

        return builder;
    }

    /// <summary>
    /// 添加 MVC 控制器
    /// <remarks>
    /// <para>包括对视图的支持</para>
    /// <para>自带框架内部的一些注入</para>
    /// </remarks>
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddControllersWithViews(this WebApplicationBuilder builder)
    {
        // 添加控制器
        builder.Services.AddControllersWithViews();

        // 添加控制器之后的服务注册
        builder.AddControllersInjection();

        return builder;
    }

    /// <summary>
    /// 添加控制器之后的服务注册
    /// <remarks>必须在 AddControllers 或 AddControllersWithViews 之后注册</remarks>
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddControllersInjection(this WebApplicationBuilder builder)
    {
        IWebHostBuilder webHostBuilder = builder.WebHost;

        // 先判断是否是在 AddControllers 或 AddControllersWithViews 之后注册
        webHostBuilder.ConfigureServices((hostContext, services) =>
        {
            if (services.All(a => a.ServiceType != typeof(ApplicationPartManager)))
            {
                throw new InvalidOperationException(
                    $"`{nameof(AddControllersInjection)}` must be invoked after `AddControllers` or `AddControllersWithViews`.");
            }
        });

        var iControllersInjectionType = typeof(IControllersInjection);

        // 查找所有继承了 IControllersInjection 的类型
        var iControllersInjectionTypes = IaaSContext.EffectiveTypes.Where(wh =>
            iControllersInjectionType.IsAssignableFrom(wh) && wh.IsClass && !wh.IsInterface && !wh.IsAbstract).Select(sl =>
        {
            var controllersInjection = Activator.CreateInstance(sl) as IControllersInjection;

            return new {Type = controllersInjection, Order = controllersInjection?.Order ?? -1};
        }).OrderByDescending(ob => ob.Order).Select(sl => sl.Type);

        foreach (var controllersInjection in iControllersInjectionTypes)
        {
            controllersInjection?.Configure(webHostBuilder);
        }

        return builder;
    }
}