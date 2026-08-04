[简体中文](GETTING_STARTED.zh.md) | [**English**](GETTING_STARTED.md) · [Back to README](../README.md)

# Getting started with Fast.NET

This guide covers the shortest path from selecting modules to running an application. See the [module guide](MODULES.md) for package boundaries and configuration entry points.

## Prerequisites

- Consumer applications target .NET 8, .NET 9, or .NET 10.
- Building this repository requires the .NET 10 SDK selected by [`global.json`](../global.json).
- Redis, Consul, and databases are required only by their corresponding modules. Basic SDK setup has no external service dependency.

## Create an application

```bash
dotnet new webapi -n MyFastApp -f net10.0
cd MyFastApp
dotnet add package Fast.NET.Core
dotnet add package Fast.Serialization.System.Text.Json
dotnet add package Fast.Swagger
```

Most applications choose either `Fast.Serialization.System.Text.Json` or `Fast.Serialization.Newtonsoft.Json`. Do not install every Fast.NET package for a single capability.

## Minimal setup

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

`appsettings.json`:

```json
{
  "SwaggerSettings": {
    "Enable": true,
    "DocumentTitle": "My Fast.NET API",
    "EnableAuthorized": false
  }
}
```

When using `Fast.DynamicApplication`, call `AddControllers()` or `AddControllersWithViews()` before `AddDynamicApplication()`.

## Build from source

```bash
dotnet restore Fast.NET.sln
dotnet build Fast.NET.sln -c Release --no-restore
dotnet pack Fast.NET.sln -c Release --no-build --no-restore -p:WarnOnPackingNonPackableProject=false
```

On Windows, restore, build, and pack through the single repository entry point:

```bat
UploadNuget.bat pack
```

The batch file always builds before packing and doesn't publish in `pack` mode. Packages are written to `nupkgs/`; see the [release guide](RELEASING.md) for optional all-package or single-package publishing.

## Next steps

- [Modules and configuration](MODULES.md)
- [Architecture](ARCHITECTURE.md)
- [Contributing](../CONTRIBUTING.md)
- [Support](../SUPPORT.md)
