# Fast.DependencyInjection

Automatic dependency injection via marker interfaces. Register services by simply implementing an interface — no manual registration needed.

## Installation

```bash
dotnet add package Fast.DependencyInjection
```

## Registration

```csharp
builder.Services.AddDependencyInjection();
```

## Lifetime Interfaces

Implement one of these marker interfaces on your service class:

| Interface | Lifetime | Description |
| --- | --- | --- |
| `ITransientDependency` | Transient | New instance per request |
| `IScopedDependency` | Scoped | One instance per scope (HTTP request) |
| `ISingletonDependency` | Singleton | One instance for the application lifetime |

## Usage

### Basic Registration

```csharp
public interface IOrderService
{
    Task<OrderDto> GetOrderAsync(long id);
}

// Automatically registered as Transient
public class OrderService : IOrderService, ITransientDependency
{
    public Task<OrderDto> GetOrderAsync(long id)
    {
        return Task.FromResult(new OrderDto { Id = id });
    }
}
```

### Scoped Service

```csharp
public interface IUserContext
{
    long UserId { get; }
}

// One instance per HTTP request
public class UserContext : IUserContext, IScopedDependency
{
    public long UserId => /* resolve from JWT claims */;
}
```

### Singleton Service

```csharp
public interface ICacheWarmer
{
    Task WarmAsync();
}

// One instance for the entire application
public class CacheWarmer : ICacheWarmer, ISingletonDependency
{
    public Task WarmAsync() { /* ... */ }
}
```

## How It Works

When `AddDependencyInjection()` is called:

1. Scans all assemblies for types implementing `IDependency`
2. Determines the lifetime from the specific marker interface
3. Finds all public interfaces on the implementing type (excluding `IDisposable`, `IAsyncDisposable`, and the marker interfaces themselves)
4. Registers each interface → implementation pair
5. Supports generic types with matching generic arity

## Named Services

For scenarios where multiple implementations exist for the same interface:

```csharp
// Resolve named services via Func<string, IDependency, object>
var service = namedServiceProvider.GetService<IMyService>("ImplementationA");
```
