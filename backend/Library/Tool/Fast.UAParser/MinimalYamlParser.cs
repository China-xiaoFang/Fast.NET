using System;
using System.Collections.Generic;

namespace UAParser
{
  /// <summary>
  /// Just enough string parsing to recognize the regexes.yaml file format. Introduced to remove
  /// dependency on large Yaml parsing lib. Note that a unittest ensures compatibility
  /// by ensuring regexes and properties are read similar to using the full yaml lib
  /// </summary>
  internal class MinimalYamlParser
  {
    private readonly Dictionary<string, MinimalYamlParser.Mapping> _mappings = new Dictionary<string, MinimalYamlParser.Mapping>();

    public MinimalYamlParser(string yamlString) => this.ReadIntoMappingModel(yamlString);

    internal IDictionary<string, MinimalYamlParser.Mapping> Mappings => (IDictionary<string, MinimalYamlParser.Mapping>) this._mappings;

    private void ReadIntoMappingModel(string yamlInputString)
    {
      string[] strArray = yamlInputString.Split(new string[4]
      {
        Environment.NewLine,
        "\r",
        "\n",
        "\r\n"
      }, StringSplitOptions.RemoveEmptyEntries);
      int num = 0;
      MinimalYamlParser.Mapping mapping = (MinimalYamlParser.Mapping) null;
      foreach (string str1 in strArray)
      {
        ++num;
        if (!str1.Trim().StartsWith("#") && str1.Trim().Length != 0)
        {
          if (str1[0] != ' ')
          {
            int length = str1.IndexOf(':');
            string key = length != -1 ? str1.Substring(0, length).Trim() : throw new ArgumentException("YamlParsing: Expecting mapping entry to contain a ':', at line " + (object) num);
            mapping = new MinimalYamlParser.Mapping();
            this._mappings.Add(key, mapping);
          }
          else
          {
            if (mapping == null)
              throw new ArgumentException("YamlParsing: Expecting mapping entry to contain a ':', at line " + (object) num);
            string str2 = str1.Trim();
            if (str2[0] == '-')
            {
              mapping.BeginSequence();
              str2 = str2.Substring(1);
            }
            int length = str2.IndexOf(':');
            string key = length != -1 ? str2.Substring(0, length).Trim() : throw new ArgumentException("YamlParsing: Expecting scalar mapping entry to contain a ':', at line " + (object) num);
            string str3 = MinimalYamlParser.ReadQuotedValue(str2.Substring(length + 1).Trim());
            mapping.AddToSequence(key, str3);
          }
        }
      }
    }

    private static string ReadQuotedValue(string value) => value.StartsWith("'") && value.EndsWith("'") || value.StartsWith("\"") && value.EndsWith("\"") ? value.Substring(1, value.Length - 2) : value;

    public IEnumerable<Dictionary<string, string>> ReadMapping(string mappingName)
    {
      MinimalYamlParser.Mapping mapping;
      if (this._mappings.TryGetValue(mappingName, out mapping))
      {
        foreach (Dictionary<string, string> sequence in mapping.Sequences)
          yield return sequence;
      }
    }

    internal class Mapping
    {
      private Dictionary<string, string> _lastEntry;

      public Mapping() => this.Sequences = new List<Dictionary<string, string>>();

      public List<Dictionary<string, string>> Sequences { get; }

      public void BeginSequence()
      {
        this._lastEntry = new Dictionary<string, string>();
        this.Sequences.Add(this._lastEntry);
      }

      public void AddToSequence(string key, string value) => this._lastEntry[key] = value;
    }
  }
}
