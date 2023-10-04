using Fast.Cache.Extensions;
using Fast.Core.Extensions;
using Fast.CorsAccessor.Extensions;
using Fast.DataValidation.Extensions;
using Fast.DynamicApplication.Extensions;
using Fast.Exception.Extensions;
using Fast.Logging.Extensions;
using Fast.SpecificationDocument.Extensions;
using Fast.Test.Api;
using Fast.UnifyResult.Extensions;

var builder = WebApplication.CreateBuilder(args).Initialize();

// Customize the console log output template.
builder.Logging.AddConsoleFormatter(options => { options.DateFormat = "yyyy-MM-dd HH:mm:ss"; });

builder.Services.AddLogging();

builder.Services.AddControllers();

builder.Services.AddDynamicApiControllers();

builder.Services.AddDataValidation();

builder.Services.AddFriendlyException();

builder.Services.AddUnifyResult<RESTfulResultProvider>();

builder.Services.AddSpecificationDocuments();

builder.Services.AddCache();

//builder.Services.AddSqlSugar();

// Add event bus.
//builder.Services.AddEventBus(options =>
//{
//    // 创建连接工厂
//    var factory = App.GetConfig<ConnectionFactory>("RabbitMQConnection");

//    // 创建默认内存通道事件源对象
//    var mqEventSourceStorer = new RabbitMQEventSourceStorer(factory, "WMS.Event.Bus", 3000);

//    // 替换默认事件总线存储器
//    options.ReplaceStorer(serviceProvider => mqEventSourceStorer);

//    // 注册事件总线重试服务
//    options.AddFallbackPolicy<EventFallbackPolicy>();
//});
//builder.Services.AddEventBus();

var app = builder.Build();

// Mandatory Https.
app.UseHttpsRedirection();

// Enable compression.
//app.UseResponseCompression();

// Add the status code interception middleware.
app.UseUnifyResultStatusCodes();

// Add cross-domain middleware.
app.UseCorsAccessor();

app.UseStaticFiles();

app.UseRouting();

// Here, the default address is/API if no argument is entered, and/directory if string.empty is entered. If any string is entered, the/arbitrary string directory.
app.UseSpecificationDocuments();

app.MapControllers();

app.Run();