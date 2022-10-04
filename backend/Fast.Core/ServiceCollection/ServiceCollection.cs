using Fast.Core.AdminFactory.ServiceFactory.InitDataBase;
using Fast.Core.Filter;
using Fast.Core.Filter.Restful;
using Fast.Core.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace Fast.Core.ServiceCollection;

/// <summary>
/// 服务集
/// 一键构建程序所需要用的服务
/// </summary>
public static class ServiceCollection
{
    /// <summary>
    /// 添加上传文件大小限制
    /// </summary>
    public static bool LimitUploadedFile { get; set; } = true;

    /// <summary>
    /// 添加Gzip Brotli 压缩
    /// </summary>
    public static bool GzipBrotliCompression { get; set; } = true;

    /// <summary>
    /// 启用JWT
    /// </summary>
    public static bool JWT { get; set; } = true;

    /// <summary>
    /// 多语言
    /// </summary>
    public static bool AppLocalization { get; set; } = true;

    /// <summary>
    /// 数据验证
    /// </summary>
    public static bool DataValidation { get; set; } = true;

    /// <summary>
    /// JSON序列化格式
    /// </summary>
    public static string JsonOptionFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

    /// <summary>
    /// JSON序列化
    /// </summary>
    public static bool JsonOptions { get; set; } = true;

    /// <summary>
    /// 雪花Id WorkerId 
    /// </summary>
    public static string SnowIdWorkerId { get; set; } = "1";

    /// <summary>
    /// 即时通讯
    /// </summary>
    public static bool SignalR { get; set; } = true;

    /// <summary>
    /// 日志文件格式
    /// </summary>
    public static string LogFileFormat { get; set; } = "{0:yyyy-MM-dd}";

    /// <summary>
    /// 日志文件
    /// </summary>
    public static bool Log { get; set; } = true;

    /// <summary>
    /// 事件总线
    /// </summary>
    public static bool EventBusService { get; set; } = true;

    /// <summary>
    /// 运行程序
    /// </summary>
    /// <param name="builder"></param>
    public static void RunProgram(this WebApplicationBuilder builder)
    {
        // Run style.
        builder.Services.AddRunStyle(r => r.UseDefault());

        // Customize the console log output template.
        builder.Logging.AddConsoleFormatter(options => { options.DateFormat = "yyyy-MM-dd hh:mm:ss(zzz) dddd"; });

        // Config.
        builder.Services.AddConfigurableOptions();

        // Cross origin.
        builder.Services.AddCorsAccessor();

        // The remote request.
        builder.Services.AddRemoteRequest();

        // Limit the size of uploaded files
        builder.Services.AddLimitUploadedFile(LimitUploadedFile);

        // Gzip brotli compression.
        builder.Services.AddGzipBrotliCompression(GzipBrotliCompression);

        // JWT validation.
        builder.Services.AddJwt<JwtHandler>(enableGlobalAuthorize: JWT);

        if (AppLocalization)
        {
            builder.Services.AddControllersWithViews()
                // Register multiple languages.
                .AddAppLocalization();
        }
        else
        {
            builder.Services.AddControllersWithViews();
        }

        if (DataValidation)
        {
            // Global data validation.
            builder.Services.AddDataValidation();
        }

        // Request the log interception middleware.
        builder.Services.AddMvcFilter<RequestActionFilter>();

        // Restful Return style normalization.
        builder.Services.AddInjectWithUnifyResult<XnRestfulResultProvider>();

        // Add json options.
        builder.Services.AddJsonOptions(JsonOptionFormat, JsonOptions);

        builder.Services.AddViewEngine();

        if (SignalR)
        {
            // Add Instant Messaging.
            builder.Services.AddSignalR();
        }

        // Add Snowflakes Id.
        builder.Services.AddSnowflakeId(SnowIdWorkerId);

        // Logging, error level logging, create a log file every day.
        builder.Services.AddLogging(LogFileFormat, Log);

        // Sign up for EventBus.
        builder.Services.AddEventBusService(EventBusService);

        // Init sqlSugar.
        builder.Services.SqlSugarClientConfigure();

        var app = builder.Build();

        // Add the status code interception middleware.
        app.UseUnifyResultStatusCodes();

        if (GzipBrotliCompression)
        {
            // Enable compression.
            app.UseResponseCompression();
        }

        // Mandatory Https.
        app.UseHttpsRedirection();

        if (AppLocalization)
        {
            // Multilingual configuration must be performed before route registration.
            app.UseAppLocalization();
        }

        app.UseStaticFiles();

        app.UseRouting();

        // Add cross-domain middleware.
        app.UseCorsAccessor();

        app.UseAuthentication();

        app.UseAuthorization();

        // Here, the default address is/API if no argument is entered, and/directory if string.empty is entered. If any string is entered, the/arbitrary string directory.
        app.UseInject(string.Empty);

        app.UseEndpoints(endpoints =>
        {
            if (SignalR)
            {
                //// Register the hub.
                //endpoints.MapHub<ChatHub>("/hubs/chathub");
                //endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            }
        });

        app.MapControllers();

        if (GlobalContext.SystemSettingsOptions?.InitDataBase == true)
        {
            // It is recommended to disable the initialization of the database except for the first time.
            Task.Run(async () => { await GetService<IInitDataBaseService>().InitDataBase(); });
        }

        app.Run();
    }
}