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

using Fast.Admin.Core.EventSubscriber.Policies;
using Fast.Admin.Core.EventSubscriber.Sources;
using Fast.EventBus;
using Fast.EventBus.Contexts;
using Fast.EventBus.Interfaces;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Fast.Admin.Core.EventSubscriber.SysLogSql;

/// <summary>
/// <see cref="SysLogSqlEventSubscriber"/> 系统Sql日志事件订阅者
/// <remarks>这里为什么不写成同一个是为了方便查找问题</remarks>
/// </summary>
public class SysLogSqlEventSubscriber : IEventSubscriber
{
    private readonly ILogger<SysLogSqlEventSubscriber> _logger;

    public SysLogSqlEventSubscriber(ILogger<SysLogSqlEventSubscriber> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 添加Sql执行日志
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(SysLogSqlEventSubscriberEnum.AddExecuteLog, NumRetries = 3, FallbackPolicy = typeof(EventFallbackPolicy))]
    public async Task AddExecuteLog(EventHandlerExecutingContext context)
    {
        var source = (SqlSugarChannelEventSource) context.Source;

        if (source == null)
            return;

        var db = new SqlSugarClient(source.ConnectionConfig);

        // 保存数据
        await db.InsertableByObject(source.Payload).SplitTable().ExecuteCommandAsync();
    }

    /// <summary>
    /// 添加Sql执行超时日志
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(SysLogSqlEventSubscriberEnum.AddTimeoutLog, NumRetries = 3, FallbackPolicy = typeof(EventFallbackPolicy))]
    public async Task AddTimeoutLog(EventHandlerExecutingContext context)
    {
        var source = (SqlSugarChannelEventSource) context.Source;

        if (source == null)
            return;

        var db = new SqlSugarClient(source.ConnectionConfig);

        // 保存数据
        await db.InsertableByObject(source.Payload).ExecuteCommandAsync();
    }

    /// <summary>
    /// 添加Sql执行差异日志
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(SysLogSqlEventSubscriberEnum.AddDiffLog, NumRetries = 3, FallbackPolicy = typeof(EventFallbackPolicy))]
    public async Task AddDiffLog(EventHandlerExecutingContext context)
    {
        var source = (SqlSugarChannelEventSource) context.Source;

        if (source == null)
            return;

        var db = new SqlSugarClient(source.ConnectionConfig);

        // 保存数据
        await db.InsertableByObject(source.Payload).SplitTable().ExecuteCommandAsync();
    }

    /// <summary>
    /// 添加Sql执行错误日志
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(SysLogSqlEventSubscriberEnum.AddErrorLog, NumRetries = 3, FallbackPolicy = typeof(EventFallbackPolicy))]
    public async Task AddErrorLog(EventHandlerExecutingContext context)
    {
        var source = (SqlSugarChannelEventSource) context.Source;

        if (source == null)
            return;

        var db = new SqlSugarClient(source.ConnectionConfig);

        // 保存数据
        await db.InsertableByObject(source.Payload).ExecuteCommandAsync();
    }

    /// <summary>
    /// 添加访问日志
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(SysLogSqlEventSubscriberEnum.AddVisLog, NumRetries = 3, FallbackPolicy = typeof(EventFallbackPolicy))]
    public async Task AddVisLog(EventHandlerExecutingContext context)
    {
        var source = (SqlSugarChannelEventSource) context.Source;

        if (source == null)
            return;

        var db = new SqlSugarClient(source.ConnectionConfig);

        // 保存数据
        await db.InsertableByObject(source.Payload).ExecuteCommandAsync();
    }

    /// <summary>
    /// 添加操作日志
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [EventSubscribe(SysLogSqlEventSubscriberEnum.AddOpLog, NumRetries = 3, FallbackPolicy = typeof(EventFallbackPolicy))]
    public async Task AddOpLog(EventHandlerExecutingContext context)
    {
        var source = (SqlSugarChannelEventSource) context.Source;

        if (source == null)
            return;

        var db = new SqlSugarClient(source.ConnectionConfig);

        // 保存数据
        await db.InsertableByObject(source.Payload).SplitTable().ExecuteCommandAsync();
    }
}