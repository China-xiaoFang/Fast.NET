# Changelog

All notable changes to Fast.NET are documented in this file. The repository follows independent package versioning; a release entry must name every affected package.

## Unreleased

### Breaking changes

- Primary SDK packages now target .NET 8, .NET 9, and .NET 10. .NET 6 and .NET 7 assets are no longer produced.
- Repository builds use C# 14 and the .NET 10 SDK.
- `RetryUtil.InvokeAsync` now awaits retry callbacks, invokes the fallback for suppressed terminal failures, and accepts a cancellation token.

### Added

- Central NuGet dependency management through `Directory.Packages.props`.
- GitHub CI for restore, dependency auditing, build, packaging, and artifact upload.
- A single Windows entry point for build, pack, optional all-package publishing, and optional single-package publishing.
- Bilingual getting-started, module, and release guides plus security and community policies.

### Changed

- Updated `Mapster` and `Mapster.DependencyInjection` to 10.0.12, `MiniExcel` to 1.45.0, `SQLitePCLRaw.bundle_e_sqlite3` to 3.0.5, `SqlSugarCore` to 5.1.4.217, and `Swashbuckle.AspNetCore` to 10.2.3.
- Updated ASP.NET Core package references to 8.0.30, 9.0.19, and 10.0.11 for their corresponding target frameworks.
- Migrated `Fast.Swagger` custom filters and security registration to the Microsoft.OpenApi 2.x API required by Swashbuckle 10.
- Established `FAST-AES-256-GCM-V1` as the initial cross-language password-based AES payload for `Fast.IaaS`.
- Simplified XML documentation by removing type references already conveyed by signatures, inheriting existing implementation contracts, and retaining links to related APIs and constraints.
- Marked internal leaf implementations and JSON converters as sealed where inheritance is not supported.
- Incremented package versions: `Fast.Cache` 3.5.31, `Fast.Consul` 3.5.7, `Fast.NET.Core` 3.5.33, `Fast.DependencyInjection` 3.5.27, `Fast.DynamicApplication` 3.5.33, `Fast.EventBus` 3.5.26, `Fast.IaaS` 3.5.24, `Fast.JwtBearer` 3.5.38, `Fast.Logging` 3.5.29, `Fast.Mapster` 3.5.25, `Fast.OpenApi` 3.5.32, `Fast.Runtime` 3.5.28, `Fast.Serialization.Newtonsoft.Json` 3.5.25, `Fast.Serialization.System.Text.Json` 3.5.20, `Fast.SqlSugar` 3.5.63, `Fast.Swagger` 3.5.32, and `Fast.UnifyResult` 3.5.31.
- Incremented `I18nTranslateTool` to 1.0.1 and moved its `MiniExcel` version into central package management.

### Fixed

- `Fast.OpenApi` TypeScript clients now emit separate type imports in rule-compliant order, explicit `Promise<T>` return types, and `unknown` for unspecified schemas to satisfy the current Fast ESLint Config rules.
- `Fast.DependencyInjection` now registers marked concrete services that do not expose a business interface.
- `Fast.JwtBearer` now accepts the standard `access_token` query parameter regardless of a SignalR hub's route prefix, preserves custom token extraction, and resolves the HTTP context during endpoint and hub-method authorization.
- WebSocket detection now relies on ASP.NET Core's WebSocket feature instead of treating every request to `/ws` as a WebSocket request.
- Dynamic application discovery no longer inserts duplicate MVC application parts.
- Runtime dependency discovery now works under test and plugin hosts whose entry assembly has no adjacent `.deps.json` file.
- `Fast.Runtime` disposable cleanup now drains the concurrent collection without forcing a full garbage collection or discarding objects added during cleanup.
- `Fast.Runtime` remote IPv4 metadata lookups now use a bounded set of lock stripes, avoiding keyed-lock removal races and unbounded lock retention.
- `Fast.IaaS`, `Fast.DynamicApplication`, and `Fast.UnifyResult` async method inspection now recognizes `ValueTask`, `ValueTask<T>`, and compiler-generated async state machines.
- `Fast.IaaS`, `Fast.DynamicApplication`, `Fast.Swagger`, and `Fast.Logging` identifier casing and rolling-file numbering are now stable across server cultures.
- `Fast.Cache` named Redis contexts now expose the effective inherited key prefix used by the client.
- `Fast.EventBus` retries now stop immediately when host or event cancellation is requested.
- `Fast.Logging` now reports queue saturation and write failures, and safely handles file-name rule changes during rolling.
- Build and pack are separate operations; normal builds no longer emit NuGet packages implicitly.
- NuGet repository metadata now uses the canonical Gitee URL and the standard `git` repository type.

For changes before this changelog was introduced, see the [repository history](https://gitee.com/FastDotnet/Fast.NET/commits/master).
