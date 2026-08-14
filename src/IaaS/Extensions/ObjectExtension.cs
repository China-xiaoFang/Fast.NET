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

namespace Fast.IaaS;

/// <summary>
/// 为 <see cref="object"/> 提供扩展方法
/// </summary>
public static class ObjectExtension
{
    /// <summary>
    /// 将一个对象转换为指定类型
    /// </summary>
    /// <param name="obj">要处理的对象</param>
    /// <param name="type">目标类型</param>
    /// <returns>将一个对象转换为指定类型</returns>
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
        // 将 DateTime 按配置的时区规则转换为 DateTimeOffset

        if (obj is DateTime dateTime && (underlyingType ?? type) == typeof(DateTimeOffset))
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
        }
        // 将 DateTimeOffset 按配置的时区规则转换为 DateTime

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
                    property.SetValue(o, p.GetValue(obj, null)
                        .ChangeType(property.PropertyType), null);
                }
            }

            return o;
        }

        return obj;
    }

    /// <summary>
    /// 将一个 Object 对象转为 字典
    /// </summary>
    /// <param name="obj">要处理的对象</param>
    /// <param name="includeNull">是否在结果中包含值为 <see langword="null"/> 的属性</param>
    /// <returns>IDictionary{TKey,TValue}</returns>
    public static IDictionary<string, object> ToDictionary(this object obj, bool includeNull = false)
    {
        var dictionary = new Dictionary<string, object>();

        var t = obj.GetType(); // 获取对象对应的类， 对应的类型

        var pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance); // 获取当前type公共属性

        foreach (var p in pi)
        {
            var m = p.GetGetMethod();

            if (m == null || !m.IsPublic)
                continue;

            var o = m.Invoke(obj, Array.Empty<object>());
            if (o != null || includeNull)
            {
                dictionary.Add(p.Name, o); // 向字典添加元素
            }
        }

        return dictionary;
    }

    /// <summary>
    /// 将一个对象转换为指定类型
    /// </summary>
    /// <param name="obj">要处理的对象</param>
    /// <typeparam name="T">转换操作使用的对象类型</typeparam>
    /// <returns>将一个对象转换为指定类型</returns>
    public static T ChangeType<T>(this object obj)
    {
        return (T) obj.ChangeType(typeof(T));
    }

    /// <summary>
    /// 将对象的公共属性转换为查询字符串
    /// </summary>
    /// <remarks>字符串和整数列表会使用带 <c>[]</c> 后缀的重复查询参数；其他复杂属性使用其字符串表示形式</remarks>
    /// <param name="obj">要转换的对象</param>
    /// <param name="isToLower">是否将属性名的首字母转换为小写</param>
    /// <returns>由非空公共属性组成的查询字符串；对象为空时返回空字符串</returns>
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
            if (m.Invoke(obj, new object[] { }) == null)
                continue;

            var value = m.Invoke(obj, new object[] { });

            // 进行 List 集合处理
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
    /// <param name="obj">要处理的对象</param>
    /// <param name="count">得到的元素数量</param>
    /// <returns>能够读取字符、字符串、集合或公开 <c>Count</c> 属性的数量时返回 <see langword="true"/>；否则返回 <see langword="false"/></returns>
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
