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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="object"/> 拓展类
/// </summary>
public static class ObjectExtension
{
    /// <summary>
    /// 将一个对象转换为指定类型
    /// </summary>
    /// <param name="obj">待转换的对象</param>
    /// <param name="type">目标类型</param>
    /// <returns>转换后的对象</returns>
    public static object ChangeType(this object obj, Type type)
    {
        if (type == null)
            return obj;
        if (type == typeof(string))
            return obj?.ToString();
        if (type == typeof(Guid) && obj != null)
            return Guid.Parse(obj.ToString());
        if (type == typeof(bool) && obj != null && !(obj is bool))
        {
            var objStr = obj.ToString()
                ?.ToLower();
            if (objStr == "1" || objStr == "true" || objStr == "yes" || objStr == "on")
                return true;
            return false;
        }

        if (obj == null)
            return type.IsValueType ? Activator.CreateInstance(type) : null;

        var underlyingType = Nullable.GetUnderlyingType(type);
        if (type.IsInstanceOfType(obj))
            return obj;
        if ((underlyingType ?? type).IsEnum)
        {
            if (underlyingType != null && string.IsNullOrWhiteSpace(obj.ToString()))
                return null;
            return Enum.Parse(underlyingType ?? type, obj.ToString());
        }
        // 处理DateTime -> DateTimeOffset 类型

        if (obj is DateTime dateTime && (underlyingType ?? type) == typeof(DateTimeOffset))
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        }
        // 处理 DateTimeOffset -> DateTime 类型

        if (obj is DateTimeOffset dateTimeOffset && (underlyingType ?? type) == typeof(DateTime))
        {
            return dateTimeOffset.ParseToDateTime();
        }

        if (typeof(IConvertible).IsAssignableFrom(underlyingType ?? type))
        {
            try
            {
                return Convert.ChangeType(obj, underlyingType ?? type, null);
            }
            catch
            {
                return underlyingType == null ? Activator.CreateInstance(type) : null;
            }
        }

        var converter = TypeDescriptor.GetConverter(type);
        if (converter.CanConvertFrom(obj.GetType()))
            return converter.ConvertFrom(obj);

        var constructor = type.GetConstructor(Type.EmptyTypes);
        if (constructor != null)
        {
            var o = constructor.Invoke(null);
            var propertyArr = type.GetProperties();
            var oldType = obj.GetType();

            foreach (var property in propertyArr)
            {
                var p = oldType.GetProperty(property.Name);
                if (property.CanWrite && p != null && p.CanRead)
                {
                    property.SetValue(o, ChangeType(p.GetValue(obj, null), property.PropertyType), null);
                }
            }

            return o;
        }

        return obj;
    }

    /// <summary>
    /// 将一个Object对象转为 字典
    /// </summary>
    /// <param name="obj"><see cref="object"/></param>
    /// <returns><see cref="IDictionary{TKey,TValue}"/></returns>
    public static IDictionary<string, object> ToDictionary(this object obj)
    {
        var dictionary = new Dictionary<string, object>();

        var t = obj.GetType(); // 获取对象对应的类， 对应的类型

        var pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance); // 获取当前type公共属性

        foreach (var p in pi)
        {
            var m = p.GetGetMethod();

            if (m == null || !m.IsPublic)
                continue;
            // 进行判NULL处理
            if (m.Invoke(obj, Array.Empty<object>()) != null)
            {
                dictionary.Add(p.Name, m.Invoke(obj, Array.Empty<object>())); // 向字典添加元素
            }
        }

        return dictionary;
    }

    /// <summary>
    /// 将一个对象转换为指定类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T ChangeType<T>(this object obj)
    {
        return (T) ChangeType(obj, typeof(T));
    }

    /// <summary>
    /// 将一个对象转化为 Get 请求的String字符串
    /// 注：List，Array，Object属性不支持
    /// </summary>
    /// <param name="obj"><see cref="object"/></param>
    /// <param name="isToLower">首字母是否小写</param>
    /// <returns><see cref="string"/></returns>
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
                        dictionary.Add(p.Name, m.Invoke(obj, new object[] { })
                            ?.ToString()); // 向字典添加元素
                        break;
                }
            }
            else
            {
                dictionary.Add(p.Name, m.Invoke(obj, new object[] { })
                    ?.ToString()); // 向字典添加元素
            }
        }

        return dictionary.ToQueryString(isToLower: isToLower);
    }

    /// <summary>
    /// 尝试获取对象的数量
    /// </summary>
    /// <param name="obj"><see cref="object"/></param>
    /// <param name="count">数量</param>
    /// <returns><see cref="bool"/></returns>
    public static bool TryGetCount(this object obj, out int count)
    {
        // 处理可直接获取长度的类型

        // 检查对象是否是字符类型
        if (obj is char)
        {
            count = 1;
            return true;
        }
        // 检查对象是否是字符串类型

        if (obj is string text)
        {
            count = text.Length;
            return true;
        }
        // 检查对象是否实现了 ICollection 接口

        if (obj is ICollection collection)
        {
            count = collection.Count;
            return true;
        }

        // 反射查找是否存在 Count 属性
        var runtimeProperty = obj.GetType()
            .GetRuntimeProperty("Count");

        // 反射获取 Count 属性值
        if (!(runtimeProperty is null) && runtimeProperty.CanRead && runtimeProperty.PropertyType == typeof(int))
        {
            count = (int) runtimeProperty.GetValue(obj)!;
            return true;
        }

        count = -1;
        return false;
    }
}