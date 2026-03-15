[中](README.zh.md) | **En**

<div align="center">

<img src="Fast.png" alt="Fast.NET Logo" width="128" />

# Fast.NET

**A modular, high-performance .NET framework for rapidly building modern applications.**

[![license](https://img.shields.io/badge/license-Apache--2.0-blue.svg)](LICENSE)
[![nuget](https://img.shields.io/nuget/v/Fast.NET.Core.svg?label=Fast.NET.Core)](https://www.nuget.org/packages/Fast.NET.Core)
[![dotnet](https://img.shields.io/badge/.NET-6.0%20%7C%207.0%20%7C%208.0%20%7C%209.0%20%7C%2010.0-purple.svg)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23-12.0-brightgreen.svg)](https://learn.microsoft.com/dotnet/csharp/)

[Documentation](docs/) · [Quick Start](#quick-start) · [Modules](#modules) · [Contributing](#contributing) · [中文](README.zh.md)

</div>

---

## ✨ Features

- 🧩 **Modular Architecture** — Pick only the modules you need; each module is an independent NuGet package
- 🚀 **Dynamic API Generation** — Automatically convert service classes into RESTful API endpoints without writing controllers
- 💉 **Auto Dependency Injection** — Register services by simply implementing marker interfaces (`ITransientDependency`, `IScopedDependency`, `ISingletonDependency`)
- 📡 **Event Bus** — In-process event-driven architecture with retry policies and monitoring
- 🗄️ **ORM Integration** — Seamlessly integrated [SqlSugar](https://github.com/DotNetNext/SqlSugar) with repository pattern, multi-tenancy, and soft delete
- 🔐 **JWT Authentication** — Configurable JWT Bearer authentication with custom authorization handlers
- 📝 **Unified Logging** — File and console logging with automatic rotation, per-level separation, and trace correlation
- 🔄 **Serialization** — Drop-in support for both `System.Text.Json` and `Newtonsoft.Json` with custom converters
- 📦 **Redis Caching** — Redis cache abstraction based on [CSRedisCore](https://github.com/2881099/csredis)
- 📖 **Swagger Integration** — Enhanced Swagger documentation with grouping, authorization, and enum support
- 🌐 **Consul Service Discovery** — Service registration, health checks, and KV configuration
- 🛡️ **Unified Result** — Standardized RESTful response format with global exception handling and model validation
- 🗺️ **Object Mapping** — Integrated [Mapster](https://github.com/MapsterMapper/Mapster) for zero-config object mapping
- 🧰 **Infrastructure Utilities** — Comprehensive utility library (encryption, data masking, tree building, validation, and more)
- 🌍 **Cross-Platform** — Runs on Windows, Linux, and macOS

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        Your Application                        │
├──────────┬──────────┬──────────┬──────────┬──────────┬─────────┤
│ Dynamic  │ Unified  │ Swagger  │ OpenApi  │ JwtBearer│ Consul  │
│Application│ Result  │          │          │          │         │
├──────────┴──────────┴──────────┴──────────┴──────────┴─────────┤
│         Fast.NET.Core  (Configuration, Middleware, Utils)       │
├──────────┬──────────┬──────────┬──────────┬──────────┬─────────┤
│Dependency│  Event   │ SqlSugar │  Cache   │ Logging  │ Mapster │
│Injection │  Bus     │          │          │          │         │
├──────────┴──────────┼──────────┴──────────┼──────────┴─────────┤
│   Serialization     │     Fast.Runtime    │     Fast.IaaS      │
│ (STJ / Newtonsoft)  │  (Context, DI, Ext) │   (Utilities)      │
└─────────────────────┴─────────────────────┴────────────────────┘
```

## 📋 Requirements

- .NET 6.0, 7.0, 8.0, 9.0, or 10.0
- C# 12.0 (LangVersion)
- Visual Studio 2022+ / VS Code / Rider

## 🚀 Quick Start

### 1. Install packages

Install only the modules you need:

```bash
dotnet add package Fast.NET.Core
dotnet add package Fast.DependencyInjection
dotnet add package Fast.DynamicApplication
dotnet add package Fast.Serialization.System.Text.Json
```

### 2. Configure `Program.cs`

```csharp
using Fast.DependencyInjection;
using Fast.DynamicApplication;
using Fast.NET.Core;
using Fast.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Initialize the Fast.NET framework
builder.Initialize();

// Add services
builder.Services.AddSerialization();
builder.Services.AddDependencyInjection();
builder.Services.AddControllers();
builder.Services.AddDynamicApplication();

var app = builder.Build();

app.UseHttpsRedirection();
app.EnableBuffering();
app.UseRouting();
app.MapControllers();
app.Run();
```

### 3. Create a Dynamic API service

```csharp
using Fast.DynamicApplication;

namespace MyApp.Services;

/// <summary>
/// User Service — automatically becomes a REST API controller
/// </summary>
public class UserService : IDynamicApplication
{
    public string GetHello()
    {
        return "Hello from Fast.NET!";
    }

    public Task<UserDto> GetUserAsync(long id)
    {
        return Task.FromResult(new UserDto { Id = id, Name = "Test User" });
    }
}

public class UserDto
{
    public long Id { get; set; }
    public string Name { get; set; }
}
```

### 4. Auto Dependency Injection

```csharp
using Fast.DependencyInjection;

// Automatically registered as Transient
public class OrderService : IOrderService, ITransientDependency
{
    public Task<string> CreateOrderAsync() => Task.FromResult("Order created");
}

public interface IOrderService
{
    Task<string> CreateOrderAsync();
}
```

### 5. Event Bus

```csharp
using Fast.EventBus;

// Publish an event
public class OrderController
{
    private readonly IEventPublisher _publisher;

    public OrderController(IEventPublisher publisher) => _publisher = publisher;

    public async Task CreateOrder()
    {
        await _publisher.PublishAsync("OrderCreated", new { OrderId = 1 });
    }
}

// Subscribe to the event
public class OrderEventSubscriber : IEventSubscriber
{
    [EventSubscribe("OrderCreated")]
    public async Task OnOrderCreated(EventHandlerExecutingContext context)
    {
        var payload = context.Payload;
        // Handle event...
    }
}
```

## 📦 Modules

| Module | Version | Description |
| --- | --- | --- |
| [Fast.Runtime](src/Runtime) | [![nuget](https://img.shields.io/nuget/v/Fast.Runtime.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Runtime) | Core runtime library — application context, service resolution, and extension methods |
| [Fast.IaaS](src/IaaS) | [![nuget](https://img.shields.io/nuget/v/Fast.IaaS.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.IaaS) | Infrastructure utilities — encryption, validation, data masking, tree building |
| [Fast.NET.Core](src/Core) | [![nuget](https://img.shields.io/nuget/v/Fast.NET.Core.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.NET.Core) | Core framework — initialization, configuration, middleware, CORS, Gzip |
| [Fast.DependencyInjection](src/DependencyInjection) | [![nuget](https://img.shields.io/nuget/v/Fast.DependencyInjection.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.DependencyInjection) | Auto dependency injection via marker interfaces |
| [Fast.DynamicApplication](src/DynamicApplication) | [![nuget](https://img.shields.io/nuget/v/Fast.DynamicApplication.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.DynamicApplication) | Dynamic API generation from service classes |
| [Fast.EventBus](src/EventBus) | [![nuget](https://img.shields.io/nuget/v/Fast.EventBus.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.EventBus) | In-process event bus with retry policies |
| [Fast.SqlSugar](src/SqlSugar) | [![nuget](https://img.shields.io/nuget/v/Fast.SqlSugar.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.SqlSugar) | ORM integration with repository pattern and multi-tenancy ([SqlSugar](https://github.com/DotNetNext/SqlSugar)) |
| [Fast.Cache](src/Cache) | [![nuget](https://img.shields.io/nuget/v/Fast.Cache.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Cache) | Redis cache abstraction ([CSRedisCore](https://github.com/2881099/csredis)) |
| [Fast.Logging](src/Logging) | [![nuget](https://img.shields.io/nuget/v/Fast.Logging.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Logging) | File and console logging with rotation and trace correlation |
| [Fast.JwtBearer](src/JwtBearer) | [![nuget](https://img.shields.io/nuget/v/Fast.JwtBearer.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.JwtBearer) | JWT Bearer authentication and authorization |
| [Fast.Swagger](src/Swagger) | [![nuget](https://img.shields.io/nuget/v/Fast.Swagger.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Swagger) | Enhanced Swagger documentation ([Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)) |
| [Fast.OpenApi](src/OpenApi) | [![nuget](https://img.shields.io/nuget/v/Fast.OpenApi.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.OpenApi) | OpenAPI document generation and TypeScript codegen support |
| [Fast.UnifyResult](src/UnifyResult) | [![nuget](https://img.shields.io/nuget/v/Fast.UnifyResult.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.UnifyResult) | Unified RESTful response format with exception handling |
| [Fast.Mapster](src/Mapster) | [![nuget](https://img.shields.io/nuget/v/Fast.Mapster.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Mapster) | Object mapping integration ([Mapster](https://github.com/MapsterMapper/Mapster)) |
| [Fast.Serialization.System.Text.Json](src/Serialization.System.Text.Json) | [![nuget](https://img.shields.io/nuget/v/Fast.Serialization.System.Text.Json.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Serialization.System.Text.Json) | System.Text.Json serialization with custom converters |
| [Fast.Serialization.Newtonsoft.Json](src/Serialization.Newtonsoft.Json) | [![nuget](https://img.shields.io/nuget/v/Fast.Serialization.Newtonsoft.Json.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Serialization.Newtonsoft.Json) | Newtonsoft.Json serialization with custom converters |
| [Fast.Consul](src/Consul) | [![nuget](https://img.shields.io/nuget/v/Fast.Consul.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Consul) | Consul service discovery, health checks, and KV store |

## 📄 Full Example

A complete `Program.cs` using all modules:

```csharp
using Fast.Cache;
using Fast.DependencyInjection;
using Fast.DynamicApplication;
using Fast.EventBus;
using Fast.JwtBearer;
using Fast.Logging;
using Fast.Mapster;
using Fast.NET.Core;
using Fast.Serialization;
using Fast.SqlSugar;
using Fast.Swagger;
using Fast.UnifyResult;

var builder = WebApplication.CreateBuilder(args);

// Initialize framework
builder.Initialize();

// Register services
builder.Services.AddSerialization();
builder.Services.AddLoggingService(builder.Environment);
builder.Services.AddCorsAccessor();
builder.Services.AddGzipCompression();
builder.Services.AddMapster();
builder.Services.AddDependencyInjection();
builder.Services.AddEventBus();
builder.Services.AddCache();
builder.Services.AddSqlSugar(builder.Configuration);
builder.Services.AddJwtBearer(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddDynamicApplication();
builder.Services.AddUnifyResult(builder.Configuration);
builder.Services.AddSwaggerDocuments(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.EnableBuffering();
app.UseRouting();
app.UseSwaggerDocuments();
app.MapControllers();
app.Run();
```

## 📖 Documentation

Comprehensive documentation is available in the [`docs/`](docs/) directory:

- [Getting Started](docs/getting-started.md) — Installation, setup, and first project
- [Architecture Overview](docs/architecture.md) — Framework design and module dependencies
- [Module Guide](docs/modules/) — Detailed documentation for each module
  - [Runtime](docs/modules/runtime.md) · [Core](docs/modules/core.md) · [DI](docs/modules/dependency-injection.md) · [Dynamic API](docs/modules/dynamic-application.md)
  - [SqlSugar](docs/modules/sqlsugar.md) · [Event Bus](docs/modules/event-bus.md) · [Cache](docs/modules/cache.md) · [Logging](docs/modules/logging.md)
  - [JwtBearer](docs/modules/jwt-bearer.md) · [Swagger](docs/modules/swagger.md) · [UnifyResult](docs/modules/unify-result.md) · [Serialization](docs/modules/serialization.md)
  - [Mapster](docs/modules/mapster.md) · [IaaS](docs/modules/iaas.md) · [OpenApi](docs/modules/openapi.md) · [Consul](docs/modules/consul.md)
- [Configuration Reference](docs/configuration.md) — All configuration options
- [Changelog](https://gitee.com/FastDotnet/Fast.NET/commits/master)

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/my-feature`)
3. **Commit** your changes (`git commit -m 'Add my feature'`)
4. **Push** to the branch (`git push origin feature/my-feature`)
5. **Open** a Pull Request

Please ensure your code follows the existing coding style and conventions.

## 📝 License

Fast.NET is licensed under the [Apache License 2.0](LICENSE).

```
Copyright © 2018-Now XiaoFang

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
```

## 👥 Team

| Member | Role | Nickname |
| --- | --- | --- |
| XiaoFang (小方) | Full Stack Developer | 1.8K 仔 |

## ⭐ Support

If you find Fast.NET helpful, please consider giving it a **Star** ⭐ — it means a lot and helps others discover the project!

## 🙏 Acknowledgements

Fast.NET stands on the shoulders of excellent open-source projects:

- [SqlSugar](https://github.com/DotNetNext/SqlSugar) — High-performance ORM
- [CSRedisCore](https://github.com/2881099/csredis) — Redis client
- [Mapster](https://github.com/MapsterMapper/Mapster) — Object mapping
- [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) — Swagger/OpenAPI
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) — JSON framework
- [Furion](https://github.com/MonkSoul/Furion) — Architecture inspiration
