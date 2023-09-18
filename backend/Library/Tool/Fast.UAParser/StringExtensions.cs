using System;

namespace Fast.UAParser;

internal static class StringExtensions
{
    public static string ReplaceFirstOccurence(this string input, string search, string replacement)
    {
        var length = input != null
            ? input.IndexOf(search, StringComparison.Ordinal)
            : throw new ArgumentNullException(nameof(input));
        return length < 0 ? input : input.Substring(0, length) + replacement + input.Substring(length + search.Length);
    }
}