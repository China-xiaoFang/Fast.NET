[中](https://gitee.com/Net-18K/Fast.NET) | **En**

# Fast.NET（v3）

An application utility class (framework) that you can integrate into any `.NET` application.

## Technology selection

- `Fast.NET` v3 version is developed using `C#10` and `.NET6` `.NET7` `.NET8`.

## Background of the project

In the past, `.NET` did not have a good open source environment and community in China. As more and more programmers use `.NET` in China, the domestic open source environment and community are gradually getting better and better.

Various `.NET` open source frameworks have also emerged in response to the times.

As a newbie who has been working in the `.NET` industry for N years, I have also used many open source frameworks, so I want to make a small contribution to `.NET` open source based on my own work experience and experience. Small contribution.

## Install

Select the tool module library you need to install. For example:

```
dotnet add package Fast.xxx
```

## example

Introduce and use as needed~~~

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

// Initialize the frame
builder.Initialize();

// Add log service
builder.Services.AddLoggingService(builder.Environment);

// Add Gzip compression
builder.Services.AddGzipCompression();

// Add cross-domain service
builder.Services.AddCorsAccessor();

// Add Mapster service
builder.Services.AddMapster();

// Add serialization service
builder.Services.AddSerialization();

// Add dependency injection service
builder.Services.AddDependencyInjection();

// Add event bus service
builder.Services.AddEventBus();

// Add Redis cache service
builder.Services.AddCache();

// Add JwtBearer service
builder.Services.AddJwtBearer(builder.Configuration);

// Add SqlSugar service
builder.Services.AddSqlSugar(builder.Configuration);

// Add controller.
builder.Services.AddControllers();

// Add dynamic API service
builder.Services.AddDynamicApplication();

// Add normalized return service
builder.Services.AddUnifyResult(builder.Configuration);

// Add standardized document service
builder.Services.AddSwaggerDocuments(builder.Configuration);

var app = builder.Build();

// Mandatory HTTPS.
app.UseHttpsRedirection();

app.UseStaticFiles();

// Enable backward reading.
app.EnableBuffering();

app.UseRouting();

app.MapControllers();

// Enable normalized documents
app.UseSwaggerDocuments();

app.Run();
```

## document

Sorry, I'm working on it...

## Update log

Update log [Click to view](https://gitee.com/Net-18K/Fast.NET/commits/master)

## Detailed functions (module description)

| Module Name | Status | Version | Description | Notes |
| ------ | --- | ---- | --- | --- |
| [Fast.Cache](https://gitee.com/Net-18K/Fast.NET/tree/master/src/Cache) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Cache.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Cache) | Fast.NET Framework Cache Module Library | A Redis cache library commonly used by a `novice` who has been working in the .NET industry for `N years`, based on the [CSRedisCore](https://github.com/2881099/csredis) package |
| [Fast.DependencyInjection](https://gitee.com/Net-18K/Fast.NET/tree/master/src/DependencyInjection) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.DependencyInjection.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.DependencyInjection) | Fast.NET Framework Dependency Injection Module Library | |
| [Fast.DynamicApplication](https://gitee.com/Net-18K/Fast.NET/tree/master/src/DynamicApplication) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.DynamicApplication.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.DynamicApplication) | Fast.NET Framework Dynamic Api Module Library | |
| [Fast.EventBus](https://gitee.com/Net-18K/Fast.NET/tree/master/src/EventBus) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.EventBus.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.EventBus) | Fast.NET Framework Event Bus Module Library | |
| [Fast.IaaS](https://gitee.com/Net-18K/Fast.NET/tree/master/src/IaaS) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.IaaS.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.IaaS) | Fast.NET Framework Infrastructure Module Library | A commonly used expansion tool class by a `novice` who has been working in the `.NET` industry for `N years`. I personally recommend it, it is absolutely useful!!! |
| [Fast.JwtBearer](https://gitee.com/Net-18K/Fast.NET/tree/master/src/JwtBearer) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.JwtBearer.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.JwtBearer) | Fast.NET Framework JwtBearer module library | |
| [Fast.Logging](https://gitee.com/Net-18K/Fast.NET/tree/master/src/Logging) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Logging.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Logging) | Fast.NET Framework log module library | |
| [Fast.Mapster](https://gitee.com/Net-18K/Fast.NET/tree/master/src/Mapster) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Mapster.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Mapster) | Fast.NET Framework object mapping module library | Based on [Mapster](https://github.com/MapsterMapper/Mapster) encapsulation |
| [Fast.NET.Core](https://gitee.com/Net-18K/Fast.NET/tree/master/src/Core) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.NET.Core.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.NET.Core) | Fast.NET Framework core module library | Because Fast.Core already has a Nuget package, it is renamed [Fast.NET.Core](https://gitee.com/Net-18K/Fast.NET/tree/master/src.NET/Core) |
| [Fast.Runtime](https://gitee.com/Net-18K/Fast.NET/tree/master/src/Runtime) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Runtime.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Runtime) | Fast.NET Framework Core Runtime Library | |
| [Fast.Serialization.Newtonsoft.Json](https://gitee.com/Net-18K/Fast.NET/tree/master/src/Serialization) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Serialization.Newtonsoft.Json.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Serialization.Newtonsoft.Json) | Fast.NET Framework Newtonsoft.Json Serialization Module Library | Based on [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) encapsulation |
| [Fast.Serialization.System.Text.Json](https://gitee.com/Net-18K/Fast.NET/tree/master/src/Serialization) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Serialization.System.Text.Json.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Serialization.System.Text.Json) | Fast.NET Framework System.Text.Json serialization module library | Based on [System.Text.Json](https://learn.microsoft.com/zh-cn/dotnet/api/system.text.json) encapsulation |
| [Fast.SqlSugar](https://gitee.com/Net-18K/Fast.NET/tree/master/src/SqlSugar) | ⏳ | [![nuget](https://img.shields.io/nuget/v/Fast.SqlSugar.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.SqlSugar) | Fast.NET Framework SqlSugar Module Library | Based on [SqlSugar](https://gitee.com/dotnetchina/SqlSugar) Encapsulation |
| [Fast.Swagger](https://gitee.com/Net-18K/Fast.NET/tree/master/src/Swagger) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Swagger.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Swagger) | Fast.NET Framework Swagger Module Library | |
| [Fast.UnifyResult](https://gitee.com/Net-18K/Fast.NET/tree/master/src/UnifyResult) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.UnifyResult.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.UnifyResult) | Fast.NET Framework RESTful Style Unified Return Module Library | |

## Recent plans

- [✅] Infrastructure module
- [✅] Core module
- [✅] Cross-domain processing module
- [✅] Object mapping module
- [✅] Redis cache module
- [✅] Serialization module
- [✅] Dependency injection module
- [✅] Dynamic API module
- [✅] Normalized document module
- [✅] Normalized return module
- [✅] Log module
- [✅] Event bus
- [✅] SqlSugar
- [⚠️] ...

> Status description
>
> | Icon | Description |
> | ---- | -------- |
> | ⚠️ | TBA |
> | ⏳ | In progress |
> | ✅ | Complete |
> | 💔 | Throw away at any time |

## protocol

[Fast.NET](https://gitee.com/Net-18K/Fast.NET) Follow [Apache-2.0](https://gitee.com/Net-18K/Fast.NET/blob/master/LICENSE ) Open source license, everyone is welcome to submit `PR` or `Issue`.

```
Apache Open Source License

Copyright © 2018-Now xiaoFang

License:
This Agreement grants any individual or organization that obtains a copy of this software and its related documentation (hereinafter referred to as the "Software").
Subject to the terms of this Agreement, you have the right to use, copy, modify, merge, publish, distribute, sublicense, and sell copies of the Software:
1.All copies or major parts of the Software must retain this Copyright Notice and this License Agreement.
2.The use, copying, modification, or distribution of the Software shall not violate applicable laws or infringe upon the legitimate rights and interests of others.
3.Modified or derivative works must clearly indicate the original author and the source of the original Software.

Special Statement:
- This Software is provided "as is" without any express or implied warranty of any kind, including but not limited to the warranty of merchantability, fitness for purpose, and non-infringement.
- In no event shall the author or copyright holder be liable for any direct or indirect loss caused by the use or inability to use this Software.
- Including but not limited to data loss, business interruption, etc.

Disclaimer:
It is prohibited to use this software to engage in illegal activities such as endangering national security, disrupting social order, or infringing on the legitimate rights and interests of others.
The author does not assume any responsibility for any legal disputes and liabilities caused by the secondary development of this software.
```

`
Since it is still in the development stage and all functions are not very stable, the Apache-2.0 open source protocol is used for the time being. After all functions are stable, the MIT open source license will be adopted.
`

## team member

| Members | Technology | Nickname | Motto |
| --- | ---- | ---- | ---- |
| XiaoFang | Full Stack | 1.8K仔 | Accepting your own mediocrity and ordinaryness is a required course for growth<br> The life you envy is the hardship you have not survived<br> When your ability cannot support you When you are ambitious, you need to calm down and study hard |

## Coding environment
| Name | Remarks |
| --- | ---- |
| Visual Studio 2022 | |
| Visual Studio Code | |
| Resharper | The comments starting with `// ReSharper` that you see in the code are generated by this application to avoid unnecessary warnings or prompts |

## Disclaimer

     Please do not use it for projects that violate the laws of our country.

     This framework can be said to continue to reinvent the wheel based on the predecessors, but it is simpler and more convenient than some frameworks on the market. It's better to use. I don't know if we can talk about it.

## Supplementary instructions

     If it is helpful to you, you can click "Star" in the upper right corner to collect it and get the latest updates. Thank you!