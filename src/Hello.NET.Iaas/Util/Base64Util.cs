namespace Hello.NET.Infrastructure.Util;

public static class Base64Util
{
    // 随机字符长度
    public const int RandomPrefixStrLength = 6;

    public const string RandomStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    private static string GetRandomStr(string randomStr = RandomStr, int randomPrefixStrLength = RandomPrefixStrLength)
    {
        // ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=
        var result = "";
        var random = new Random(Convert.ToInt32($"{DateTime.Now:HHmmssfff}"));

        for (var i = 0; i < randomPrefixStrLength; i++)
        {
            var randomInt = random.Next(0, randomStr.Length);
            var randomChar = randomStr[randomInt];
            result += randomChar;
        }

        Thread.Sleep(1); // 休眠,以使随机数不重叠.

        return result;
    }

    static readonly Encoding encoding = Encoding.UTF8;

    /// <summary>
    /// 普通 字符串 转换为 base64 字符串
    /// </summary>
    public static string ToBase64(this string str, int randomPrefixStrLength = RandomPrefixStrLength)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return "";
        }

        try
        {
            var randomPrefixStr = GetRandomStr(RandomStr, randomPrefixStrLength);
            var buffer = encoding.GetBytes(str);
            var base64Str = Convert.ToBase64String(buffer);

            base64Str = randomPrefixStrLength == 0 ? base64Str : InsertRandomStrToBase64Str(base64Str);

            return $"{randomPrefixStr}{base64Str}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Base64Utility.ToBase64: {ex}");
        }

        return string.Empty;
    }

    /// <summary>
    /// base64 字符串 转换为 普通 字符串
    /// </summary>
    public static string Base64ToString(this string base64Str, int randomPrefixStrLength = RandomPrefixStrLength)
    {
        var result = base64Str.Trim();
        try
        {
            if (string.IsNullOrWhiteSpace(base64Str?.Trim()))
            {
                return "";
            }

            base64Str = base64Str?.Trim();
            var input = base64Str?.Substring(randomPrefixStrLength);

            input = randomPrefixStrLength == 0 ? input : RemoveBase64StrRandomStr(input);
            var buffer = Convert.FromBase64String(input);
            result = encoding.GetString(buffer);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Base64Utility.Base64ToString: {ex}");
        }

        return result;
    }

    public struct PwdDic
    {
        public string Version { get; set; }

        public List<PwdDicItem> Item { get; set; }
    }

    public struct PwdDicItem
    {
        public int Index { get; set; }

        public int RandomIndex { get; set; }
    }

    public static readonly PwdDic dic = new PwdDic
    {
        Version = "3",
        Item = new List<PwdDicItem>
        {
            new() {Index = 950, RandomIndex = 188},
            new() {Index = 900, RandomIndex = 201},
            new() {Index = 800, RandomIndex = 225},
            new() {Index = 700, RandomIndex = 255},
            new() {Index = 600, RandomIndex = 268},
            new() {Index = 500, RandomIndex = 277},
            new() {Index = 400, RandomIndex = 288},
            new() {Index = 330, RandomIndex = 327},
            new() {Index = 300, RandomIndex = 180},
            new() {Index = 200, RandomIndex = 178},
            new() {Index = 100, RandomIndex = 124},
            // 100 以内字典
            new() {Index = 98, RandomIndex = 95},
            new() {Index = 92, RandomIndex = 90},
            new() {Index = 91, RandomIndex = 87},
            new() {Index = 88, RandomIndex = 84},
            new() {Index = 82, RandomIndex = 79},
            new() {Index = 78, RandomIndex = 71},
            new() {Index = 72, RandomIndex = 69},
            new() {Index = 68, RandomIndex = 66},
            new() {Index = 59, RandomIndex = 55},
            new() {Index = 48, RandomIndex = 43},
            new() {Index = 42, RandomIndex = 37},
            new() {Index = 36, RandomIndex = 30},
            new() {Index = 33, RandomIndex = 27},
            new() {Index = 24, RandomIndex = 20},
            new() {Index = 23, RandomIndex = 18},
            new() {Index = 21, RandomIndex = 16},
            new() {Index = 17, RandomIndex = 14},
            new() {Index = 13, RandomIndex = 9},
            new() {Index = 7, RandomIndex = 4},
            new() {Index = 5, RandomIndex = 3},
            new() {Index = 2, RandomIndex = 1},
        }
    };

    private static string InsertRandomStrToBase64Str(string base64Str)
    {
        var strResult = $"{base64Str}";

        dic.Item.ForEach(item =>
        {
            if (item.Index < base64Str.Length)
            {
                var randomChar = base64Str[item.RandomIndex];
                strResult = strResult.Insert(item.Index, $"{randomChar}");
            }
        });

        return strResult;
    }

    private static string RemoveBase64StrRandomStr(string input)
    {
        var items = dic.Item.OrderBy(x => x.Index).ToList();

        var strResult = $"{input}";

        items.ForEach(item =>
        {
            if (item.Index < strResult.Length)
            {
                //var randomChar = input[item.RandomIndex];
                strResult = strResult.Remove(item.Index, 1);
            }
        });

        return strResult;
    }
}