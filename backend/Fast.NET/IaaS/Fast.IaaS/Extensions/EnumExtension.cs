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