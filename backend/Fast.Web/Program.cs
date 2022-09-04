using System.IO.Compression;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Fast.Core;
using Fast.Core.AdminFactory.ServiceFactory.InitDataBase;
using Fast.Core.Cache;
using Fast.Core.EventSubscriber;
using Fast.Core.Filter;
using Fast.Core.Handlers;
using Fast.Core.SqlSugar;
using Fast.Core.Util;
using Fast.Core.Util.Json.JsonConverter;
using Fast.Core.Util.Restful;
using Fast.Core.Util.SnowflakeId.Extension;
using Furion;
using Furion.Logging;
using Furion.Templates;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args).Inject();

#region Config Info

// Run style.
builder.Services.AddRunStyle(r => r.UseDefault());
// Database config.
builder.Services.AddConfigurableOptions<ConnectionStringsOptions>();
// Cache config.
builder.Services.AddConfigurableOptions<CacheOptions>();
// System config.
builder.Services.AddConfigurableOptions<SystemSettingsOptions>();
// Upload file config.
builder.Services.AddConfigurableOptions<UploadFileOptions>();

#endregion

// Cross origin.
builder.Services.AddCorsAccessor();

// The remote request.
builder.Services.AddRemoteRequest(option =>
{
    // The weather forecast GZIP.
    option.AddHttpClient("weatherCdn", c => { c.BaseAddress = new Uri("http://wthrcdn.etouch.cn/"); })
        .ConfigurePrimaryHttpMessageHandler(_ => new HttpClientHandler {AutomaticDecompression = DecompressionMethods.GZip});
});

#region Limit the size of uploaded files

var maxRequestBodySize = App.GetOptions<SystemSettingsOptions>().MaxRequestBodySize;
builder.Services.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = maxRequestBodySize; });
builder.Services.Configure<IISServerOptions>(options => { options.MaxRequestBodySize = maxRequestBodySize; });
builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = maxRequestBodySize; });

#endregion

#region Gzip brotli compression.

builder.Services.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
builder.Services.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "text/html; charset=utf-8", "application/xhtml+xml", "application/atom+xml", "image/svg+xml"
    });
});

#endregion

// JWT validation.
builder.Services.AddJwt<JwtHandler>(enableGlobalAuthorize: false);

builder.Services.AddControllersWithViews()
    // Register multiple languages.
    .AddAppLocalization();

builder.Services.AddDataValidation();

// Request the log interception middleware.
builder.Services.AddMvcFilter<RequestActionFilter>();

// Restful Return style normalization.
builder.Services.AddInjectWithUnifyResult<XnRestfulResultProvider>();

builder.Services.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.AddDateFormatString("yyyy-MM-dd HH:mm:ss");
    // Configuring too long integer types to return to the front end can cause a loss of precision.
    options.JsonSerializerOptions.Converters.Add(new LongJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new DecimalJsonConverter());
    // Ignore circular references only .NET 6 support.
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    // JSON garbled code problem.
    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
});

builder.Services.AddViewEngine();

// Init sqlSugar.
builder.Services.SqlSugarClientConfigure();

// Add Instant Messaging.
builder.Services.AddSignalR();

// Add Snowflakes Id.
builder.Services.AddSnowflakeId();

// Sign up for EventBus.
builder.Services.AddEventBus(eventBuilder =>
{
    // Register as a Log subscriber.
    eventBuilder.AddSubscriber<LogEventSubscriber>();
});

#region Logging, error level logging, create a log file every day.

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddFile("logs/error/{0:yyyy-MM-dd}_.log", options => { SetLogOptions(options, LogLevel.Error); });
    // Environments other than the development environment are not logged.
    if (!App.HostEnvironment.IsDevelopment())
        return;
    loggingBuilder.AddFile("logs/info/{0:yyyy-MM-dd}_.log", options => { SetLogOptions(options, LogLevel.Information); });
    loggingBuilder.AddFile("logs/warn/{0:yyyy-MM-dd}_.log", options => { SetLogOptions(options, LogLevel.Warning); });
});

void SetLogOptions(FileLoggerOptions options, LogLevel logLevel)
{
    options.WriteFilter = logMsg => logMsg.LogLevel == logLevel;
    options.FileNameRule = fileName => string.Format(fileName, DateTime.UtcNow);
    options.FileSizeLimitBytes = 10 * 1024;
    options.MessageFormat = logMsg =>
    {
        var msg = new List<string>
        {
            $"##日志时间## {DateTime.Now:yyyy-MM-dd HH:mm:ss}", $"##日志等级## {logLevel}", $"##日志内容## {logMsg.Message}",
        };
        if (!string.IsNullOrEmpty(logMsg.Exception?.ToString()))
            msg.Add($"##异常信息## {logMsg.Exception}");

        // Generating template strings.
        var template = TP.Wrapper($"{logMsg.LogName}", "", msg.ToArray());
        return template;
    };
}

#endregion

var app = builder.Build();

// Add the status code interception middleware.
app.UseUnifyResultStatusCodes();

// Enable compression.
app.UseResponseCompression();

// Mandatory Https.
app.UseHttpsRedirection();

// Multilingual configuration must be performed before route registration.
app.UseAppLocalization();

app.UseStaticFiles();

app.UseRouting();

// Add cross-domain middleware.
app.UseCorsAccessor();

app.UseAuthentication();

app.UseAuthorization();

// Here, the default address is/API if no argument is entered, and/directory if string.empty is entered. If any string is entered, the/arbitrary string directory.
app.UseInject(string.Empty);

//app.UseEndpoints(endpoints =>
//{
//    // Register the hub.
//    endpoints.MapHub<ChatHub>("/hubs/chathub");
//    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
//});

app.MapControllers();

// It is recommended to disable the initialization of the database except for the first time.
Task.Run(async () => { await App.GetService<IInitDataBaseService>().InitDataBase(); });

app.Run();