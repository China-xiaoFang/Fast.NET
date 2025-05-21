**中** | [En](https://github.com/Net-18K/Fast.NET)

# Fast.NET（v3）

一个应用程序工具类（框架），您可以将它集成到任何 `.NET` 应用程序中。

## 技术选择

- `Fast.NET` v3 版本采用 `C#10` 和 `.NET6` `.NET7` `.NET8` 进行开发。

## 项目背景

过去 `.NET` 在国内并没有很好的开源环境和社区，随着国内使用 `.NET` 的程序猿越来越多，慢慢的国内的开源环境和社区也越来越好。

各种 `.NET` 开源框架，也应时代而生。

本人作为在 `.NET` 行业中从业 `N年` 的 `小菜鸟`，也用过了很多开源的框架，所以想基于自己的工作经验和经历，为 `.NET` 开源做出一份小小的贡献。

## 安装

选择您需要的工具模块库进行安装。例如：

```
dotnet add package Fast.xxx
```

## 例子

按需引入使用~~~

```cs
using Fast.Cache;
using Fast.DependencyInjection;
using Fast.DynamicApplication;
using Fast.EventBus;
using Fast.JwtBearer;
using Fast.Logging;
using Fast.Mapster;
using Fast.NET.Core;
using Fast.Serialization;
using Fast.SqlSugar;
using Fast.Swagger;
using Fast.UnifyResult;

var builder = WebApplication.CreateBuilder(args);

// 初始化 框架
builder.Initialize();

// 添加日志服务
builder.Services.AddLoggingService(builder.Environment);

// 添加 Gzip 压缩
builder.Services.AddGzipCompression();

// 添加跨域服务
builder.Services.AddCorsAccessor();

// 添加 Mapster 服务
builder.Services.AddMapster();

// 添加序列化服务
builder.Services.AddSerialization();

// 添加依赖注入服务
builder.Services.AddDependencyInjection();

// 添加事件总线服务
builder.Services.AddEventBus();

// 添加 Redis 缓存服务
builder.Services.AddCache();

// 添加 JwtBearer 服务
builder.Services.AddJwtBearer(builder.Configuration);

// 添加 SqlSugar 服务
builder.Services.AddSqlSugar(builder.Configuration);

// Add controller.
builder.Services.AddControllers();

// 添加动态Api服务
builder.Services.AddDynamicApplication();

// 添加规范化返回服务
builder.Services.AddUnifyResult(builder.Configuration);

// 添加规范化文档服务
builder.Services.AddSwaggerDocuments(builder.Configuration);

var app = builder.Build();

// Mandatory Https.
app.UseHttpsRedirection();

app.UseStaticFiles();

// Enable backward reading.
app.EnableBuffering();

app.UseRouting();

app.MapControllers();

// 启用规范化文档
app.UseSwaggerDocuments();

app.Run();
```

## 文档

很抱歉，我正在努力中...

## 更新日志

更新日志 [点击查看](https://gitee.com/Net-18K/Fast.NET/commits/master)

## 详细功能（模块说明）

| 模块名称 | 状态 | 版本 | 说明 | 备注 |
| ------  | --- | ---- | --- | --- |
| [Fast.Cache](https://gitee.com/Net-18K/Fast.NET/tree/master/src/Cache) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Cache.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Cache) | Fast.NET 框架缓存模块库 | 一个在 .NET 行业中从业 `N年` 的 `小菜鸟` 常用的 Redis 缓存库，基于 [CSRedisCore](https://github.com/2881099/csredis) 封装 |
| [Fast.DependencyInjection](https://gitee.com/Net-18K/Fast.NET/tree/master/src/DependencyInjection) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.DependencyInjection.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.DependencyInjection) | Fast.NET 框架依赖注入模块库 |  |
| [Fast.DynamicApplication](https://gitee.com/Net-18K/Fast.NET/tree/master/src/DynamicApplication) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.DynamicApplication.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.DynamicApplication) | Fast.NET 框架动态Api模块库 |  |
| [Fast.EventBus](https://gitee.com/Net-18K/Fast.NET/tree/master/src/EventBus) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.EventBus.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.EventBus) | Fast.NET 框架事件总线模块库 |  |
| [Fast.IaaS](https://gitee.com/Net-18K/Fast.NET/tree/master/src/IaaS) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.IaaS.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.IaaS) | Fast.NET 框架基础设施模块库 | 一个在 `.NET` 行业中从业 `N年` 的 `小菜鸟` 常用的拓展工具类，亲自推荐，绝对好用！！！ |
| [Fast.JwtBearer](https://gitee.com/Net-18K/Fast.NET/tree/master/src/JwtBearer) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.JwtBearer.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.JwtBearer) | Fast.NET 框架 JwtBearer 模块库 |  |
| [Fast.Logging](https://gitee.com/Net-18K/Fast.NET/tree/master/src/Logging) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Logging.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Logging) | Fast.NET 框架日志模块库 |  |
| [Fast.Mapster](https://gitee.com/Net-18K/Fast.NET/tree/master/src/Mapster) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Mapster.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Mapster) | Fast.NET 框架对象映射模块库 | 基于 [Mapster](https://github.com/MapsterMapper/Mapster) 封装 |
| [Fast.NET.Core](https://gitee.com/Net-18K/Fast.NET/tree/master/src/Core) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.NET.Core.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.NET.Core) | Fast.NET 框架核心模块库 | 因 Fast.Core 已存在 Nuget 包，故改名 [Fast.NET.Core](https://gitee.com/Net-18K/Fast.NET/tree/master/src.NET/Core) |
| [Fast.Runtime](https://gitee.com/Net-18K/Fast.NET/tree/master/src/Runtime) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Runtime.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Runtime) | Fast.NET 框架 核心运行库 |  |
| [Fast.Serialization.Newtonsoft.Json](https://gitee.com/Net-18K/Fast.NET/tree/master/src/Serialization) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Serialization.Newtonsoft.Json.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Serialization.Newtonsoft.Json) | Fast.NET 框架 Newtonsoft.Json 序列化模块库 | 基于 [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) 封装 |
| [Fast.Serialization.System.Text.Json](https://gitee.com/Net-18K/Fast.NET/tree/master/src/Serialization) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Serialization.System.Text.Json.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Serialization.System.Text.Json) | Fast.NET 框架 System.Text.Json 序列化模块库 | 基于 [System.Text.Json](https://learn.microsoft.com/zh-cn/dotnet/api/system.text.json) 封装 |
| [Fast.SqlSugar](https://gitee.com/Net-18K/Fast.NET/tree/master/src/SqlSugar) | ⏳ | [![nuget](https://img.shields.io/nuget/v/Fast.SqlSugar.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.SqlSugar) | Fast.NET 框架SqlSugar模块库 | 基于 [SqlSugar](https://gitee.com/dotnetchina/SqlSugar) 封装 |
| [Fast.Swagger](https://gitee.com/Net-18K/Fast.NET/tree/master/src/Swagger) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Swagger.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Swagger) | Fast.NET 框架Swagger模块库 |  |
| [Fast.UnifyResult](https://gitee.com/Net-18K/Fast.NET/tree/master/src/UnifyResult) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.UnifyResult.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.UnifyResult) | Fast.NET 框架RESTful风格统一返回模块库 |  |

## 近期计划

- [✅] 基础设施模块
- [✅] 核心模块
- [✅] 跨域处理模块
- [✅] 对象映射模块
- [✅] Redis缓存模块
- [✅] 序列化模块
- [✅] 依赖注入模块
- [✅] 动态API模块
- [✅] 规范化文档模块
- [✅] 规范化返回模块
- [✅] 日志模块
- [✅] 事件总线
- [⏳] SqlSugar
- [⚠️] ...

> 状态说明
>
> | 图标 | 描述     |
> | ---- | -------- |
> | ⚠️   | 待定     |
> | ⏳   | 进行中   |
> | ✅   | 完成     |
> | 💔   | 随时抛弃 |

## 协议

[Fast.NET](https://gitee.com/Net-18K/Fast.NET) 遵循 [Apache-2.0](https://gitee.com/Net-18K/Fast.NET/blob/master/LICENSE) 开源协议，欢迎大家提交 `PR` 或 `Issue`。

```
Apache开源许可证

版权所有 © 2018-Now 小方

许可授权：
本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
3.修改或衍生作品须明确标注原作者及原软件出处。

特别声明：
- 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
- 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
- 包括但不限于数据丢失、业务中断等情况。

免责条款：
禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
```

`
由于目前还属于开发阶段，所有功能皆不是很稳定，所以暂且使用 Apache-2.0 开源协议。后续等所有功能都稳定后，会采用 MIT 开源协议。
`

## 团队成员

| 成员 | 技术 | 昵称 | 座右铭 |
| --- | ---- | ---- | ---- | 
| 小方 | 全栈 | 1.8K仔 | 接受自己的平庸和普通，是成长的必修课 <br> 你羡慕的生活都是你没熬过的苦 <br> 当你的能力还撑不起你的野心时，你就需要静下心来 好好学习 | 

## 编码环境
| 名称 | 备注 |
| --- | ---- |
| Visual Studio 2022 |  |
| Visual Studio Code |  |
| Resharper | 您在代码中所看到的以 `// ReSharper` 开头的注释，就是此应用生成的为了避免一些不必要的警告或提示 |

## 免责申明

    请勿用于违反我国法律的项目上

    本框架可以说是在各位前辈的基础上继续进行造轮子，只是比市面上的一些框架会更简洁，更方便。更好用我不知道是否能相谈。

## 补充说明

    如果对您有帮助，您可以点右上角 “Star” 收藏一下 ，获取第一时间更新，谢谢！