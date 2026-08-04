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

`publish-all` 会依次构建、打包并发布当前全部 17 个包。`publish-one` 会完成构建和打包，但只发布指定包 ID。不带参数运行 `UploadNuget.bat` 时，会显示构建配置和包选择菜单。任何发布模式都会先展示发布范围，并要求输入精确的 `PUBLISH`，之后才调用 `dotnet nuget push`。

不要把 API Key 写入仓库、脚本、日志、命令历史或 Issue。发布完成后清除当前进程中的环境变量，并在 NuGet.org 检查包元数据、README、依赖和符号包状态。
