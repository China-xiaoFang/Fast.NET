# Architecture Overview / 架构概览

## Design Philosophy / 设计理念

Fast.NET follows these core principles:

1. **Modularity** — Each feature is an independent NuGet package. Use only what you need.
2. **Convention over Configuration** — Auto-discovery and registration reduce boilerplate code.
3. **Extensibility** — Core interfaces allow customization at every layer.
4. **Performance** — IL-emit based property setters, channel-based event queues, and efficient caching.

## Layer Architecture / 分层架构

```
┌─────────────────────────────────────────────────────────────────┐
│                      Application Layer                         │
│              (Your Controllers, Services, Models)              │
├──────────┬──────────┬──────────┬──────────┬──────────┬─────────┤
│ Dynamic  │ Unified  │ Swagger  │ OpenApi  │ JwtBearer│ Consul  │
│Application│ Result  │          │          │          │         │
│          │          │          │          │          │         │
│ Auto-gen │ Response │ API Docs │ Codegen  │ Auth     │ Service │
│ REST APIs│ Wrapping │          │ Support  │          │ Disc.   │
├──────────┴──────────┴──────────┴──────────┴──────────┴─────────┤
│                     Fast.NET.Core                              │
│        (Initialization, Configuration, Middleware)             │
├──────────┬──────────┬──────────┬──────────┬──────────┬─────────┤
│Dependency│  Event   │ SqlSugar │  Cache   │ Logging  │ Mapster │
│Injection │  Bus     │          │ (Redis)  │          │         │
│          │          │          │          │          │         │
│ Auto-scan│ Pub/Sub  │ ORM +    │ CSRedis  │ File +   │ Object  │
│ Register │ + Retry  │ Repo     │ Wrapper  │ Console  │ Mapping │
├──────────┴──────────┼──────────┴──────────┼──────────┴─────────┤
│   Serialization     │   Fast.Runtime      │    Fast.IaaS       │
│ System.Text.Json OR │ (FastContext,        │ (Utilities:        │
│ Newtonsoft.Json     │  Extensions, DI)    │  Crypto, Mask,     │
│                     │                     │  Validate, Tree)   │
└─────────────────────┴─────────────────────┴────────────────────┘
```

## Module Dependency Graph / 模块依赖图

```
Fast.IaaS (netstandard2.1, standalone)

Fast.Runtime
  └── depends on: Fast.IaaS

Fast.NET.Core
  └── depends on: Fast.Runtime

Fast.Serialization.System.Text.Json
  └── depends on: Fast.Runtime

Fast.Serialization.Newtonsoft.Json
  └── depends on: Fast.Runtime

Fast.DependencyInjection
  └── depends on: Fast.Runtime

Fast.Logging
  └── depends on: Fast.Runtime

Fast.Mapster
  └── depends on: Fast.Runtime

Fast.Cache
  └── depends on: Fast.Runtime

Fast.EventBus
  └── depends on: Fast.Runtime

Fast.SqlSugar
  └── depends on: Fast.Runtime

Fast.JwtBearer
  └── depends on: Fast.Runtime

Fast.DynamicApplication
  └── depends on: Fast.Runtime

Fast.UnifyResult
  └── depends on: Fast.Runtime

Fast.Swagger
  └── depends on: Fast.Runtime

Fast.OpenApi
  └── depends on: Fast.Runtime

Fast.Consul
  └── depends on: Fast.Runtime
```

## Key Patterns / 关键设计模式

### 1. Marker Interface DI / 标记接口依赖注入

Services are automatically registered by implementing lifetime interfaces:

```csharp
// Registered as Transient
public class MyService : IMyService, ITransientDependency { }

// Registered as Scoped
public class MyScopedService : IMyScopedService, IScopedDependency { }

// Registered as Singleton
public class MySingletonService : IMySingletonService, ISingletonDependency { }
```

### 2. Dynamic API Convention / 动态 API 约定

Service classes implementing `IDynamicApplication` are automatically exposed as REST endpoints:

| Method Prefix | HTTP Verb | Example |
| --- | --- | --- |
| `Get*` | GET | `GetUser()` → `GET /api/user` |
| `Post*` | POST | `PostCreate()` → `POST /api/create` |
| `Put*` | PUT | `PutUpdate()` → `PUT /api/update` |
| `Delete*` | DELETE | `DeleteRemove()` → `DELETE /api/remove` |

### 3. FastContext / 全局上下文

`FastContext` provides global access to application state:

```csharp
// Access configuration anywhere
var config = FastContext.Configuration;

// Resolve services
var service = FastContext.GetService<IMyService>();

// Access HTTP context
var httpContext = FastContext.HttpContext;
```

### 4. IPostConfigure Pattern / 后置配置模式

Configuration classes implementing `IPostConfigure` are automatically initialized after binding:

```csharp
public class MySettings : IPostConfigure
{
    public string ConnectionString { get; set; }

    public void PostConfigure()
    {
        // Set defaults or validate after binding
        if (string.IsNullOrEmpty(ConnectionString))
            ConnectionString = "default-connection";
    }
}
```

### 5. Unified Result Pattern / 统一返回模式

All API responses are automatically wrapped in a standard format:

```json
{
  "code": 200,
  "success": true,
  "data": { /* actual response data */ },
  "message": null,
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### 6. Event-Driven Architecture / 事件驱动架构

In-process event bus with channel-based queue:

```
Publisher → Channel (bounded, 3000 capacity) → Background Service → Subscriber
                                                     ↓
                                              Retry Policy (configurable)
                                                     ↓
                                              Fallback Policy (on failure)
```

## Startup Flow / 启动流程

1. `builder.Initialize()` — Registers base services, loads configuration files
2. `builder.Services.Add*()` — Register module services
3. `builder.Build()` — Build the application
4. `app.Use*()` — Configure middleware pipeline
5. `app.Run()` — Start the application

During `Initialize()`:
- Displays Fast.NET startup banner
- Stores environment references in `FastContext`
- Auto-loads JSON configuration from standard directories (`AppConfig/`, `AppSettings/`, `Config/`, `Settings/`, `JsonConfig/`)
- Registers `IHttpContextAccessor`, `IMemoryCache`, `ForwardedHeaders`
- Configures reverse proxy client IP resolution

## Project Structure / 项目结构

```
src/
├── Runtime/            # Core runtime (FastContext, extensions, attributes)
├── IaaS/               # Infrastructure utilities (standalone, netstandard2.1)
├── Core/               # Framework core (initialization, middleware, config)
├── DependencyInjection/# Auto DI registration
├── DynamicApplication/ # Dynamic API generation
├── EventBus/           # In-process event bus
├── SqlSugar/           # ORM with repository pattern
├── Cache/              # Redis cache abstraction
├── Logging/            # File and console logging
├── JwtBearer/          # JWT authentication
├── Swagger/            # Enhanced Swagger docs
├── OpenApi/            # OpenAPI codegen support
├── UnifyResult/        # Unified response format
├── Mapster/            # Object mapping
├── Serialization.System.Text.Json/    # STJ serialization
├── Serialization.Newtonsoft.Json/     # Newtonsoft serialization
└── Consul/             # Service discovery
```
