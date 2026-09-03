# Changelog

All notable changes to Fast.NET are documented in this file. The repository follows independent package versioning; a release entry must name every affected package.

## Unreleased

### Breaking changes

- Primary SDK packages now target .NET 8, .NET 9, and .NET 10. .NET 6 and .NET 7 assets are no longer produced.
- Repository builds use C# 14 and the .NET 10 SDK.
- `RetryUtil.InvokeAsync` now awaits retry callbacks, invokes the fallback for suppressed terminal failures, and accepts a cancellation token.

### Added

- `Fast.Runtime` 3.5.29 adds `MAppContext.ConsoleWrite` with a `ConsoleWriter` callback for segmented text and color output, automatic foreground/background restoration, serialized synchronous callbacks, and nested writes. `Fast.IaaS` includes an internal writer while retaining its standalone `netstandard2.1` target.
- Central NuGet dependency management through `Directory.Packages.props`.
- GitHub CI for restore, dependency auditing, build, packaging, and artifact upload.
- A single Windows entry point for build, pack, optional all-package publishing, and optional single-package publishing.
- Bilingual getting-started, module, and release guides plus security and community policies.

### Changed

- Updated `Fast.NET.Core` from 3.5.35 to 3.5.36. The `builder.Initialize()` banner now displays application identity, application/framework informational and assembly versions, .NET runtime, environment, host, OS, OS/process architectures, and local startup time. Environment values use distinct colors; startup time is now `White`. The banner also adjusts spacing and adds a lawful-use reminder and learning messages while retaining `MAppContext.ConsoleWrite` color restoration and plain redirected output.
- Updated `Mapster` and `Mapster.DependencyInjection` to 10.0.12, `MiniExcel` to 1.45.0, `SQLitePCLRaw.bundle_e_sqlite3` to 3.0.5, `SqlSugarCore` to 5.1.4.218, and `Swashbuckle.AspNetCore` to 10.2.3.
- Updated ASP.NET Core package references to 8.0.30, 9.0.19, and 10.0.11 for their corresponding target frameworks.
- Migrated `Fast.Swagger` custom filters and security registration to the Microsoft.OpenApi 2.x API required by Swashbuckle 10.
- Established `FAST-AES-256-GCM-V1` as the initial cross-language password-based AES payload for `Fast.IaaS`.
- Simplified XML documentation by removing type references already conveyed by signatures, inheriting existing implementation contracts, and retaining links to related APIs and constraints.
- Marked internal leaf implementations and JSON converters as sealed where inheritance is not supported.
- Incremented 15 SDK package versions; the two serialization packages retain their existing versions. Current package versions: `Fast.Cache` 3.5.32, `Fast.Consul` 3.5.8, `Fast.NET.Core` 3.5.37, `Fast.DependencyInjection` 3.5.28, `Fast.DynamicApplication` 3.5.34, `Fast.EventBus` 3.5.27, `Fast.IaaS` 3.5.27, `Fast.JwtBearer` 3.5.39, `Fast.Logging` 3.5.31, `Fast.Mapster` 3.5.26, `Fast.OpenApi` 3.5.38, `Fast.Runtime` 3.5.29, `Fast.Serialization.Newtonsoft.Json` 3.5.25, `Fast.Serialization.System.Text.Json` 3.5.20, `Fast.SqlSugar` 3.5.64, `Fast.Swagger` 3.5.33, and `Fast.UnifyResult` 3.5.32.
- Packages referencing `Fast.Runtime` now require 3.5.29 or later. Upgrade installed Fast.NET modules to the versions listed above together to keep transitive dependencies aligned.
- Incremented `I18nTranslateTool` to 1.0.1 and moved its `MiniExcel` version into central package management.

### Fixed

- `UploadNuget.bat` now uses Simplified Chinese CLI diagnostics within the script, classifies redirected UTF-8 NuGet output before converting it to CMD CP936, and displays skipped results in dark gray. With PowerShell unavailable, output remains plain text and a zero exit code is conservatively reported as a warning because detailed diagnostics cannot be classified.
- `Fast.IaaS` 3.5.27 now includes the correct unit when `TimeSpanExtension.ToDescription()` receives exactly one minute, one hour, or one day, producing `01分00秒`, `01时00分00秒`, or `01天00时00分00秒` respectively.
- `Fast.OpenApi` 3.5.38 now generates `Promise<void>` and calls `axiosUtil.request<void>` for actions returning non-generic `Task` or `ValueTask`. `Download` and `Export` actions retain their file response and expose an `autoDownloadFile` parameter that defaults to `true`: Web clients use `AxiosResponse<Blob>`, while mobile clients allow `AxiosResponse<Blob | ArrayBuffer | string>`. Missing response schemas for other value-returning actions continue to use `unknown`.
- `UploadNuget.bat` now checks NuGet diagnostics as well as exit codes, reports existing packages as skipped, separates warnings and partial symbol results from confirmed success, and groups all results with one package per line. Success is green, skips and warnings are yellow, and failures are red; redirected output remains plain text. Exit codes are `0` for success/skips only, `2` for warnings, and `1` for failures.
- Replaced bare ANSI output in startup banners and direct diagnostics starting with `Fast.NET.Core` 3.5.35, `Fast.Runtime` 3.5.29, `Fast.OpenApi` 3.5.37, `Fast.SqlSugar` 3.5.64, and `Fast.IaaS` 3.5.26 with `ConsoleWriter`. Redirected output remains plain text, and colors are restored even when output fails. The existing `Fast.Logging` formatter and `ConsoleLoggerProvider` pipeline are unchanged.
- Fast.IaaS tree building now supports partial node sets whose parent nodes are not included and uses parent lookups to avoid repeated collection scans during recursive construction.
- `Fast.Logging` now restores foreground and background state after independently colored log segments.
- `Fast.OpenApi` Web and mobile upload clients now expose an optional Axios `onUploadProgress` callback and forward it to the corresponding adapter.
- `Fast.OpenApi` TypeScript clients now emit separate type imports in rule-compliant order, explicit `Promise<T>` return types, and `unknown` for unspecified schemas to satisfy the current Fast ESLint Config rules.
- `Fast.DependencyInjection` now registers marked concrete services that do not expose a business interface.
- `Fast.JwtBearer` now accepts the standard `access_token` query parameter regardless of a SignalR hub's route prefix, preserves custom token extraction, and resolves the HTTP context during endpoint and hub-method authorization.
- WebSocket detection now relies on ASP.NET Core's WebSocket feature instead of treating every request to `/ws` as a WebSocket request.
- `Fast.OpenApi` now resolves duration, dictionary, collection, nested, and composed OpenAPI schemas to concrete TypeScript types and unwraps nullable schemas without emitting explicit `| null` unions.
- `Fast.OpenApi` now maps `Int64` and OpenAPI `int64` values, including nullable, collection, and dictionary forms, to TypeScript `string` to match the default JSON serialization behavior.
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
