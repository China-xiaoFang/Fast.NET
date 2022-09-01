using Furion.EventBus;

namespace Fast.Core.EventSubscriber;

public class LogEventSubscriber : IEventSubscriber
{
    public LogEventSubscriber(IServiceProvider services)
    {
        Services = services;
    }

    public IServiceProvider Services { get; }

    [EventSubscribe("Create:OpLog")]
    public async Task CreateOpLog(EventHandlerExecutingContext context)
    {
        using var scope = Services.CreateScope();
        var _repository = scope.ServiceProvider.GetRequiredService<SqlSugarRepository<SysLogOpModel>>();
        var log = (SysLogOpModel) context.Source.Payload;
        await _repository.InsertAsync(log);
    }

    [EventSubscribe("Create:ExLog")]
    public async Task CreateExLog(EventHandlerExecutingContext context)
    {
        using var scope = Services.CreateScope();
        var _repository = scope.ServiceProvider.GetRequiredService<SqlSugarRepository<SysLogExModel>>();
        var log = (SysLogExModel) context.Source.Payload;
        await _repository.InsertAsync(log);
    }

    [EventSubscribe("Create:VisLog")]
    public async Task CreateVisLog(EventHandlerExecutingContext context)
    {
        using var scope = Services.CreateScope();
        var _repository = scope.ServiceProvider.GetRequiredService<SqlSugarRepository<SysLogVisModel>>();
        var log = (SysLogVisModel) context.Source.Payload;
        await _repository.InsertAsync(log);
    }

    [EventSubscribe("Update:UserLoginInfo")]
    public async Task UpdateUserLoginInfo(EventHandlerExecutingContext context)
    {
        using var scope = Services.CreateScope();
        var _repository = scope.ServiceProvider.GetRequiredService<SqlSugarRepository<SysUserModel>>();
        var log = (SysUserModel) context.Source.Payload;
        await _repository.Context.Updateable(log).UpdateColumns(m => new {m.LastLoginTime, m.LastLoginIp}).ExecuteCommandAsync();
    }
}