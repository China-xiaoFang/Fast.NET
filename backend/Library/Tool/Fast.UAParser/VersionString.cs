using System;
using System.Collections.Generic;
using System.Linq;

namespace UAParser
{
    internal static class VersionString
    {
        public static string Format(params string[] parts) => string.Join(".", ((IEnumerable<string>) parts).Where<string>((Func<string, bool>) (v => !string.IsNullOrEmpty(v))).ToArray<string>());
    }
}