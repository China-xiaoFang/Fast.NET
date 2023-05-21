using Fast.Core;
using Fast.Core.Filter;
using Fast.Core.Handlers;
using Fast.Core.Middleware;
using Fast.Core.Options;
using Fast.Core.Restful;
using Fast.Core.SqlSugar;
using Fast.Iaas.EventSubscriber;
using Fast.ServiceCollection.Extension;
using Fast.ServiceCollection.ServiceCollection;
using Fast.ServiceCollection.Util;
using Furion;
using Furion.FriendlyException;
using Furion.Schedule;
using Furion.SpecificationDocument;
using IGeekFan.AspNetCore.Knife4jUI;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args).Inject();

// Run style.
builder.Services.AddRunStyle(r => r.UseDefault());

// System config.
GlobalContext.SystemSettingsOptions = App.GetConfig<SystemSettingsOptions>("SystemSettings");
// Copyright Info
GlobalContext.CopyrightInfoOptions = App.GetConfig<CopyrightInfoOptions>("CopyrightInfo");
// Upload file config.
GlobalContext.UploadFileOptions = App.GetConfig<UploadFileOptions>("UploadFile");

// Check
if (GlobalContext.SystemSettingsOptions.Environment.IsNullOrZero())
    throw Oops.Oh("系统配置错误，请检查系统配置！");

// Customize the console log output template.
builder.Logging.AddConsoleFormatter(options => { options.DateFormat = "yyyy-MM-dd hh:mm:ss(zzz) dddd"; });

// Cross origin.
builder.Services.AddCorsAccessor();

// The remote request.
builder.Services.AddRemoteRequest();

// Limit the size of uploaded files.
builder.Services.AddLimitUploadedFile(GlobalContext.SystemSettingsOptions.MaxRequestBodySize);

// Gzip brotli compression.
builder.Services.AddGzipBrotliCompression();

// Add json options.
builder.Services.AddJsonOptions();

// Logging, error level logging, create a log file every day.
builder.Services.AddLogging();

// Sign up for EventBus.
builder.Services.AddEventBus(options =>
{
    // 创建连接工厂
    var factory = App.GetConfig<ConnectionFactory>("RabbitMQConnection");

    // 创建默认内存通道事件源对象
    var mqEventSourceStorer = new RabbitMQEventSourceStorer(factory, "Fat.Event.Bus", 3000);

    // 替换默认事件总线存储器
    options.ReplaceStorer(serviceProvider => mqEventSourceStorer);
});

// JWT validation.
builder.Services.AddJwt<JwtHandler>(enableGlobalAuthorize: true);

if (GlobalContext.SystemSettingsOptions.AppLocalization)
{
    builder.Services.AddControllers()
        // Register multiple languages.
        .AddAppLocalization(settings =>
        {
            // Integrate third-party json configurations.
            builder.Services.AddJsonLocalization(options => options.ResourcesPath = settings.ResourcesPath);
        });
}
else
{
    builder.Services.AddControllers();
}

// Request the log interception middleware.
builder.Services.AddMvcFilter<RequestActionFilter>();

// Restful Return style normalization.
builder.Services.AddInjectWithUnifyResult<XnRestfulResultProvider>();

// Add Instant Messaging.
builder.Services.AddSignalR();

// Init sqlSugar.
builder.Services.AddSqlSugarClientService();

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

var app = builder.Build();

// Mandatory Https.
app.UseHttpsRedirection();

// Enable compression.
app.UseResponseCompression();

if (GlobalContext.SystemSettingsOptions.AppLocalization)
{
    // Multilingual configuration must be performed before route registration.
    app.UseAppLocalization();
}

// Add the status code interception middleware.
app.UseUnifyResultStatusCodes();

// Add cross-domain middleware.
app.UseCorsAccessor();

app.UseStaticFiles();

// Enable backward reading.
app.EnableBuffering();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<RequestInfoMiddleware>();

// Request interface flow limit.
app.UseMiddleware<RequestLimitMiddleware>();

// Demonstrates environmental request judgment.
app.UseMiddleware<DemoEnvironmentMiddleware>();

// Request AES decryption middleware.
app.UseMiddleware<AESDecryptMiddleware>();

// Start the task scheduling UI.
app.UseScheduleUI();

app.UseKnife4UI(options =>
{
    options.RoutePrefix = "knife4j";
    foreach (var groupInfo in SpecificationDocumentBuilder.GetOpenApiGroups())
    {
        options.SwaggerEndpoint("/" + groupInfo.RouteTemplate, groupInfo.Title);
    }
});

// Here, the default address is/API if no argument is entered, and/directory if string.empty is entered. If any string is entered, the/arbitrary string directory.
app.UseInject(string.Empty);

// Register the hub.

app.MapControllers();

app.Run();