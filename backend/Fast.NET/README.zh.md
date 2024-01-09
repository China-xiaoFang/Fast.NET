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

本人作为在 .NET 行业中从业 `N年` 的 `小菜鸟`，也用过了很多开源的框架，所以想基于自己的工作经验和经历，为 .NET 开源做出一份小小的贡献。

所以 Fast.NET（v3）诞生了。

**持续集百家所长，完善与丰富本框架基础设施，为.NET生态增加一种选择！**

### 声明

```
本框架可以说是在各位前辈的基础上继续进行造轮子，只是比市面上的一些框架会更简洁，更方便。更好用我不知道是否能相谈。

还有一点关于最近 “Furion” 的商业化之路引起了很多人的关注，也包括我。在此本人不做任何的建议和意见。
只能说的是，如果您觉得好用，能用，那么您继续使用。如果您觉得不好用，不能用，那么可以使用别的框架，或者选择自己造轮子。
```

## 详细功能（模块说明）

| 模块名称 | 状态 | 版本 | 说明 | 备注 |
| ------  | --- | ---- | --- | --- |
| [Fast.IaaS](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/IaaS) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.IaaS.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.IaaS) | Fast.NET 框架基础设施模块库 （默认其余模块库全部继承此模块库） | 一个在 .NET 行业中从业 `N年` 的 `小菜鸟` 常用的拓展工具类，亲自推荐，绝对好用！！！ |
| [Fast.NET.Core](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Core) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.NET.Core.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.NET.Core) | Fast.NET 框架核心模块库 | 因 Fast.Core 已存在 Nuget 包，故改名 [Fast.NET.Core](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET.NET/Core) |
| [Fast.Logging](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Logging) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Logging.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Logging) | Fast.NET 框架日志模块库 |  |
| [Fast.CorsAccessor](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/CorsAccessor) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.CorsAccessor.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.CorsAccessor) | Fast.NET 框架跨域处理模块库 | |
| [Fast.Mapster](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Mapster) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Mapster.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Mapster) | Fast.NET 框架对象映射模块库 | 基于 [Mapster](https://github.com/MapsterMapper/Mapster) 封装 |
| [Fast.Serialization](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Serialization) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Serialization.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Serialization) | Fast.NET 框架序列化模块库 | 基于 [System.Text.Json](https://learn.microsoft.com/zh-cn/dotnet/api/system.text.json) 封装 |
| [Fast.DependencyInjection](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/DependencyInjection) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.DependencyInjection.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.DependencyInjection) | Fast.NET 框架依赖注入模块库 |  |
| [Fast.Cache](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Cache) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Cache.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Cache) | Fast.NET 框架缓存模块库 | 一个在 .NET 行业中从业 `N年` 的 `小菜鸟` 常用的 Redis 缓存库，基于 [CSRedisCore](https://github.com/2881099/csredis) 封装 |
| [Fast.JwtBearer](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/JwtBearer) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.JwtBearer.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.JwtBearer) | Fast.NET 框架 JwtBearer 模块库 |  |
| [Fast.SqlSugar](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/SqlSugar) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.SqlSugar.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.SqlSugar) | Fast.NET 框架SqlSugar模块库 | 基于 [SqlSugar](https://gitee.com/dotnetchina/SqlSugar) 封装 |
| [Fast.ApplicationCore](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/ApplicationCore) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.ApplicationCore.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.ApplicationCore) | Fast.NET 框架Application核心模块库 |  |
| [Fast.DynamicApplication](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/DynamicApplication) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.DynamicApplication.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.DynamicApplication) | Fast.NET 框架动态Api模块库 |  |
| [Fast.UnifyResult](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/UnifyResult) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.UnifyResult.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.UnifyResult) | Fast.NET 框架RESTful风格统一返回模块库 |  |
| [Fast.Swagger](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Swagger) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Swagger.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Swagger) | Fast.NET 框架Swagger模块库 |  |
| [Fast.EventBus](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/EventBus) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.EventBus.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.EventBus) | Fast.NET 框架事件总线模块库 |  |
| [Fast.Consul](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Consul) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Consul.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Consul) | Fast.NET 框架Consul模块库 | 一些 [Consul](https://github.com/hashicorp/consul) 常用的远程服务请求封装 |

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
- [✅] SqlSugar
- [✅] 事件总线
- [⚠️] ...

> 状态说明
>
> | 图标 | 描述     |
> | ---- | -------- |
> | ⚠️   | 待定     |
> | ⏳   | 进行中   |
> | ✅   | 完成     |
> | 💔   | 随时抛弃 |