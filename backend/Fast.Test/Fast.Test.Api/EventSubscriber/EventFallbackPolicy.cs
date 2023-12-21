//using Fast.EventBus;
//using Fast.EventBus.Contexts;

//namespace Fast.Test.Api.EventSubscriber;

//public class EventFallbackPolicy : IEventFallbackPolicy
//{
//    private readonly ILogger<EventFallbackPolicy> _logger;

//    public EventFallbackPolicy(ILogger<EventFallbackPolicy> logger)
//    {
//        _logger = logger;
//    }

//    public async Task CallbackAsync(EventHandlerExecutingContext context, Exception ex)
//    {
//        _logger.LogError(ex, "重试了多次最终还是失败了");
//        await Task.CompletedTask;
//    }
//}