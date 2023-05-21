using Fast.ServiceCollection.Extension;

namespace Fast.ServiceCollection.Util;

/// <summary>
/// String工具类
/// </summary>
public static class StringUtil
{
    /// <summary>
    /// 得到验证码
    /// </summary>
    /// <param name="len"></param>
    /// <returns></returns>
    public static int GetVerCode(int len = 6)
    {
        return new Random().Next((int) Math.Pow(10, len - 1), (int) Math.Pow(10, len) - 1);
    }

    /// <summary>
    /// 将字符串生转化为固定长度左对齐，右补空格
    /// </summary>
    /// <param name="strTemp"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string AlignmentStrFunc(this string strTemp, int length)
    {
        var byteStr = Encoding.Default.GetBytes(strTemp.Trim());
        var iLength = byteStr.Length;
        var iNeed = length - iLength;
        var spaceLen = Encoding.Default.GetBytes(" "); //一个空格的长度
        iNeed = iNeed / spaceLen.Length;
        var spaceString = SpaceStrFunc(iNeed);
        return strTemp + spaceString;
    }

    /// <summary>
    /// 生成固定长度的空格字符串
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static string SpaceStrFunc(int length)

    {
        var strReturn = string.Empty;

        if (length <= 0)
            return strReturn;
        for (var i = 0; i < length; i++)
        {
            strReturn += " ";
        }

        return strReturn;
    }

    /// <summary>
    /// 截取指定长度的字符串
    /// </summary>
    /// <param name="value"></param>
    /// <param name="length"></param>
    /// <param name="ellipsis"></param>
    /// <returns></returns>
    public static string GetSubString(this string value, int length, bool ellipsis = false)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        if (value.Length <= length)
            return value;
        value = value[..length];
        if (ellipsis)
        {
            value += "...";
        }

        return value;
    }

    /// <summary>
    /// 生成一个Guid
    /// N ece4f4a60b764339b94a07c84e338a27
    /// D 5bf99df1-dc49-4023-a34a-7bd80a42d6bb
    /// B {2280f8d7-fd18-4c72-a9ab-405de3fcfbc9}
    /// P (25e6e09f-fb66-4cab-b4cd-bfb429566549)
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string GetGuid(string format = "N")
    {
        return Guid.NewGuid().ToString(format);
    }

    /// <summary>
    /// 生成一个短的Guid
    /// </summary>
    /// <returns></returns>
    public static string GetShortGuid()
    {
        var i = Guid.NewGuid().ToByteArray().Aggregate<byte, long>(1, (current, b) => current * ((int) b + 1));

        return $"{i - DateTime.Now.Ticks:x}";
    }


    #region Xml

    /// <summary>
    /// 将Dic字典转换成字符串
    /// </summary>
    /// <param name="dic"></param>
    /// <returns></returns>
    public static string DicToXmlStr(this Dictionary<string, object> dic)
    {
        var xml = "<xml>";
        foreach (var (key, value) in dic)
        {
            switch (value)
            {
                case int:
                    xml += "<" + key + ">" + value + "</" + key + ">";
                    break;
                case string:
                    xml += "<" + key + ">" + "<![CDATA[" + value + "]]></" + key + ">";
                    break;
            }
        }

        xml += "</xml>";
        return xml;
    }

    /// <summary>
    /// 将字符串转换为Dic字典
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static Dictionary<string, object> XmlStrToDic(this string xml)
    {
        if (xml.IsEmpty())
        {
            throw new Exception("不能转换空字符串！");
        }

        var rltDic = new Dictionary<string, object>();
        var xmlDoc = new XmlDocument {XmlResolver = null};
        xmlDoc.LoadXml(xml);
        var xmlNode = xmlDoc.FirstChild; //获取到根节点<xml>
        var nodes = xmlNode.ChildNodes;
        foreach (XmlNode xn in nodes)
        {
            var xe = (XmlElement) xn;
            rltDic[xe.Name] = xe.InnerText; //获取xml的键值对到WxPayData内部的数据中
        }

        return rltDic;
    }

    /// <summary>
    /// 将Dic字典转换成字符串
    /// </summary>
    /// <param name="dic"></param>
    /// <returns></returns>
    public static string SortDicToXmlStr(this SortedDictionary<string, object> dic)
    {
        var xml = "<xml>";
        foreach (var (key, value) in dic)
        {
            switch (value)
            {
                case int:
                    xml += "<" + key + ">" + value + "</" + key + ">";
                    break;
                case string:
                    xml += "<" + key + ">" + "<![CDATA[" + value + "]]></" + key + ">";
                    break;
            }
        }

        xml += "</xml>";
        return xml;
    }

    /// <summary>
    /// 将字符串转换为Dic字典
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    public static SortedDictionary<string, object> XmlStrToSortDic(this string xml)
    {
        if (xml.IsEmpty())
        {
            throw new Exception("不能转换空字符串！");
        }

        var rltDic = new SortedDictionary<string, object>();
        var xmlDoc = new XmlDocument {XmlResolver = null};
        xmlDoc.LoadXml(xml);
        var xmlNode = xmlDoc.FirstChild; //获取到根节点<xml>
        var nodes = xmlNode.ChildNodes;
        foreach (XmlNode xn in nodes)
        {
            var xe = (XmlElement) xn;
            rltDic[xe.Name] = xe.InnerText; //获取xml的键值对到WxPayData内部的数据中
        }

        return rltDic;
    }

    #endregion

    #region MD5

    /// <summary>
    /// 生成MD5
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string GenerateMd5(this string content)
    {
        using var mi = MD5.Create();
        var buffer = Encoding.Default.GetBytes(content);
        //开始加密
        var newBuffer = mi.ComputeHash(buffer);
        var sb = new StringBuilder();
        foreach (var by in newBuffer)
        {
            sb.Append(by.ToString("x2"));
        }

        return sb.ToString();
    }

    /// <summary>
    /// Unicode编码
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string EnUnicode(this string str)
    {
        var strResult = new StringBuilder();
        if (string.IsNullOrEmpty(str))
            return strResult.ToString();
        foreach (var c in str)
        {
            strResult.Append("\\u");
            strResult.Append(((int) c).ToString("x"));
        }

        return strResult.ToString();
    }

    /// <summary>
    /// Unicode解码
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string DeUnicode(this string str)
    {
        //最直接的方法Regex.Unescape(str);
        var reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
        return reg.Replace(str, m => ((char) Convert.ToInt32(m.Groups[1].Value, 16)).ToString());
    }

    #endregion

    #region SHA1

    /// <summary>
    /// 得到文件SHA1
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static string GetFileSHA1(this string filePath)
    {
        var strResult = "";
        var strHashData = "";
        FileStream oFileStream = null;
        var osha1 = SHA1.Create();
        oFileStream = new FileStream(filePath.Replace("\"", ""), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var arrBytHashValue = osha1.ComputeHash(oFileStream);
        oFileStream.Close();
        //由以连字符分隔的十六进制对构成的String，其中每一对表示value 中对应的元素；例如“F-2C-4A”
        strHashData = BitConverter.ToString(arrBytHashValue);
        //替换-
        strHashData = strHashData.Replace("-", "");
        strResult = strHashData.ToLower();
        return strResult;
    }

    #endregion

    #region ASCII

    /// <summary>
    /// ASCII编码
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string EnAscii(this string str)
    {
        var textBuf = Encoding.Default.GetBytes(str);
        return textBuf.Aggregate(string.Empty, (current, t) => current + t.ToString("X"));
    }

    /// <summary>
    /// ASCII解码
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string DeAscii(this string str)
    {
        var k = 0;
        var buffer = new byte[str.Length / 2];
        for (var i = 0; i < str.Length / 2; i++)
        {
            buffer[i] = byte.Parse(str.Substring(k, 2), NumberStyles.HexNumber);
            k += 2;
        }

        return Encoding.Default.GetString(buffer);
    }

    #endregion

    #region 已淘汰

    /// <summary>
    /// [弃用]把中文字符串转换为Unicode串
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string ToUnicode(string content)
    {
        var tmp = "";
        foreach (var c in content)
        {
            // 将中文字符转化为Unicode串
            if (IsChinese(c))
                tmp += "\\u" + ToString(c, 16);
            else
                tmp += c;
        }

        return tmp;
    }

    /// <summary>
    /// [弃用]把Unicode串转换为中文字符串
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string ToChinese(string content)
    {
        var tmp = "";
        var regex = new Regex("\\\\u[0-9a-fA-F]{4}", RegexOptions.IgnoreCase);
        var matches = regex.Matches(tmp);
        foreach (Match match in matches)
        {
            // 获取16进制串
            var hexStr = match.Value.Substring("\\u".Length);
            var c = ToChar(match.Value, 16).ToString();
            tmp = tmp.Replace(match.Value, c);
        }

        return tmp;
    }

    /// <summary>
    /// [弃用]判断字符是否为中文字符
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static bool IsChinese(char c)
    {
        // 中文字符范围
        return 0x4e00 <= c && c <= 0x9fbb;
    }

    /// <summary>
    /// [弃用]把数值转换为radix进制表示的字符串
    /// </summary>
    /// <param name="num"></param>
    /// <param name="radix"></param>
    /// <returns></returns>
    public static string ToString(int num, int radix = 10)
    {
        var str = "";
        while (num > 0)
        {
            // 取余数
            var remainder = num % radix;
            // 取商
            num /= radix;
            str = ToChar(remainder) + str;
        }

        return str;
    }

    /// <summary>
    /// [弃用]将数值转换为字符
    /// 将数值n转化为字符，0 <= n <= 35，依次转化为字符0-9a-z;
    /// 最大可表示36进制数
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public static char ToChar(int n)
    {
        n %= 36;
        if (n < 10)
            n += '0';
        else
            n += 'a' - 10;
        return (char) n;
    }

    /// <summary>
    /// [弃用]将字符转换为数值
    /// 将字符0-9a-z依次转化为数值0-35
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static int ToInt(char c)
    {
        if (c > '9')
            return c - 'a' + 10;
        return c - '0';
    }

    /// <summary>
    /// [弃用]将字符串radix进制的串str转化为字符
    /// </summary>
    /// <param name="str"></param>
    /// <param name="radix"></param>
    /// <returns></returns>
    public static char ToChar(string str, int radix)
    {
        var n = 0;
        foreach (var c in str)
        {
            n = n * radix + ToInt(c);
        }

        return (char) n;
    }

    #endregion 已淘汰
}