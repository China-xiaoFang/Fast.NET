// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="Enum"/> 拓展类
/// </summary>
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
        if (enumName is null)
        {
            throw new ArgumentNullException(nameof(enumName));
        }

        // 获取枚举字段
        var enumField = enumType.GetField(enumName);

        // 空检查
        if (enumField is null)
        {
            throw new ArgumentNullException(nameof(enumField));
        }

        // 获取 [Description] 特性描述
        return enumField.GetCustomAttribute<DescriptionAttribute>(false)
                   ?.Description
               ?? enumName;
    }

    /// <summary>
    /// 将枚举转成枚举信息集合
    /// </summary>
    /// <param name="enumType"><see cref="Type"/>枚举值类型</param>
    /// <returns><see cref="List{T}"/></returns>
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

        return Enum.GetValues(enumType)
            .Cast<Enum>()
            .Select(enumValue => new EnumItem<TProperty>
            {
                Name = enumValue.ToString(),
                Describe = enumValue.GetDescription(enumType),
                Value = (TProperty) Convert.ChangeType(enumValue, propertyType)
            })
            .ToList();
    }
}