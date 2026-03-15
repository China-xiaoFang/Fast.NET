# Fast.DynamicApplication

Automatically generate RESTful API endpoints from service classes — no controllers needed.

## Installation

```bash
dotnet add package Fast.DynamicApplication
```

## Registration

```csharp
builder.Services.AddControllers();
builder.Services.AddDynamicApplication();
```

## Basic Usage

Implement `IDynamicApplication` on your service class:

```csharp
using Fast.DynamicApplication;

public class UserService : IDynamicApplication
{
    public Task<UserDto> GetUserAsync(long id)
    {
        return Task.FromResult(new UserDto { Id = id, Name = "Test User" });
    }

    public Task<List<UserDto>> GetListAsync()
    {
        return Task.FromResult(new List<UserDto>());
    }

    public Task PostCreateAsync(CreateUserInput input)
    {
        // Create user...
        return Task.CompletedTask;
    }

    public Task PutUpdateAsync(long id, UpdateUserInput input)
    {
        // Update user...
        return Task.CompletedTask;
    }

    public Task DeleteRemoveAsync(long id)
    {
        // Delete user...
        return Task.CompletedTask;
    }
}
```

This automatically generates:
- `GET /api/user/user/{id}` → `GetUserAsync`
- `GET /api/user/list` → `GetListAsync`
- `POST /api/user/create` → `PostCreateAsync`
- `PUT /api/user/update/{id}` → `PutUpdateAsync`
- `DELETE /api/user/remove/{id}` → `DeleteRemoveAsync`

## HTTP Verb Convention

The method name prefix determines the HTTP verb:

| Prefix | HTTP Verb |
| --- | --- |
| `Get*` | GET |
| `Post*` | POST |
| `Put*` | PUT |
| `Delete*` | DELETE |
| `Patch*` | PATCH |

## ApiDescriptionSettings Attribute

Use `[ApiDescriptionSettings]` to customize API behavior:

```csharp
[ApiDescriptionSettings(Name = "UserManagement", Module = "Admin")]
public class UserService : IDynamicApplication
{
    [ApiDescriptionSettings(Name = "GetById", Groups = new[] { "v1", "v2" })]
    public Task<UserDto> GetUserAsync(long id) { /* ... */ }

    [ApiDescriptionSettings(IgnoreApi = true)]
    public void InternalMethod() { /* hidden from API */ }
}
```

### Properties

| Property | Type | Description |
| --- | --- | --- |
| `Name` | `string` | Custom API name |
| `Module` | `string` | Module/group name |
| `Version` | `string` | API version |
| `Groups` | `string[]` | Multiple group support (uses `##` separator) |
| `Order` | `int` | Display/execution order |
| `Description` | `string` | HTML-formatted description |
| `IgnoreApi` | `bool` | Hide from API documentation |

## Route Prefix

Customize the route prefix:

```csharp
builder.Services.AddDynamicApplication("api/v1");
```

## With Dependency Injection

Combine with `Fast.DependencyInjection`:

```csharp
public class OrderService : IDynamicApplication, ITransientDependency
{
    private readonly IOrderRepository _repo;

    public OrderService(IOrderRepository repo)
    {
        _repo = repo;
    }

    public Task<OrderDto> GetOrderAsync(long id)
    {
        return _repo.GetByIdAsync(id);
    }
}
```
