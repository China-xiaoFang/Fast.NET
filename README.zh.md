**中** | [En](README.md)

<div align="center">

<img src="Fast.png" alt="Fast.NET Logo" width="128" />

# Fast.NET

**一个模块化、高性能的 .NET 框架，用于快速构建现代应用程序。**

[![license](https://img.shields.io/badge/license-Apache--2.0-blue.svg)](LICENSE)
[![nuget](https://img.shields.io/nuget/v/Fast.NET.Core.svg?label=Fast.NET.Core)](https://www.nuget.org/packages/Fast.NET.Core)
[![dotnet](https://img.shields.io/badge/.NET-6.0%20%7C%207.0%20%7C%208.0%20%7C%209.0%20%7C%2010.0-purple.svg)](https://dotnet.microsoft.com)
[![C#](https://img.shields.io/badge/C%23-12.0-brightgreen.svg)](https://learn.microsoft.com/dotnet/csharp/)

[文档](docs/) · [快速开始](#快速开始) · [模块说明](#模块说明) · [参与贡献](#参与贡献) · [English](README.md)

</div>

---

## ✨ 特性

- 🧩 **模块化架构** — 按需选择模块，每个模块都是独立的 NuGet 包
- 🚀 **动态 API 生成** — 自动将服务类转换为 RESTful API 端点，无需手写控制器
- 💉 **自动依赖注入** — 通过实现标记接口（`ITransientDependency`、`IScopedDependency`、`ISingletonDependency`）自动注册服务
- 📡 **事件总线** — 进程内事件驱动架构，支持重试策略和执行监控
- 🗄️ **ORM 集成** — 无缝集成 [SqlSugar](https://gitee.com/dotnetchina/SqlSugar)，提供仓储模式、多租户和软删除
- 🔐 **JWT 认证** — 可配置的 JWT Bearer 认证，支持自定义授权处理
- 📝 **统一日志** — 文件和控制台日志，支持自动轮转、按级别分离和链路追踪
- 🔄 **序列化** — 同时支持 `System.Text.Json` 和 `Newtonsoft.Json`，内置自定义转换器
- 📦 **Redis 缓存** — 基于 [CSRedisCore](https://github.com/2881099/csredis) 的 Redis 缓存抽象
- 📖 **Swagger 集成** — 增强的 Swagger 文档，支持分组、授权和枚举展示
- 🌐 **Consul 服务发现** — 服务注册、健康检查和 KV 配置管理
- 🛡️ **统一返回** — 标准化的 RESTful 响应格式，全局异常处理和模型验证
- 🗺️ **对象映射** — 集成 [Mapster](https://github.com/MapsterMapper/Mapster)，零配置对象映射
- 🧰 **基础设施工具** — 全面的工具库（加密、数据脱敏、树形构建、验证等）
- 🌍 **跨平台** — 支持 Windows、Linux 和 macOS

## 🏗️ 架构概览

```
┌─────────────────────────────────────────────────────────────────┐
│                         您的应用程序                            │
├──────────┬──────────┬──────────┬──────────┬──────────┬─────────┤
│  动态API  │ 统一返回 │ Swagger  │ OpenApi  │ JwtBearer│ Consul  │
│          │          │          │          │          │         │
├──────────┴──────────┴──────────┴──────────┴──────────┴─────────┤
│         Fast.NET.Core  (配置、中间件、工具类)                    │
├──────────┬──────────┬──────────┬──────────┬──────────┬─────────┤
│ 依赖注入  │ 事件总线 │ SqlSugar │   缓存   │   日志   │ Mapster │
│          │          │          │          │          │         │
├──────────┴──────────┼──────────┴──────────┼──────────┴─────────┤
│     序列化           │    Fast.Runtime    │    Fast.IaaS       │
│  (STJ / Newtonsoft) │  (上下文、DI、扩展)  │   (基础工具类)      │
└─────────────────────┴─────────────────────┴────────────────────┘
```

## 📋 环境要求

- .NET 6.0、7.0、8.0、9.0 或 10.0
- C# 12.0（语言版本）
- Visual Studio 2022+ / VS Code / Rider

## 🚀 快速开始

### 1. 安装 NuGet 包

按需安装所需的模块：

```bash
dotnet add package Fast.NET.Core
dotnet add package Fast.DependencyInjection
dotnet add package Fast.DynamicApplication
dotnet add package Fast.Serialization.System.Text.Json
```

### 2. 配置 `Program.cs`

```csharp
using Fast.DependencyInjection;
using Fast.DynamicApplication;
using Fast.NET.Core;
using Fast.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 初始化 Fast.NET 框架
builder.Initialize();

// 添加服务
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

### 3. 创建动态 API 服务

```csharp
using Fast.DynamicApplication;

namespace MyApp.Services;

/// <summary>
/// 用户服务 — 自动生成为 REST API 控制器
/// </summary>
public class UserService : IDynamicApplication
{
    public string GetHello()
    {
        return "Hello from Fast.NET!";
    }

    public Task<UserDto> GetUserAsync(long id)
    {
        return Task.FromResult(new UserDto { Id = id, Name = "测试用户" });
    }
}

public class UserDto
{
    public long Id { get; set; }
    public string Name { get; set; }
}
```

### 4. 自动依赖注入

```csharp
using Fast.DependencyInjection;

// 自动注册为瞬时服务
public class OrderService : IOrderService, ITransientDependency
{
    public Task<string> CreateOrderAsync() => Task.FromResult("订单已创建");
}

public interface IOrderService
{
    Task<string> CreateOrderAsync();
}
```

### 5. 事件总线

```csharp
using Fast.EventBus;

// 发布事件
public class OrderController
{
    private readonly IEventPublisher _publisher;

    public OrderController(IEventPublisher publisher) => _publisher = publisher;

    public async Task CreateOrder()
    {
        await _publisher.PublishAsync("OrderCreated", new { OrderId = 1 });
    }
}

// 订阅事件
public class OrderEventSubscriber : IEventSubscriber
{
    [EventSubscribe("OrderCreated")]
    public async Task OnOrderCreated(EventHandlerExecutingContext context)
    {
        var payload = context.Payload;
        // 处理事件...
    }
}
```

## 📦 模块说明

| 模块 | 版本 | 说明 |
| --- | --- | --- |
| [Fast.Runtime](src/Runtime) | [![nuget](https://img.shields.io/nuget/v/Fast.Runtime.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Runtime) | 核心运行库 — 应用上下文、服务解析、扩展方法 |
| [Fast.IaaS](src/IaaS) | [![nuget](https://img.shields.io/nuget/v/Fast.IaaS.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.IaaS) | 基础设施工具 — 加密、验证、数据脱敏、树形构建 |
| [Fast.NET.Core](src/Core) | [![nuget](https://img.shields.io/nuget/v/Fast.NET.Core.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.NET.Core) | 核心框架 — 初始化、配置、中间件、跨域、Gzip |
| [Fast.DependencyInjection](src/DependencyInjection) | [![nuget](https://img.shields.io/nuget/v/Fast.DependencyInjection.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.DependencyInjection) | 自动依赖注入（标记接口） |
| [Fast.DynamicApplication](src/DynamicApplication) | [![nuget](https://img.shields.io/nuget/v/Fast.DynamicApplication.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.DynamicApplication) | 动态 API 生成 |
| [Fast.EventBus](src/EventBus) | [![nuget](https://img.shields.io/nuget/v/Fast.EventBus.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.EventBus) | 进程内事件总线（支持重试策略） |
| [Fast.SqlSugar](src/SqlSugar) | [![nuget](https://img.shields.io/nuget/v/Fast.SqlSugar.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.SqlSugar) | ORM 集成，仓储模式和多租户（[SqlSugar](https://gitee.com/dotnetchina/SqlSugar)） |
| [Fast.Cache](src/Cache) | [![nuget](https://img.shields.io/nuget/v/Fast.Cache.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Cache) | Redis 缓存抽象（[CSRedisCore](https://github.com/2881099/csredis)） |
| [Fast.Logging](src/Logging) | [![nuget](https://img.shields.io/nuget/v/Fast.Logging.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Logging) | 文件和控制台日志（自动轮转、链路追踪） |
| [Fast.JwtBearer](src/JwtBearer) | [![nuget](https://img.shields.io/nuget/v/Fast.JwtBearer.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.JwtBearer) | JWT Bearer 认证和授权 |
| [Fast.Swagger](src/Swagger) | [![nuget](https://img.shields.io/nuget/v/Fast.Swagger.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Swagger) | 增强的 Swagger 文档（[Swashbuckle](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)） |
| [Fast.OpenApi](src/OpenApi) | [![nuget](https://img.shields.io/nuget/v/Fast.OpenApi.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.OpenApi) | OpenAPI 文档生成和 TypeScript 代码生成 |
| [Fast.UnifyResult](src/UnifyResult) | [![nuget](https://img.shields.io/nuget/v/Fast.UnifyResult.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.UnifyResult) | RESTful 风格统一返回（异常处理） |
| [Fast.Mapster](src/Mapster) | [![nuget](https://img.shields.io/nuget/v/Fast.Mapster.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Mapster) | 对象映射集成（[Mapster](https://github.com/MapsterMapper/Mapster)） |
| [Fast.Serialization.System.Text.Json](src/Serialization.System.Text.Json) | [![nuget](https://img.shields.io/nuget/v/Fast.Serialization.System.Text.Json.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Serialization.System.Text.Json) | System.Text.Json 序列化（自定义转换器） |
| [Fast.Serialization.Newtonsoft.Json](src/Serialization.Newtonsoft.Json) | [![nuget](https://img.shields.io/nuget/v/Fast.Serialization.Newtonsoft.Json.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Serialization.Newtonsoft.Json) | Newtonsoft.Json 序列化（自定义转换器） |
| [Fast.Consul](src/Consul) | [![nuget](https://img.shields.io/nuget/v/Fast.Consul.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Consul) | Consul 服务发现、健康检查和 KV 存储 |

## 📄 完整示例

使用所有模块的完整 `Program.cs`：

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

// 初始化框架
builder.Initialize();

// 注册服务
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

## 📖 文档

完整文档在 [`docs/`](docs/) 目录中：

- [快速开始](docs/getting-started.md) — 安装、配置和第一个项目
- [架构概览](docs/architecture.md) — 框架设计和模块依赖
- [模块指南](docs/modules/) — 每个模块的详细文档
  - [Runtime](docs/modules/runtime.md) · [Core](docs/modules/core.md) · [依赖注入](docs/modules/dependency-injection.md) · [动态API](docs/modules/dynamic-application.md)
  - [SqlSugar](docs/modules/sqlsugar.md) · [事件总线](docs/modules/event-bus.md) · [缓存](docs/modules/cache.md) · [日志](docs/modules/logging.md)
  - [JwtBearer](docs/modules/jwt-bearer.md) · [Swagger](docs/modules/swagger.md) · [统一返回](docs/modules/unify-result.md) · [序列化](docs/modules/serialization.md)
  - [Mapster](docs/modules/mapster.md) · [IaaS](docs/modules/iaas.md) · [OpenApi](docs/modules/openapi.md) · [Consul](docs/modules/consul.md)
- [配置参考](docs/configuration.md) — 所有配置选项
- [更新日志](https://gitee.com/FastDotnet/Fast.NET/commits/master)

## �� 参与贡献

欢迎参与贡献！以下是参与步骤：

1. **Fork** 本仓库
2. **创建** 功能分支（`git checkout -b feature/my-feature`）
3. **提交** 更改（`git commit -m '添加新功能'`）
4. **推送** 到分支（`git push origin feature/my-feature`）
5. **提交** Pull Request

请确保代码遵循项目的编码风格和规范。

## 📝 许可证

Fast.NET 采用 [Apache License 2.0](LICENSE) 开源协议。

```
版权所有 © 2018-Now 小方

根据 Apache 许可证 2.0 版（"许可证"）获得许可；
除非遵守许可证，否则您不得使用此文件。
您可以在以下网址获取许可证副本：

    http://www.apache.org/licenses/LICENSE-2.0

除非适用法律要求或书面同意，否则根据许可证分发的软件
按"原样"分发，不附带任何明示或暗示的保证或条件。
请参阅许可证以了解特定语言的权限和限制。
```

## 👥 团队成员

| 成员 | 职位 | 昵称 |
| --- | --- | --- |
| 小方 | 全栈开发 | 1.8K 仔 |

## ⭐ 支持

如果 Fast.NET 对您有帮助，请点一个 **Star** ⭐ — 这对我们意义重大，也能帮助更多人发现这个项目！

## 🙏 致谢

Fast.NET 站在优秀开源项目的肩膀上：

- [SqlSugar](https://gitee.com/dotnetchina/SqlSugar) — 高性能 ORM 框架
- [CSRedisCore](https://github.com/2881099/csredis) — Redis 客户端
- [Mapster](https://github.com/MapsterMapper/Mapster) — 对象映射
- [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) — Swagger/OpenAPI
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) — JSON 框架
- [Furion](https://github.com/MonkSoul/Furion) — 架构灵感
