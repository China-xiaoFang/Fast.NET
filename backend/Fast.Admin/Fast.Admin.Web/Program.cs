using Fast.IaaS;
using Fast.NET.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

// ³õÊ¼»¯ ¿ò¼Ü
builder.Initialize();

// Add controller.
builder.AddControllers();

var app = builder.Build();

// Mandatory Https.
app.UseHttpsRedirection();

app.UseStaticFiles();

// Enable backward reading.
app.EnableBuffering();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//app.UseKnife4UI(options =>
//{
//    options.RoutePrefix = "knife4j";
//    foreach (var groupInfo in SwaggerDocumentBuilder.GetOpenApiGroups())
//    {
//        options.SwaggerEndpoint("/" + groupInfo.RouteTemplate, groupInfo.Title);
//    }
//});

app.MapControllers();

app.Run();