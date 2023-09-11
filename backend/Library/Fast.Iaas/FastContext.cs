using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.IO;
using System.Linq;

namespace Fast.Iaas;

/// <summary>
/// Fast.NET 上下文
/// </summary>
public static class FastContext
{
    /// <summary>
    /// 配置
    /// </summary>
    public static IConfiguration Configuration { get; set; }

    /// <summary>
    /// 请求上下文
    /// </summary>
    public static HttpContext HttpContext { get; set; }

    /// <summary>
    /// 获取泛型主机环境，如，是否是开发环境，生产环境等
    /// </summary>
    public static IHostEnvironment HostEnvironment { get; set; }

    /// <summary>
    /// 应用有效程序集
    /// </summary>
    public static readonly IEnumerable<Assembly> Assemblies;

    /// <summary>
    /// 有效程序集类型
    /// </summary>
    public static readonly IEnumerable<Type> EffectiveTypes;

    static FastContext()
    {
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    /// <typeparam name="TOptions">强类型选项类</typeparam>
    /// <param name="path">配置中对应的Key</param>
    /// <returns>TOptions</returns>
    public static TOptions GetConfig<TOptions>(string path)
    {
        return Configuration.GetSection(path).Get<TOptions>();
    }
}