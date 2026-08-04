# Security policy / 安全策略

## Supported versions / 支持版本

Security fixes are developed for the current source branch and currently supported targets: .NET 8, .NET 9, .NET 10, and the `netstandard2.1` target of `Fast.IaaS`. Unsupported .NET runtime versions do not receive validation.

安全修复面向当前源码分支以及仍受支持的 .NET 8、.NET 9、.NET 10 和 `Fast.IaaS` 的 `netstandard2.1` 目标。已停止支持的 .NET 运行时不在验证范围内。

## Reporting a vulnerability / 报告漏洞

Do not publish undisclosed vulnerability details, exploits, credentials, tokens, or connection strings in a public issue. Contact the repository maintainer through the private contact channel on the [primary Gitee repository](https://gitee.com/FastDotnet/Fast.NET). If no private channel is available, open a minimal issue requesting private contact and omit all technical exploit details.

请勿在公开 Issue 中披露尚未修复的漏洞细节、利用代码、凭证、令牌或连接字符串。请通过[主 Gitee 仓库](https://gitee.com/FastDotnet/Fast.NET)维护者的私密联系方式报告；如果暂时找不到私密渠道，只提交一个请求私下联系的最小 Issue，不要附带技术利用细节。

Include the affected package and version, runtime and operating system, impact, reproducible conditions, and a suggested mitigation when available. Maintainers will acknowledge the report, assess severity and affected versions, prepare a fix, and coordinate disclosure.

报告应包含受影响包和版本、运行时和操作系统、影响范围、可复现条件以及可用的缓解建议。维护者会确认收到、评估严重性与影响版本、准备修复并协调披露时间。
