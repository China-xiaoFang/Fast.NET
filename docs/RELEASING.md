[简体中文](RELEASING.zh.md) | [**English**](RELEASING.md) · [Back to README](../README.md)

# Release guide

The 17 Fast.NET NuGet packages are independently versioned. Publishing is an irreversible external action and must run only after maintainer review and explicit confirmation.

## Pre-release checklist

1. Update `<Version>` in every affected project.
2. Update [`CHANGELOG.md`](../CHANGELOG.md), READMEs, and module documentation.
3. Confirm that `Directory.Packages.props` versions match their target frameworks.
4. Build and pack all SDK projects:

```bat
UploadNuget.bat pack
```

The batch file restores the solution, runs `dotnet build`, and runs `dotnet pack` only after the build succeeds. `pack` mode stops without publishing.

## Artifacts

All `.nupkg` and `.snupkg` files are written to `nupkgs/`. Existing files aren't recursively deleted. The batch file identifies the current package for each project's `PackageId` and `PackageVersion`, so old package versions in the directory aren't selected for publishing.

Before publishing, inspect at least:

- Package name, version, description, license, repository URL, and dependency versions in the `.nuspec`.
- `Fast.IaaS` contains only `lib/netstandard2.1`.
- Every other package contains `lib/net8.0`, `lib/net9.0`, and `lib/net10.0`.
- Each target contains both the assembly and XML API documentation.
- Symbols use the `.snupkg` format.

## Optional publishing

Set `NUGET_API_KEY` in the current environment before publishing. `NUGET_SOURCE` is optional and defaults to NuGet.org.

```bat
UploadNuget.bat publish-all
UploadNuget.bat publish-one Fast.Cache
```

`publish-all` builds, packs, and publishes all 17 current packages. `publish-one` builds and packs everything, then publishes only the specified package ID. Running `UploadNuget.bat` without arguments opens the interactive configuration and package-selection menu, where another package can be selected after a single-package push. The script lists the selected packages and source before calling `dotnet nuget push`.

## Publish results

The script checks both the NuGet exit code and diagnostic output. With `--skip-duplicate`, an existing package can return exit code 0, so that code alone does not count as a successful publication. NuGet diagnostics use English for stable classification; script prompts remain Chinese.

| Result | Color | Meaning |
| --- | --- | --- |
| Success | Green | Exit code 0 with an explicit publication confirmation and no warnings, errors, or duplicate messages |
| Already exists, skipped | Yellow | Exit code 0 with only existing versions skipped; excluded from the success count |
| Warning | Yellow | A warning, mixed success and skips across the package and its symbols, or an unconfirmed overall result |
| Failure | Red | A nonzero exit code or an error diagnostic, even if the command returns 0 |

The summary counts each push attempt as success, skip, warning, or failure and lists packages requiring attention. The script returns `0` for success or skips only, `2` for warnings without failures, and `1` if any attempt fails. Ending an interactive session also shows the summary when its only results were skips or warnings.

The summary groups counts and package names by result, using the corresponding color and one package per line, including successful packages. System PowerShell applies colors through the console API and restores the original color afterward. Redirected output and systems without PowerShell use plain text without generated ANSI escape sequences.

Never store an API key in the repository, scripts, logs, shell history, or issues. Clear the process environment variable after publishing, then verify package metadata, README rendering, dependencies, and symbol status on NuGet.org.
