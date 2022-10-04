using Furion.DependencyInjection;
using Furion.EventBus;
using Furion.TaskScheduler;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Scheduler;

public class SpareTimeListener : ISpareTimeListener, ISingleton
{
    /// <summary>
    /// 白名单
    /// 不会被监听
    /// </summary>
    private readonly List<string> Whitelist = new() {"初始化数据库"};

    /// <summary>
    /// 监听所有任务
    /// </summary>
    /// <param name="executer"></param>
    /// <returns></returns>
    public async Task OnListener(SpareTimerExecuter executer)
    {
        if (Whitelist.Contains(executer.Timer.WorkerName))
            return;

        await Scoped.CreateAsync(async (_, scope) =>
        {
            var service = scope.ServiceProvider;

            var _eventPublisher = service.GetService<IEventPublisher>();

            switch (executer.Status)
            {
                // 执行开始通知
                case 0:
                    Console.WriteLine($"{executer.Timer.WorkerName} 任务开始通知");
                    break;
                // 任务执行之前通知
                case 1:
                    Console.WriteLine($"{executer.Timer.WorkerName} 执行之前通知");
                    break;
                // 执行成功通知
                case 2:
                    Console.WriteLine($"{executer.Timer.WorkerName} 执行成功通知");
                    break;
                // 任务执行失败通知
                case 3:
                    Console.WriteLine($"{executer.Timer.WorkerName} 执行失败通知");
                    break;
                // 任务执行停止通知
                case -1:
                    Console.WriteLine($"{executer.Timer.WorkerName} 执行停止通知");
                    break;
                // 任务执行取消通知
                case -2:
                    Console.WriteLine($"{executer.Timer.WorkerName} 执行取消通知");
                    break;
            }
        });
    }
}