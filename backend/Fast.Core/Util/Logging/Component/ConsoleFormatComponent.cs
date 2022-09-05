using Fast.Core.Util.Logging.Format;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Fast.Core.Util.Logging.Component;

public sealed class ConsoleFormatComponent : IWebComponent
{
    public void Load(WebApplicationBuilder builder, ComponentContext componentContext)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole(options => { options.FormatterName = "custom_format"; })
            .AddConsoleFormatter<ConsoleFormat, ConsoleFormatterOptions>();
    }
}