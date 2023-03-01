using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.Core.AdminFactory.ModelFactory.Tenant;
using Fast.SqlSugar.Tenant.Extension;
using Furion.EventBus;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core.Internal.EventSubscriber;

public class LogEventSubscriber : IEventSubscriber
{
    public LogEventSubscriber(IServiceProvider services)
    {
        Services = services;
    }

    private readonly IServiceProvider Services;

    [EventSubscribe("Create:ExLog")]
    public async Task CreateExLog(EventHandlerExecutingContext context)
    {
        using var scope = Services.CreateScope();
        var _repository = scope.ServiceProvider.GetRequiredService<ISqlSugarRepository<SysLogExModel>>();
        var log = (SysLogExModel) context.Source.Payload;
        log.RecordCreate();
        await _repository.InsertAsync(log);
    }

    [EventSubscribe("Create:OpLog")]
    public async Task CreateOpLog(EventHandlerExecutingContext context)
    {
        using var scope = Services.CreateScope();
        if (context.Source is FastChannelEventSource source)
        {
            var _db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>().LoadSqlSugar<SysLogOpModel>(source.TenantId);
            var log = (SysLogOpModel) context.Source.Payload;
            await _db.Insertable(log).ExecuteCommandAsync();
        }
    }

    [EventSubscribe("Create:VisLog")]
    public async Task CreateVisLog(EventHandlerExecutingContext context)
    {
        using var scope = Services.CreateScope();
        if (context.Source is FastChannelEventSource source)
        {
            var _db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>().LoadSqlSugar<SysLogVisModel>(source.TenantId);
            var log = (SysLogVisModel) context.Source.Payload;
            await _db.Insertable(log).ExecuteCommandAsync();
        }
    }

    [EventSubscribe("Create:DiffLog")]
    public async Task CreateDiffLog(EventHandlerExecutingContext context)
    {
        using var scope = Services.CreateScope();
        if (context.Source is FastChannelEventSource source)
        {
            var _db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>().LoadSqlSugar<SysLogDiffModel>(source.TenantId);
            var log = (SysLogDiffModel) context.Source.Payload;
            await _db.Insertable(log).ExecuteCommandAsync();
        }
    }

    [EventSubscribe("Update:UserLoginInfo")]
    public async Task UpdateUserLoginInfo(EventHandlerExecutingContext context)
    {
        using var scope = Services.CreateScope();
        if (context.Source is FastChannelEventSource source)
        {
            var _db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>().LoadSqlSugar<TenUserModel>(source.TenantId);
            var log = (TenUserModel) context.Source.Payload;
            await _db.Updateable(log).UpdateColumns(m => new {m.LastLoginTime, m.LastLoginIp}).ExecuteCommandAsync();
        }
    }

    [EventSubscribe("Create:SchedulerJobLog")]
    public async Task CreateSchedulerJobLog(EventHandlerExecutingContext context)
    {
        using var scope = Services.CreateScope();
        if (context.Source is FastChannelEventSource source)
        {
            var _db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>()
                .LoadSqlSugar<SysLogSchedulerJobModel>(source.TenantId);
            var log = (SysLogSchedulerJobModel) context.Source.Payload;
            await _db.Insertable(log).ExecuteCommandAsync();
        }
    }

    [EventSubscribe("Update:SchedulerJobLog")]
    public async Task UpdateSchedulerJobLog(EventHandlerExecutingContext context)
    {
        using var scope = Services.CreateScope();
        if (context.Source is FastChannelEventSource source)
        {
            var _db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>()
                .LoadSqlSugar<SysLogSchedulerJobModel>(source.TenantId);
            var log = (SysLogSchedulerJobModel) context.Source.Payload;
            await _db.Updateable(log).ExecuteCommandAsync();
        }
    }
}