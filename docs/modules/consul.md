# Fast.Consul

Consul service discovery, health checks, and Key/Value configuration store.

## Installation

```bash
dotnet add package Fast.Consul
```

## Registration

Consul is automatically registered via the host injection system. No manual registration is needed — just configure it in `appsettings.json`.

## Configuration

```json
{
  "ConsulSettings": {
    "Enable": true,
    "Address": "http://127.0.0.1:8500",
    "HealthCheck": "/healthCheck",
    "DeregisterCriticalServiceAfter": 5,
    "HealthCheckInterval": 10,
    "HealthCheckTimeout": 5
  }
}
```

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `Enable` | `bool` | `true` | Enable Consul integration |
| `Address` | `string` | `"http://127.0.0.1:8500"` | Consul agent URL |
| `HealthCheck` | `string` | `"/healthCheck"` | Health check endpoint |
| `DeregisterCriticalServiceAfter` | `int` | `5` | Deregister delay (seconds) |
| `HealthCheckInterval` | `int` | `10` | Check interval (seconds) |
| `HealthCheckTimeout` | `int` | `5` | Check timeout (seconds) |

## Features

### Service Registration

Services are automatically registered with Consul at startup and deregistered on shutdown.

### Health Checks

Consul periodically calls the configured health check endpoint to verify service availability.

### Key/Value Store

Access Consul's KV store for distributed configuration:

```csharp
public class ConfigService
{
    private readonly IKeyValueService _kv;

    public ConfigService(IKeyValueService kv) => _kv = kv;

    // Read configuration
    public async Task<T> GetConfigAsync<T>(string key)
    {
        return await _kv.GetKeyValue<T>(key, "dc1");
    }

    // Read string value
    public async Task<string> GetValueAsync(string key)
    {
        return await _kv.GetKeyValue(key, "dc1");
    }

    // Update configuration
    public async Task<bool> SetConfigAsync(string key, string value)
    {
        return await _kv.EditKeyValue(key, "dc1", value);
    }
}
```
