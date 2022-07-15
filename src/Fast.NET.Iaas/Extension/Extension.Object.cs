namespace Fast.NET.Iaas.Extension;

/// <summary>
/// 字典扩展
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// 将一个对象转化为 Get 请求的String字符串
    /// 注：List，Array，Object属性不支持
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="isToLower">首字母是否小写</param>
    /// <returns></returns>
    public static string ToQueryString(this object obj, bool isToLower = false)
    {
        if (obj == null)
            return string.Empty;

        var dictionary = new Dictionary<string, string>();

        var t = obj.GetType(); // 获取对象对应的类， 对应的类型

        var pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance); // 获取当前type公共属性

        foreach (var p in pi)
        {
            var m = p.GetGetMethod();

            if (m == null || !m.IsPublic)
                continue;
            // 进行判NULL处理
            if (m.Invoke(obj, new object[] { }) == null)
                continue;

            var value = m.Invoke(obj, new object[] { });

            // 进行List集合处理
            var valType = value?.GetType();
            if (valType is {IsGenericType: true})
            {
                // 这里如果还有别的参数，需要再次添加
                switch (value)
                {
                    case List<string> strList:
                        var strListVal = strList.Aggregate("",
                            (current, item) => current + $"{item}&{(isToLower ? p.Name.FirstCharToLower() : p.Name)}[]=");

                        strListVal = strListVal[..^$"&{(isToLower ? p.Name.FirstCharToLower() : p.Name)}[]=".Length];

                        dictionary.Add($"{(isToLower ? p.Name.FirstCharToLower() : p.Name)}[]", strListVal); // 向字典添加元素
                        break;
                    case List<int> intList:
                        var intListVal = intList.Aggregate("",
                            (current, item) => current + $"{item}&{(isToLower ? p.Name.FirstCharToLower() : p.Name)}[]=");

                        intListVal = intListVal[..^$"&{(isToLower ? p.Name.FirstCharToLower() : p.Name)}[]=".Length];

                        dictionary.Add($"{(isToLower ? p.Name.FirstCharToLower() : p.Name)}[]", intListVal); // 向字典添加元素
                        break;
                    default:
                        dictionary.Add(p.Name, m.Invoke(obj, new object[] { })?.ToString()); // 向字典添加元素
                        break;
                }
            }
            else
            {
                dictionary.Add(p.Name, m.Invoke(obj, new object[] { })?.ToString()); // 向字典添加元素
            }
        }

        return dictionary.ToQueryString(isToLower: isToLower);
    }

    public static Dictionary<string, string> ToDictionary(this object obj)
    {
        var dictionary = new Dictionary<string, string>();

        var t = obj.GetType(); // 获取对象对应的类， 对应的类型

        var pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance); // 获取当前type公共属性

        foreach (var p in pi)
        {
            var m = p.GetGetMethod();

            if (m == null || !m.IsPublic)
                continue;
            // 进行判NULL处理
            if (m.Invoke(obj, new object[] { }) != null)
            {
                dictionary.Add(p.Name, m.Invoke(obj, new object[] { })?.ToString()); // 向字典添加元素
            }
        }

        return dictionary;
    }
}