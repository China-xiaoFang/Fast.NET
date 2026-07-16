// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Fast.EventBus;

/// <summary>
/// <see cref="EventBusHostedService"/> 事件总线后台主机服务
/// </summary>
internal sealed class EventBusHostedService : BackgroundService
{
    /// <summary>
    /// GC 回收默认间隔
    /// </summary>
    private const int GC_COLLECT_INTERVAL_SECONDS = 3;

    /// <summary>
    /// 避免由 CLR 的终结器捕获该异常从而终止应用程序，让所有未觉察异常被觉察
    /// </summary>
    internal event EventHandler<UnobservedTaskExceptionEventArgs> UnobservedTaskException;

    /// <summary>
    /// 日志对象
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// 服务提供器
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// 事件源存储器
    /// </summary>
    private readonly IEventSourceStorer _eventSourceStorer;

    /// <summary>
    /// 事件处理程序集合
    /// </summary>
    private readonly ConcurrentDictionary<EventHandlerWrapper, EventHandlerWrapper> _eventHandlers = new();

    /// <summary>
    /// 最近一次显式 GC 的 UTC Tick。
    /// </summary>
    private long _lastGCCollectTicks;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger">日志对象</param>
    /// <param name="serviceProvider">服务提供器</param>
    /// <param name="eventSourceStorer">事件源存储器</param>
    /// <param name="eventSubscribers">事件订阅者集合</param>
    public EventBusHostedService(ILogger<EventBusService> logger, IServiceProvider serviceProvider,
        IEventSourceStorer eventSourceStorer, IEnumerable<IEventSubscriber> eventSubscribers)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _eventSourceStorer = eventSourceStorer;
        Monitor = serviceProvider.GetService<IEventHandlerMonitor>();

        var bindingAttr = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        // 逐条获取事件处理程序并进行包装
        foreach (var eventSubscriber in eventSubscribers)
        {
            // 获取事件订阅者类型
            var eventSubscriberType = eventSubscriber.GetType();

            // 查找所有公开且贴有 [EventSubscribe] 的实例方法
            var eventHandlerMethods = eventSubscriberType.GetMethods(bindingAttr)
                .Where(u => u.IsDefined(typeof(EventSubscribeAttribute), false));

            // 遍历所有事件订阅者处理方法
            foreach (var eventHandlerMethod in eventHandlerMethods)
            {
                // 将方法转换成 Func<EventHandlerExecutingContext, Task> 委托
                var handler = (Func<EventHandlerExecutingContext, Task>) eventHandlerMethod.CreateDelegate(
                    typeof(Func<EventHandlerExecutingContext, Task>), eventSubscriber);

                // 处理同一个事件处理程序支持多个事件 Id 情况
                var eventSubscribeAttributes = eventHandlerMethod.GetCustomAttributes<EventSubscribeAttribute>(false);

                // 逐条包装并添加到 _eventHandlers 集合中
                foreach (var eventSubscribeAttribute in eventSubscribeAttributes)
                {
                    var wrapper = new EventHandlerWrapper(eventSubscribeAttribute.EventId)
                    {
                        Handler = handler,
                        HandlerMethod = eventHandlerMethod,
                        Attribute = eventSubscribeAttribute,
                        GCCollect = CheckIsSetGCCollect(eventSubscribeAttribute.GCCollect),
                        Order = eventSubscribeAttribute.Order
                    };

                    _eventHandlers.TryAdd(wrapper, wrapper);
                }
            }
        }
    }

    /// <summary>
    /// 事件处理程序监视器
    /// </summary>
    private IEventHandlerMonitor Monitor { get; }

    /// <summary>
    /// 执行后台任务
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log(LogLevel.Information, "EventBus hosted service is running.");

        // 注册后台主机服务停止监听
        stoppingToken.Register(() => Log(LogLevel.Debug, "EventBus hosted service is stopping."));

        // 监听服务是否取消
        while (!stoppingToken.IsCancellationRequested)
        {
            // 执行具体任务
            await BackgroundProcessing(stoppingToken);
        }

        Log(LogLevel.Critical, "EventBus hosted service is stopped.");
    }

    /// <summary>
    /// 后台调用处理程序
    /// </summary>
    /// <param name="stoppingToken">后台主机服务停止时取消任务 Token</param>
    /// <returns><see cref="Task"/> 实例</returns>
    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        // 从事件存储器中读取一条
        var eventSource = await _eventSourceStorer.ReadAsync(stoppingToken);

        // 处理动态新增/删除事件订阅器
        if (eventSource is EventSubscribeOperateSource subscribeOperateSource)
        {
            ManageEventSubscribers(subscribeOperateSource);

            return;
        }

        // 空检查
        if (string.IsNullOrWhiteSpace(eventSource?.EventId))
        {
            Log(LogLevel.Warning, "Invalid EventId, EventId cannot be <null> or an empty string.");

            return;
        }

        // 查找事件 Id 匹配的事件处理程序
        var eventHandlersThatShouldRun = _eventHandlers.Where(t => t.Key.ShouldRun(eventSource.EventId))
            .OrderByDescending(u => u.Value.Order)
            .Select(u => u.Key)
            .ToList();

        // 空订阅
        if (!eventHandlersThatShouldRun.Any())
        {
            Log(LogLevel.Warning, "Subscriber with event ID <{EventId}> was not found.", new object[] {eventSource.EventId});

            return;
        }

        // 创建共享上下文数据对象
        // 多个订阅器会并行执行，普通 Dictionary 在并发读写时可能损坏内部状态。
        var properties = new ConcurrentDictionary<object, object>();

        using var processingTokenSource =
            CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, eventSource.CancellationToken);
        var processingToken = processingTokenSource.Token;

        async Task InvokeAsync(Func<Task> action, int numRetries, int retryTimeout = 1000, bool finalThrow = true,
            Type[] exceptionTypes = null, Func<Exception, Task> fallbackPolicy = null, Action<int, int> retryAction = null)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            // 如果重试次数小于或等于 0，则直接调用
            if (numRetries <= 0)
            {
                await action();
                return;
            }

            // 存储总的重试次数
            var totalNumRetries = numRetries;

            // 不断重试
            while (true)
            {
                try
                {
                    await action();
                    break;
                }
                catch (Exception ex)
                {
                    // 如果可重试次数小于或等于0，则终止重试
                    if (--numRetries < 0)
                    {
                        if (finalThrow)
                        {
                            if (fallbackPolicy != null)
                                await fallbackPolicy.Invoke(ex);
                            throw;
                        }

                        return;
                    }

                    // 如果填写了 exceptionTypes 且异常类型不在 exceptionTypes 之内，则终止重试
                    if (exceptionTypes != null
                        && exceptionTypes.Length > 0
                        && !exceptionTypes.Any(u => u.IsAssignableFrom(ex.GetType())))
                    {
                        if (finalThrow)
                        {
                            if (fallbackPolicy != null)
                                await fallbackPolicy.Invoke(ex);
                            throw;
                        }

                        return;
                    }

                    // 重试调用委托
                    retryAction?.Invoke(totalNumRetries, totalNumRetries - numRetries);

                    // 如果可重试异常数大于 0，则间隔指定时间后继续执行
                    if (retryTimeout > 0)
                        await Task.Delay(retryTimeout, processingToken);
                }
            }
        }

        async Task HandleEventAsync(EventHandlerWrapper eventHandlerThatShouldRun)
        {
            // 获取特性信息，可能为 null
            var eventSubscribeAttribute = eventHandlerThatShouldRun.Attribute;

            // 创建执行前上下文
            var eventHandlerExecutingContext =
                new EventHandlerExecutingContext(eventSource, properties, eventHandlerThatShouldRun.HandlerMethod,
                    eventSubscribeAttribute) {ExecutingTime = DateTime.Now};

            // 执行异常对象
            InvalidOperationException executionException = null;

            try
            {
                processingToken.ThrowIfCancellationRequested();

                // 调用执行前监视器
                if (Monitor != null)
                {
                    await Monitor.OnExecutingAsync(eventHandlerExecutingContext);
                }

                // 判断是否自定义了重试失败回调服务
                var fallbackPolicyService = eventSubscribeAttribute?.FallbackPolicy == null
                    ? null
                    : _serviceProvider.GetService(eventSubscribeAttribute.FallbackPolicy) as IEventFallbackPolicy;

                // 调用事件处理程序并配置出错执行重试
                await InvokeAsync(() => eventHandlerThatShouldRun.Handler!(eventHandlerExecutingContext),
                    eventSubscribeAttribute?.NumRetries ?? 0, eventSubscribeAttribute?.RetryTimeout ?? 1000,
                    exceptionTypes: eventSubscribeAttribute?.ExceptionTypes,
                    fallbackPolicy: fallbackPolicyService == null
                        ? null
                        : ex => fallbackPolicyService.CallbackAsync(eventHandlerExecutingContext, ex),
                    retryAction: (total, times) =>
                    {
                        _logger.LogWarning("Retrying {Times}/{Total} times for {EventId}", times, total, eventSource.EventId);
                    });
            }
            catch (OperationCanceledException) when (processingToken.IsCancellationRequested)
            {
                // 主机停止或事件自身取消属于正常控制流，不记录为执行错误。
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, "Error occurred executing {EventId}.", new object[] {eventSource.EventId}, ex);

                executionException = new InvalidOperationException($"Error occurred executing {eventSource.EventId}.", ex);

                if (UnobservedTaskException != null)
                {
                    var args = new UnobservedTaskExceptionEventArgs(ex as AggregateException ?? new AggregateException(ex));
                    UnobservedTaskException.Invoke(this, args);
                }
            }
            finally
            {
                if (Monitor != null)
                {
                    var eventHandlerExecutedContext =
                        new EventHandlerExecutedContext(eventSource, properties, eventHandlerThatShouldRun.HandlerMethod,
                            eventSubscribeAttribute) {ExecutedTime = DateTime.Now, Exception = executionException};

                    try
                    {
                        await Monitor.OnExecutedAsync(eventHandlerExecutedContext);
                    }
                    catch (Exception ex)
                    {
                        Log(LogLevel.Error, "Error occurred in event handler monitor for {EventId}.",
                            new object[] {eventSource.EventId}, ex);
                    }
                }

                TryCollectGarbage(eventHandlerThatShouldRun.GCCollect);
            }
        }

        // 同一事件的订阅器并行执行，但等待全部完成以观察异常并让有界通道形成有效背压。
        await Task.WhenAll(eventHandlersThatShouldRun.Select(HandleEventAsync));
    }

    /// <summary>
    /// 管理事件订阅器动态
    /// </summary>
    /// <param name="subscribeOperateSource"></param>
    private void ManageEventSubscribers(EventSubscribeOperateSource subscribeOperateSource)
    {
        // 获取实际订阅事件 Id
        var eventId = subscribeOperateSource.SubscribeEventId;

        // 确保事件订阅 Id 和传入的特性 EventId 一致
        if (subscribeOperateSource.Attribute != null && subscribeOperateSource.Attribute.EventId != eventId)
            throw new InvalidOperationException(
                "Ensure that the <eventId> is consistent with the <EventId> attribute of the EventSubscribeAttribute object.");

        // 处理动态新增
        if (subscribeOperateSource.Operate == EventSubscribeOperates.Append)
        {
            var wrapper = new EventHandlerWrapper(eventId)
            {
                Attribute = subscribeOperateSource.Attribute,
                HandlerMethod = subscribeOperateSource.HandlerMethod,
                Handler = subscribeOperateSource.Handler,
                GCCollect = CheckIsSetGCCollect(subscribeOperateSource.Attribute?.GCCollect),
                Order = subscribeOperateSource.Attribute?.Order ?? 0
            };

            // 追加到集合中
            var succeeded = _eventHandlers.TryAdd(wrapper, wrapper);

            // 输出日志
            if (succeeded)
            {
                Log(LogLevel.Information, "Subscriber with event ID <{EventId}> was appended successfully.",
                    new object[] {eventId});
            }
        }
        // 处理动态删除
        else if (subscribeOperateSource.Operate == EventSubscribeOperates.Remove)
        {
            // 删除所有匹配事件 Id 的处理程序
            foreach (var wrapper in _eventHandlers.Keys)
            {
                if (wrapper.EventId != eventId)
                    continue;

                var succeeded = _eventHandlers.TryRemove(wrapper, out _);
                if (!succeeded)
                    continue;

                // 输出日志
                Log(LogLevel.Warning, "Subscriber<{Name}> with event ID <{EventId}> was remove.",
                    new object[] {wrapper.HandlerMethod?.Name, eventId});
            }
        }
    }

    /// <summary>
    /// 检查是否开启执行完成触发 GC 回收
    /// </summary>
    /// <param name="gcCollect"></param>
    /// <returns></returns>
    private bool CheckIsSetGCCollect(object gcCollect)
    {
        return gcCollect != null && Convert.ToBoolean(gcCollect);
    }

    /// <summary>
    /// 在调用方明确开启时限制显式垃圾回收频率。
    /// </summary>
    private void TryCollectGarbage(bool enabled)
    {
        if (!enabled)
            return;

        var nowTicks = DateTime.UtcNow.Ticks;
        var previousTicks = Interlocked.Read(ref _lastGCCollectTicks);
        if (previousTicks != 0
            && nowTicks - previousTicks
            <= TimeSpan.FromSeconds(GC_COLLECT_INTERVAL_SECONDS)
                .Ticks)
            return;

        if (Interlocked.CompareExchange(ref _lastGCCollectTicks, nowTicks, previousTicks) == previousTicks)
            GC.Collect();
    }

    /// <summary>
    /// 记录日志
    /// </summary>
    /// <param name="logLevel">日志级别</param>
    /// <param name="message">消息</param>
    /// <param name="args">参数</param>
    /// <param name="ex">异常</param>
    private void Log(LogLevel logLevel, string message, object[] args = null, Exception ex = null)
    {
        if (logLevel == LogLevel.Error)
        {
            _logger.LogError(ex, message, args);
        }
        else
        {
            _logger.Log(logLevel, message, args);
        }
    }
}