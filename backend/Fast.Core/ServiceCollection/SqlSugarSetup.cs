using Fast.SqlSugar.Tenant;
using Fast.SqlSugar.Tenant.Setup;
using Furion.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core.ServiceCollection;

/// <summary>
/// SqlSugar
/// </summary>
public static class SqlSugarSetup
{
    /// <summary>
    /// SqlSugarClient的配置
    /// Client不能单例注入
    /// </summary>
    /// <param name="service"></param>
    public static void AddSqlSugarClientService(this IServiceCollection service)
    {
        // Add Snowflakes Id.
        service.AddSnowflakeId();

        // Set SqlSugar log Func.
        SugarContext.SetSugarLogFunc(message => { Log.Information(message); }, message => { Log.Warning(message); },
            message => { Log.Error(message); });

        // Set SqlSugar tenant and user Func.
        SugarContext.SetSugarFunc(() => GlobalContext.TenantId, () => GlobalContext.UserId, () => GlobalContext.UserName,
            () => GlobalContext.IsSuperAdmin, () => GlobalContext.IsSystemAdmin, () => GlobalContext.IsTenantAdmin);

        // Init sqlSugar.
        service.SqlSugarClientConfigure(App.HostEnvironment);
    }
}