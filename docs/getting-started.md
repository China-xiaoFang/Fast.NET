# Getting Started / 快速开始

This guide will help you get up and running with Fast.NET in minutes.

本指南将帮助您在几分钟内开始使用 Fast.NET。

## Prerequisites / 环境要求

- [.NET SDK](https://dotnet.microsoft.com/download) 6.0 or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/) / [VS Code](https://code.visualstudio.com/) / [JetBrains Rider](https://www.jetbrains.com/rider/)

## Create a New Project / 创建新项目

```bash
# Create a new ASP.NET Core Web API project
dotnet new webapi -n MyApp
cd MyApp
```

## Install Packages / 安装包

Install the modules you need:

```bash
# Core framework (required)
dotnet add package Fast.NET.Core

# Pick the modules you need:
dotnet add package Fast.DependencyInjection
dotnet add package Fast.DynamicApplication
dotnet add package Fast.Serialization.System.Text.Json
dotnet add package Fast.Logging
dotnet add package Fast.UnifyResult
dotnet add package Fast.Swagger
```

## Minimal Setup / 最小配置

Replace the contents of `Program.cs` with:

```csharp
using Fast.DependencyInjection;
using Fast.DynamicApplication;
using Fast.Logging;
using Fast.NET.Core;
using Fast.Serialization;
using Fast.Swagger;
using Fast.UnifyResult;

var builder = WebApplication.CreateBuilder(args);

// Initialize Fast.NET framework
builder.Initialize();

// Add services
builder.Services.AddSerialization();
builder.Services.AddLoggingService(builder.Environment);
builder.Services.AddCorsAccessor();
builder.Services.AddDependencyInjection();
builder.Services.AddControllers();
builder.Services.AddDynamicApplication();
builder.Services.AddUnifyResult(builder.Configuration);
builder.Services.AddSwaggerDocuments(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.EnableBuffering();
app.UseRouting();
app.UseSwaggerDocuments();
app.MapControllers();
app.Run();
```

## Create Your First Service / 创建第一个服务

Create a file `Services/HelloService.cs`:

```csharp
using Fast.DynamicApplication;

namespace MyApp.Services;

/// <summary>
/// This service automatically becomes a REST API controller.
/// GET /api/hello/say-hello → returns "Hello from Fast.NET!"
/// GET /api/hello/greet?name=World → returns "Hello, World!"
/// </summary>
public class HelloService : IDynamicApplication
{
    public string GetSayHello()
    {
        return "Hello from Fast.NET!";
    }

    public string GetGreet(string name)
    {
        return $"Hello, {name}!";
    }
}
```

## Run the Application / 运行应用

```bash
dotnet run
```

Open your browser and navigate to:
- `https://localhost:5001/swagger` — Swagger UI
- `https://localhost:5001/api/hello/say-hello` — Your first API endpoint

## Add Dependency Injection / 添加依赖注入

Create an interface and implementation:

```csharp
using Fast.DependencyInjection;

public interface IGreetingService
{
    string Greet(string name);
}

// Automatically registered as Transient
public class GreetingService : IGreetingService, ITransientDependency
{
    public string Greet(string name) => $"Hello, {name}! Welcome to Fast.NET.";
}
```

Use it in your Dynamic API service:

```csharp
using Fast.DynamicApplication;

public class GreetingApiService : IDynamicApplication
{
    private readonly IGreetingService _greetingService;

    public GreetingApiService(IGreetingService greetingService)
    {
        _greetingService = greetingService;
    }

    public string GetGreet(string name)
    {
        return _greetingService.Greet(name);
    }
}
```

## Add Configuration / 添加配置

Fast.NET automatically scans for JSON configuration files in standard directories. Create an `appsettings.json`:

```json
{
  "Logging": {
    "Fast": {
      "FileFormat": "/{0:yyyy}/{0:MM}/{0:dd}/{0:HH}",
      "FileSizeLimit": 10485760,
      "MiniLogLevel": "Information"
    }
  },
  "SwaggerSettings": {
    "Enable": true,
    "DocumentTitle": "My App API Documentation"
  }
}
```

## Next Steps / 下一步

- [Architecture Overview](architecture.md) — Understand how Fast.NET is structured
- [Module Guide](modules/) — Explore each module in detail
- [Configuration Reference](configuration.md) — All configuration options
