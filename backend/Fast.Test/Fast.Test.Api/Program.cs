using Fast.NET.Core.Extensions;
using Fast.SpecificationProcessor.Swagger.Extensions;
using Fast.Test.Api;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

var builder = WebApplication.CreateBuilder(args).Initialize();

//// 日志
//builder.Services.AddLogging(builder.Configuration);

//// 跨域配置
//builder.Services.AddCorsAccessor(builder.Configuration);

//// GZIP 压缩
//builder.Services.AddGzipBrotliCompression();

//// JSON 序列化配置
//builder.Services.AddJsonOptions();

//// 注册全局依赖注入
//builder.Services.AddDependencyInjection();

//// 添加对象映射
//builder.Services.AddObjectMapper();

//builder.Services.AddJwt();

//builder.Services.AddSqlSugar(builder.Configuration);

var a1 = builder.Services.FirstOrDefault(f => f.ServiceType == typeof(ApplicationPartManager));

var a = builder.Services.FirstOrDefault(f => f.ServiceType == typeof(ApplicationPartManager))?.ImplementationInstance as
    ApplicationPartManager;

builder.Services.AddControllers();

var b1 = builder.Services.FirstOrDefault(f => f.ServiceType == typeof(ApplicationPartManager));

var b = builder.Services.FirstOrDefault(f => f.ServiceType == typeof(ApplicationPartManager))?.ImplementationInstance as
    ApplicationPartManager;

// 文档
builder.Services.AddSwaggerDocument(builder.Configuration);

//// 动态 API
//builder.Services.AddDynamicApplication();

//// 数据验证
//builder.Services.AddDataValidation();

//// 友好异常
//builder.Services.AddFriendlyException();

//// 规范返回
//builder.Services.AddUnifyResult<RESTfulResultProvider>();


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

//// 跨域中间件
//app.UseCorsAccessor();

// Enable compression.
//app.UseResponseCompression();

//// Add the status code interception middleware.
//app.UseUnifyResultStatusCodes();


app.UseStaticFiles();

app.UseRouting();

// Here, the default address is/API if no argument is entered, and/directory if string.empty is entered. If any string is entered, the/arbitrary string directory.
app.UseSwaggerDocument();

app.MapControllers();

app.Run();