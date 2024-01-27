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

using Fast.Admin.Service.System.CodeFirst;
using Fast.Admin.Service.Tenant.CodeFirst;
using Fast.NET.Core;

namespace Fast.Admin.Web.CodeFirst;

#if DEBUG

/// <summary>
/// <see cref="CodeFirstFilter"/> Code First
/// </summary>
public class CodeFirstFilter : IStartupFilter
{
    /// <summary>
    /// Extends the provided <paramref name="action" /> and returns an <see cref="T:System.Action" /> of the same type.
    /// </summary>
    /// <param name="action">The Configure method to extend.</param>
    /// <returns>A modified <see cref="T:System.Action" />.</returns>
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> action)
    {
        return app =>
        {
            // 判断是否初始化数据库
            if (FastContext.Configuration.GetSection("AppSettings:InitializeDatabase").Get<bool>())
            {
                // 获取 IHostApplicationLifetime 实例
                var hostApplicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

                // 订阅 ApplicationStarted 事件
                hostApplicationLifetime?.ApplicationStarted.Register(() =>
                {
                    // 在应用程序完全启动后执行自定义逻辑
                    // 初始化数据库
                    var systemCodeFirstService = app.ApplicationServices.GetService<ISystemCodeFirstService>();
                    var tenantCodeFirstService = app.ApplicationServices.GetService<ITenantCodeFirstService>();

                    var (sysTenantModel, sysTenantMainDatabaseModel) = systemCodeFirstService.InitDatabase().Result;

                    if (sysTenantModel != null && sysTenantMainDatabaseModel != null)
                    {
                        tenantCodeFirstService.InitNewTenant(sysTenantModel, sysTenantMainDatabaseModel, true).Wait();
                    }
                });
            }

            // 调用启动层的 Startup
            action(app);
        };
    }
}

#endif