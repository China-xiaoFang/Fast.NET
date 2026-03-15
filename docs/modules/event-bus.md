# Fast.EventBus

In-process event bus with publish/subscribe pattern, retry policies, and monitoring.

## Installation

```bash
dotnet add package Fast.EventBus
```

## Registration

```csharp
builder.Services.AddEventBus();
```

## Publishing Events

Inject `IEventPublisher` and publish events:

```csharp
public class OrderService
{
    private readonly IEventPublisher _publisher;

    public OrderService(IEventPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task CreateOrderAsync(OrderInput input)
    {
        // Create order logic...

        // Publish event with string ID
        await _publisher.PublishAsync("OrderCreated", new { OrderId = 123, Amount = 99.99 });

        // Publish with enum ID
        await _publisher.PublishAsync(OrderEvents.Created, new { OrderId = 123 });

        // Publish with delay (milliseconds)
        await _publisher.PublishDelayAsync("SendReminder", 60000, new { OrderId = 123 });
    }
}
```

## Subscribing to Events

Implement `IEventSubscriber` and decorate handler methods:

```csharp
public class OrderEventSubscriber : IEventSubscriber
{
    private readonly ILogger<OrderEventSubscriber> _logger;

    public OrderEventSubscriber(ILogger<OrderEventSubscriber> logger)
    {
        _logger = logger;
    }

    [EventSubscribe("OrderCreated")]
    public async Task OnOrderCreated(EventHandlerExecutingContext context)
    {
        var payload = context.Payload;
        _logger.LogInformation("Order created: {Payload}", payload);
        // Handle event...
    }

    [EventSubscribe("SendReminder")]
    public async Task OnSendReminder(EventHandlerExecutingContext context)
    {
        // Send reminder email...
    }
}
```

## EventSubscribe Attribute Options

```csharp
[EventSubscribe("OrderCreated",
    NumRetries = 3,           // Retry up to 3 times
    RetryTimeout = 2000,      // Wait 2 seconds between retries
    GCCollect = false,        // Trigger GC after execution
    Order = 1)]               // Execution order (higher = first)
public async Task OnOrderCreated(EventHandlerExecutingContext context)
{
    // Handler implementation
}
```

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `EventId` | `string` | (required) | Event identifier |
| `NumRetries` | `int` | `0` | Number of retry attempts |
| `RetryTimeout` | `int` | `1000` | Retry interval in milliseconds |
| `ExceptionTypes` | `Type[]` | `null` | Specific exception types to retry on |
| `FallbackPolicy` | `string` | `null` | Fallback strategy name |
| `GCCollect` | `bool` | `false` | Force GC after execution |
| `Order` | `int` | `0` | Execution priority (higher first) |

## Handler Context

```csharp
[EventSubscribe("MyEvent")]
public async Task OnMyEvent(EventHandlerExecutingContext context)
{
    // Event source information
    var eventId = context.Source.EventId;
    var payload = context.Source.Payload;
    var createdTime = context.Source.CreatedTime;

    // Handler attribute
    var attribute = context.Attribute;
}
```

## Custom Interfaces

### IEventHandlerMonitor

Monitor event handler execution:

```csharp
public class MyEventMonitor : IEventHandlerMonitor
{
    public Task OnExecutingAsync(EventHandlerExecutingContext context)
    {
        // Before handler execution
        return Task.CompletedTask;
    }

    public Task OnExecutedAsync(EventHandlerExecutedContext context)
    {
        // After handler execution
        return Task.CompletedTask;
    }
}
```

### IEventFallbackPolicy

Handle failed events:

```csharp
public class MyFallbackPolicy : IEventFallbackPolicy
{
    public Task CallbackAsync(EventHandlerExecutingContext context, Exception ex)
    {
        // Handle failed event after all retries exhausted
        return Task.CompletedTask;
    }
}
```

## Architecture

```
Publisher → Channel (bounded, 3000 capacity) → EventBusHostedService → Subscriber(s)
                                                       ↓
                                                 Retry Policy
                                                       ↓
                                                Fallback Policy
```

Events are queued in a bounded `Channel<IEventSource>` and processed by a background hosted service.
