[简体中文](MODULES.zh.md) | [**English**](MODULES.md) · [Back to README](../README.md)

# Module guide

Fast.NET consists of 17 independently published NuGet packages. The primary modules target .NET 8, .NET 9, and .NET 10; `Fast.IaaS` targets .NET Standard 2.1.

| Package | Purpose | Main entry point | Default configuration / external dependency |
| --- | --- | --- | --- |
| `Fast.Runtime` | Shared ASP.NET Core context, configuration, and MVC extensions | `AddConfigurableOptions<T>()`, `AddMvcFilter()` | Usually referenced transitively |
| `Fast.IaaS` | Encoding, validation, masking, retry, tree, and general utilities | `MaskingUtil`, `RetryUtil`, and related APIs | No web host dependency |
| `Fast.NET.Core` | Application initialization, configuration scanning, CORS, compression, and buffering | `builder.Initialize()`, `AddCorsAccessor()`, `AddGzipCompression()` | `CorsAccessorSettings` |
| `Fast.Cache` | CSRedisCore cache wrapper | `services.AddCache()` | `RedisSettings`; requires Redis |
| `Fast.Consul` | Service registration, health checks, and KV | `services.AddFastConsul()` | `ConsulSettings`; requires Consul |
| `Fast.DependencyInjection` | Marker-based convention registration | `services.AddDependencyInjection()` | `ITransientDependency`, `IScopedDependency`, `ISingletonDependency` |
| `Fast.DynamicApplication` | Dynamic API discovery for application services | `services.AddDynamicApplication()` | Call `AddControllers()` first |
| `Fast.EventBus` | Bounded in-process channel, publishing, subscription, and background handling | `services.AddEventBus()` | Default capacity 3000; not distributed or durable |
| `Fast.JwtBearer` | JWT Bearer authentication, authorization, and token helpers | `services.AddJwtBearer()` | `JWTSettings` |
| `Fast.Logging` | Console and file logging extensions | `services.AddLoggingService()` | `Logging:Fast`; file path must be writable |
| `Fast.Mapster` | Mapster registration and mapping | `services.AddMapster()` | Mapster 10 |
| `Fast.OpenApi` | OpenAPI DTOs, schemas, and utilities | `services.AddOpenApi()` | `OpenApiSettings` |
| `Fast.Serialization.System.Text.Json` | Shared System.Text.Json options, converters, and masking | `services.AddSerialization()` | Usually choose one serialization module |
| `Fast.Serialization.Newtonsoft.Json` | Shared Newtonsoft.Json options, converters, and masking | `services.AddSerialization()` and MVC extension | Usually choose one serialization module |
| `Fast.SqlSugar` | SqlSugar integration, multi-database settings, repositories, and paging | `services.AddSqlSugar()`, `AddSnowflake()` | `ConnectionSettings`, `SnowflakeSettings`; requires a database |
| `Fast.Swagger` | Swagger documents, grouping, security definitions, and UI | `AddSwaggerDocuments()`, `UseSwaggerDocuments()` | `SwaggerSettings` |
| `Fast.UnifyResult` | Unified responses, model validation, and friendly exceptions | `services.AddUnifyResult()` | Designed for MVC/controller applications |

## Recommended registration order

```csharp
builder.Initialize();

builder.Services.AddSerialization();
builder.Services.AddControllers();

builder.Services.AddDependencyInjection();
builder.Services.AddEventBus();
builder.Services.AddUnifyResult();
builder.Services.AddDynamicApplication();
builder.Services.AddSwaggerDocuments(builder.Configuration);
```

Order matters only where modules have an actual dependency. The explicit requirement is that `AddDynamicApplication()` runs after MVC controller registration.

## Dependency injection conventions

`AddDependencyInjection()` scans non-abstract classes that implement `ITransientDependency`, `IScopedDependency`, or `ISingletonDependency`. Services that expose business interfaces are registered against those interfaces; services that only implement a lifetime marker are registered by their concrete type and can be injected directly.

## Infrastructure notes

- `Fast.Cache`, `Fast.Consul`, and `Fast.SqlSugar` require real external services. Use an isolated environment for validation and never point repository checks at production data.
- `Fast.EventBus` is in-process. Pending events are not recovered after application restart. Use a dedicated message broker when you need cross-process, durable, or at-least-once delivery.
- Both serialization packages use the `Fast.Serialization` namespace and similar extension names. Select and configure one deliberately.
- File logging requires a writable directory. In containers, map it to a writable volume or prefer standard output.

## Configuration source of truth

Option types participate in the repository post-configuration convention. The corresponding `*SettingsOptions` type and XML API documentation are authoritative for supported fields. Never include real tokens, connection strings, or passwords in documentation, samples, or issues.
