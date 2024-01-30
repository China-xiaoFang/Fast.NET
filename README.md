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
dotnet add package Fast.NET.Core
```

## example

Two lines of code are injected into the entire framework, that’s how Nice ~~~

```cs
using Fast.IaaS;
using Fast.NET.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

//Key code 1: initialization framework
builder.Initialize();

// Key code 2: Add controller.
builder.AddControllers();

var app = builder.Build();

// Mandatory HTTPS.
app.UseHttpsRedirection();

app.UseStaticFiles();

// Enable backward reading.
app.EnableBuffering();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

## document

Sorry, I'm working on it...

## Update log

Update log [Click to view](https://gitee.com/Net-18K/Fast.NET/commits/master)

## Detailed functions (module description)

| Module name | Status | Version | Description | Remarks |
| ------ | --- | ---- | --- | --- |
| [Fast.IaaS](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/IaaS) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.IaaS.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.IaaS) | Fast.NET framework infrastructure module library (default other module libraries All inherit this module library) | A `newbie` who has been working in the `.NET` industry for `N years` uses commonly used expansion tools. I personally recommend them and they are absolutely easy to use! ! ! |
| [Fast.NET.Core](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Core) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.NET.Core.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.NET.Core) | Fast.NET Framework Core Module Library | Because Fast.Core already exists in the Nuget package, it was renamed [Fast.NET.Core](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET.NET/Core) |
| [Fast.Logging](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Logging) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Logging.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Logging) | Fast.NET Framework Logging Module Library | |
| [Fast.CorsAccessor](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/CorsAccessor) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.CorsAccessor.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.CorsAccessor) | Fast.NET framework cross-domain processing module library | |
| [Fast.Mapster](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Mapster) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Mapster.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Mapster) | Fast.NET Framework Object Mapping Module Library | Based on [Mapster](https://github.com/MapsterMapper/Mapster) Package |
| [Fast.Serialization](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Serialization) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Serialization.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Serialization) | Fast.NET framework serialization module library | Based on [System. Text.Json](https://learn.microsoft.com/zh-cn/dotnet/api/system.text.json) Package |
| [Fast.DependencyInjection](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/DependencyInjection) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.DependencyInjection.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.DependencyInjection) | Fast.NET Framework Dependency Injection Module Library | |
| [Fast.Cache](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Cache) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Cache.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Cache) | Fast.NET Framework cache module library | One in the .NET industry A commonly used Redis cache library for `little rookie` who has been in the industry for `N years`, based on [CSRedisCore](https://github.com/2881099/csredis) package |
| [Fast.JwtBearer](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/JwtBearer) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.JwtBearer.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.JwtBearer) | Fast.NET Framework JwtBearer module library | |
| [Fast.SqlSugar](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/SqlSugar) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.SqlSugar.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.SqlSugar) | Fast.NET Framework SqlSugar module library | Based on [SqlSugar]( https://gitee.com/dotnetchina/SqlSugar) Package |
| [Fast.ApplicationCore](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/ApplicationCore) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.ApplicationCore.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.ApplicationCore) | Fast.NET Framework Application Core Module Library | |
| [Fast.DynamicApplication](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/DynamicApplication) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.DynamicApplication.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.DynamicApplication) | Fast.NET Framework Dynamic Api Module Library | |
| [Fast.UnifyResult](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/UnifyResult) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.UnifyResult.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.UnifyResult) | Fast.NET framework RESTful style unified return module library | |
| [Fast.Swagger](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Swagger) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Swagger.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Swagger) | Fast.NET Framework Swagger Module Library | |
| [Fast.EventBus](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/EventBus) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.EventBus.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.EventBus) | Fast.NET Framework Event Bus Module Library | |
| [Fast.Consul](https://gitee.com/Net-18K/Fast.NET/tree/master/backend/Fast.NET/Consul) | ✅ | [![nuget](https://img.shields.io/nuget/v/Fast.Consul.svg?cacheSeconds=10800)](https://www.nuget.org/packages/Fast.Consul) | Fast.NET Framework Consul module library | Some [Consul]( https://github.com/hashicorp/consul) Commonly used remote service request encapsulation |

## Recent plans

- [✅] Infrastructure module
- [✅] Core module
- [✅] Cross-domain processing module
- [✅] Object mapping module
- [✅] Redis cache module
- [✅] Serialization module
- [✅] User-Agent parsing module
- [✅] Dependency injection module
- [✅] Dynamic API module
- [✅] Standardized Documentation Module
- [✅] Data verification module
- [✅] Exception module
- [✅] Normalized return module
- [✅] Log module
- [✅] SqlSugar
- [✅] Event bus
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

Copyright © 2018-2024 1.8KZi

The right to deal in the Software is hereby granted free of charge to any person obtaining a copy of this software and its related documentation (the "Software"),
Including but not limited to using, copying, modifying, merging, publishing, distributing, sublicensing, selling copies of the Software,
and permit individuals in possession of a copy of the software to do so, subject to the following conditions:

The above copyright notice and this license notice must be included on all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS AND NON-INFRINGEMENT.
In no event shall the author or copyright holder be liable for any claim, damages or other liability,
WHETHER ARISING IN CONTRACT, TORT OR OTHERWISE, IN CONNECTION WITH THE SOFTWARE OR ITS USE OR OTHER DEALINGS.
```

`
Since it is still in the development stage and all functions are not very stable, the Apache-2.0 open source protocol is used for the time being. After all functions are stable, the MIT open source license will be adopted.
`

## team member

| Members | Technology | Nickname | Motto |
| --- | ---- | ---- | ---- |
| 小方 | Full Stack | 1.8K仔 | Accepting your own mediocrity and ordinaryness is a required course for growth<br> The life you envy is the hardship you have not survived<br> When your ability cannot support you When you are ambitious, you need to calm down and study hard |

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