using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Fast.Core.DependencyInjection.Extensions;
using Fast.Core.Diagnostics;
using Fast.Core.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Fast.Core;

/// <summary>
/// 内部 App 副本
/// </summary>
internal class InternalApp
{
    /// <summary>
    /// 应用服务
    /// </summary>
    internal static IServiceCollection InternalServices;

    /// <summary>
    /// 根服务
    /// </summary>
    internal static IServiceProvider RootServices;

    /// <summary>
    /// 配置对象
    /// </summary>
    internal static IConfiguration Configuration;

    /// <summary>
    /// 获取Web主机环境
    /// </summary>
    internal static IWebHostEnvironment WebHostEnvironment;

    /// <summary>
    /// 获取泛型主机环境
    /// </summary>
    internal static IHostEnvironment HostEnvironment;

    /// <summary>
    /// 配置配置文件扫描目录
    /// </summary>
    internal static IEnumerable<string> InternalConfigurationScanDirectories { get; private set; } =
        new[] {"AppConfig", "JsonConfig"};

    internal static void ConfigureApplication(IWebHostBuilder builder, IHostBuilder hostBuilder = default)
    {
        // 自动装载配置
        if (hostBuilder == null)
        {
            builder.ConfigureAppConfiguration((hostContext, configurationBuilder) =>
            {
                // 存储环境对象
                HostEnvironment = WebHostEnvironment = hostContext.HostingEnvironment;

                // 加载配置
                Debugging.Info("加载JSON文件配置中......");
                AddJsonFiles(configurationBuilder, hostContext.HostingEnvironment);
            });
        }
        else
        {
            // 自动装载配置
            ConfigureHostAppConfiguration(hostBuilder);
        }

        // 应用初始化服务
        builder.ConfigureServices((hostContext, services) =>
        {
            // 存储配置对象
            Configuration = hostContext.Configuration;

            // 存储服务提供器
            InternalServices = services;

            // 跨域配置
            Debugging.Info("正在配置跨域请求......");
            services.AddCorsAccessor();

            // 注册 HttpContextAccessor 服务
            Debugging.Info("正在注册 HttpContextAccessor 服务......");
            services.AddHttpContextAccessor();

            // Gzip 压缩
            Debugging.Info("正在注册 Gzip压缩......");
            services.AddGzipBrotliCompression();

            // JSON 序列化配置
            services.AddJsonOptions();

            // 添加日志服务
            IServiceCollectionExtension.AddLogging(services);

            // 注册内存和分布式内存
            Debugging.Info("正在注册 MemoryCache......");
            services.AddMemoryCache();
            Debugging.Info("正在注册 DistributedMemoryCache......");
            services.AddDistributedMemoryCache();

            // 注册全局依赖注入
            Debugging.Info("正在注册全局依赖注入......");
            services.AddInnerDependencyInjection();

            // 添加对象映射
            services.AddObjectMapper();

            // 默认内置 GBK，Windows-1252, Shift-JIS, GB2312 编码支持
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // 添加缓存
            services.AddCache();

            // 添加 SqlSugar
            services.AddSqlSugar();
        });
    }

    /// <summary>
    /// 自动装载主机配置
    /// </summary>
    /// <param name="builder"></param>
    private static void ConfigureHostAppConfiguration(IHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((hostContext, configurationBuilder) =>
        {
            // 存储环境对象
            HostEnvironment = hostContext.HostingEnvironment;

            // 加载配置
            Debugging.Info("加载JSON文件配置中......");
            AddJsonFiles(configurationBuilder, hostContext.HostingEnvironment);
        });
    }

    internal static void AddJsonFiles(IConfigurationBuilder configurationBuilder, IHostEnvironment hostEnvironment)
    {
        // 获取程序执行目录
        var executeDirectory = AppContext.BaseDirectory;

        // 扫描自定义配置扫描目录
        var jsonFiles = new[] {executeDirectory}.Concat(InternalConfigurationScanDirectories)
            .SelectMany(s => Directory.GetFiles(s, "*.json", SearchOption.TopDirectoryOnly)).ToList();

        // 如果没有配置文件，中止执行
        if (!jsonFiles.Any())
            return;

        // 获取环境变量名，如果没找到，则读取 NETCORE_ENVIRONMENT 环境变量信息识别（用于非 Web 环境）
        var envName = hostEnvironment?.EnvironmentName ?? Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? "Unknown";

        // 处理控制台应用程序
        var _excludeJsonPrefixArr = hostEnvironment == default
            ? excludeJsonPrefixArr.Where(u => !u.Equals("appsettings"))
            : excludeJsonPrefixArr;

        // 将所有文件进行分组
        var jsonFilesGroups = SplitConfigFileNameToGroups(jsonFiles).Where(u =>
            !_excludeJsonPrefixArr.Contains(u.Key, StringComparer.OrdinalIgnoreCase) && !u.Any(c =>
                runtimeJsonSuffixArr.Any(z => c.EndsWith(z, StringComparison.OrdinalIgnoreCase))));

        // 遍历所有配置分组
        foreach (var group in jsonFilesGroups)
        {
            // 限制查找的 json 文件组
            var limitFileNames = new[] {$"{group.Key}.json", $"{group.Key}.{envName}.json"};

            // 查找默认配置和环境配置
            var files = group.Where(u => limitFileNames.Contains(Path.GetFileName(u), StringComparer.OrdinalIgnoreCase))
                .OrderBy(u => Path.GetFileName(u).Length);

            // 循环加载
            foreach (var jsonFile in files)
            {
                configurationBuilder.AddJsonFile(jsonFile, optional: true, reloadOnChange: true);
            }
        }
    }

    /// <summary>
    /// 排除的配置文件前缀
    /// </summary>
    private static readonly string[] excludeJsonPrefixArr = {"appsettings", "bundleconfig", "compilerconfig"};

    /// <summary>
    /// 排除运行时 Json 后缀
    /// </summary>
    private static readonly string[] runtimeJsonSuffixArr =
    {
        "deps.json", "runtimeconfig.dev.json", "runtimeconfig.prod.json", "runtimeconfig.json", "staticwebassets.runtime.json"
    };

    /// <summary>
    /// 对配置文件名进行分组
    /// </summary>
    /// <param name="configFiles"></param>
    /// <returns></returns>
    private static IEnumerable<IGrouping<string, string>> SplitConfigFileNameToGroups(IEnumerable<string> configFiles)
    {
        // 分组
        return configFiles.GroupBy(Function);

        // 本地函数
        static string Function(string file)
        {
            // 根据 . 分隔
            var fileNameParts = Path.GetFileName(file).Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (fileNameParts.Length == 2)
                return fileNameParts[0];

            return string.Join('.', fileNameParts.Take(fileNameParts.Length - 2));
        }
    }
}