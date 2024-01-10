using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using Fast.IaaS;
using Fast.NET.Core.Extensions;

var builder = WebApplication.CreateBuilder(args).Initialize();

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

app.MapControllers();

app.Run();