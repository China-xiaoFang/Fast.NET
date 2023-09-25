// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

namespace Fast.UAParser.UAParser;

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