using Furion.EventBus;

namespace Fast.Core.EventSubscriber;

public class LogEventSubscriber : IEventSubscriber
{
    public LogEventSubscriber(IServiceProvider services, ISqlSugarClient sqlSugarClient)
    {
        Services = services;
        _sqlSugarClient = sqlSugarClient;
    }

    private readonly IServiceProvider Services;
    private readonly ISqlSugarClient _sqlSugarClient;

    [EventSubscribe("Create:ExLog")]
    public async Task CreateExLog(EventHandlerExecutingContext context)
    {
        using var scope = Services.CreateScope();
        var _repository = scope.ServiceProvider.GetRequiredService<ISqlSugarRepository<SysLogExModel>>();
        var log = (SysLogExModel) context.Source.Payload;
        await _repository.InsertAsync(log);
    }

    [EventSubscribe("Create:OpLog")]
    public async Task CreateOpLog(EventHandlerExecutingContext context)
    {
        using var scope = Services.CreateScope();
        if (context.Source is FastChannelEventSource source)
        {
            var _repository = _sqlSugarClient.LoadSqlSugar<SysLogOpModel>(source.TenantId);
            var log = (SysLogOpModel) context.Source.Payload;
            await _repository.Insertable(log).ExecuteCommandAsync();
        }
    }

    [EventSubscribe("Create:VisLog")]
    public async Task CreateVisLog(EventHandlerExecutingContext context)
    {
        using var scope = Services.CreateScope();
        if (context.Source is FastChannelEventSource source)
        {
            var _repository = _sqlSugarClient.LoadSqlSugar<SysLogVisModel>(source.TenantId);
            var log = (SysLogVisModel) context.Source.Payload;
            await _repository.Insertable(log).ExecuteCommandAsync();
        }
    }

    [EventSubscribe("Update:UserLoginInfo")]
    public async Task UpdateUserLoginInfo(EventHandlerExecutingContext context)
    {
        using var scope = Services.CreateScope();
        if (context.Source is FastChannelEventSource source)
        {
            var _repository = _sqlSugarClient.LoadSqlSugar<SysUserModel>(source.TenantId);
            var log = (SysUserModel) context.Source.Payload;
            await _repository.Updateable(log).UpdateColumns(m => new {m.LastLoginTime, m.LastLoginIp}).ExecuteCommandAsync();
        }
    }
}