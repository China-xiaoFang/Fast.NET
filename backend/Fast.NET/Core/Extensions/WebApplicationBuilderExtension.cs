// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using System.Text;
using Fast.IaaS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fast.NET.Core.Extensions;

/// <summary>
/// <see cref="WebApplicationBuilder"/> 拓展类
/// </summary>
public static class WebApplicationBuilderExtension
{
    /// <summary>
    /// 框架初始化
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/></param>
    /// <returns><see cref="WebApplicationBuilder"/></returns>
    public static WebApplicationBuilder EighteenK(this WebApplicationBuilder builder)
    {
        return builder.Initialize();
    }

    /// <summary>
    /// 框架初始化
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/></param>
    /// <returns><see cref="WebApplicationBuilder"/></returns>
    public static WebApplicationBuilder HelloNet(this WebApplicationBuilder builder)
    {
        return builder.Initialize();
    }

    /// <summary>
    /// 框架初始化
    /// </summary>
    /// <param name="builder"><see cref="WebApplicationBuilder"/></param>
    /// <returns><see cref="WebApplicationBuilder"/></returns>
    public static WebApplicationBuilder Initialize(this WebApplicationBuilder builder)
    {
        // 运行控制台输出
        UseDefault();

        FastContext.WebHostEnvironment = builder.Environment;

        // 初始化配置
        ConfigureApplication(builder.WebHost);

        return builder;
    }

    static void UseDefault()
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(@$"
        Fast.NET 程序启动时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}
        ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"
          ______                _         _   _   ______   _______ 
         |  ____|              | |       | \ | | |  ____| |__   __|
         | |__     __ _   ___  | |_      |  \| | | |__       | |   
         |  __|   / _` | / __| | __|     | . ` | |  __|      | |   
         | |     | (_| | \__ \ | |_   _  | |\  | | |____     | |   
         |_|      \__,_| |___/  \__| (_) |_| \_| |______|    |_|   
                                                                   
        ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(@"
        Gitee：https://gitee.com/Net-18K/Fast.NET
        ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(@"
        持续集百家所长，完善与丰富本框架基础设施，为.NET生态增加一种选择！

        期待您的PR，让.NET更好！
        ");
        Console.ResetColor();
    }

    /// <summary>
    /// 配置 Application
    /// </summary>
    /// <param name="builder"></param>
    private static void ConfigureApplication(IWebHostBuilder builder)
    {
        // 自动装载配置
        builder.ConfigureAppConfiguration((hostContext, configurationBuilder) =>
        {
            // 存储环境对象
            FastContext.HostEnvironment = FastContext.WebHostEnvironment = hostContext.HostingEnvironment;

            // 加载配置
            AddJsonFiles(configurationBuilder, hostContext.HostingEnvironment);
        });

        // 应用初始化服务
        builder.ConfigureServices((hostContext, services) =>
        {
            // 存储配置对象
            FastContext.Configuration = hostContext.Configuration;

            // 存储服务提供器
            FastContext.InternalServices = services;

            // 注册 HttpContextAccessor 服务
            services.AddHttpContextAccessor();

            // 注册 内存缓存
            services.AddMemoryCache();

            // 默认内置 GBK，Windows-1252, Shift-JIS, GB2312 编码支持
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // 注册 Startup 过滤器
            services.AddStartupFilter();
        });

        // 添加管道启动服务
        builder.HostingInjection();
    }

    /// <summary>
    /// 默认配置文件扫描目录
    /// </summary>
    private static IEnumerable<string> InternalConfigurationScanDirectories =>
        new[] {"AppConfig", "AppSettings", "JsonConfig", "Config", "Settings"};

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
    /// 添加 JSON 文件
    /// </summary>
    /// <param name="configurationBuilder"></param>
    /// <param name="hostEnvironment"></param>
    private static void AddJsonFiles(IConfigurationBuilder configurationBuilder, IHostEnvironment hostEnvironment)
    {
        // 获取程序执行目录
        var executeDirectory = AppContext.BaseDirectory;

        // 扫描自定义配置扫描目录
        var jsonFiles = new[] {executeDirectory}
            .Concat(InternalConfigurationScanDirectories.Select(sl => $"{executeDirectory}{sl}")).Where(Directory.Exists)
            .SelectMany(s => Directory.GetFiles(s, "*.json", SearchOption.TopDirectoryOnly)).ToList();

        // 如果没有配置文件，中止执行
        if (jsonFiles.Count == 0)
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