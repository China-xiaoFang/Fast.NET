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

using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// ReSharper disable once CheckNamespace
namespace Fast.NET.Core;

/// <summary>
/// <see cref="WebApplicationBuilder"/> 拓展类
/// </summary>
[SuppressSniffer]
public static class WebApplicationBuilderExtension
{
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
        ConfigureApplication(builder.WebHost, builder.Host);

        return builder;
    }

    static void UseDefault()
    {
        var sb = new StringBuilder();
        sb.Append("\u001b[1m\u001b[34m");
        sb.Append(Environment.NewLine);
        sb.Append($"Fast.NET 程序启动时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        sb.Append(Environment.NewLine);
        sb.Append("\u001b[39m\u001b[22m\u001b[49m");
        sb.Append("\u001b[1m\u001b[32m");
        sb.Append(@"
      ______                _         _   _   ______   _______ 
     |  ____|              | |       | \ | | |  ____| |__   __|
     | |__     __ _   ___  | |_      |  \| | | |__       | |   
     |  __|   / _` | / __| | __|     | . ` | |  __|      | |   
     | |     | (_| | \__ \ | |_   _  | |\  | | |____     | |   
     |_|      \__,_| |___/  \__| (_) |_| \_| |______|    |_|   
");
        sb.Append("\u001b[39m\u001b[22m\u001b[49m");
        sb.Append("\u001b[1m\u001b[31m");
        sb.Append(Environment.NewLine);
        sb.Append(Environment.NewLine);
        sb.Append("    Gitee：https://gitee.com/China-xiaoFang/Fast.NET");
        sb.Append(Environment.NewLine);
        sb.Append(Environment.NewLine);
        sb.Append("\u001b[39m\u001b[22m\u001b[49m");
        sb.Append("\u001b[1m\u001b[35m");
        sb.Append("    持续集百家所长，完善与丰富本框架基础设施，为.NET生态增加一种选择！");
        sb.Append(Environment.NewLine);
        sb.Append(Environment.NewLine);
        sb.Append("期待您的PR，让.NET更好！");
        sb.Append(Environment.NewLine);
        Console.WriteLine(sb.ToString());
    }

    /// <summary>
    /// 配置 Application
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="hostBuilder"></param>
    private static void ConfigureApplication(IWebHostBuilder builder, IHostBuilder hostBuilder = null)
    {
        if (hostBuilder == null)
        {
            // 自动装载配置
            builder.ConfigureAppConfiguration((hostContext, configurationBuilder) =>
            {
                // 存储环境对象
                FastContext.HostEnvironment = FastContext.WebHostEnvironment = hostContext.HostingEnvironment;

                // 处理命令行启动参数 公共JSON 文件地址
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

                // 处理命令行启动参数 公共JSON 文件地址
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
            // 解决 IIS 或者 Nginx 反向代理获取不到真实客户端IP的问题
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                //options.ForwardedHeaders = ForwardedHeaders.All;

                // 若上面配置无效可尝试下列代码，比如在 IIS 中
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
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
            var fileNameParts = Path.GetFileName(file)
                .Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (fileNameParts.Length == 2)
                return fileNameParts[0];

            return string.Join('.', fileNameParts.Take(fileNameParts.Length - 2));
        }
    }
}