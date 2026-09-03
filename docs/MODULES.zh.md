[**简体中文**](MODULES.zh.md) | [English](MODULES.md) · [返回 README](../README.zh.md)

# 模块手册

Fast.NET 由 17 个可独立发布的 NuGet 包组成。除 `Fast.IaaS` 外，主要模块面向 .NET 8、.NET 9 和 .NET 10；`Fast.IaaS` 面向 .NET Standard 2.1。

本次发布同步更新 15 个 SDK 包，并更新引用 `Fast.Runtime` 3.5.29 的模块依赖版本；两个序列化包沿用原版本。升级时请按 [CHANGELOG](../CHANGELOG.md) 中的包版本清单同步更新已安装的模块。

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
| `Fast.OpenApi` | OpenAPI DTO 与 JavaScript/TypeScript 客户端生成 | `services.AddOpenApi()` | `OpenApiSettings` |
| `Fast.Serialization.System.Text.Json` | System.Text.Json 统一选项、转换器和脱敏 | `services.AddSerialization()` | 与 Newtonsoft.Json 模块通常二选一 |
| `Fast.Serialization.Newtonsoft.Json` | Newtonsoft.Json 统一选项、转换器和脱敏 | `services.AddSerialization()`、MVC 扩展 | 与 System.Text.Json 模块通常二选一 |
| `Fast.SqlSugar` | SqlSugar、多数据库配置、仓储和分页 | `services.AddSqlSugar()`、`AddSnowflake()` | `ConnectionSettings`、`SnowflakeSettings`；需要数据库 |
| `Fast.Swagger` | Swagger 文档、分组、安全定义和 UI | `AddSwaggerDocuments()`、`UseSwaggerDocuments()` | `SwaggerSettings` |
| `Fast.UnifyResult` | 统一响应、模型验证和友好异常 | `services.AddUnifyResult()` | 面向 MVC/控制器应用 |

`Fast.OpenApi` 生成的 TypeScript 客户端使用独立的 `import type` 和显式 `Promise<T>` 返回类型。返回非泛型 `Task` 或 `ValueTask` 的接口映射为 `Promise<void>`。`Download`、`Export` 接口保留文件响应：Web 端使用 `AxiosResponse<Blob>`，移动端使用 `AxiosResponse<Blob | ArrayBuffer | string>`，兼容浏览器 Blob、二进制缓冲区和原生临时文件路径。`autoDownloadFile` 参数默认值为 `true`；传入 `false` 时仅返回文件响应，不请求自动保存。其他缺少明确 Schema 的值仍映射为 `unknown`。生成结果兼容 `verbatimModuleSyntax` 与当前 Fast ESLint Config 规则。生成的 Web 端与移动端 multipart 上传方法还会暴露可选的 Axios `onUploadProgress` 回调。

`Fast.IaaS` 3.5.27 修复 `ToDescription()` 的整数时间边界：一分钟输出 `01分00秒`，一小时输出 `01时00分00秒`，一天输出 `01天00时00分00秒`。

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

## 控制台输出

从 `Fast.Runtime` 3.5.29 起，可通过 `MAppContext.ConsoleWrite` 分段设置控制台颜色并输出文本：

```csharp
MAppContext.ConsoleWrite(e =>
{
    e.ForegroundColor = ConsoleColor.Blue;
    e.WriteLine();
    e.WriteLine("Fast.NET");
});
```

回调接收 `Fast.Runtime.ConsoleWriter`，支持 `ForegroundColor`、`BackgroundColor`、`Write`、`WriteLine` 和 `ResetColor()`。写入器自动记录进入回调时的前景色和背景色，并在 `finally` 中恢复；`ResetColor()` 可在回调中提前恢复这组颜色。同一输出流上的回调串行执行，也支持嵌套调用；回调必须同步完成，回调或写入异常继续向调用方传播。`Console.IsOutputRedirected` 为 `true` 时忽略颜色设置，仅输出文本，不生成 ANSI 颜色码。

`Fast.NET.Core` 的启动横幅及 Runtime、OpenApi、SqlSugar 的直接诊断输出均使用此入口，适配 Windows CMD、PowerShell、Windows Terminal 和 Linux 终端。`Fast.IaaS` 3.5.27 继续面向 `netstandard2.1`，使用 `src/IaaS/Internals/ConsoleWriter.cs` 中的内部写入器，保持独立包兼容性，不引入 Runtime 依赖。

`Fast.NET.Core` 3.5.36 中，`builder.Initialize()` 在 ASCII Logo 前输出应用及宿主信息。应用名称和版本取自入口程序集，框架版本取自 Core 程序集；两者均同时显示去除 `+` 构建元数据后的信息版本与程序集版本，无法获取的名称或版本使用 `Unknown`。运行环境取自 `builder.Environment`，启动时间为输出横幅时的本地时间，格式为 `yyyy-MM-dd HH:mm:ss`。

| 输出内容 | 颜色 |
| --- | --- |
| 信息字段标签 | `DarkGray` |
| 应用名称、主机名称、操作系统、系统平台与架构、进程架构 | `Gray` |
| .NET 运行时描述、Fast.NET 框架版本 | `Cyan` |
| 应用版本、ASCII Logo | `Green` |
| 运行环境名称 | Production 为 `Green`，Development 为 `Yellow`，其他环境为 `Magenta` |
| 程序启动时间 | `White` |
| Gitee 地址 | `Red` |
| 使用提示、框架说明、学习寄语及 PR 邀请 | `Magenta` |

横幅同时调整缩进和空行，并增加合法使用提示及学习寄语。输出重定向时保留相同文本，不生成颜色码；输出结束后由写入器恢复原始颜色。

`Fast.Logging` 继续通过 `ConsoleFormatter` 接收的 `TextWriter` 写入，由 `ConsoleLoggerProvider` 负责终端适配、必要的 ANSI 解析和日志队列输出。格式化器不直接修改 `Console.ForegroundColor`；`LoggerColorBehavior.Default` 在输出重定向时停止生成颜色码，ANSI 支持由 Provider 处理，不仅根据重定向状态判断。

## 依赖注入注册约定

`AddDependencyInjection()` 会扫描实现 `ITransientDependency`、`IScopedDependency` 或 `ISingletonDependency` 的非抽象类。有业务接口时，服务按业务接口注册；仅实现生命周期标记接口时，服务按具体类型注册，可直接注入该实现类。

## 基础设施模块注意事项

- `Fast.Cache`、`Fast.Consul` 和 `Fast.SqlSugar` 需要真实外部服务。验证时应使用隔离环境，不要在仓库检查流程中直接修改生产数据。
- `Fast.EventBus` 是进程内事件总线，应用重启后不会恢复尚未处理的事件。需要跨进程、持久化或至少一次投递语义时，应选择专用消息中间件。
- 两个序列化包都使用 `Fast.Serialization` 命名空间和相近扩展名，同一应用应明确选择并配置其中一个。
- `Fast.Logging` 的文件日志依赖运行目录权限；容器中应将目录映射到可写卷或改用标准日志输出。

## 配置来源

选项类型实现了仓库约定的后配置流程。最终支持字段以各模块的 `*SettingsOptions` 类型和 XML API 文档为准。不要在文档、示例或 Issue 中提交真实令牌、连接字符串或密码。
