[**简体中文**](MODULES.zh.md) | [English](MODULES.md) · [返回 README](../README.zh.md)

# 模块手册

Fast.NET 由 17 个可独立发布的 NuGet 包组成。除 `Fast.IaaS` 外，主要模块面向 .NET 8、.NET 9 和 .NET 10；`Fast.IaaS` 面向 .NET Standard 2.1。

| 包 | 用途 | 主要入口 | 默认配置节点 / 外部依赖 |
| --- | --- | --- | --- |
| `Fast.Runtime` | ASP.NET Core 共享上下文、配置与 MVC 扩展 | `AddConfigurableOptions<T>()`、`AddMvcFilter()` | 通常由其他 Fast.NET 包传递引用 |
| `Fast.IaaS` | 编码、校验、脱敏、重试、树结构与通用扩展 | `MaskingUtil`、`RetryUtil` 等 | 无 Web 宿主依赖 |
| `Fast.NET.Core` | 应用初始化、配置扫描、CORS、压缩、请求缓冲 | `builder.Initialize()`、`AddCorsAccessor()`、`AddGzipCompression()` | `CorsAccessorSettings` |
| `Fast.Cache` | CSRedisCore 缓存封装 | `services.AddCache()` | `RedisSettings`；需要 Redis |
| `Fast.Consul` | 服务注册、健康检查和 KV | `services.AddFastConsul()` | `ConsulSettings`；需要 Consul |
| `Fast.DependencyInjection` | 基于标记接口的约定式依赖注入 | `services.AddDependencyInjection()` | 使用 `ITransientDependency`、`IScopedDependency`、`ISingletonDependency` |
| `Fast.DynamicApplication` | 将应用服务发现为动态 API | `services.AddDynamicApplication()` | 必须先调用 `AddControllers()` |
| `Fast.EventBus` | 进程内有界通道、发布订阅和后台处理 | `services.AddEventBus()` | 默认容量 3000；不提供分布式持久化 |
| `Fast.JwtBearer` | JWT Bearer 认证、授权和令牌工具 | `services.AddJwtBearer()` | `JWTSettings` |
| `Fast.Logging` | 控制台与文件日志扩展 | `services.AddLoggingService()` | `Logging:Fast`；文件目录必须可写 |
| `Fast.Mapster` | Mapster 注册与对象映射 | `services.AddMapster()` | Mapster 10 |
| `Fast.OpenApi` | OpenAPI DTO、Schema 和辅助工具 | `services.AddOpenApi()` | `OpenApiSettings` |
| `Fast.Serialization.System.Text.Json` | System.Text.Json 统一选项、转换器和脱敏 | `services.AddSerialization()` | 与 Newtonsoft.Json 模块通常二选一 |
| `Fast.Serialization.Newtonsoft.Json` | Newtonsoft.Json 统一选项、转换器和脱敏 | `services.AddSerialization()`、MVC 扩展 | 与 System.Text.Json 模块通常二选一 |
| `Fast.SqlSugar` | SqlSugar、多数据库配置、仓储和分页 | `services.AddSqlSugar()`、`AddSnowflake()` | `ConnectionSettings`、`SnowflakeSettings`；需要数据库 |
| `Fast.Swagger` | Swagger 文档、分组、安全定义和 UI | `AddSwaggerDocuments()`、`UseSwaggerDocuments()` | `SwaggerSettings` |
| `Fast.UnifyResult` | 统一响应、模型验证和友好异常 | `services.AddUnifyResult()` | 面向 MVC/控制器应用 |

## 推荐注册顺序

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

顺序约束只有在模块之间存在实际依赖时才重要。最明确的一项是：`AddDynamicApplication()` 必须位于 MVC 控制器注册之后。

## 基础设施模块注意事项

- `Fast.Cache`、`Fast.Consul` 和 `Fast.SqlSugar` 需要真实外部服务。验证时应使用隔离环境，不要在仓库检查流程中直接修改生产数据。
- `Fast.EventBus` 是进程内事件总线，应用重启后不会恢复尚未处理的事件。需要跨进程、持久化或至少一次投递语义时，应选择专用消息中间件。
- 两个序列化包都使用 `Fast.Serialization` 命名空间和相近扩展名，同一应用应明确选择并配置其中一个。
- `Fast.Logging` 的文件日志依赖运行目录权限；容器中应将目录映射到可写卷或改用标准日志输出。

## 配置来源

选项类型实现了仓库约定的后配置流程。最终支持字段以各模块的 `*SettingsOptions` 类型和 XML API 文档为准。不要在文档、示例或 Issue 中提交真实令牌、连接字符串或密码。
