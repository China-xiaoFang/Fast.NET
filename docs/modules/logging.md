# Fast.Logging

File and console logging with automatic rotation, per-level separation, and trace correlation.

## Installation

```bash
dotnet add package Fast.Logging
```

## Registration

```csharp
builder.Services.AddLoggingService(builder.Environment);
```

## Configuration

```json
{
  "Logging": {
    "Fast": {
      "FileFormat": "/{0:yyyy}/{0:MM}/{0:dd}/{0:HH}",
      "FileSizeLimit": 10485760,
      "MiniLogLevel": "Information",
      "EnableCritical": false
    }
  }
}
```

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `FileFormat` | `string` | `"/{0:yyyy}/{0:MM}/{0:dd}/{0:HH}"` | Date-based file path format |
| `FileSizeLimit` | `long` | `10485760` (10 MB) | Max file size before rotation |
| `MiniLogLevel` | `LogLevel` | `Information` | Minimum log level |
| `EnableCritical` | `bool` | `false` | Enable critical level logging |

## Features

### Per-Level File Separation

Logs are automatically separated by level:

```
logs/
├── 2024/01/15/10/
│   ├── trace.log
│   ├── debug.log
│   ├── information.log
│   ├── warning.log
│   ├── error.log
│   └── critical.log
```

### Automatic File Rotation

When a log file exceeds `FileSizeLimit`, a new file is created with a `_alt` suffix.

### Trace Correlation

Each log entry includes the `TraceId` from the HTTP request for distributed tracing.

### Stack Frame Tracking

Error and exception logs include method name and line number information.

### Console Logging

Enhanced console output with formatting, including colorized log levels.

## Usage

Use standard `ILogger` interface:

```csharp
public class MyService
{
    private readonly ILogger<MyService> _logger;

    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }

    public void DoWork()
    {
        _logger.LogInformation("Starting work...");
        _logger.LogWarning("Something might be wrong");
        _logger.LogError("An error occurred");
    }
}
```

## Log Output Format

Each log entry includes:
- Timestamp
- Log level
- Logger name (category)
- Message
- TraceId (if in HTTP context)
- Exception details (if applicable)
- Stack frame info (for errors)
