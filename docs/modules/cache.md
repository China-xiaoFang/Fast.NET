# Fast.Cache

Redis cache abstraction built on [CSRedisCore](https://github.com/2881099/csredis).

## Installation

```bash
dotnet add package Fast.Cache
```

## Registration

```csharp
// From configuration
builder.Services.AddCache();

// Or with custom options
builder.Services.AddCache(options =>
{
    options.Add(new RedisSettingsOptions
    {
        ServiceIp = "127.0.0.1",
        Port = 6379,
        DbName = 0
    });
});
```

## Configuration

```json
{
  "RedisSettings": [
    {
      "ServiceIp": "127.0.0.1",
      "Port": 6379,
      "DbName": 0,
      "DbPwd": "",
      "Prefix": "myapp:",
      "Poolsize": 100,
      "SSL": false
    }
  ]
}
```

## Usage

Inject `ICache` and use:

```csharp
public class UserService
{
    private readonly ICache _cache;

    public UserService(ICache cache)
    {
        _cache = cache;
    }

    // Get value
    public async Task<UserDto> GetUserAsync(long id)
    {
        return await _cache.GetAsync<UserDto>($"user:{id}");
    }

    // Set value with expiration
    public async Task SetUserAsync(long id, UserDto user)
    {
        await _cache.SetAsync($"user:{id}", user, 3600); // 1 hour
    }

    // Get or compute and cache
    public async Task<UserDto> GetOrSetUserAsync(long id)
    {
        return await _cache.GetAndSetAsync($"user:{id}",
            async () => await LoadUserFromDb(id),
            3600);
    }

    // Check existence
    public async Task<bool> ExistsAsync(long id)
    {
        return await _cache.ExistsAsync($"user:{id}");
    }

    // Delete
    public async Task DeleteAsync(long id)
    {
        await _cache.DelAsync($"user:{id}");
    }

    // Delete by pattern
    public async Task ClearUserCacheAsync()
    {
        await _cache.DelByPatternAsync("user:*");
    }
}
```

## API Reference

| Method | Description |
| --- | --- |
| `Get<T>(key)` / `GetAsync<T>(key)` | Get cached value |
| `Set(key, value, seconds?)` / `SetAsync(...)` | Set value with optional expiration |
| `Del(keys)` / `DelAsync(keys)` | Delete by key(s) |
| `DelByPattern(pattern)` | Delete by key pattern |
| `Exists(key)` / `ExistsAsync(key)` | Check if key exists |
| `GetAllKeys()` / `GetAllKeysAsync()` | Get all cache keys |
| `GetAndSet<T>(key, func, seconds?)` | Get or compute and cache |

## Properties

| Property | Type | Description |
| --- | --- | --- |
| `Prefix` | `string` | Cache key prefix |
| `Client` | `CSRedisClient` | Underlying CSRedis client for advanced operations |
