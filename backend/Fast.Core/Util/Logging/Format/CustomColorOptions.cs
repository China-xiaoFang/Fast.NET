#nullable enable
using Microsoft.Extensions.Logging.Console;

namespace Fast.Core.Util.Logging.Format;

public class CustomColorOptions : SimpleConsoleFormatterOptions
{
    public string? CustomPrefix { get; set; }
}