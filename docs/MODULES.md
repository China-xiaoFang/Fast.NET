[简体中文](MODULES.zh.md) | [**English**](MODULES.md) · [Back to README](../README.md)

# Module guide

Fast.NET consists of 17 independently published NuGet packages. The primary modules target .NET 8, .NET 9, and .NET 10; `Fast.IaaS` targets .NET Standard 2.1.

This release updates 15 SDK packages, including the dependency versions of modules that reference `Fast.Runtime` 3.5.29; the two serialization packages retain their existing versions. Upgrade installed modules together using the package versions in [CHANGELOG](../CHANGELOG.md).

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
| `Fast.OpenApi` | OpenAPI DTO and JavaScript/TypeScript client generation | `services.AddOpenApi()` | `OpenApiSettings` |
| `Fast.Serialization.System.Text.Json` | Shared System.Text.Json options, converters, and masking | `services.AddSerialization()` | Usually choose one serialization module |
| `Fast.Serialization.Newtonsoft.Json` | Shared Newtonsoft.Json options, converters, and masking | `services.AddSerialization()` and MVC extension | Usually choose one serialization module |
| `Fast.SqlSugar` | SqlSugar integration, multi-database settings, repositories, and paging | `services.AddSqlSugar()`, `AddSnowflake()` | `ConnectionSettings`, `SnowflakeSettings`; requires a database |
| `Fast.Swagger` | Swagger documents, grouping, security definitions, and UI | `AddSwaggerDocuments()`, `UseSwaggerDocuments()` | `SwaggerSettings` |
| `Fast.UnifyResult` | Unified responses, model validation, and friendly exceptions | `services.AddUnifyResult()` | Designed for MVC/controller applications |

`Fast.OpenApi` generates TypeScript clients with separate `import type` declarations, explicit `Promise<T>` return types, and `unknown` for values without a concrete schema, compatible with `verbatimModuleSyntax` and the current Fast ESLint Config rules. Generated Web and mobile multipart upload methods also expose an optional Axios `onUploadProgress` callback.

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

## Console output

Starting with `Fast.Runtime` 3.5.29, use `MAppContext.ConsoleWrite` to write text with colors applied to individual segments:

```csharp
MAppContext.ConsoleWrite(e =>
{
    e.ForegroundColor = ConsoleColor.Blue;
    e.WriteLine();
    e.WriteLine("Fast.NET");
});
```

The callback receives a `Fast.Runtime.ConsoleWriter` with `ForegroundColor`, `BackgroundColor`, `Write`, `WriteLine`, and `ResetColor()`. The writer captures both colors on entry and restores them in `finally`; `ResetColor()` restores the same colors within the callback. Callbacks sharing an output stream execute serially and support nesting. Callbacks must complete synchronously, and callback or write exceptions propagate to the caller. When `Console.IsOutputRedirected` is `true`, color assignments are ignored and only text is written, without generated ANSI color codes.

Startup banners in `Fast.NET.Core` 3.5.35 and direct diagnostics in Runtime, OpenApi, and SqlSugar use this entry point for Windows CMD, PowerShell, Windows Terminal, and Linux terminals. Banner wording, the ASCII logo, and color meanings are preserved: the startup time is `Blue`, the ASCII logo is `Green`, the Gitee address is `Red`, and the descriptive text is `Magenta`. `Fast.IaaS` 3.5.26 remains a standalone `netstandard2.1` package and uses its internal writer in `src/IaaS/Internals/ConsoleWriter.cs` without a Runtime dependency.

`Fast.Logging` continues to write through the `TextWriter` supplied to its `ConsoleFormatter`. `ConsoleLoggerProvider` handles terminal adaptation, ANSI parsing where needed, and queued console writes. The formatter does not change `Console.ForegroundColor`; `LoggerColorBehavior.Default` suppresses generated colors for redirected output, while ANSI support is handled by the provider rather than inferred solely from redirection.

## Dependency injection conventions

`AddDependencyInjection()` scans non-abstract classes that implement `ITransientDependency`, `IScopedDependency`, or `ISingletonDependency`. Services that expose business interfaces are registered against those interfaces; services that only implement a lifetime marker are registered by their concrete type and can be injected directly.

## Infrastructure notes

- `Fast.Cache`, `Fast.Consul`, and `Fast.SqlSugar` require real external services. Use an isolated environment for validation and never point repository checks at production data.
- `Fast.EventBus` is in-process. Pending events are not recovered after application restart. Use a dedicated message broker when you need cross-process, durable, or at-least-once delivery.
- Both serialization packages use the `Fast.Serialization` namespace and similar extension names. Select and configure one deliberately.
- File logging requires a writable directory. In containers, map it to a writable volume or prefer standard output.

## Configuration source of truth

Option types participate in the repository post-configuration convention. The corresponding `*SettingsOptions` type and XML API documentation are authoritative for supported fields. Never include real tokens, connection strings, or passwords in documentation, samples, or issues.
