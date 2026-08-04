[**简体中文**](GETTING_STARTED.zh.md) | [English](GETTING_STARTED.md) · [返回 README](../README.zh.md)

# Fast.NET 入门

本文提供从选择模块、注册服务到验证应用的最短路径。完整模块边界和配置入口见[模块手册](MODULES.zh.md)。

## 前置条件

- 使用 Fast.NET 的应用面向 .NET 8、.NET 9 或 .NET 10。
- 构建本仓库需要 [`global.json`](../global.json) 指定的 .NET 10 SDK。
- Redis、Consul、数据库等外部服务只在安装对应模块时需要；基础 SDK 接入无需外部服务。

## 创建应用

```bash
dotnet new webapi -n MyFastApp -f net10.0
cd MyFastApp
dotnet add package Fast.NET.Core
dotnet add package Fast.Serialization.System.Text.Json
dotnet add package Fast.Swagger
```

`Fast.Serialization.System.Text.Json` 与 `Fast.Serialization.Newtonsoft.Json` 通常二选一。不要为了使用某个单独能力安装所有 Fast.NET 包。

## 最小配置

```csharp
using Fast.NET.Core;
using Fast.Serialization;
using Fast.Swagger;

var builder = WebApplication.CreateBuilder(args);

builder.Initialize();
builder.Services.AddSerialization();
builder.Services.AddControllers();
builder.Services.AddSwaggerDocuments(builder.Configuration);

var app = builder.Build();

app.UseSwaggerDocuments();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new {status = "ok"}));

app.Run();
```

`appsettings.json`：

```json
{
  "SwaggerSettings": {
    "Enable": true,
    "DocumentTitle": "My Fast.NET API",
    "EnableAuthorized": false
  }
}
```

如果使用 `Fast.DynamicApplication`，必须先调用 `AddControllers()` 或 `AddControllersWithViews()`，再调用 `AddDynamicApplication()`。

## 从源码构建

```bash
dotnet restore Fast.NET.sln
dotnet build Fast.NET.sln -c Release --no-restore
dotnet pack Fast.NET.sln -c Release --no-build --no-restore -p:WarnOnPackingNonPackableProject=false
```

Windows 用户可以通过仓库的单文件入口依次还原、构建并打包：

```bat
UploadNuget.bat pack
```

批处理文件始终先构建再打包，`pack` 模式不会发布。生成的包位于 `nupkgs/`；可选的全量或单包发布流程见[发布指南](RELEASING.zh.md)。

## 下一步

- [模块与配置入口](MODULES.zh.md)
- [架构说明](ARCHITECTURE.zh.md)
- [贡献指南](../CONTRIBUTING.zh.md)
- [支持渠道](../SUPPORT.md)
