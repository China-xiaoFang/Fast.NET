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

- Updated `MiniExcel` to 1.45.0, `SQLitePCLRaw.bundle_e_sqlite3` to 3.0.5, and `Swashbuckle.AspNetCore` to 10.2.3.
- Migrated `Fast.Swagger` custom filters and security registration to the Microsoft.OpenApi 2.x API required by Swashbuckle 10.
- Established `FAST-AES-256-GCM-V1` as the initial cross-language password-based AES payload for `Fast.IaaS`.
- Simplified XML parameter documentation by removing type references already conveyed by method signatures while retaining links to related APIs and constraints.
- Incremented package versions: `Fast.Cache` 3.5.28, `Fast.Consul` 3.5.4, `Fast.NET.Core` 3.5.30, `Fast.DependencyInjection` 3.5.24, `Fast.DynamicApplication` 3.5.30, `Fast.EventBus` 3.5.23, `Fast.IaaS` 3.5.21, `Fast.JwtBearer` 3.5.33, `Fast.Logging` 3.5.26, `Fast.Mapster` 3.5.22, `Fast.OpenApi` 3.5.27, `Fast.Runtime` 3.5.25, `Fast.Serialization.Newtonsoft.Json` 3.5.22, `Fast.Serialization.System.Text.Json` 3.5.17, `Fast.SqlSugar` 3.5.60, `Fast.Swagger` 3.5.29, and `Fast.UnifyResult` 3.5.28.
- Incremented `I18nTranslateTool` to 1.0.1 and moved its `MiniExcel` version into central package management.

### Fixed

- `Fast.DependencyInjection` now registers marked concrete services that do not expose a business interface.
- `Fast.JwtBearer` now accepts the standard `access_token` query parameter for SignalR hub endpoints under `/hubs` while preserving custom token extraction logic.
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
