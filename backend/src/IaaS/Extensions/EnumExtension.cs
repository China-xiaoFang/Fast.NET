// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
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

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="Enum"/> 拓展类
/// </summary>
[SuppressSniffer]
public static class EnumExtension
{
    /// <summary>
    /// 获取枚举值的描述
    /// </summary>
    /// <remarks>需要有 [Description] 特性，否则返回的是枚举值的Name</remarks>
    /// <typeparam name="TEnum"></typeparam>
    /// <param name="value">枚举值</param>
    /// <returns><see cref="string"/>枚举的 [Description] 特性描述</returns>
    /// <exception cref="ArgumentNullException">传入的枚举值为空</exception>
    /// <exception cref="ArgumentException">The parameter is not an enum type.</exception>
    public static string GetDescription<TEnum>(this TEnum value) where TEnum : struct, Enum
    {
        return GetDescription(value, typeof(TEnum));
    }

    /// <summary>
    /// 获取枚举值的描述
    /// </summary>
    /// <remarks>需要有 [Description] 特性，否则返回的是枚举值的Name</remarks>
    /// <param name="value"><see cref="Enum"/>枚举值</param>
    /// <param name="enumType"><see cref="Type"/>枚举类型</param>
    /// <returns><see cref="string"/>枚举的 [Description] 特性描述</returns>
    /// <exception cref="ArgumentNullException">传入的枚举值为空</exception>
    /// <exception cref="ArgumentException">The parameter is not an enum type.</exception>
    public static string GetDescription(this Enum value, Type enumType)
    {
        // 检查是否是枚举类型
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("The parameter is not an enum type.", nameof(value));
        }

        // 判断是否有效
        if (!Enum.IsDefined(enumType, value))
        {
            throw new ArgumentNullException(nameof(value), "传入的枚举值为空");
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
    /// <param name="enumType"><see cref="Type"/>枚举值类型</param>
    /// <returns><see cref="List{EnumEntity}"/></returns>
    /// <exception cref="ArgumentException">类型不是一个枚举类型</exception>
    public static List<EnumItem<int>> EnumToList(this Type enumType)
    {
        return enumType.EnumToList<int>();
    }

    /// <summary>
    /// 将枚举转成枚举信息集合
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="enumType"><see cref="Type"/>枚举值类型</param>
    /// <returns><see cref="List{EnumEntity}"/></returns>
    /// <exception cref="ArgumentException">类型不是一个枚举类型</exception>
    public static List<EnumItem<TProperty>> EnumToList<TProperty>(this Type enumType)
        where TProperty : struct, IComparable, IConvertible, IFormattable
    {
        if (!enumType.IsEnum)
            throw new ArgumentException("Type '" + enumType.Name + "' is not an enum.", nameof(enumType));

        var propertyType = typeof(TProperty);

        return Enum.GetValues(enumType).Cast<Enum>().Select(enumValue => new EnumItem<TProperty>
        {
            Name = enumValue.ToString(),
            Describe = enumValue.GetDescription(enumType),
            Value = (TProperty) Convert.ChangeType(enumValue, propertyType)
        }).ToList();
    }
}