using Fast.Cache.Extensions;
using Fast.Core;
using Fast.Core.Extensions;
using Fast.CorsAccessor.Extensions;
using Fast.DataValidation.Extensions;
using Fast.DynamicApplication.Extensions;
using Fast.EventBus.Extensions;
using Fast.Exception.Extensions;
using Fast.Logging.Extensions;
using Fast.Sugar.Extensions;
using Fast.Test.Api.EventSubscriber;
using Fast.UnifyResult.Extensions;
using Furion.UnifyResult;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args).Initialize();

// Customize the console log output template.
builder.Logging.AddConsoleFormatter(options => { options.DateFormat = "yyyy-MM-dd HH:mm:ss"; });

builder.Services.AddLogging();

builder.Services.AddControllers();

//builder.Services.AddSpecificationDocuments();

builder.Services.AddDynamicApiControllers();

builder.Services.AddDataValidation();

builder.Services.AddFriendlyException();

builder.Services.AddUnifyResult<RESTfulResultProvider>();

builder.Services.AddCache();

//builder.Services.AddSqlSugar();

// Add event bus.
builder.Services.AddEventBus(options =>
{
    // �������ӹ���
    var factory = App.GetConfig<ConnectionFactory>("RabbitMQConnection");

    // ����Ĭ���ڴ�ͨ���¼�Դ����
    var mqEventSourceStorer = new RabbitMQEventSourceStorer(factory, "WMS.Event.Bus", 3000);

    // �滻Ĭ���¼����ߴ洢��
    options.ReplaceStorer(serviceProvider => mqEventSourceStorer);

    // ע���¼��������Է���
    options.AddFallbackPolicy<EventFallbackPolicy>();
});
//builder.Services.AddEventBus();

builder.Services.AddSwaggerGen();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.MapControllers();

app.Run();