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

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="IWebHostBuilder"/> 拓展类
/// </summary>
[SuppressSniffer]
public static class IWebHostBuilderExtension
{
    /// <summary>
    /// 添加管道启动服务注册
    /// </summary>
    /// <param name="builder"><see cref="IWebHostBuilder"/></param>
    /// <returns></returns>
    public static IWebHostBuilder HostingInjection(this IWebHostBuilder builder)
    {
        var iHostingStartupType = typeof(IHostingStartup);

        // 查找所有继承了 IHostingStartup 的类型
        var iHostingStartupTypes = IaaSContext.EffectiveTypes.Where(wh =>
            iHostingStartupType.IsAssignableFrom(wh) && wh.IsClass && !wh.IsInterface && !wh.IsAbstract).Select(sl =>
        {
            var hostingStartup = Activator.CreateInstance(sl) as IHostingStartup;

            // 默认为 -1；
            var order = -1;
            // 尝试获取Order值
            var orderProperty = sl.GetProperty("Order");

            if (orderProperty != null && orderProperty.PropertyType == typeof(int))
            {
                var orderVal = orderProperty.GetValue(hostingStartup)?.ToString();
                if (!orderVal.IsEmpty())
                {
                    order = orderVal.ParseToInt();
                }
            }

            return new {Type = hostingStartup, Order = order};
        }).OrderByDescending(ob => ob.Order).Select(sl => sl.Type);

        foreach (var hostingStartup in iHostingStartupTypes)
        {
            hostingStartup?.Configure(builder);
        }

        return builder;
    }

    /// <summary>
    /// 添加Api管道启动服务注册
    /// <remarks>必须在 AddControllers 或 AddControllersWithViews 之后注册</remarks>
    /// </summary>
    /// <param name="builder"><see cref="IWebHostBuilder"/></param>
    /// <returns></returns>
    public static IWebHostBuilder ApiHostingInjection(this IWebHostBuilder builder)
    {
        // 先判断是否是在 AddControllers 或 AddControllersWithViews 之后注册
        builder.ConfigureServices((hostContext, services) =>
        {
            if (services.All(a => a.ServiceType != typeof(ApplicationPartManager)))
            {
                throw new InvalidOperationException(
                    $"`{nameof(ApiHostingInjection)}` must be invoked after `AddControllers` or `AddControllersWithViews`.");
            }
        });

        var iApiHostingStartupType = typeof(IApiHostingStartup);

        // 查找所有继承了 IHostingStartup 的类型
        var iApiHostingStartupTypes = IaaSContext.EffectiveTypes.Where(wh =>
            iApiHostingStartupType.IsAssignableFrom(wh) && wh.IsClass && !wh.IsInterface && !wh.IsAbstract).Select(sl =>
        {
            var apiHostingStartup = Activator.CreateInstance(sl) as IApiHostingStartup;

            // 默认为 -1；
            var order = -1;
            // 尝试获取Order值
            var orderProperty = sl.GetProperty("Order");

            if (orderProperty != null && orderProperty.PropertyType == typeof(int))
            {
                var orderVal = orderProperty.GetValue(apiHostingStartup)?.ToString();
                if (!orderVal.IsEmpty())
                {
                    order = orderVal.ParseToInt();
                }
            }

            return new {Type = apiHostingStartup, Order = order};
        }).OrderByDescending(ob => ob.Order).Select(sl => sl.Type);

        foreach (var apiHostingStartup in iApiHostingStartupTypes)
        {
            apiHostingStartup?.Configure(builder);
        }

        return builder;
    }
}