using Fast.Core.Handlers;
using Fast.Core.Internal.Filter;
using Fast.Core.Internal.Middleware;
using Fast.Core.Internal.Restful;
using Fast.Core.Util;
using Furion.Schedule;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fast.Core.ServiceCollection;

/// <summary>
/// 服务集
/// 一键构建程序所需要用的服务
/// </summary>
public static class ServiceCollection
{
    /// <summary>
    /// 运行程序
    /// </summary>
    /// <param name="builder"></param>
    public static void RunProgram(this WebApplicationBuilder builder)
    {
        // Config.
        builder.Services.AddConfigurableOptions();

        // Get service Collection Options.
        var serviceCollectionOptions = GlobalContext.ServiceCollectionOptions;

        // Run style.
        builder.Services.AddRunStyle(r => r.UseDefault());

        // Customize the console log output template.
        builder.Logging.AddConsoleFormatter(options => { options.DateFormat = "yyyy-MM-dd hh:mm:ss(zzz) dddd"; });

        // Cross origin.
        builder.Services.AddCorsAccessor();

        // The remote request.
        builder.Services.AddRemoteRequest();

        // Limit the size of uploaded files
        builder.Services.AddLimitUploadedFile(serviceCollectionOptions.LimitUploadedFile);

        // Gzip brotli compression.
        builder.Services.AddGzipBrotliCompression(serviceCollectionOptions.GzipBrotliCompression);

        // JWT validation.
        builder.Services.AddJwt<JwtHandler>(enableGlobalAuthorize: serviceCollectionOptions.JWT);

        if (serviceCollectionOptions.AppLocalization)
        {
            builder.Services.AddControllers()
                // Register multiple languages.
                .AddAppLocalization();
        }
        else
        {
            builder.Services.AddControllers();
        }

        // Request the log interception middleware.
        builder.Services.AddMvcFilter<RequestActionFilter>();

        // Restful Return style normalization.
        builder.Services.AddInjectWithUnifyResult<XnRestfulResultProvider>();

        // Add json options.
        builder.Services.AddJsonOptions(serviceCollectionOptions.JsonOptionDateTimeFormat, serviceCollectionOptions.JsonOptions);

        //builder.Services.AddViewEngine();

        if (serviceCollectionOptions.SignalR)
        {
            // Add Instant Messaging.
            builder.Services.AddSignalR();
        }

        // Logging, error level logging, create a log file every day.
        builder.Services.AddLogging(serviceCollectionOptions.LogFileFormat, serviceCollectionOptions.LogFileSizeLimitBytes,
            serviceCollectionOptions.Log);

        // Sign up for EventBus.
        builder.Services.AddEventBusService(serviceCollectionOptions.EventBusService);

        // Init sqlSugar.
        builder.Services.AddSqlSugarClientService();

        if (serviceCollectionOptions.Scheduler)
        {
            // Register the task scheduling service.
            builder.Services.AddSchedule(options =>
            {
                // Enabled job log.
                options.JobDetail.LogEnabled = true;

                // Add the task scheduling job execution scheduler.
                options.AddMonitor<SchedulerJobMonitorFilter>();

                // Scan all task scheduling jobs.
                options.AddJob(App.EffectiveTypes.ScanToBuilders());
            });
        }

        var app = builder.Build();

        // Mandatory Https.
        app.UseHttpsRedirection();

        app.UseMiddleware<RequestInfoMiddleware>();

        if (serviceCollectionOptions.GzipBrotliCompression)
        {
            // Enable compression.
            app.UseResponseCompression();
        }

        if (serviceCollectionOptions.AppLocalization)
        {
            // Multilingual configuration must be performed before route registration.
            app.UseAppLocalization();
        }

        // Add the status code interception middleware.
        app.UseUnifyResultStatusCodes();

        app.UseStaticFiles();

        // Enable backward reading.
        app.EnableBuffering();

        app.UseRouting();

        if (serviceCollectionOptions.RequestLimit)
        {
            // Request interface flow limit.
            app.UseMiddleware<RequestLimitMiddleware>();
        }

        if (serviceCollectionOptions.DemoEnvironmentRequest)
        {
            // Demonstrates environmental request judgment.
            app.UseMiddleware<DemoEnvironmentMiddleware>();
        }

        if (serviceCollectionOptions.RequestAESDecrypt)
        {
            // Request AES decryption middleware.
            app.UseMiddleware<AESDecryptMiddleware>();
        }

        if (serviceCollectionOptions.Scheduler)
        {
            // Start the task scheduling UI.
            app.UseScheduleUI();
        }

        // Add cross-domain middleware.
        app.UseCorsAccessor();

        app.UseAuthentication();

        app.UseAuthorization();

        // Here, the default address is/API if no argument is entered, and/directory if string.empty is entered. If any string is entered, the/arbitrary string directory.
        app.UseInject(string.Empty);

        app.UseEndpoints(endpoints =>
        {
            if (serviceCollectionOptions.SignalR)
            {
                //// Register the hub.
                //endpoints.MapHub<ChatHub>("/hubs/chathub");
                //endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
            }
        });

        app.MapControllers();

        app.Run();
    }
}