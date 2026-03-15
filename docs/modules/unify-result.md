# Fast.UnifyResult

Standardized RESTful response format with global exception handling and model validation.

## Installation

```bash
dotnet add package Fast.UnifyResult
```

## Registration

```csharp
builder.Services.AddUnifyResult(builder.Configuration);
```

This registers:
- `DataValidationFilter` — Automatic model validation
- `FriendlyExceptionFilter` — Global exception handling
- `SucceededUnifyResultFilter` — Success response wrapping
- Status code middleware for 404, 500, etc.

## Response Format

### Success Response

```json
{
  "code": 200,
  "success": true,
  "data": {
    "id": 1,
    "name": "Test User"
  },
  "message": null,
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Error Response

```json
{
  "code": 400,
  "success": false,
  "data": null,
  "message": "Validation failed: Name is required",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

### Exception Response (Development)

```json
{
  "code": 500,
  "success": false,
  "data": {
    "type": "System.InvalidOperationException",
    "message": "Something went wrong",
    "stackTrace": "..."
  },
  "message": "Internal server error",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## Attributes

### [NonUnify]

Skip unified response wrapping for specific actions:

```csharp
[NonUnify]
public IActionResult DownloadFile()
{
    return File(bytes, "application/pdf", "report.pdf");
}
```

### [NonValidation]

Skip model validation for specific actions:

```csharp
[NonValidation]
public Task ImportDataAsync(RawInput input) { /* ... */ }
```

## Custom Providers

### IUnifyResultProvider

Customize the result format:

```csharp
public class MyResultProvider : IUnifyResultProvider
{
    // Custom implementation
}
```

### IGlobalExceptionHandler

Custom global exception handling:

```csharp
public class MyExceptionHandler : IGlobalExceptionHandler
{
    public Task HandleAsync(ExceptionContext context)
    {
        // Custom exception handling
        return Task.CompletedTask;
    }
}
```
