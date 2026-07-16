[简体中文](CONTRIBUTING.zh.md) | [**English**](CONTRIBUTING.md) · [Back to README](README.md)

# Contributing to Fast.NET

Thank you for helping improve Fast.NET. Follow this guide so changes remain reviewable, releasable, and compatible with every target framework.

## Before you start

- Install a .NET SDK compatible with [`global.json`](global.json).
- Read the [architecture guide](docs/ARCHITECTURE.md) to understand the affected layer and allowed dependency direction.
- Search existing issues to avoid duplicate work. Open an issue before implementing a significant design change.

## Local development

```bash
git clone https://gitee.com/FastDotnet/Fast.NET.git
cd Fast.NET
dotnet restore Fast.NET.sln
dotnet build Fast.NET.sln -c Release
```

The primary modules target .NET 6–10, while `Fast.IaaS` targets .NET Standard 2.1. Changes to shared build configuration, conditional compilation, or dependency versions should be validated against every affected target.

## Code conventions

- Follow [`.editorconfig`](.editorconfig) and existing naming conventions.
- Keep each module focused and avoid reverse or circular dependencies.
- Public APIs should have accurate XML documentation.
- Add Chinese comments that explain the reason behind complex compatibility, concurrency, and security logic.
- Avoid unnecessary synchronous blocking in asynchronous APIs and dispose streams, tokens, and other resources correctly.
- Use target-specific conditional references for framework-specific dependencies.
- Update `README.zh.md`, `README.md`, and relevant architecture documentation when behavior changes.

## Validation checklist

Run at least:

```bash
dotnet restore Fast.NET.sln
dotnet build Fast.NET.sln -c Release --no-restore
dotnet pack Fast.NET.sln -c Release --no-restore --no-build
```

Also verify that:

- The build introduces no warnings or errors.
- The `Fast.IaaS` package contains only `lib/netstandard2.1`.
- Other SDK packages contain the supported .NET 6–10 targets.
- `bin/`, `obj/`, `nupkgs/`, secrets, and local configuration are not committed.
- Public Chinese and English documentation remains structurally and semantically synchronized.

## Pull requests

An effective pull request should:

1. Address one clear problem without unrelated formatting changes.
2. Explain motivation, design decisions, compatibility impact, and validation results.
3. Highlight breaking changes and provide migration guidance.
4. Link the related issue when one exists.
5. Use a clear commit message, such as `fix: prevent duplicate cache population` or `feat: add an infrastructure module`.

## Reporting issues

- [Open an issue](https://gitee.com/FastDotnet/Fast.NET/issues)
- [Open a pull request](https://gitee.com/FastDotnet/Fast.NET/pulls)

Never publish API keys, connection strings, tokens, or other sensitive data in issues, logs, or example configuration.
