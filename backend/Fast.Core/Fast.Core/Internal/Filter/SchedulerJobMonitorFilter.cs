using Furion.EventBus;
using Furion.Schedule;
using Microsoft.Extensions.Logging;

namespace Fast.Core.Internal.Filter;

/// <summary>
/// 任务调度执行过滤器
/// </summary>
public class SchedulerJobMonitorFilter : IJobMonitor
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<SchedulerJobMonitorFilter> _logger;

    public SchedulerJobMonitorFilter(IEventPublisher eventPublisher, ILogger<SchedulerJobMonitorFilter> logger)
    {
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    /// <summary>作业处理程序执行前</summary>
    /// <param name="context">作业处理程序执行前上下文</param>
    /// <param name="stoppingToken">取消任务 Token</param>
    /// <returns><see cref="T:System.Threading.Tasks.Task" /> 实例</returns>
    public async Task OnExecutingAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        Console.WriteLine("任务执行之前通知");
        Console.WriteLine(context.ConvertToJSON());
        await Task.CompletedTask;
    }

    /// <summary>作业处理程序执行后</summary>
    /// <param name="context">作业处理程序执行后上下文</param>
    /// <param name="stoppingToken">取消任务 Token</param>
    /// <returns><see cref="T:System.Threading.Tasks.Task" /> 实例</returns>
    public async Task OnExecutedAsync(JobExecutedContext context, CancellationToken stoppingToken)
    {
        if (context.Exception != null)
        {
            Console.WriteLine("任务执行失败通知");
            Console.WriteLine(context.ConvertToJSON());
            _logger.LogError(context.Exception, $"任务执行失败：{context}", context);
        }
        else
        {
            Console.WriteLine("任务执行成功通知");
            Console.WriteLine(context.ConvertToJSON());
        }

        await Task.CompletedTask;
    }
}