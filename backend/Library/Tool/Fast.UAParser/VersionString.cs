using System;
using System.Linq;

namespace Fast.UAParser;

internal static class VersionString
{
    public static string Format(params string[] parts) =>
        string.Join(".", parts.Where((Func<string, bool>) (v => !string.IsNullOrEmpty(v))).ToArray());
}