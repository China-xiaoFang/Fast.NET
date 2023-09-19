using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Fast.UAParser;

/// <summary>Represents a parser of a user agent string</summary>
public sealed class Parser
{
    /// <summary>
    /// The constant string value used to signal an unknown match for a given property or value. This
    /// is by default the string "Other".
    /// </summary>
    public const string Other = "Other";

    private readonly Func<string, OS> _osParser;
    private readonly Func<string, Device> _deviceParser;
    private readonly Func<string, UserAgent> _userAgentParser;

    private Parser(MinimalYamlParser yamlParser, ParserOptions options)
    {
        var config = new Config(options ?? new ParserOptions());
        _userAgentParser = CreateParser(Read(yamlParser.ReadMapping("user_agent_parsers"), config.UserAgentSelector),
            new UserAgent(nameof(Other), null, null, null));
        _osParser = CreateParser(Read(yamlParser.ReadMapping("os_parsers"), config.OSSelector),
            new OS(nameof(Other), null, null, null, null));
        _deviceParser = CreateParser(Read(yamlParser.ReadMapping("device_parsers"), config.DeviceSelector),
            new Device(nameof(Other), string.Empty, string.Empty));
    }

    private static IEnumerable<T> Read<T>(IEnumerable<Dictionary<string, string>> entries, Func<Func<string, string>, T> selector)
    {
        return entries.Select((Func<Dictionary<string, string>, T>) (cm => selector(cm.Find)));
    }

    /// <summary>
    /// Returns a <see cref="T:Fast.UAParser.Parser" /> instance based on the regex definitions in a yaml string
    /// </summary>
    /// <param name="yaml">a string containing yaml definitions of reg-ex</param>
    /// <param name="parserOptions">specifies the options for the parser</param>
    /// <returns>A <see cref="T:Fast.UAParser.Parser" /> instance parsing user agent strings based on the regexes defined in the yaml string</returns>
    public static Parser FromYaml(string yaml, ParserOptions parserOptions = null) =>
        new Parser(new MinimalYamlParser(yaml), parserOptions);

    /// <summary>
    /// Returns a <see cref="T:Fast.UAParser.Parser" /> instance based on the embedded regex definitions.
    /// <remarks>The embedded regex definitions may be outdated. Consider passing in external yaml definitions using <see cref="M:Fast.UAParser.Parser.FromYaml(System.String,Fast.UAParser.ParserOptions)" /></remarks>
    /// </summary>
    /// <param name="parserOptions">specifies the options for the parser</param>
    /// <returns></returns>
    public static Parser GetDefault(ParserOptions parserOptions = null)
    {
        using (var manifestResourceStream =
               typeof(Parser).GetTypeInfo().Assembly.GetManifestResourceStream("UAParser.regexes.yaml"))
        {
            using (var streamReader = new StreamReader(manifestResourceStream))
                return new Parser(new MinimalYamlParser(streamReader.ReadToEnd()), parserOptions);
        }
    }

    /// <summary>
    /// Parse a user agent string and obtain all client information
    /// </summary>
    public ClientInfo Parse(string uaString)
    {
        var os = ParseOS(uaString);
        var device = ParseDevice(uaString);
        var userAgent = ParseUserAgent(uaString);
        return new ClientInfo(uaString, os, device, userAgent);
    }

    /// <summary>
    /// Parse a user agent string and obtain the OS information
    /// </summary>
    public OS ParseOS(string uaString) => _osParser(uaString);

    /// <summary>
    /// Parse a user agent string and obtain the device information
    /// </summary>
    public Device ParseDevice(string uaString) => _deviceParser(uaString);

    /// <summary>
    /// Parse a user agent string and obtain the UserAgent information
    /// </summary>
    public UserAgent ParseUserAgent(string uaString) => _userAgentParser(uaString);

    private static Func<string, T> CreateParser<T>(IEnumerable<Func<string, T>> parsers, T defaultValue) where T : class
    {
        return CreateParser(parsers, defaultValue, t => t);
    }

    private static Func<string, TResult> CreateParser<T, TResult>(IEnumerable<Func<string, T>> parsers, T defaultValue,
        Func<T, TResult> selector) where T : class
    {
        var source = parsers;
        parsers = (source != null ? source.ToArray() : (IEnumerable<Func<string, T>>) null) ??
                  Enumerable.Empty<Func<string, T>>();
        return ua => selector(parsers.Select((Func<Func<string, T>, T>) (p => p(ua)))
            .FirstOrDefault((Func<T, bool>) (m => m != null)) ?? defaultValue);
    }

    private class Config
    {
        private readonly ParserOptions _options;

        internal Config(ParserOptions options) => _options = options;

        public Func<string, OS> OSSelector(Func<string, string> indexer)
        {
            var regex = Regex(indexer, "OS");
            var str1 = indexer("os_replacement");
            var str2 = indexer("os_v1_replacement");
            var str3 = indexer("os_v2_replacement");
            var str4 = indexer("os_v3_replacement");
            var str5 = indexer("os_v4_replacement");
            var osReplacement = str1;
            var v1Replacement = str2;
            var v2Replacement = str3;
            var v3Replacement = str4;
            var v4Replacement = str5;
            return Parsers.OS(regex, osReplacement, v1Replacement, v2Replacement, v3Replacement, v4Replacement);
        }

        public Func<string, UserAgent> UserAgentSelector(Func<string, string> indexer)
        {
            var regex = Regex(indexer, "User agent");
            var str1 = indexer("family_replacement");
            var str2 = indexer("v1_replacement");
            var str3 = indexer("v2_replacement");
            var str4 = indexer("v3_replacement");
            var familyReplacement = str1;
            var majorReplacement = str2;
            var minorReplacement = str3;
            var patchReplacement = str4;
            return Parsers.UserAgent(regex, familyReplacement, majorReplacement, minorReplacement, patchReplacement);
        }

        public Func<string, Device> DeviceSelector(Func<string, string> indexer)
        {
            var regex = Regex(indexer, "Device", indexer("regex_flag"));
            var str1 = indexer("device_replacement");
            var str2 = indexer("brand_replacement");
            var str3 = indexer("model_replacement");
            var familyReplacement = str1;
            var brandReplacement = str2;
            var modelReplacement = str3;
            return Parsers.Device(regex, familyReplacement, brandReplacement, modelReplacement);
        }

        private Regex Regex(Func<string, string> indexer, string key, string regexFlag = null)
        {
            var pattern = indexer("regex");
            if (pattern == null)
                throw new Exception(key + " is missing regular expression specification.");
            if (pattern.IndexOf("\\_", StringComparison.Ordinal) >= 0)
                pattern = pattern.Replace("\\_", "_");
            var options = RegexOptions.Singleline | RegexOptions.CultureInvariant;
            if ("i".Equals(regexFlag))
                options |= RegexOptions.IgnoreCase;
            if (_options.UseCompiledRegex)
                options |= RegexOptions.Compiled;
            return new Regex(pattern, options, _options.MatchTimeOut);
        }
    }

    private static class Parsers
    {
        private static readonly string[] _allReplacementTokens = new string[9]
        {
            "$1", "$2", "$3", "$4", "$5", "$6", "$7", "$8", "$91"
        };

        public static Func<string, OS> OS(Regex regex, string osReplacement, string v1Replacement, string v2Replacement,
            string v3Replacement, string v4Replacement)
        {
            if (!(v1Replacement == "$1"))
                return Create(regex,
                    Replace(osReplacement, "$1")
                        .SelectMany(family => Replace(v1Replacement, "$2"), (family, v1) => new {family, v1})
                        .SelectMany(_param1 => Replace(v2Replacement, "$3"),
                            (_param1, v2) => new {Eh__TransparentIdentifier0 = _param1, v2})
                        .SelectMany(_param1 => Replace(v3Replacement, "$4"),
                            (_param1, v3) => new {Eh__TransparentIdentifier1 = _param1, v3}).SelectMany(
                            _param1 => Replace(v4Replacement, "$5"),
                            (_param1, v4) => new OS(_param1.Eh__TransparentIdentifier1.Eh__TransparentIdentifier0.family,
                                _param1.Eh__TransparentIdentifier1.Eh__TransparentIdentifier0.v1,
                                _param1.Eh__TransparentIdentifier1.v2, _param1.v3, v4)));
            return v2Replacement == "$2"
                ? Create(regex,
                    Replace(v1Replacement, "$1").SelectMany(v1 => Replace(v2Replacement, "$2"), (v1, v2) => new {v1, v2})
                        .SelectMany(_param1 => Replace(v3Replacement, "$3"),
                            (_param1, v3) => new {Eh__TransparentIdentifier0 = _param1, v3})
                        .SelectMany(_param1 => Replace(v4Replacement, "$4"),
                            (_param1, v4) => new {Eh__TransparentIdentifier1 = _param1, v4})
                        .SelectMany(_param1 => Replace(osReplacement, "$5"),
                            (_param1, family) => new OS(family, _param1.Eh__TransparentIdentifier1.Eh__TransparentIdentifier0.v1,
                                _param1.Eh__TransparentIdentifier1.Eh__TransparentIdentifier0.v2,
                                _param1.Eh__TransparentIdentifier1.v3, _param1.v4)))
                : Create(regex,
                    Replace(v1Replacement, "$1").SelectMany(v1 => Replace(osReplacement, "$2"), (v1, family) => new {v1, family})
                        .SelectMany(_param1 => Replace(v2Replacement, "$3"),
                            (_param1, v2) => new {Eh__TransparentIdentifier0 = _param1, v2})
                        .SelectMany(_param1 => Replace(v3Replacement, "$4"),
                            (_param1, v3) => new {Eh__TransparentIdentifier1 = _param1, v3}).SelectMany(
                            _param1 => Replace(v4Replacement, "$5"),
                            (_param1, v4) => new OS(_param1.Eh__TransparentIdentifier1.Eh__TransparentIdentifier0.family,
                                _param1.Eh__TransparentIdentifier1.Eh__TransparentIdentifier0.v1,
                                _param1.Eh__TransparentIdentifier1.v2, _param1.v3, v4)));
        }

        public static Func<string, Device> Device(Regex regex, string familyReplacement, string brandReplacement,
            string modelReplacement)
        {
            return Create(regex,
                ReplaceAll(familyReplacement)
                    .SelectMany(family => ReplaceAll(brandReplacement), (family, brand) => new {family, brand})
                    .SelectMany(_param1 => ReplaceAll(modelReplacement),
                        (_param1, model) => new Device(_param1.family, _param1.brand, model)));
        }

        public static Func<string, UserAgent> UserAgent(Regex regex, string familyReplacement, string majorReplacement,
            string minorReplacement, string patchReplacement)
        {
            return Create(regex,
                Replace(familyReplacement, "$1")
                    .SelectMany(family => Replace(majorReplacement, "$2"), (family, v1) => new {family, v1})
                    .SelectMany(_param1 => Replace(minorReplacement, "$3"),
                        (_param1, v2) => new {Eh__TransparentIdentifier0 = _param1, v2}).SelectMany(
                        _param1 => Replace(patchReplacement, "$4"),
                        (_param1, v3) => new UserAgent(_param1.Eh__TransparentIdentifier0.family,
                            _param1.Eh__TransparentIdentifier0.v1, _param1.v2, v3)));
        }

        private static Func<Match, IEnumerator<int>, string> Replace(string replacement) =>
            replacement == null ? Select() : Select(_ => replacement);

        private static Func<Match, IEnumerator<int>, string> Replace(string replacement, string token) =>
            replacement == null || !replacement.Contains(token)
                ? Replace(replacement)
                : Select(s => s == null ? replacement : replacement.ReplaceFirstOccurence(token, s));

        private static Func<Match, IEnumerator<int>, string> ReplaceAll(string replacement)
        {
            return replacement == null
                ? Select()
                : (m, num) =>
                {
                    var replacementString = replacement;
                    if (replacementString.Contains("$"))
                    {
                        var groups = m.Groups;
                        for (var index = 0; index < _allReplacementTokens.Length; ++index)
                        {
                            var groupnum = index + 1;
                            var replacementToken = _allReplacementTokens[index];
                            if (replacementString.Contains(replacementToken))
                            {
                                var empty = string.Empty;
                                Group group;
                                if (groupnum <= groups.Count && (group = groups[groupnum]).Success)
                                    empty = group.Value;
                                replacementString = ReplaceFunction(replacementString, empty, replacementToken);
                            }

                            if (!replacementString.Contains("$"))
                                break;
                        }
                    }

                    return replacementString;
                };

            static string ReplaceFunction(string replacementString, string matchedGroup, string token) =>
                matchedGroup == null ? replacementString : replacementString.ReplaceFirstOccurence(token, matchedGroup);
        }

        private static Func<Match, IEnumerator<int>, string> Select() => Select(v => v);

        private static Func<Match, IEnumerator<int>, T> Select<T>(Func<string, T> selector) =>
            (m, num) =>
            {
                if (!num.MoveNext())
                    throw new InvalidOperationException();
                var groups = m.Groups;
                Group group;
                return selector(num.Current > groups.Count || !(group = groups[num.Current]).Success ? null : group.Value);
            };

        private static Func<string, T> Create<T>(Regex regex, Func<Match, IEnumerator<int>, T> binder) =>
            input =>
            {
                try
                {
                    var match = regex.Match(input);
                    var enumerator = Generate(1, n => n + 1);
                    return match.Success ? binder(match, enumerator) : default(T);
                }
                catch (RegexMatchTimeoutException ex)
                {
                    return default(T);
                }
            };

        private static IEnumerator<T> Generate<T>(T initial, Func<T, T> next)
        {
            var state = initial;
            while (true)
            {
                yield return state;
                state = next(state);
            }
        }
    }
}