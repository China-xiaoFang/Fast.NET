// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fast.NET.Core;

/// <summary>
/// 为 <see cref="WebApplicationBuilder"/> 提供扩展方法
/// </summary>
[SuppressSniffer]
public static class WebApplicationBuilderExtension
{
    /// <summary>
    /// 框架初始化
    /// </summary>
    /// <param name="builder">要配置的应用构建器</param>
    /// <returns>返回 <paramref name="builder"/>，便于链式调用</returns>
    public static WebApplicationBuilder Initialize(this WebApplicationBuilder builder)
    {
        // 运行控制台输出
        UseDefault(builder.Environment);

        FastContext.WebHostEnvironment = builder.Environment;

        // 初始化配置
        ConfigureApplication(builder.WebHost, builder.Host);

        return builder;
    }

    static void UseDefault(IWebHostEnvironment environment)
    {
        var coreAssembly = typeof(WebApplicationBuilderExtension).Assembly;
        var appAssembly = Assembly.GetEntryAssembly();

        // 获取框架版本和程序集版本
        var coreVersion = coreAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                              ?.InformationalVersion.Split('+')[0]
                          ?? "Unknown";
        var coreAssemblyVersion = coreAssembly.GetName()
                                      .Version?.ToString()
                                  ?? "Unknown";

        // 获取应用版本和程序集版本
        var appVersion = appAssembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                             ?.InformationalVersion.Split('+')[0]
                         ?? "Unknown";
        var appAssemblyVersion = appAssembly?.GetName()
                                     .Version?.ToString()
                                 ?? "Unknown";

        MAppContext.ConsoleWrite(console =>
        {
            console.ForegroundColor = ConsoleColor.Blue;
            console.WriteLine();

            console.ForegroundColor = ConsoleColor.DarkGray;
            console.Write("应用名称          ：");
            console.ForegroundColor = ConsoleColor.Gray;
            console.WriteLine(appAssembly?.GetName()
                                  .Name
                              ?? "Unknown");

            console.ForegroundColor = ConsoleColor.DarkGray;
            console.Write(".NET 运行时版本   ：");
            console.ForegroundColor = ConsoleColor.Cyan;
            console.WriteLine(RuntimeInformation.FrameworkDescription);

            console.ForegroundColor = ConsoleColor.DarkGray;
            console.Write("Fast.NET 框架版本 ：");
            console.ForegroundColor = ConsoleColor.Cyan;
            console.WriteLine($"v{coreVersion}（程序集：v{coreAssemblyVersion}）");

            console.ForegroundColor = ConsoleColor.DarkGray;
            console.Write("Fast.NET 程序版本 ：");
            console.ForegroundColor = ConsoleColor.Green;
            console.WriteLine($"v{appVersion}（程序集：v{appAssemblyVersion}）");

            console.ForegroundColor = ConsoleColor.DarkGray;
            console.Write("运行环境          ：");
            console.ForegroundColor = environment.IsProduction() ? ConsoleColor.Green :
                environment.IsDevelopment() ? ConsoleColor.Yellow : ConsoleColor.Magenta;
            console.WriteLine(environment.EnvironmentName);

            console.ForegroundColor = ConsoleColor.DarkGray;
            console.Write("主机名称          ：");
            console.ForegroundColor = ConsoleColor.Gray;
            console.WriteLine(Environment.MachineName);

            console.ForegroundColor = ConsoleColor.DarkGray;
            console.Write("操作系统          ：");
            console.ForegroundColor = ConsoleColor.Gray;
            console.WriteLine(RuntimeInformation.OSDescription);

            console.ForegroundColor = ConsoleColor.DarkGray;
            console.Write("系统架构          ：");
            console.ForegroundColor = ConsoleColor.Gray;
            console.WriteLine($"{Environment.OSVersion.Platform.ToString()} {RuntimeInformation.OSArchitecture.ToString()}");

            console.ForegroundColor = ConsoleColor.DarkGray;
            console.Write("进程架构          ：");
            console.ForegroundColor = ConsoleColor.Gray;
            console.WriteLine(RuntimeInformation.ProcessArchitecture.ToString());

            console.ForegroundColor = ConsoleColor.DarkGray;
            console.Write("程序启动时间      ：");
            console.ForegroundColor = ConsoleColor.White;
            console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            console.WriteLine();

            console.ForegroundColor = ConsoleColor.Green;
            console.WriteLine(@"
        ______                _         _   _   ______   _______ 
       |  ____|              | |       | \ | | |  ____| |__   __|
       | |__     __ _   ___  | |_      |  \| | | |__       | |   
       |  __|   / _` | / __| | __|     | . ` | |  __|      | |   
       | |     | (_| | \__ \ | |_   _  | |\  | | |____     | |   
       |_|      \__,_| |___/  \__| (_) |_| \_| |______|    |_|   
");

            console.ForegroundColor = ConsoleColor.Red;
            console.WriteLine();
            console.WriteLine("      Gitee：https://gitee.com/FastDotnet/Fast.NET");
            console.WriteLine();

            console.ForegroundColor = ConsoleColor.Magenta;
            console.WriteLine("      请勿用于违反我国法律的项目上！");
            console.WriteLine("      持续集百家所长，完善与丰富本框架基础设施，为.NET生态增加一种选择！");
            console.WriteLine();
            console.WriteLine("      接受自己的平庸和普通，是成长的必修课。");
            console.WriteLine("      当你的能力还撑不起你的野心时，你就需要静下心来 好好学习。");
            console.WriteLine();
            console.WriteLine("期待您的PR，让.NET更好！");
            console.WriteLine();
        });
    }

    /// <summary>
    /// 配置 Application
    /// </summary>
    /// <param name="builder">要配置的应用构建器</param>
    /// <param name="hostBuilder">要配置的主机构建器</param>
    private static void ConfigureApplication(IWebHostBuilder builder, IHostBuilder hostBuilder = null)
    {
        if (hostBuilder == null)
        {
            // 自动装载配置
            builder.ConfigureAppConfiguration((hostContext, configurationBuilder) =>
            {
                // 存储环境对象
                FastContext.HostEnvironment = FastContext.WebHostEnvironment = hostContext.HostingEnvironment;

                // 处理命令行启动参数 公共 JSON 文件地址
                var publicJsonPath = hostContext.Configuration["publicJsonPath"];

                if (!string.IsNullOrWhiteSpace(publicJsonPath))
                {
                    // 根据,分割
                    var publicJsonPathArr = publicJsonPath.Split(",");
                    if (publicJsonPathArr.Length > 0)
                    {
                        foreach (var jsonPath in publicJsonPathArr)
                        {
                            if (!string.IsNullOrWhiteSpace(jsonPath) && Path.IsPathRooted(jsonPath))
                            {
                                configurationBuilder.AddJsonFile(jsonPath, true, true);
                            }
                        }
                    }
                }

                // 加载配置
                AddJsonFiles(configurationBuilder, hostContext.HostingEnvironment);
            });
        }
        else
        {
            hostBuilder.ConfigureAppConfiguration((hostContext, configurationBuilder) =>
            {
                // 存储环境对象
                FastContext.HostEnvironment = hostContext.HostingEnvironment;

                // 处理命令行启动参数 公共 JSON 文件地址
                var publicJsonPath = hostContext.Configuration["publicJsonPath"];

                if (!string.IsNullOrWhiteSpace(publicJsonPath))
                {
                    // 根据,分割
                    var publicJsonPathArr = publicJsonPath.Split(",");
                    if (publicJsonPathArr.Length > 0)
                    {
                        foreach (var jsonPath in publicJsonPathArr)
                        {
                            if (!string.IsNullOrWhiteSpace(jsonPath) && Path.IsPathRooted(jsonPath))
                            {
                                configurationBuilder.AddJsonFile(jsonPath, true, true);
                            }
                        }
                    }
                }

                // 加载配置
                AddJsonFiles(configurationBuilder, hostContext.HostingEnvironment);
            });
        }

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

            // 注册 Core Startup 过滤器
            services.AddTransient(typeof(IStartupFilter), typeof(CoreStartupFilter));

            Debugging.Info("Registering forwarded headers......");
            // 信任代理转发的客户端地址和协议；部署端仍需限制可信代理边界
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

#if NET10_0_OR_GREATER
                options.KnownIPNetworks.Clear();
#else
                options.KnownNetworks.Clear();
#endif

                options.KnownProxies.Clear();
            });
        });
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
    /// 排除运行时 JSON 后缀
    /// </summary>
    private static readonly string[] runtimeJsonSuffixArr =
    {
        "deps.json", "runtimeconfig.dev.json", "runtimeconfig.prod.json", "runtimeconfig.json", "staticwebassets.runtime.json"
    };

    /// <summary>
    /// 添加 JSON 文件
    /// </summary>
    /// <param name="configurationBuilder">要添加配置源的配置构建器</param>
    /// <param name="hostEnvironment">当前应用的宿主环境</param>
    private static void AddJsonFiles(IConfigurationBuilder configurationBuilder, IHostEnvironment hostEnvironment)
    {
        // 获取根配置
        var configuration = configurationBuilder as ConfigurationManager ?? configurationBuilder.Build();

        // 获取程序执行目录
        var executeDirectory = AppContext.BaseDirectory;

        // 获取自定义配置扫描目录
        var configurationScanDirectories = (configuration.GetSection("ConfigurationScanDirectories")
                                                .Get<string[]>()
                                            ?? Array.Empty<string>()).Select(u => Path.Combine(executeDirectory, u));

        // 扫描执行目录及自定义配置目录下的 *.json 文件
        var jsonFiles = new[] {executeDirectory}.Concat(configurationScanDirectories)
            .Concat(InternalConfigurationScanDirectories.Where(Directory.Exists))
            .SelectMany(u => Directory.GetFiles(u, "*.json", SearchOption.TopDirectoryOnly))
            .ToList();

        // 如果没有配置文件，中止执行
        if (!jsonFiles.Any())
            return;

        // 获取环境变量名，如果没找到，则读取 NETCORE_ENVIRONMENT 环境变量信息识别（用于非 Web 环境）
        var envName = hostEnvironment?.EnvironmentName ?? Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT") ?? "Unknown";

        // 处理控制台应用程序
        var _excludeJsonPrefixArr = hostEnvironment == null
            ? excludeJsonPrefixArr.Where(u => !u.Equals("appsettings"))
            : excludeJsonPrefixArr;

        // 将所有文件进行分组
        var jsonFilesGroups = SplitConfigFileNameToGroups(jsonFiles)
            .Where(u => !_excludeJsonPrefixArr.Contains(u.Key, StringComparer.OrdinalIgnoreCase)
                        && !u.Any(c => runtimeJsonSuffixArr.Any(z => c.EndsWith(z, StringComparison.OrdinalIgnoreCase))));

        // 遍历所有配置分组
        foreach (var group in jsonFilesGroups)
        {
            // 限制查找的 json 文件组
            var limitFileNames = new[] {$"{group.Key}.json", $"{group.Key}.{envName}.json"};

            // 查找默认配置和环境配置
            var files = group.Where(u => limitFileNames.Contains(Path.GetFileName(u), StringComparer.OrdinalIgnoreCase))
                .OrderBy(u => Path.GetFileName(u)
                    .Length);

            // 循环加载
            foreach (var jsonFile in files)
            {
                configurationBuilder.AddJsonFile(jsonFile, true, true);
            }
        }
    }

    /// <summary>
    /// 对配置文件名进行分组
    /// </summary>
    /// <param name="configFiles">要按名称分组的配置文件集合</param>
    /// <returns>对配置文件名进行分组集合</returns>
    private static IEnumerable<IGrouping<string, string>> SplitConfigFileNameToGroups(IEnumerable<string> configFiles)
    {
        // 分组
        return configFiles.GroupBy(Function);

        static string Function(string file)
        {
            // 根据 . 分隔
            var fileNameParts = Path.GetFileName(file)
                .Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (fileNameParts.Length == 2)
                return fileNameParts[0];

            return string.Join('.', fileNameParts.Take(fileNameParts.Length - 2));
        }
    }
}
