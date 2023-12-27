**中** | [En](https://github.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET)

# Fast.NET（v3）

### 框架介绍

一个快速构建项目，集百家所长，专注于 Web Api 应用，提供开箱即用的功能，并紧跟最新的前沿技术的 .NET 框架。

### 技术选择

- Fast.NET v3 版本采用 C# 10 和 .NET 6 进行开发。

#### 项目特点：

- **集百家所长**：集百家好用功能于一体。
- **开箱即用**：提供众多黑科技，无需额外的配置或开发工作，可以快速构建项目。
- **紧随前沿技术**：采用最新的技术和框架，始终保持与行业的最新趋势和发展同步。

### 项目背景

过去 .NET 在国内并没有很好的开源环境和社区，随着国内使用 .NET 的程序猿越来越多，慢慢的国内的开源环境和社区也越来越好。

各种 .NET 开源框架，也应时代而生。

本人作为在 .NET 行业中从业5年的小菜鸟，也用过了很多开源的框架，所以想基于自己的工作经验和经历，为 .NET 开源做出一份小小的贡献。

所以 Fast.NET（v3）诞生了。

这里由衷感谢 .NET 开源框架的大佬。

- 👉 **[百小僧 Furion](https://gitee.com/dotnetchina/Furion)**
- 👉 **[小杰 SqlSugar](https://gitee.com/dotnetchina/SqlSugar)**

正是因为这些 .NET 大佬，才有了 [Fast.NET](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET) 的诞生。

此处要特别感谢 [Furion](https://gitee.com/dotnetchina/Furion) 的作者 [百小僧](https://gitee.com/monksoul)。

[Fast.NET](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET) 框架底层部分核心代码都是由 [Furion](https://gitee.com/dotnetchina/Furion) 提供，借鉴或引用了 [Furion v4](https://gitee.com/dotnetchina/Furion) 的部分源码，也征得了作者 [百小僧](https://gitee.com/monksoul) 的同意授权。

**持续集百家所长，完善与丰富本框架基础设施，为.NET生态增加一种选择！**

## 详细功能（模块说明）

| 模块名称 | 状态 | 版本 | 说明 | 备注 |
| ------  | --- | ---- | --- | --- |
| [Fast.Cache](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Cache/Fast.Cache) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Cache.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Cache) | Fast.NET 框架缓存模块库 | 一个在 .NET 行业中从业5年的小菜鸟常用的 Redis 缓存库，基于 [CSRedisCore](https://github.com/2881099/csredis) 封装 |
| [Fast.Consul](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Consul/Fast.Consul) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Consul.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Consul) | Fast.NET 框架Consul模块库 | 一些 [Consul](https://github.com/hashicorp/consul) 常用的远程服务请求封装 |
| [Fast.NET.Core](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Core/Fast.NET.Core) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.NET.Core.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.NET.Core) | Fast.NET 框架核心模块库 | 因 Fast.Core 已存在 Nuget 包，故改名 [Fast.NET.Core](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET.NET/Core/Fast.NET.Core) |
| [Fast.CorsAccessor](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/CorsAccessor/Fast.CorsAccessor) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.CorsAccessor.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.CorsAccessor) | Fast.NET 框架跨域处理模块库 | |
| [Fast.DependencyInjection](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/DependencyInjection/Fast.DependencyInjection) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.DependencyInjection.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.DependencyInjection) | Fast.NET 框架依赖注入模块库 | 基于 [Furion v4](https://gitee.com/dotnetchina/Furion) 部分源码 |
| [Fast.EventBus](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/EventBus/Fast.EventBus) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.EventBus.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.EventBus) | Fast.NET 框架事件总线模块库 | 引用 [Furion v4](https://gitee.com/dotnetchina/Furion) 源码 |
| [Fast.IaaS](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/IaaS/Fast.IaaS) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.IaaS.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.IaaS) | Fast.NET 框架基础设施模块库 | 一个在 .NET 行业中从业5年的小菜鸟常用的拓展工具类 |
| [Fast.Logging](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Logging/Fast.Logging) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Logging.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Logging) | Fast.NET 框架日志模块库 | 引用 [Furion v4](https://gitee.com/dotnetchina/Furion) 源码 |
| [Fast.Mapster](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Mapster/Fast.Mapster) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Mapster.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Mapster) | Fast.NET 框架对象映射模块库 | 基于 [Mapster](https://github.com/MapsterMapper/Mapster) 封装 |
| [Fast.Serialization](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Serialization/Fast.Serialization) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Serialization.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Serialization) | Fast.NET 框架序列化模块库 | 基于 [System.Text.Json](https://learn.microsoft.com/zh-cn/dotnet/api/system.text.json) 封装 |
| [Fast.SpecificationProcessor](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/SpecificationProcessor/Fast.SpecificationProcessor) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.SpecificationProcessor.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.SpecificationProcessor) | Fast.NET 框架规范化处理模块库 | 基于 [Furion v4](https://gitee.com/dotnetchina/Furion) 源码 |
| [Fast.SqlSugar](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/SqlSugar/Fast.SqlSugar) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.SqlSugar.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.SqlSugar) | Fast.NET 框架SqlSugar模块库 | 基于 [SqlSugar](https://gitee.com/dotnetchina/SqlSugar) 封装 |

## 近期计划

- [✅] 基础设施模块
- [✅] 核心模块
- [✅] 跨域处理模块
- [✅] 对象映射模
- [✅] Redis缓存模块
- [✅] 序列化模块
- [✅] User-Agent解析模块
- [✅] 依赖注入模块
- [✅] 动态API模块
- [✅] 规范化文档模块
- [✅] 数据验证模块
- [✅] 异常模块
- [✅] 规范化返回模块
- [✅] 日志模块
- [✅] 事件总线
- [✅] SqlSugar
- [⚠️] ...

> 状态说明
>
> | 图标 | 描述     |
> | ---- | -------- |
> | ⚠️   | 待定     |
> | ⏳   | 进行中   |
> | ✅   | 完成     |
> | 💔   | 随时抛弃 |