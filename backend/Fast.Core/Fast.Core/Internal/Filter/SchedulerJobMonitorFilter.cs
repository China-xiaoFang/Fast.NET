using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.Core.Const;
using Fast.Core.Internal.EventSubscriber;
using Furion.EventBus;
using Furion.Schedule;
using Microsoft.Extensions.Logging;

namespace Fast.Core.Internal.Filter;

/// <summary>
/// 任务调度执行过滤器
/// </summary>
public class SchedulerJobMonitorFilter : IJobMonitor
{
    /// <summary>
    /// 任务调度JobId白名单
    /// 不会记录日志
    /// </summary>
    private readonly List<string> jobIdWhiteList = new() {"初始化数据库"};

    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<SchedulerJobMonitorFilter> _logger;

    public SchedulerJobMonitorFilter(IEventPublisher eventPublisher, ILogger<SchedulerJobMonitorFilter> logger)
    {
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    /// <summary>
    /// 白名单检测
    /// </summary>
    /// <param name="jobId"></param>
    /// <returns></returns>
    private bool CheckWhiteList(string jobId)
    {
        return !jobIdWhiteList.Contains(jobId);
    }

    /// <summary>作业处理程序执行前</summary>
    /// <param name="context">作业处理程序执行前上下文</param>
    /// <param name="stoppingToken">取消任务 Token</param>
    /// <returns><see cref="T:System.Threading.Tasks.Task" /> 实例</returns>
    public async Task OnExecutingAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        /*
         *{
  "jobDetail": {
  "jobId": "初始化数据库",
  "groupName": null,
  "jobType": "Fast.Scheduler.DataBase.DataBaseJobWorker",
  "assemblyName": "Fast.Scheduler",
  "description": null,
  "concurrent": true,
  "includeAnnotations": false,
  "properties": "{}",
  "updatedTime": "2023-01-30 13:06:12.602"
},
  "trigger": {
  "triggerId": "初始化数据库",
  "jobId": "初始化数据库",
  "triggerType": "Furion.Schedule.CronTrigger",
  "assemblyName": "Furion",
  "args": "[\"@secondly\",0]",
  "description": "程序启动时，初始化数据库，同步枚举字典",
  "status": 8,
  "startTime": null,
  "endTime": null,
  "lastRunTime": "2023-01-30 13:06:11.567",
  "nextRunTime": null,
  "numberOfRuns": 1,
  "maxNumberOfRuns": 1,
  "numberOfErrors": 0,
  "maxNumberOfErrors": 0,
  "numRetries": 0,
  "retryTimeout": 1000,
  "startNow": true,
  "runOnStart": true,
  "resetOnlyOnce": true,
  "updatedTime": "2023-01-30 13:06:12.602"
}
}

        任务执行成功通知
{
  "jobDetail": {
  "jobId": "初始化数据库",
  "groupName": null,
  "jobType": "Fast.Scheduler.DataBase.DataBaseJobWorker",
  "assemblyName": "Fast.Scheduler",
  "description": null,
  "concurrent": true,
  "includeAnnotations": false,
  "properties": "{}",
  "updatedTime": "2023-01-30 13:11:11.995"
},
  "trigger": {
  "triggerId": "初始化数据库",
  "jobId": "初始化数据库",
  "triggerType": "Furion.Schedule.CronTrigger",
  "assemblyName": "Furion",
  "args": "[\"@secondly\",0]",
  "description": "程序启动时，初始化数据库，同步枚举字典",
  "status": 8,
  "startTime": null,
  "endTime": null,
  "lastRunTime": "2023-01-30 13:10:53.412",
  "nextRunTime": null,
  "numberOfRuns": 1,
  "maxNumberOfRuns": 1,
  "numberOfErrors": 0,
  "maxNumberOfErrors": 0,
  "numRetries": 0,
  "retryTimeout": 1000,
  "startNow": true,
  "runOnStart": true,
  "resetOnlyOnce": true,
  "updatedTime": "2023-01-30 13:11:11.995"
}
}
         *
         */
        if (CheckWhiteList(context.JobId))
        {
            await Task.CompletedTask;

            // TODO：这里的租户Id暂且不知道从哪里来，目前先使用系统默认的租户Id
            await _eventPublisher.PublishAsync(new FastChannelEventSource("Create:SchedulerJobLog",
                ClaimConst.DefaultSuperAdminTenantId,
                new SysLogSchedulerJobModel
                {
                    Id = context.RunId,
                    JobId = context.JobId,
                    Running = YesOrNotEnum.Y,
                    Success = null,
                    LastRunTime = context.Trigger.LastRunTime,
                    NextRunTime = null,
                    UpdatedTime = context.Trigger.UpdatedTime!.Value,
                }));
        }
    }

    /// <summary>作业处理程序执行后</summary>
    /// <param name="context">作业处理程序执行后上下文</param>
    /// <param name="stoppingToken">取消任务 Token</param>
    /// <returns><see cref="T:System.Threading.Tasks.Task" /> 实例</returns>
    public async Task OnExecutedAsync(JobExecutedContext context, CancellationToken stoppingToken)
    {
        if (CheckWhiteList(context.JobId))
        {
            SysLogSchedulerJobModel sysLogSchedulerJobModel;
            if (context.Exception != null)
            {
                sysLogSchedulerJobModel = new SysLogSchedulerJobModel
                {
                    Id = context.RunId,
                    JobId = context.JobId,
                    Running = YesOrNotEnum.N,
                    Success = YesOrNotEnum.N,
                    LastRunTime = context.Trigger.LastRunTime,
                    NextRunTime = context.Trigger.NextRunTime,
                    ExceptionMsg = context.Exception.Message,
                    ExceptionSource = context.Exception.Source,
                    ExceptionStackTrace = context.Exception.StackTrace,
                    UpdatedTime = context.Trigger.UpdatedTime,
                };
                _logger.LogError(context.Exception, $"任务执行失败：{context.JobId}", context);
            }
            else
            {
                sysLogSchedulerJobModel = new SysLogSchedulerJobModel
                {
                    Id = context.RunId,
                    JobId = context.JobId,
                    Running = YesOrNotEnum.N,
                    Success = YesOrNotEnum.Y,
                    LastRunTime = context.Trigger.LastRunTime,
                    NextRunTime = context.Trigger.NextRunTime,
                    UpdatedTime = context.Trigger.UpdatedTime,
                };
            }

            await _eventPublisher.PublishAsync(new FastChannelEventSource("Update:SchedulerJobLog",
                ClaimConst.DefaultSuperAdminTenantId, sysLogSchedulerJobModel));

            await Task.CompletedTask;
        }
    }
}