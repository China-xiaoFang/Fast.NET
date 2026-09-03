[**简体中文**](RELEASING.zh.md) | [English](RELEASING.md) · [返回 README](../README.zh.md)

# 发布指南

Fast.NET 的 17 个 NuGet 包独立维护版本。发布属于不可逆外部操作，只有维护者完成审核并明确确认后才能执行。

## 发布前检查

1. 在受影响项目的 `.csproj` 中更新 `<Version>`。
2. 更新 [`CHANGELOG.md`](../CHANGELOG.md)、README 和模块文档。
3. 确认 `Directory.Packages.props` 中的依赖版本与目标框架一致。
4. 构建并打包全部 SDK 项目：

```bat
UploadNuget.bat pack
```

批处理文件依次还原解决方案、执行 `dotnet build`，构建成功后才执行 `dotnet pack`。`pack` 模式完成打包后直接结束，不执行发布。

## 产物

所有 `.nupkg` 和 `.snupkg` 位于 `nupkgs/`，批处理文件不会递归清理已有目录。发布时会根据每个项目当前的 `PackageId` 和 `PackageVersion` 识别包，因此不会选中目录中遗留的旧版本。

发布前至少抽查：

- `.nuspec` 中的包名、版本、说明、许可证、仓库地址和依赖版本。
- `Fast.IaaS` 仅包含 `lib/netstandard2.1`。
- 其他包包含 `lib/net8.0`、`lib/net9.0` 和 `lib/net10.0`。
- 每个目标框架同时包含 DLL 与 XML API 文档。
- 符号包为 `.snupkg`。

## 可选发布

发布前在当前环境中设置 `NUGET_API_KEY`。`NUGET_SOURCE` 为可选项，默认发布到 NuGet.org。

```bat
UploadNuget.bat publish-all
UploadNuget.bat publish-one Fast.Cache
```

`publish-all` 会依次构建、打包并发布当前全部 17 个包。`publish-one` 会完成构建和打包，但只发布指定包Id。不带参数运行 `UploadNuget.bat` 时，会显示构建配置和包选择菜单，单包发布后可继续选择其他包。发布前会展示目标包和发布源，随后调用 `dotnet nuget push`。

## 发布结果

脚本同时检查 NuGet 的退出码和诊断输出。`--skip-duplicate` 遇到已存在的包时可能返回 0，因此不会仅凭退出码将其计为发布成功。脚本通过进程内的 `DOTNET_CLI_UI_LANGUAGE=zh-CN` 使用简体中文诊断，退出后恢复调用方的语言设置；结果判断同时兼容中文和英文的成功、重复包、警告及错误诊断。HTTP 方法和状态名称等未本地化内容仍可能显示英文。

| 结果 | 颜色 | 判定 |
| --- | --- | --- |
| 成功 | 绿色 | 退出码为 0，存在明确的发布成功消息，且没有警告、错误或重复包提示 |
| 已存在跳过 | 黄色 | 退出码为 0，仅跳过已存在的版本，不计入成功数 |
| 警告 | 黄色 | 出现警告、主包与符号包部分成功/部分跳过，或无法确认完整发布结果 |
| 失败 | 红色 | 退出码非 0，或诊断输出包含错误，即使退出码为 0 也按失败处理 |

汇总按每次发布尝试分别统计成功、跳过、警告和失败，并列出需要检查的包。仅成功或跳过时返回 `0`；存在警告但没有失败时返回 `2`；存在失败时返回 `1`。交互模式即使只产生跳过或警告，选择结束时也会显示汇总。

汇总按结果分组，计数和包名使用对应颜色，每个包名单独占一行；成功的包也会列出。颜色通过系统 PowerShell 的控制台 API 设置，输出后恢复原色。输出重定向或 PowerShell 不可用时使用纯文本，不生成 ANSI 转义序列。

不要把 API Key 写入仓库、脚本、日志、命令历史或 Issue。发布完成后清除当前进程中的环境变量，并在 NuGet.org 检查包元数据、README、依赖和符号包状态。
