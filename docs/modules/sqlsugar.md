# Fast.SqlSugar

ORM integration with repository pattern, multi-tenancy, and soft delete support, built on [SqlSugar](https://github.com/DotNetNext/SqlSugar).

## Installation

```bash
dotnet add package Fast.SqlSugar
```

## Registration

```csharp
builder.Services.AddSqlSugar(builder.Configuration);
```

## Configuration

```json
{
  "ConnectionSettings": [
    {
      "ConfigId": "default",
      "DbType": "MySql",
      "ConnectionString": "server=localhost;Database=mydb;Uid=root;Pwd=password;",
      "IsAutoCloseConnection": true,
      "SlaveConnectionConfigs": []
    }
  ]
}
```

## Repository Pattern

### Inject and Use

```csharp
public class UserService : IDynamicApplication
{
    private readonly ISqlSugarRepository<User> _userRepo;

    public UserService(ISqlSugarRepository<User> userRepo)
    {
        _userRepo = userRepo;
    }

    // Query
    public Task<User> GetUserAsync(long id)
        => _userRepo.SingleOrDefaultAsync(id);

    public Task<List<User>> GetListAsync()
        => _userRepo.ToListAsync();

    public Task<bool> ExistsAsync(long id)
        => _userRepo.AnyAsync(u => u.Id == id);

    // Insert
    public Task<User> CreateAsync(User user)
        => _userRepo.InsertReturnEntityAsync(user);

    // Update
    public Task<bool> UpdateAsync(User user)
        => _userRepo.UpdateAsync(user);

    // Delete
    public Task<bool> DeleteAsync(long id)
        => _userRepo.DeleteAsync(id);
}
```

### Available Operations

**Query:** `Count`, `Any`, `SingleOrDefault`, `FirstOrDefault`, `ToList`, `Entities` (IQueryable)

**Insert:** `Insert`, `InsertReturnIdentity`, `InsertReturnEntity`, `ExecuteReturnBigIdentity`

**Update:** `Update`, `UpdateNoPrimaryKey`

**Delete:** `Delete` (by ID, entity, or expression)

## Base Entities

### IBaseEntity

Provides audit fields:

```csharp
public class User : BaseEntity
{
    public string Name { get; set; }
    public string Email { get; set; }
    // Inherits: Id, CreatedTime, CreatedUserId, UpdatedTime, UpdatedUserId, etc.
}
```

### Multi-Tenancy

```csharp
public class TenantUser : BaseTEntity
{
    public string Name { get; set; }
    // Inherits: TenantId + all BaseEntity fields
    // Automatic tenant filtering applied
}
```

### Soft Delete

```csharp
public class Article : BaseEntity, IDeletedEntity
{
    public string Title { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedTime { get; set; }
    public long? DeletedUserId { get; set; }
}
```

### Optimistic Concurrency

```csharp
public class Order : BaseEntity, IUpdateVersion
{
    public decimal Amount { get; set; }
    public string RowVersion { get; set; } // Automatic conflict detection
}
```

## Snowflake ID

```csharp
builder.Services.AddSnowflake(builder.Configuration);
```

```json
{
  "SnowflakeSettings": {
    "WorkerId": 1
  }
}
```

## Switching Tenant/Database Context

```csharp
var otherRepo = _userRepo.Change<OtherTenantEntity>();
```
