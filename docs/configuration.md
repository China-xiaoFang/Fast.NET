# Configuration Reference / 配置参考

Fast.NET modules are configured through `appsettings.json` or any JSON configuration file. Configuration files are auto-loaded from standard directories.

## Auto-Loaded Configuration Directories / 自动加载配置目录

Fast.NET scans the following directories for `*.json` files during initialization:

- `AppConfig/`
- `AppSettings/`
- `Config/`
- `Settings/`
- `JsonConfig/`

Files are loaded as `{filename}.json` and `{filename}.{Environment}.json` (e.g., `database.Development.json`).

---

## Logging / 日志配置

**Section:** `Logging:Fast`

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
| `FileFormat` | `string` | `"/{0:yyyy}/{0:MM}/{0:dd}/{0:HH}"` | Date-based log file path format |
| `FileSizeLimit` | `long` | `10485760` (10 MB) | Maximum log file size before rotation |
| `MiniLogLevel` | `LogLevel` | `Information` | Minimum log level to write |
| `EnableCritical` | `bool` | `false` | Enable critical level log files |

---

## JWT Bearer / JWT 认证配置

**Section:** `JWTSettings`

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

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `ValidateIssuerSigningKey` | `bool` | `false` | Validate the signing key |
| `IssuerSigningKey` | `string` | (built-in) | Secret key for signing tokens |
| `ValidateIssuer` | `bool` | `false` | Validate the token issuer |
| `ValidIssuer` | `string` | `"Fast.NET.API"` | Expected token issuer |
| `ValidateAudience` | `bool` | `false` | Validate the token audience |
| `ValidAudience` | `string` | `"Fast.NET.Client"` | Expected token audience |
| `ValidateLifetime` | `bool` | `false` | Validate token expiration |
| `ClockSkew` | `int` | `5` | Clock skew tolerance in seconds |
| `TokenExpiredTime` | `int` | `20` | Access token lifetime in minutes |
| `RefreshTokenExpireTime` | `int` | `1440` | Refresh token lifetime in minutes (24 hours) |
| `Algorithm` | `string` | `"HS256"` | Signing algorithm (HS256, HS384, HS512, RS256, ES256) |
| `Enable` | `bool` | `true` | Enable JWT authentication |

---

## Redis Cache / Redis 缓存配置

**Section:** `RedisSettings`

```json
{
  "RedisSettings": [
    {
      "ServiceIp": "127.0.0.1",
      "Port": 6379,
      "DbName": 0,
      "DbPwd": "",
      "Prefix": "",
      "Poolsize": 100,
      "SSL": false
    }
  ]
}
```

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `ServiceIp` | `string` | `"127.0.0.1"` | Redis server IP address |
| `Port` | `int` | `6379` | Redis server port |
| `DbName` | `int` | `0` | Redis database index |
| `DbPwd` | `string` | `""` | Redis password |
| `Prefix` | `string` | `""` | Key prefix for cache entries |
| `Poolsize` | `int` | `100` | Connection pool size |
| `SSL` | `bool` | `false` | Enable SSL/TLS connection |

---

## Swagger / Swagger 文档配置

**Section:** `SwaggerSettings`

```json
{
  "SwaggerSettings": {
    "Enable": true,
    "DocumentTitle": "API Documentation",
    "DefaultGroupName": "Default",
    "EnableAuthorized": true,
    "FormatAsV2": false,
    "RoutePrefix": "swagger",
    "DocExpansionState": "List"
  }
}
```

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `Enable` | `bool` | `true` | Enable Swagger documentation |
| `DocumentTitle` | `string` | `"API Documentation"` | Document page title |
| `DefaultGroupName` | `string` | `"Default"` | Default API group name |
| `EnableAuthorized` | `bool` | `true` | Show authorization button |
| `FormatAsV2` | `bool` | `false` | Use Swagger 2.0 format |
| `RoutePrefix` | `string` | `"swagger"` | URL route prefix |
| `DocExpansionState` | `string` | `"List"` | Default expansion state (None, List, Full) |

---

## Consul / Consul 服务发现配置

**Section:** `ConsulSettings`

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
| `Address` | `string` | `"http://127.0.0.1:8500"` | Consul agent address |
| `HealthCheck` | `string` | `"/healthCheck"` | Health check endpoint path |
| `DeregisterCriticalServiceAfter` | `int` | `5` | Deregister after N seconds of critical state |
| `HealthCheckInterval` | `int` | `10` | Health check interval in seconds |
| `HealthCheckTimeout` | `int` | `5` | Health check timeout in seconds |

---

## OpenApi / OpenApi 配置

**Section:** `OpenApiSettings`

```json
{
  "OpenApiSettings": {
    "FolderGroup": true,
    "IgnoreSchemas": [],
    "PagedSchemaProperties": ["pageIndex", "pageSize", "searchValue"]
  }
}
```

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `FolderGroup` | `bool` | `true` | Group APIs by folder structure |
| `IgnoreSchemas` | `string[]` | `[]` | Schema names to ignore |
| `PagedSchemaProperties` | `string[]` | (built-in) | Properties that indicate paged requests |
| `ImportSchemaMappings` | `object[]` | `[]` | Custom schema import path mappings |
| `ImportTypeMappings` | `object[]` | (built-in) | Type name mappings for code generation |
| `BaseTypeMappings` | `Dictionary` | (built-in) | Base type to TypeScript type mappings |

---

## CORS / 跨域配置

CORS is configured via `AddCorsAccessor()` and supports the following configuration:

**Section:** `CorsAccessorSettings`

```json
{
  "CorsAccessorSettings": {
    "PolicyName": "FastCors",
    "WithOrigins": ["*"],
    "WithHeaders": ["*"],
    "WithMethods": ["*"],
    "AllowCredentials": false,
    "WithExposedHeaders": ["*"]
  }
}
```

---

## Snowflake ID / 雪花 ID 配置

**Section:** `SnowflakeSettings`

```json
{
  "SnowflakeSettings": {
    "WorkerId": 1
  }
}
```

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `WorkerId` | `int` | Auto-assigned | Worker ID for distributed ID generation |
