using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace UAParser
{
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
      Parser.Config config = new Parser.Config(options ?? new ParserOptions());
      this._userAgentParser = Parser.CreateParser<UserAgent>(Parser.Read<Func<string, UserAgent>>(yamlParser.ReadMapping("user_agent_parsers"), new Func<Func<string, string>, Func<string, UserAgent>>(config.UserAgentSelector)), new UserAgent(nameof (Other), (string) null, (string) null, (string) null));
      this._osParser = Parser.CreateParser<OS>(Parser.Read<Func<string, OS>>(yamlParser.ReadMapping("os_parsers"), new Func<Func<string, string>, Func<string, OS>>(config.OSSelector)), new OS(nameof (Other), (string) null, (string) null, (string) null, (string) null));
      this._deviceParser = Parser.CreateParser<Device>(Parser.Read<Func<string, Device>>(yamlParser.ReadMapping("device_parsers"), new Func<Func<string, string>, Func<string, Device>>(config.DeviceSelector)), new Device(nameof (Other), string.Empty, string.Empty));
    }

    private static IEnumerable<T> Read<T>(
      IEnumerable<Dictionary<string, string>> entries,
      Func<Func<string, string>, T> selector)
    {
      return entries.Select<Dictionary<string, string>, T>((Func<Dictionary<string, string>, T>) (cm => selector(new Func<string, string>(((DictionaryExtensions) cm).Find<string, string>))));
    }

    /// <summary>
    /// Returns a <see cref="T:UAParser.Parser" /> instance based on the regex definitions in a yaml string
    /// </summary>
    /// <param name="yaml">a string containing yaml definitions of reg-ex</param>
    /// <param name="parserOptions">specifies the options for the parser</param>
    /// <returns>A <see cref="T:UAParser.Parser" /> instance parsing user agent strings based on the regexes defined in the yaml string</returns>
    public static Parser FromYaml(string yaml, ParserOptions parserOptions = null) => new Parser(new MinimalYamlParser(yaml), parserOptions);

    /// <summary>
    /// Returns a <see cref="T:UAParser.Parser" /> instance based on the embedded regex definitions.
    /// <remarks>The embedded regex definitions may be outdated. Consider passing in external yaml definitions using <see cref="M:UAParser.Parser.FromYaml(System.String,UAParser.ParserOptions)" /></remarks>
    /// </summary>
    /// <param name="parserOptions">specifies the options for the parser</param>
    /// <returns></returns>
    public static Parser GetDefault(ParserOptions parserOptions = null)
    {
      using (Stream manifestResourceStream = ((Type) IntrospectionExtensions.GetTypeInfo(typeof (Parser))).Assembly.GetManifestResourceStream("UAParser.regexes.yaml"))
      {
        using (StreamReader streamReader = new StreamReader(manifestResourceStream))
          return new Parser(new MinimalYamlParser(((TextReader) streamReader).ReadToEnd()), parserOptions);
      }
    }

    /// <summary>
    /// Parse a user agent string and obtain all client information
    /// </summary>
    public ClientInfo Parse(string uaString)
    {
      OS os = this.ParseOS(uaString);
      Device device = this.ParseDevice(uaString);
      UserAgent userAgent = this.ParseUserAgent(uaString);
      return new ClientInfo(uaString, os, device, userAgent);
    }

    /// <summary>
    /// Parse a user agent string and obtain the OS information
    /// </summary>
    public OS ParseOS(string uaString) => this._osParser(uaString);

    /// <summary>
    /// Parse a user agent string and obtain the device information
    /// </summary>
    public Device ParseDevice(string uaString) => this._deviceParser(uaString);

    /// <summary>
    /// Parse a user agent string and obtain the UserAgent information
    /// </summary>
    public UserAgent ParseUserAgent(string uaString) => this._userAgentParser(uaString);

    private static Func<string, T> CreateParser<T>(
      IEnumerable<Func<string, T>> parsers,
      T defaultValue)
      where T : class
    {
      return Parser.CreateParser<T, T>(parsers, defaultValue, (Func<T, T>) (t => t));
    }

    private static Func<string, TResult> CreateParser<T, TResult>(
      IEnumerable<Func<string, T>> parsers,
      T defaultValue,
      Func<T, TResult> selector)
      where T : class
    {
      IEnumerable<Func<string, T>> source = parsers;
      parsers = (source != null ? (IEnumerable<Func<string, T>>) source.ToArray<Func<string, T>>() : (IEnumerable<Func<string, T>>) (Func<string, T>[]) null) ?? Enumerable.Empty<Func<string, T>>();
      return (Func<string, TResult>) (ua => selector(parsers.Select<Func<string, T>, T>((Func<Func<string, T>, T>) (p => p(ua))).FirstOrDefault<T>((Func<T, bool>) (m => (object) m != null)) ?? defaultValue));
    }

    private class Config
    {
      private readonly ParserOptions _options;

      internal Config(ParserOptions options) => this._options = options;

      public Func<string, OS> OSSelector(Func<string, string> indexer)
      {
        Regex regex = this.Regex(indexer, "OS");
        string str1 = indexer("os_replacement");
        string str2 = indexer("os_v1_replacement");
        string str3 = indexer("os_v2_replacement");
        string str4 = indexer("os_v3_replacement");
        string str5 = indexer("os_v4_replacement");
        string osReplacement = str1;
        string v1Replacement = str2;
        string v2Replacement = str3;
        string v3Replacement = str4;
        string v4Replacement = str5;
        return Parser.Parsers.OS(regex, osReplacement, v1Replacement, v2Replacement, v3Replacement, v4Replacement);
      }

      public Func<string, UserAgent> UserAgentSelector(Func<string, string> indexer)
      {
        Regex regex = this.Regex(indexer, "User agent");
        string str1 = indexer("family_replacement");
        string str2 = indexer("v1_replacement");
        string str3 = indexer("v2_replacement");
        string str4 = indexer("v3_replacement");
        string familyReplacement = str1;
        string majorReplacement = str2;
        string minorReplacement = str3;
        string patchReplacement = str4;
        return Parser.Parsers.UserAgent(regex, familyReplacement, majorReplacement, minorReplacement, patchReplacement);
      }

      public Func<string, Device> DeviceSelector(Func<string, string> indexer)
      {
        Regex regex = this.Regex(indexer, "Device", indexer("regex_flag"));
        string str1 = indexer("device_replacement");
        string str2 = indexer("brand_replacement");
        string str3 = indexer("model_replacement");
        string familyReplacement = str1;
        string brandReplacement = str2;
        string modelReplacement = str3;
        return Parser.Parsers.Device(regex, familyReplacement, brandReplacement, modelReplacement);
      }

      private Regex Regex(Func<string, string> indexer, string key, string regexFlag = null)
      {
        string pattern = indexer("regex");
        if (pattern == null)
          throw new Exception(key + " is missing regular expression specification.");
        if (pattern.IndexOf("\\_", StringComparison.Ordinal) >= 0)
          pattern = pattern.Replace("\\_", "_");
        RegexOptions options = RegexOptions.Singleline | RegexOptions.CultureInvariant;
        if ("i".Equals(regexFlag))
          options |= RegexOptions.IgnoreCase;
        if (this._options.UseCompiledRegex)
          options |= RegexOptions.Compiled;
        return new Regex(pattern, options, this._options.MatchTimeOut);
      }
    }

    private static class Parsers
    {
      private static readonly string[] _allReplacementTokens = new string[9]
      {
        "$1",
        "$2",
        "$3",
        "$4",
        "$5",
        "$6",
        "$7",
        "$8",
        "$91"
      };

      public static Func<string, OS> OS(
        Regex regex,
        string osReplacement,
        string v1Replacement,
        string v2Replacement,
        string v3Replacement,
        string v4Replacement)
      {
        if (!(v1Replacement == "$1"))
          return Parser.Parsers.Create<OS>(regex, Parser.Parsers.Replace(osReplacement, "$1").SelectMany((Func<string, Func<Match, IEnumerator<int>, string>>) (family => Parser.Parsers.Replace(v1Replacement, "$2")), (family, v1) => new
          {
            family = family,
            v1 = v1
          }).SelectMany(_param1 => Parser.Parsers.Replace(v2Replacement, "$3"), (_param1, v2) => new
          {
            Eh__TransparentIdentifier0 = _param1,
            v2 = v2
          }).SelectMany(_param1 => Parser.Parsers.Replace(v3Replacement, "$4"), (_param1, v3) => new
          {
            Eh__TransparentIdentifier1 = _param1,
            v3 = v3
          }).SelectMany(_param1 => Parser.Parsers.Replace(v4Replacement, "$5"), (_param1, v4) => new OS(_param1.Eh__TransparentIdentifier1.Eh__TransparentIdentifier0.family, _param1.Eh__TransparentIdentifier1.Eh__TransparentIdentifier0.v1, _param1.Eh__TransparentIdentifier1.v2, _param1.v3, v4)));
        return v2Replacement == "$2" ? Parser.Parsers.Create<OS>(regex, Parser.Parsers.Replace(v1Replacement, "$1").SelectMany((Func<string, Func<Match, IEnumerator<int>, string>>) (v1 => Parser.Parsers.Replace(v2Replacement, "$2")), (v1, v2) => new
        {
          v1 = v1,
          v2 = v2
        }).SelectMany(_param1 => Parser.Parsers.Replace(v3Replacement, "$3"), (_param1, v3) => new
        {
          Eh__TransparentIdentifier0 = _param1,
          v3 = v3
        }).SelectMany(_param1 => Parser.Parsers.Replace(v4Replacement, "$4"), (_param1, v4) => new
        {
          Eh__TransparentIdentifier1 = _param1,
          v4 = v4
        }).SelectMany(_param1 => Parser.Parsers.Replace(osReplacement, "$5"), (_param1, family) => new OS(family, _param1.Eh__TransparentIdentifier1.Eh__TransparentIdentifier0.v1, _param1.Eh__TransparentIdentifier1.Eh__TransparentIdentifier0.v2, _param1.Eh__TransparentIdentifier1.v3, _param1.v4))) : Parser.Parsers.Create<OS>(regex, Parser.Parsers.Replace(v1Replacement, "$1").SelectMany((Func<string, Func<Match, IEnumerator<int>, string>>) (v1 => Parser.Parsers.Replace(osReplacement, "$2")), (v1, family) => new
        {
          v1 = v1,
          family = family
        }).SelectMany(_param1 => Parser.Parsers.Replace(v2Replacement, "$3"), (_param1, v2) => new
        {
          Eh__TransparentIdentifier0 = _param1,
          v2 = v2
        }).SelectMany(_param1 => Parser.Parsers.Replace(v3Replacement, "$4"), (_param1, v3) => new
        {
          Eh__TransparentIdentifier1 = _param1,
          v3 = v3
        }).SelectMany(_param1 => Parser.Parsers.Replace(v4Replacement, "$5"), (_param1, v4) => new OS(_param1.Eh__TransparentIdentifier1.Eh__TransparentIdentifier0.family, _param1.Eh__TransparentIdentifier1.Eh__TransparentIdentifier0.v1, _param1.Eh__TransparentIdentifier1.v2, _param1.v3, v4)));
      }

      public static Func<string, Device> Device(
        Regex regex,
        string familyReplacement,
        string brandReplacement,
        string modelReplacement)
      {
        return Parser.Parsers.Create<Device>(regex, Parser.Parsers.ReplaceAll(familyReplacement).SelectMany((Func<string, Func<Match, IEnumerator<int>, string>>) (family => Parser.Parsers.ReplaceAll(brandReplacement)), (family, brand) => new
        {
          family = family,
          brand = brand
        }).SelectMany(_param1 => Parser.Parsers.ReplaceAll(modelReplacement), (_param1, model) => new Device(_param1.family, _param1.brand, model)));
      }

      public static Func<string, UserAgent> UserAgent(
        Regex regex,
        string familyReplacement,
        string majorReplacement,
        string minorReplacement,
        string patchReplacement)
      {
        return Parser.Parsers.Create<UserAgent>(regex, Parser.Parsers.Replace(familyReplacement, "$1").SelectMany((Func<string, Func<Match, IEnumerator<int>, string>>) (family => Parser.Parsers.Replace(majorReplacement, "$2")), (family, v1) => new
        {
          family = family,
          v1 = v1
        }).SelectMany(_param1 => Parser.Parsers.Replace(minorReplacement, "$3"), (_param1, v2) => new
        {
          Eh__TransparentIdentifier0 = _param1,
          v2 = v2
        }).SelectMany(_param1 => Parser.Parsers.Replace(patchReplacement, "$4"), (_param1, v3) => new UserAgent(_param1.Eh__TransparentIdentifier0.family, _param1.Eh__TransparentIdentifier0.v1, _param1.v2, v3)));
      }

      private static Func<Match, IEnumerator<int>, string> Replace(string replacement) => replacement == null ? Parser.Parsers.Select() : Parser.Parsers.Select<string>((Func<string, string>) (_ => replacement));

      private static Func<Match, IEnumerator<int>, string> Replace(string replacement, string token) => replacement == null || !replacement.Contains(token) ? Parser.Parsers.Replace(replacement) : Parser.Parsers.Select<string>((Func<string, string>) (s => s == null ? replacement : replacement.ReplaceFirstOccurence(token, s)));

      private static Func<Match, IEnumerator<int>, string> ReplaceAll(string replacement)
      {
        return replacement == null ? Parser.Parsers.Select() : (Func<Match, IEnumerator<int>, string>) ((m, num) =>
        {
          string replacementString = replacement;
          if (replacementString.Contains("$"))
          {
            GroupCollection groups = m.Groups;
            for (int index = 0; index < Parser.Parsers._allReplacementTokens.Length; ++index)
            {
              int groupnum = index + 1;
              string replacementToken = Parser.Parsers._allReplacementTokens[index];
              if (replacementString.Contains(replacementToken))
              {
                string empty = string.Empty;
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
        });

        static string ReplaceFunction(string replacementString, string matchedGroup, string token) => matchedGroup == null ? replacementString : replacementString.ReplaceFirstOccurence(token, matchedGroup);
      }

      private static Func<Match, IEnumerator<int>, string> Select() => Parser.Parsers.Select<string>((Func<string, string>) (v => v));

      private static Func<Match, IEnumerator<int>, T> Select<T>(Func<string, T> selector) => (Func<Match, IEnumerator<int>, T>) ((m, num) =>
      {
        if (!num.MoveNext())
          throw new InvalidOperationException();
        GroupCollection groups = m.Groups;
        Group group;
        return selector(num.Current > groups.Count || !(group = groups[num.Current]).Success ? (string) null : group.Value);
      });

      private static Func<string, T> Create<T>(Regex regex, Func<Match, IEnumerator<int>, T> binder) => (Func<string, T>) (input =>
      {
        try
        {
          Match match = regex.Match(input);
          IEnumerator<int> enumerator = Parser.Parsers.Generate<int>(1, (Func<int, int>) (n => n + 1));
          return match.Success ? binder(match, enumerator) : default (T);
        }
        catch (RegexMatchTimeoutException ex)
        {
          return default (T);
        }
      });

      private static IEnumerator<T> Generate<T>(T initial, Func<T, T> next)
      {
        T state = initial;
        while (true)
        {
          yield return state;
          state = next(state);
        }
      }
    }
  }
}
