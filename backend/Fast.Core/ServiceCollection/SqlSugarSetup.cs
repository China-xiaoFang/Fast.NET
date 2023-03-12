using Fast.Admin.Model.Enum;
using Fast.Admin.Model.Model.Sys.Log;
using Fast.SDK.Common.EventSubscriber;
using Fast.SqlSugar.Tenant;
using Fast.SqlSugar.Tenant.Setup;
using Furion.DependencyInjection;
using Furion.EventBus;
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
            message => { Log.Error(message); }, SqlSugarDiffLog);

        // Set SqlSugar tenant and user Func.
        SugarContext.SetSugarFunc(() => GlobalContext.TenantId, () => GlobalContext.UserId, () => GlobalContext.UserName,
            () => GlobalContext.IsSuperAdmin, () => GlobalContext.IsSystemAdmin, () => GlobalContext.IsTenantAdmin);

        // Init sqlSugar.
        service.SqlSugarClientConfigure(App.HostEnvironment);
    }

    /// <summary>
    /// SqlSugar 差异日志
    /// </summary>
    /// <param name="diffDescription"></param>
    /// <param name="afterData"></param>
    /// <param name="beforeData"></param>
    /// <param name="executeSql"></param>
    /// <param name="diffType"></param>
    /// <param name="diffTime"></param>
    private static void SqlSugarDiffLog(string diffDescription, List<DiffLogTableInfo> afterData,
        List<DiffLogTableInfo> beforeData, string executeSql, DiffType diffType, DateTime diffTime)
    {
        if ((afterData != null && afterData.Any()) || (beforeData != null && beforeData.Any()))
        {
            // 创建一个作用域
            Scoped.Create((_, scope) =>
            {
                var serviceScope = scope.ServiceProvider;

                // 获取事件总线服务
                var _eventPublisher = serviceScope.GetService<IEventPublisher>();

                DiffLogTableInfo firstData = null;
                if (afterData != null && afterData.Any())
                {
                    firstData = afterData.First();
                }
                else if (beforeData != null && beforeData.Any())
                {
                    firstData = beforeData.First();
                }

                var tableName = firstData?.TableName;
                var tableDescription = firstData?.TableDescription;

                var diffLogType = diffType switch
                {
                    DiffType.insert => DiffLogTypeEnum.Insert,
                    DiffType.update => DiffLogTypeEnum.Update,
                    DiffType.delete => DiffLogTypeEnum.Delete,
                    _ => DiffLogTypeEnum.None
                };

                var sysLogDiffModel = new SysLogDiffModel
                {
                    Account = GlobalContext.UserAccount,
                    UserName = GlobalContext.UserName,
                    DiffDescription = diffDescription,
                    TableName = tableName,
                    TableDescription = tableDescription,
                    AfterColumnInfo = afterData?.Select(sl => sl.Columns).ToList(),
                    BeforeColumnInfo = beforeData?.Select(sl => sl.Columns).ToList(),
                    ExecuteSql = executeSql,
                    DiffType = diffLogType,
                    DiffTime = diffTime
                };
                sysLogDiffModel.RecordCreate();

                // 记录差异日志
                _eventPublisher.PublishAsync(new FastChannelEventSource("Create:DiffLog", GlobalContext.GetTenantId(false),
                    sysLogDiffModel));
            });
        }
    }
}