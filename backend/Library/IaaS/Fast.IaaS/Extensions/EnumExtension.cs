using System.ComponentModel;
using System.Reflection;

namespace Fast.IaaS.Extensions;

/// <summary>
/// <see cref="Enum"/> 拓展类
/// </summary>
public static class EnumExtension
{
    /// <summary>
    /// 获取枚举的Description
    /// </summary>
    /// <param name="value"><see cref="object"/></param>
    /// <returns><see cref="string"/></returns>
    public static string GetEnumDescription(this object value)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(value);

        // 获取枚举类型
        var enumType = value.GetType();

        // 检查是否是枚举类型
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("The parameter is not an enumeration type.", nameof(value));
        }

        // 获取枚举名称
        var enumName = Enum.GetName(enumType, value);

        // 空检查
        ArgumentNullException.ThrowIfNull(enumName);

        // 获取枚举字段
        var enumField = enumType.GetField(enumName);

        // 空检查
        ArgumentNullException.ThrowIfNull(enumField);

        // 获取 [Description] 特性描述
        return enumField.GetCustomAttribute<DescriptionAttribute>(false)?.Description ?? enumName;
    }

    /// <summary>
    /// 将枚举转成枚举信息集合
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <returns><see cref="List{EnumEntity}"/></returns>
    public static List<EnumEntity> EnumToList(this Type type)
    {
        if (!type.IsEnum)
            throw new ArgumentException("Type '" + type.Name + "' is not an enum.");
        var arr = Enum.GetNames(type);
        return arr.Select(sl =>
        {
            var item = Enum.Parse(type, sl);
            return new EnumEntity {Name = item.ParseToString(), Describe = item.GetEnumDescription(), Value = item.ParseToInt()};
        }).ToList();
    }
}

/// <summary>
/// 枚举的Entity类
/// </summary>
public class EnumEntity
{
    /// <summary>  
    /// 枚举的描述  
    /// </summary>  
    public string Describe { set; get; }

    /// <summary>  
    /// 枚举名称  
    /// </summary>  
    public string Name { set; get; }

    /// <summary>  
    /// 枚举对象的值  
    /// </summary>  
    public int Value { set; get; }
}