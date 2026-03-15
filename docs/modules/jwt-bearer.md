# Fast.JwtBearer

JWT Bearer authentication and authorization with configurable handlers and permission-based access control.

## Installation

```bash
dotnet add package Fast.JwtBearer
```

## Registration

```csharp
// Full JWT authentication + authorization
builder.Services.AddJwtBearer(builder.Configuration);

// Or settings only (without auth middleware)
builder.Services.AddJwtBearerSetting(builder.Configuration);

// Or authentication only (without custom authorization)
builder.Services.AddJwtBearerAuthentication(builder.Configuration);
```

## Configuration

```json
{
  "JWTSettings": {
    "ValidateIssuerSigningKey": true,
    "IssuerSigningKey": "your-secret-key-at-least-32-characters",
    "ValidateIssuer": true,
    "ValidIssuer": "Fast.NET.API",
    "ValidateAudience": true,
    "ValidAudience": "Fast.NET.Client",
    "ValidateLifetime": true,
    "ClockSkew": 5,
    "TokenExpiredTime": 20,
    "RefreshTokenExpireTime": 1440,
    "Algorithm": "HS256",
    "Enable": true
  }
}
```

## Custom Authorization Handler

Implement `IJwtBearerHandle` to customize authorization behavior:

```csharp
public class MyJwtBearerHandler : IJwtBearerHandle, IScopedDependency
{
    public Task<bool> AuthorizeHandle(
        AuthorizationHandlerContext context, HttpContext httpContext)
    {
        // Custom authorization logic
        return Task.FromResult(true);
    }

    public Task<object> AuthorizeFailHandle(
        AuthorizationHandlerContext context, HttpContext httpContext, Exception ex)
    {
        // Return custom error response
        return Task.FromResult<object>(new { Code = 401, Message = "Unauthorized" });
    }

    public Task<bool> PermissionHandle(
        AuthorizationHandlerContext context,
        IAuthorizationRequirement requirement,
        HttpContext httpContext)
    {
        // Custom permission check
        return Task.FromResult(true);
    }

    public Task<object> PermissionFailHandle(
        AuthorizationHandlerContext context,
        IAuthorizationRequirement requirement,
        HttpContext httpContext,
        Exception ex)
    {
        return Task.FromResult<object>(new { Code = 403, Message = "Forbidden" });
    }
}
```

## Attributes

### [Permission]

```csharp
[Permission("admin", "user:read")]
public Task<UserDto> GetUserAsync(long id) { /* ... */ }
```

### [AllowForbidden]

```csharp
[AllowForbidden]
public Task<UserDto> GetPublicDataAsync() { /* ... */ }
```

## Supported Algorithms

- `HS256`, `HS384`, `HS512` (HMAC)
- `RS256` (RSA)
- `ES256` (ECDSA)
