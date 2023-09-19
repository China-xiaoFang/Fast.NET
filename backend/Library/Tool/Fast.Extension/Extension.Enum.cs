using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Fast.Extensions;

/// <summary>
/// 枚举扩展
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// 获取枚举的Description
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
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
    /// <param name="type"></param>
    /// <returns></returns>
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

    //public static bool ExactlyOne(this Enum value)
    //{
    //    return ToUInt64(value).ExactlyOne();
    //}

    //public static bool ExactlyOne<TEnum>(this TEnum value) where TEnum : struct
    //{
    //    return ToUInt64(value).ExactlyOne();
    //}

    ///// <summary>
    ///// 自动Flag
    ///// 注：Dto中的属性Name和枚举中的Name必须保持相同
    ///// </summary>
    ///// <typeparam name="TEnum"></typeparam>
    ///// <typeparam name="TDto"></typeparam>
    ///// <param name="enumVal"></param>
    ///// <param name="valDto"></param>
    ///// <returns></returns>
    ///// <exception cref="ArgumentException"></exception>
    //public static TEnum AutoFlag<TEnum, TDto>(this TEnum enumVal, TDto valDto) where TEnum : Enum where TDto : class
    //{
    //    var enumType = enumVal.GetType();
    //    var enumTypeCode = Type.GetTypeCode(enumType);
    //    // 获取枚举类型的所有定义
    //    var enumList = Enum.GetNames(enumType).Select(sl =>
    //    {
    //        var item = Enum.Parse(enumType, sl);
    //        return new {Name = item.ParseToString(), Value = item.ParseToInt().ParseToLong()};
    //    });

    //    var dtoType = valDto.GetType();
    //    // 获取Dto的所有Bool类型的属性
    //    var dtoProperties = dtoType.GetProperties().Where(wh => wh.PropertyType == typeof(bool)).ToList();

    //    // 深度拷贝一份
    //    var result = enumVal.DeepCopy();

    //    // 循环枚举集合，通过Name匹配对应的属性Dto
    //    foreach (var enumItem in enumList)
    //    {
    //        var dtoPropertyInfo = dtoProperties.FirstOrDefault(f => f.Name == enumItem.Name);
    //        if (dtoPropertyInfo == null)
    //            continue;

    //        var propertyVal = dtoPropertyInfo.GetValue(valDto, null).ParseToBool();
    //        var num1 = ToUInt64(result, enumTypeCode);
    //        var num2 = ToUInt64(enumItem.Value, enumTypeCode);
    //        if (propertyVal)
    //        {
    //            if ((num1 & num2) == 0L)
    //            {
    //                num1 |= num2;
    //            }
    //        }
    //        else if ((num1 & num2) != 0)
    //        {
    //            num1 ^= num2;
    //        }

    //        result = (TEnum) Enum.Parse(enumType, num1.ParseToString());
    //    }

    //    return result;
    //}

    ///// <summary>
    ///// 自动反向Flag
    ///// 注：Dto中的属性Name和枚举中的Name必须保持相同
    ///// </summary>
    ///// <typeparam name="TEnum"></typeparam>
    ///// <typeparam name="TDto"></typeparam>
    ///// <param name="enumVal"></param>
    ///// <returns></returns>
    ///// <exception cref="ArgumentException"></exception>
    //public static TDto AutoFlag<TEnum, TDto>(this TEnum enumVal) where TEnum : Enum where TDto : class
    //{
    //    var enumType = enumVal.GetType();
    //    var enumTypeCode = Type.GetTypeCode(enumType);
    //    var num1 = ToUInt64(enumVal, enumTypeCode);
    //    // 获取枚举类型的所有定义
    //    var enumList = Enum.GetNames(enumType).Select(sl =>
    //    {
    //        var item = Enum.Parse(enumType, sl);
    //        return new {Name = item.ParseToString(), Value = item.ParseToInt().ParseToLong()};
    //    });

    //    var dtoType = typeof(TDto);
    //    // 获取Dto的所有Bool类型的属性
    //    var dtoProperties = dtoType.GetProperties().Where(wh => wh.PropertyType == typeof(bool)).ToList();

    //    // 通过反射自动创建泛型对象
    //    var result = Activator.CreateInstance<TDto>();

    //    // 循环枚举集合，通过Name匹配对应的属性Dto
    //    foreach (var enumItem in enumList)
    //    {
    //        var dtoPropertyInfo = dtoProperties.FirstOrDefault(f => f.Name == enumItem.Name);
    //        if (dtoPropertyInfo == null)
    //            continue;

    //        var num2 = ToUInt64(enumItem.Value, enumTypeCode);

    //        dtoPropertyInfo.SetValue(result, Convert.ChangeType((num1 & num2) == num2, dtoPropertyInfo.PropertyType), null);
    //    }

    //    return result;
    //}

    //private static ulong ToUInt64(object value, TypeCode code)
    //{
    //    return code switch
    //    {
    //        TypeCode.SByte => (ulong) (sbyte) value,
    //        TypeCode.Byte => (byte) value,
    //        TypeCode.Int16 => (ulong) (short) value,
    //        TypeCode.UInt16 => (ushort) value,
    //        TypeCode.Int32 => (ulong) (int) value,
    //        TypeCode.UInt32 => (uint) value,
    //        TypeCode.Int64 => (ulong) (long) value,
    //        TypeCode.UInt64 => (ulong) value,
    //        _ => throw new InvalidOperationException(),
    //    };
    //}

    //private static ulong ToUInt64(object value)
    //{
    //    return ToUInt64(value, Type.GetTypeCode(value.GetType()));
    //}

    //public static IEnumerable<TEnum> SplitFlags<TEnum>(this TEnum value) where TEnum : struct
    //{
    //    var type = value.GetType();
    //    if (!type.IsEnum)
    //    {
    //        throw new ArgumentException();
    //    }

    //    if (type.GetCustomAttributes(typeof(FlagsAttribute), inherit: false).Length == 0)
    //    {
    //        throw new NotSupportedException(value.GetType().Name + "未包含Flags特性");
    //    }

    //    return Type.GetTypeCode(type) switch
    //    {
    //        TypeCode.SByte => from i in ((sbyte) (object) value).GetOnes() where type.IsEnumDefined(i) select (TEnum) (object) i,
    //        TypeCode.Byte => from i in ((byte) (object) value).GetOnes() where type.IsEnumDefined(i) select (TEnum) (object) i,
    //        TypeCode.Int16 => from i in ((short) (object) value).GetOnes() where type.IsEnumDefined(i) select (TEnum) (object) i,
    //        TypeCode.UInt16 => from i in ((ushort) (object) value).GetOnes()
    //            where type.IsEnumDefined(i)
    //            select (TEnum) (object) i,
    //        TypeCode.Int32 => from i in ((int) (object) value).GetOnes() where type.IsEnumDefined(i) select (TEnum) (object) i,
    //        TypeCode.UInt32 => from i in ((uint) (object) value).GetOnes() where type.IsEnumDefined(i) select (TEnum) (object) i,
    //        TypeCode.Int64 => from i in ((long) (object) value).GetOnes() where type.IsEnumDefined(i) select (TEnum) (object) i,
    //        TypeCode.UInt64 => from i in ((ulong) (object) value).GetOnes() where type.IsEnumDefined(i) select (TEnum) (object) i,
    //        _ => throw new NotSupportedException(),
    //    };
    //}

    //public static TEnum AndFlag<TEnum>(this TEnum value, bool isMeet, TEnum flagItem) where TEnum : struct
    //{
    //    if (!flagItem.ExactlyOne())
    //    {
    //        throw new ArgumentException("flagItem不能是组合枚举！");
    //    }

    //    var type = value.GetType();
    //    var typeCode = Type.GetTypeCode(type);
    //    var num = ToUInt64(value, typeCode);
    //    var num2 = ToUInt64(flagItem, typeCode);
    //    if (isMeet)
    //    {
    //        if ((num & num2) == 0L)
    //        {
    //            num |= num2;
    //        }
    //    }
    //    else if ((num & num2) != 0)
    //    {
    //        num ^= num2;
    //    }

    //    return (TEnum) Enum.Parse(type, num.ToString());
    //}
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