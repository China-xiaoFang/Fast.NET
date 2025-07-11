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
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="Type"/> 拓展类
/// </summary>
public static class TypeExtension
{
    /// <summary>
    /// 判断类型是否实现某个泛型
    /// </summary>
    /// <param name="type"><see cref="Type"/> 类型</param>
    /// <param name="generic"><see cref="Type"/>泛型类型</param>
    /// <returns><see cref="bool"/></returns>
    public static bool HasImplementedRawGeneric(this Type type, Type generic)
    {
        var localType = type;
        // 检查接口类型
        var isTheRawGenericType = type.GetInterfaces()
            .Any(IsTheRawGenericType);
        if (isTheRawGenericType)
            return true;

        // 检查类型
        while (localType != null && localType != typeof(object))
        {
            isTheRawGenericType = IsTheRawGenericType(localType);
            if (isTheRawGenericType)
                return true;
            localType = localType.BaseType;
        }

        return false;

        // 判断逻辑
        bool IsTheRawGenericType(Type t)
        {
            return generic == (t.IsGenericType ? t.GetGenericTypeDefinition() : t);
        }
    }

    /// <summary>
    /// 获取类型所在程序集名称
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <returns><see cref="string"/></returns>
    public static string GetAssemblyName(this Type type)
    {
        return type.GetTypeInfo()
            .GetAssemblyName();
    }

    /// <summary>
    /// 获取类型所在程序集名称
    /// </summary>
    /// <param name="typeInfo"><see cref="TypeInfo"/></param>
    /// <returns><see cref="string"/></returns>
    public static string GetAssemblyName(this TypeInfo typeInfo)
    {
        return typeInfo.Assembly.GetAssemblyName();
    }

    /// <summary>
    /// 判断是否是富基元类型
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsRichPrimitive(this Type type)
    {
        // 处理元组类型
        if (type.IsValueTuple())
            return false;

        // 处理数组类型，基元数组类型也可以是基元类型
        if (type.IsArray)
            return type.GetElementType()
                       ?.IsRichPrimitive()
                   == true;

        // 基元类型或值类型或字符串类型
        if (type.IsPrimitive || type.IsValueType || type == typeof(string))
            return true;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return type.GenericTypeArguments[0]
                .IsRichPrimitive();

        return false;
    }

    /// <summary>
    /// 判断是否是元组类型
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsValueTuple(this Type type)
    {
        return type.Namespace == "System" && type.Name.Contains("ValueTuple`");
    }

    /// <summary>
    /// 检查类型是否是静态类型
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsStatic(this Type type)
    {
        return type is {IsSealed: true, IsAbstract: true};
    }

    /// <summary>
    /// 检查类型是否是匿名类型
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsAnonymous(this Type type)
    {
        // 检查是否贴有 [CompilerGenerated] 特性
        if (!type.IsDefined(typeof(CompilerGeneratedAttribute), false))
        {
            return false;
        }

        // 类型限定名是否以 <> 开头且以 AnonymousType 结尾
        return !(type.FullName is null) && type.FullName.StartsWith("<>") && type.FullName.Contains("AnonymousType");
    }

    /// <summary>
    /// 检查类型是否可实例化
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsInstantiable(this Type type)
    {
        return type is {IsClass: true, IsAbstract: false} && !type.IsStatic();
    }

    /// <summary>
    /// 检查类型是否派生自指定类型
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="fromType"><see cref="Type"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsAlienAssignableTo(this Type type, Type fromType)
    {
        // 空检查
        if (fromType is null)
        {
            throw new ArgumentNullException(nameof(fromType));
        }

        return fromType != type && fromType.IsAssignableFrom(type);
    }

    /// <summary>
    /// 获取指定特性实例
    /// </summary>
    /// <remarks>若特性不存在则返回 null</remarks>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="inherit">是否查找基类型特性</param>
    /// <returns><typeparamref name="TAttribute"/></returns>
    public static TAttribute GetDefinedCustomAttribute<TAttribute>(this Type type, bool inherit = false)
        where TAttribute : Attribute
    {
        // 检查是否定义
        return !type.IsDefined(typeof(TAttribute), inherit) ? null : type.GetCustomAttribute<TAttribute>(inherit);
    }

    /// <summary>
    /// 检查类型是否定义了公开无参构造函数
    /// </summary>
    /// <remarks>用于 <see cref="Activator.CreateInstance(Type)"/> 实例化</remarks>
    /// <param name="type"><see cref="Type"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool HasDefinePublicParameterlessConstructor(this Type type)
    {
        return type.IsInstantiable()
               && type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null) != null;
    }

    /// <summary>
    /// 检查类型和指定类型定义是否相等
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="compareType"><see cref="Type"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsDefinitionEqual(this Type type, Type compareType)
    {
        // 空检查
        if (compareType is null)
        {
            throw new ArgumentNullException(nameof(compareType));
        }

        return type == compareType
               || (type.IsGenericType
                   && compareType.IsGenericType
                   && type.IsGenericTypeDefinition // 💡
                   && type == compareType.GetGenericTypeDefinition());
    }

    /// <summary>
    /// 检查类型和指定继承类型是否兼容
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="inheritType"><see cref="Type"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsCompatibilityTo(this Type type, Type inheritType)
    {
        // 空检查
        if (inheritType is null)
        {
            throw new ArgumentNullException(nameof(inheritType));
        }

        return inheritType != typeof(object)
               && inheritType.IsAssignableFrom(type)
               && (!type.IsGenericType
                   || (type.IsGenericType
                       && inheritType.IsGenericType
                       && type.GetTypeInfo()
                           .GenericTypeParameters.SequenceEqual(inheritType.GenericTypeArguments)));
    }

    /// <summary>
    /// 检查类型是否定义了指定方法
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="name">方法名称</param>
    /// <param name="accessibilityBindingFlags">可访问性成员绑定标记</param>
    /// <param name="methodInfo"><see cref="MethodInfo"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsDeclarationMethod(this Type type, string name, BindingFlags accessibilityBindingFlags,
        out MethodInfo methodInfo)
    {
        // 空检查
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"Argument '{name}' cannot be null or whitespace.");

        methodInfo = type.GetMethod(name, accessibilityBindingFlags | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        return methodInfo != null;
    }

    /// <summary>
    /// 检查类型是否是整数类型
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsInteger(this Type type)
    {
        // 如果是枚举或浮点类型则直接返回
        if (type.IsEnum || type.IsDecimal())
        {
            return false;
        }

        // 检查 TypeCode
        var typeCode = Type.GetTypeCode(type);
        return typeCode == TypeCode.Byte
               || typeCode == TypeCode.SByte
               || typeCode == TypeCode.Int16
               || typeCode == TypeCode.Int32
               || typeCode == TypeCode.Int64
               || typeCode == TypeCode.UInt16
               || typeCode == TypeCode.UInt32
               || typeCode == TypeCode.UInt64;
    }

    /// <summary>
    /// 检查类型是否是小数类型
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsDecimal(this Type type)
    {
        // 如果是浮点类型则直接返回
        if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
        {
            return true;
        }

        // 检查 TypeCode
        var typeCode = Type.GetTypeCode(type);
        return typeCode == TypeCode.Double || typeCode == TypeCode.Decimal;
    }

    /// <summary>
    /// 检查类型是否是数值类型
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsNumeric(this Type type)
    {
        return type.IsInteger() || type.IsDecimal();
    }

    /// <summary>
    /// 检查类型是否是字典类型
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsDictionary(this Type type)
    {
        // 如果是 IDictionary<,> 类型则直接返回
        if ((type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            || type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
        {
            return true;
        }

        // 处理 KeyValuePair<,> 集合类型
        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            // 检查是否是 KeyValuePair<,> 数组类型
            if (type.IsArray)
            {
                // 获取数组元素类型
                var elementType = type.GetElementType();

                // 检查元素类型是否是 KeyValuePair<,> 类型
                if (elementType != null
                    && elementType.IsGenericType
                    && elementType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                {
                    return true;
                }
            }
            // 检查是否是 KeyValuePair<,> 集合类型
            else
            {
                // 检查集合项类型是否是 KeyValuePair<,> 类型
                if (type.IsGenericType
                    && type.GenericTypeArguments.Length == 1
                    && type.GenericTypeArguments[0].IsGenericType
                    && type.GenericTypeArguments[0]
                        .GetGenericTypeDefinition()
                    == typeof(KeyValuePair<,>))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 获取类型自定义特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <param name="type">类类型</param>
    /// <param name="inherit">是否继承查找</param>
    /// <returns>特性对象</returns>
    public static TAttribute GetTypeAttribute<TAttribute>(this Type type, bool inherit = false) where TAttribute : Attribute
    {
        // 空检查
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        // 检查特性并获取特性对象
        return type.IsDefined(typeof(TAttribute), inherit) ? type.GetCustomAttribute<TAttribute>(inherit) : null;
    }
}