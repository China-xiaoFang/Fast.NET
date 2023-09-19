namespace Fast.UAParser;

/// <summary>
/// Just enough string parsing to recognize the regexes.yaml file format. Introduced to remove
/// dependency on large Yaml parsing lib. Note that a unittest ensures compatibility
/// by ensuring regexes and properties are read similar to using the full yaml lib
/// </summary>
internal class MinimalYamlParser
{
    private readonly Dictionary<string, Mapping> _mappings = new Dictionary<string, Mapping>();

    public MinimalYamlParser(string yamlString) => ReadIntoMappingModel(yamlString);

    internal IDictionary<string, Mapping> Mappings => _mappings;

    private void ReadIntoMappingModel(string yamlInputString)
    {
        var strArray = yamlInputString.Split(new string[4] {Environment.NewLine, "\r", "\n", "\r\n"},
            StringSplitOptions.RemoveEmptyEntries);
        var num = 0;
        Mapping mapping = null;
        foreach (var str1 in strArray)
        {
            ++num;
            if (!str1.Trim().StartsWith("#") && str1.Trim().Length != 0)
            {
                if (str1[0] != ' ')
                {
                    var length = str1.IndexOf(':');
                    var key = length != -1
                        ? str1.Substring(0, length).Trim()
                        : throw new ArgumentException("YamlParsing: Expecting mapping entry to contain a ':', at line " + num);
                    mapping = new Mapping();
                    _mappings.Add(key, mapping);
                }
                else
                {
                    if (mapping == null)
                        throw new ArgumentException("YamlParsing: Expecting mapping entry to contain a ':', at line " + num);
                    var str2 = str1.Trim();
                    if (str2[0] == '-')
                    {
                        mapping.BeginSequence();
                        str2 = str2.Substring(1);
                    }

                    var length = str2.IndexOf(':');
                    var key = length != -1
                        ? str2.Substring(0, length).Trim()
                        : throw new ArgumentException("YamlParsing: Expecting scalar mapping entry to contain a ':', at line " +
                                                      num);
                    var str3 = ReadQuotedValue(str2.Substring(length + 1).Trim());
                    mapping.AddToSequence(key, str3);
                }
            }
        }
    }

    private static string ReadQuotedValue(string value) =>
        value.StartsWith("'") && value.EndsWith("'") || value.StartsWith("\"") && value.EndsWith("\"")
            ? value.Substring(1, value.Length - 2)
            : value;

    public IEnumerable<Dictionary<string, string>> ReadMapping(string mappingName)
    {
        Mapping mapping;
        if (_mappings.TryGetValue(mappingName, out mapping))
        {
            foreach (var sequence in mapping.Sequences)
                yield return sequence;
        }
    }

    internal class Mapping
    {
        private Dictionary<string, string> _lastEntry;

        public Mapping() => Sequences = new List<Dictionary<string, string>>();

        public List<Dictionary<string, string>> Sequences { get; }

        public void BeginSequence()
        {
            _lastEntry = new Dictionary<string, string>();
            Sequences.Add(_lastEntry);
        }

        public void AddToSequence(string key, string value) => _lastEntry[key] = value;
    }
}