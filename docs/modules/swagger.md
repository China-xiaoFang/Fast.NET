# Fast.Swagger

Enhanced Swagger documentation built on [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore).

## Installation

```bash
dotnet add package Fast.Swagger
```

## Registration

```csharp
// Register services
builder.Services.AddSwaggerDocuments(builder.Configuration);

// Enable middleware
app.UseSwaggerDocuments();
```

## Configuration

```json
{
  "SwaggerSettings": {
    "Enable": true,
    "DocumentTitle": "My API Documentation",
    "DefaultGroupName": "Default",
    "EnableAuthorized": true,
    "FormatAsV2": false,
    "RoutePrefix": "swagger",
    "DocExpansionState": "List"
  }
}
```

## Features

- **JWT Authorization Button** — Built-in Bearer token input
- **API Grouping** — Organize endpoints by module
- **Enum Documentation** — Display enum values and descriptions
- **Custom Schema Filtering** — Handle complex types
- **Tag Ordering** — Custom ordering of API tags

## Attributes

### [OperationId]

```csharp
[OperationId("GetUserById")]
public Task<UserDto> GetUserAsync(long id) { /* ... */ }
```

### [SchemaId]

```csharp
[SchemaId("UserModel")]
public class UserDto { /* ... */ }
```

## Access

Navigate to `https://your-app/swagger/index.html` after starting the application.
