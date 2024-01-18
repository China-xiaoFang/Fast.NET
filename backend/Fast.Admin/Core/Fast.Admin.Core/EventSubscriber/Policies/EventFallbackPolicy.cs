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

using Fast.EventBus.Contexts;
using Fast.EventBus.Policies;
using Fast.Serialization.Extensions;
using Microsoft.Extensions.Logging;

namespace Fast.Admin.Core.EventSubscriber.Policies;

/// <summary>
/// <see cref="EventFallbackPolicy"/> 事件重试失败回调服务提供者
/// </summary>
public class EventFallbackPolicy : IEventFallbackPolicy
{
    private readonly ILogger<IEventFallbackPolicy> _logger;

    public EventFallbackPolicy(ILogger<IEventFallbackPolicy> logger)
    {
        _logger = logger;
    }

    /// <summary>重试失败回调</summary>
    /// <param name="context"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    public async Task CallbackAsync(EventHandlerExecutingContext context, Exception ex)
    {
        _logger.LogCritical(ex,
            $"事件总线重试了多次最终还是失败了！\r\n执行前时间：\r\n{context.ExecutingTime:yyyy-MM-dd HH:mm:ss..fffffff zzz dddd}\r\n事件源数据：{context.Source.ToJsonString()}");

        await Task.CompletedTask;
    }
}