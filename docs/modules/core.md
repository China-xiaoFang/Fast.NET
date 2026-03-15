# Fast.NET.Core

Core framework module providing initialization, configuration auto-loading, middleware, and utility classes.

## Installation

```bash
dotnet add package Fast.NET.Core
```

## Initialization

The `Initialize()` extension method bootstraps the Fast.NET framework:

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Initialize();
```

### What Initialize() Does

1. Displays the Fast.NET startup banner
2. Stores environment references in `FastContext`
3. Auto-loads JSON configuration files from standard directories:
   - `AppConfig/`, `AppSettings/`, `Config/`, `Settings/`, `JsonConfig/`
   - Loads both `{filename}.json` and `{filename}.{Environment}.json`
4. Registers `IHttpContextAccessor`
5. Registers `IMemoryCache`
6. Configures `ForwardedHeaders` for reverse proxy support (IIS/Nginx)

## Middleware

### Enable Request Body Buffering

```csharp
app.EnableBuffering();
```

Enables reading the request body multiple times (e.g., for logging or validation).

### CORS

```csharp
// In service registration
builder.Services.AddCorsAccessor();

// Or with configuration
builder.Services.AddCorsAccessor(builder.Configuration);
```

Configurable via `CorsAccessorSettings` section in `appsettings.json`.

### Gzip Compression

```csharp
// Register
builder.Services.AddGzipCompression();

// Enable in pipeline
app.UseGzipCompression();
```

### SignalR Hub Mapping

```csharp
app.UseMapHub();
```

Automatically discovers and maps SignalR hubs decorated with mapping attributes.

## Utility Classes

### AssemblyUtil

```csharp
// Get all application assemblies
var assemblies = AssemblyUtil.GetAssemblies();
```

### JsonUtils

```csharp
// Serialize/deserialize
var json = JsonUtils.Serialize(obj);
var obj = JsonUtils.Deserialize<MyClass>(json);
```

### MachineUtil

```csharp
// Get machine information
var info = MachineUtil.GetMachineInfo();
```

### RemoteRequestUtil

```csharp
// Make HTTP requests
var response = await RemoteRequestUtil.GetAsync("https://api.example.com/data");
```

### ShellUtil

```csharp
// Execute shell commands
var output = ShellUtil.Execute("dotnet --version");
```

## Configuration

Fast.NET automatically scans standard directories for JSON configuration files. You can also specify custom paths:

```json
{
  "publicJsonPath": ["CustomConfig"]
}
```
