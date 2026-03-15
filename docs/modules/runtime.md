# Fast.Runtime

Core runtime library providing the application context, service resolution, and essential extension methods.

## Installation

```bash
dotnet add package Fast.Runtime
```

> **Note:** Most higher-level modules (e.g., `Fast.NET.Core`) depend on `Fast.Runtime` automatically. You typically don't need to install it separately.

## FastContext

`FastContext` is a static class that provides global access to the application's runtime state.

### Properties

| Property | Type | Description |
| --- | --- | --- |
| `WebHostEnvironment` | `IWebHostEnvironment` | Web host environment information |
| `HostEnvironment` | `IHostEnvironment` | Host environment information |
| `Configuration` | `IConfiguration` | Global configuration (reloadable) |
| `HttpContext` | `HttpContext` | Current HTTP request context |
| `RootServices` | `IServiceProvider` | Root service provider |
| `InternalServices` | `IServiceCollection` | Internal service collection |

### Service Resolution

```csharp
// Resolve a service from the current request scope
var myService = FastContext.GetService<IMyService>();

// Resolve or throw if not registered
var required = FastContext.GetRequiredService<IMyService>();

// Resolve all implementations
var services = FastContext.GetServices<IMyService>();

// Get strongly-typed configuration
var settings = FastContext.GetConfig<MySettings>("MySection");

// Get IOptions<T>
var options = FastContext.GetOptions<MySettings>();
```

## Extension Methods

### IServiceCollection Extensions

```csharp
// Register strongly-typed options with validation support
services.AddConfigurableOptions<MySettings>("MySection");

// Register an MVC filter
services.AddMvcFilter<MyActionFilter>();
```

### Type Extensions

```csharp
// High-performance property setter using IL emit
var setter = typeof(MyClass).GetProperty("Name").CreatePropertySetter();
setter(instance, "value");
```

## IPostConfigure Interface

Configuration classes implementing `IPostConfigure` have their `PostConfigure()` method called after property binding:

```csharp
public class DatabaseSettings : IPostConfigure
{
    public string ConnectionString { get; set; }
    public int Timeout { get; set; }

    public void PostConfigure()
    {
        if (Timeout <= 0) Timeout = 30;
    }
}
```
