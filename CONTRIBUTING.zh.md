[**简体中文**](CONTRIBUTING.zh.md) | [English](CONTRIBUTING.md) · [返回 README](README.zh.md)

# 为 Fast.NET 做贡献

感谢你愿意帮助改进 Fast.NET。为了让修改可以被清晰审查、稳定发布并兼容所有目标框架，请遵循本指南。

## 开始之前

- 安装与 [`global.json`](global.json) 兼容的 .NET SDK。
- 阅读[架构说明](docs/ARCHITECTURE.zh.md)，确认修改所在层级和允许的依赖方向。
- 搜索现有 Issue，避免重复工作；较大的设计变更建议先创建 Issue 讨论。

## 本地开发

```bash
git clone https://gitee.com/FastDotnet/Fast.NET.git
cd Fast.NET
dotnet restore Fast.NET.sln
dotnet build Fast.NET.sln -c Release
```

仓库主要模块同时面向 .NET 6–10，`Fast.IaaS` 面向 .NET Standard 2.1。修改公共构建配置、条件编译或依赖版本时，应验证所有受影响目标。

## 代码约定

- 遵循 [`.editorconfig`](.editorconfig) 和现有命名风格。
- 保持模块职责单一，不为便利引入反向或循环依赖。
- 公共 API 应具有准确的 XML 文档。
- 复杂兼容逻辑、并发控制和安全相关代码应添加说明原因的中文注释。
- 异步 API 避免不必要的同步阻塞，并正确释放流、令牌和其他资源。
- 新增框架专属依赖时，按 `TargetFramework` 使用条件引用。
- 行为变更应同步更新 `README.zh.md`、`README.md` 及相关架构文档。

## 验证清单

提交前至少完成：

```bash
dotnet restore Fast.NET.sln
dotnet build Fast.NET.sln -c Release --no-restore
dotnet pack Fast.NET.sln -c Release --no-restore --no-build
```

同时检查：

- 构建没有新增警告或错误。
- `Fast.IaaS` 包只包含 `lib/netstandard2.1`。
- 其他 SDK 包包含所支持的 .NET 6–10 目标。
- 没有提交 `bin/`、`obj/`、`nupkgs/`、密钥或本地配置。
- 中英文公共文档结构和信息保持一致。

## Pull Request

一个易于审查的 PR 应当：

1. 聚焦一个明确问题，避免混入无关格式化。
2. 在描述中说明动机、设计选择、兼容影响和验证结果。
3. 对破坏性变更进行醒目标注，并提供迁移方式。
4. 关联对应 Issue（如果存在）。
5. 使用清晰的提交说明，例如 `fix: 修复缓存并发回填` 或 `feat: 增加新的基础设施模块`。

## 问题反馈

- [提交 Issue](https://gitee.com/FastDotnet/Fast.NET/issues)
- [提交 Pull Request](https://gitee.com/FastDotnet/Fast.NET/pulls)

请勿在 Issue、日志或示例配置中公开 API Key、连接字符串、令牌或其他敏感信息。
