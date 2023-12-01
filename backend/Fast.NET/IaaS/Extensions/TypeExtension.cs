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

#nullable enable
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="Type"/> 拓展类
/// </summary>
[SuppressSniffer]
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
        // 检查接口类型
        var isTheRawGenericType = type.GetInterfaces().Any(IsTheRawGenericType);
        if (isTheRawGenericType)
            return true;

        // 检查类型
        while (type != null && type != typeof(object))
        {
            isTheRawGenericType = IsTheRawGenericType(type);
            if (isTheRawGenericType)
                return true;
            type = type.BaseType;
        }

        return false;

        // 判断逻辑
        bool IsTheRawGenericType(Type t) => generic == (t.IsGenericType ? t.GetGenericTypeDefinition() : t);
    }

    /// <summary>
    /// 获取类型所在程序集名称
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <returns><see cref="string"/></returns>
    public static string GetAssemblyName(this Type type)
    {
        return type.GetTypeInfo().GetAssemblyName();
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
            return type.GetElementType()?.IsRichPrimitive() == true;

        // 基元类型或值类型或字符串类型
        if (type.IsPrimitive || type.IsValueType || type == typeof(string))
            return true;

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return type.GenericTypeArguments[0].IsRichPrimitive();

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
        return type.FullName is not null && type.FullName.StartsWith("<>") && type.FullName.Contains("AnonymousType");
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
        ArgumentNullException.ThrowIfNull(fromType);

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
    public static TAttribute? GetDefinedCustomAttribute<TAttribute>(this Type type, bool inherit = false)
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
        return type.IsInstantiable() &&
               type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, Type.EmptyTypes) is not null;
    }

    /// <summary>
    /// 检查类型和指定类型定义是否相等
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="compareType"><see cref="Type"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsDefinitionEqual(this Type type, Type? compareType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(compareType);

        return type == compareType || (type.IsGenericType && compareType.IsGenericType && type.IsGenericTypeDefinition // 💡
                                       && type == compareType.GetGenericTypeDefinition());
    }

    /// <summary>
    /// 检查类型和指定继承类型是否兼容
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="inheritType"><see cref="Type"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsCompatibilityTo(this Type type, Type? inheritType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(inheritType);

        return inheritType != typeof(object) && inheritType.IsAssignableFrom(type) && (!type.IsGenericType ||
            (type.IsGenericType && inheritType.IsGenericType &&
             type.GetTypeInfo().GenericTypeParameters.SequenceEqual(inheritType.GenericTypeArguments)));
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
        out MethodInfo? methodInfo)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(type);
        //ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"Argument '{name}' cannot be null or whitespace.");

        methodInfo = type.GetMethod(name, accessibilityBindingFlags | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        return methodInfo is not null;
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
        return Type.GetTypeCode(type) is TypeCode.Byte or TypeCode.SByte or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64
            or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64;
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
        return Type.GetTypeCode(type) is TypeCode.Double or TypeCode.Decimal;
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
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>) || type.GetInterfaces()
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
                if (elementType is not null && elementType.IsGenericType &&
                    elementType.GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                {
                    return true;
                }
            }
            // 检查是否是 KeyValuePair<,> 集合类型
            else
            {
                // 检查集合项类型是否是 KeyValuePair<,> 类型
                if (type is {IsGenericType: true, GenericTypeArguments.Length: 1} &&
                    type.GenericTypeArguments[0].GetGenericTypeDefinition() == typeof(KeyValuePair<,>))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 创建属性值设置器
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="propertyInfo"><see cref="PropertyInfo"/></param>
    /// <returns><see cref="Action{T1, T2}"/></returns>
    public static Action<object, object?> CreatePropertySetter(this Type type, PropertyInfo propertyInfo)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(propertyInfo);

        // 创建一个新的动态方法，并为其命名，命名格式为类型全名_设置_属性名
        var setterMethod = new DynamicMethod($"{type.FullName}_Set_{propertyInfo.Name}", null,
            new[] {typeof(object), typeof(object)}, typeof(TypeExtensions).Module);

        // 获取动态方法的 IL 生成器
        var ilGenerator = setterMethod.GetILGenerator();

        // 获取属性的设置方法，并允许非公开访问
        var setMethod = propertyInfo.GetSetMethod(nonPublic: true);

        // 空检查
        ArgumentNullException.ThrowIfNull(setMethod);

        // 将目标对象加载到堆栈上，并将其转换为所需的类型
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Castclass, type);

        // 将要分配的值加载到堆栈上
        ilGenerator.Emit(OpCodes.Ldarg_1);

        // 检查属性类型是否为值类型
        if (propertyInfo.PropertyType.IsValueType)
        {
            // 对值进行拆箱，转换为适当的值类型
            ilGenerator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
        }
        else
        {
            // 将值转换为属性类型
            ilGenerator.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
        }

        // 在目标对象上调用设置方法
        ilGenerator.Emit(OpCodes.Callvirt, setMethod);

        // 从动态方法返回
        ilGenerator.Emit(OpCodes.Ret);

        // 创建一个委托并将其转换为适当的 Action 类型
        return (Action<object, object?>) setterMethod.CreateDelegate(typeof(Action<object, object>));
    }

    /// <summary>
    /// 创建字段值设置器
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="fieldInfo"><see cref="FieldInfo"/></param>
    /// <returns><see cref="Action{T1, T2}"/></returns>
    public static Action<object, object?> CreateFieldSetter(this Type type, FieldInfo fieldInfo)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(fieldInfo);

        // 创建一个新的动态方法，并为其命名，命名格式为类型全名_设置_字段名
        var setterMethod = new DynamicMethod($"{type.FullName}_Set_{fieldInfo.Name}", null,
            new[] {typeof(object), typeof(object)}, typeof(TypeExtensions).Module);

        // 获取动态方法的 IL 生成器
        var ilGenerator = setterMethod.GetILGenerator();

        // 将目标对象加载到堆栈上，并将其转换为所需的类型
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Castclass, type);

        // 将要分配的值加载到堆栈上
        ilGenerator.Emit(OpCodes.Ldarg_1);

        // 检查字段类型是否为值类型
        if (fieldInfo.FieldType.IsValueType)
        {
            // 对值进行拆箱，转换为适当的值类型
            ilGenerator.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
        }
        else
        {
            // 将值转换为字段类型
            ilGenerator.Emit(OpCodes.Castclass, fieldInfo.FieldType);
        }

        // 将堆栈上的值存储到字段中
        ilGenerator.Emit(OpCodes.Stfld, fieldInfo);

        // 从动态方法返回
        ilGenerator.Emit(OpCodes.Ret);

        // 创建一个委托并将其转换为适当的 Action 类型
        return (Action<object, object?>) setterMethod.CreateDelegate(typeof(Action<object, object>));
    }

    /// <summary>
    /// 创建属性值访问器
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="propertyInfo"><see cref="PropertyInfo"/></param>
    /// <returns><see cref="Func{T1, T2}"/></returns>
    public static Func<object, object?> CreatePropertyGetter(this Type type, PropertyInfo propertyInfo)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(propertyInfo);
        ArgumentNullException.ThrowIfNull(propertyInfo.DeclaringType);

        // 创建一个新的动态方法，并为其命名，命名格式为类型全名_获取_属性名
        var dynamicMethod = new DynamicMethod($"{type.FullName}_Get_{propertyInfo.Name}", typeof(object), new[] {typeof(object)},
            typeof(TypeExtensions).Module, true);

        // 获取动态方法的 IL 生成器
        var ilGenerator = dynamicMethod.GetILGenerator();

        // 获取属性的获取方法，并允许非公开访问
        var getMethod = propertyInfo.GetGetMethod(nonPublic: true);

        // 空检查
        ArgumentNullException.ThrowIfNull(getMethod);

        // 将目标对象加载到堆栈上，并将其转换为声明类型
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Castclass, propertyInfo.DeclaringType);

        // 调用获取方法
        ilGenerator.EmitCall(OpCodes.Callvirt, getMethod, null);

        // 如果属性类型为值类型，则装箱为 object 类型
        if (propertyInfo.PropertyType.IsValueType)
        {
            ilGenerator.Emit(OpCodes.Box, propertyInfo.PropertyType);
        }

        // 从动态方法返回
        ilGenerator.Emit(OpCodes.Ret);

        // 创建一个委托并将其转换为适当的 Func 类型
        return (Func<object, object?>) dynamicMethod.CreateDelegate(typeof(Func<object, object>));
    }

    /// <summary>
    /// 创建字段值访问器
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="fieldInfo"><see cref="FieldInfo"/></param>
    /// <returns><see cref="Func{T1, T2}"/></returns>
    public static Func<object, object?> CreateFieldGetter(this Type type, FieldInfo fieldInfo)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(fieldInfo);
        ArgumentNullException.ThrowIfNull(fieldInfo.DeclaringType);

        // 创建一个新的动态方法，并为其命名，命名格式为类型全名_获取_字段名
        var dynamicMethod = new DynamicMethod($"{type.FullName}_Get_{fieldInfo.Name}", typeof(object), new[] {typeof(object)},
            typeof(TypeExtensions).Module, true);

        // 获取动态方法的 IL 生成器
        var ilGenerator = dynamicMethod.GetILGenerator();

        // 将目标对象加载到堆栈上，并将其转换为字段的声明类型
        ilGenerator.Emit(OpCodes.Ldarg_0);
        ilGenerator.Emit(OpCodes.Castclass, fieldInfo.DeclaringType);

        // 加载字段的值到堆栈上
        ilGenerator.Emit(OpCodes.Ldfld, fieldInfo);

        // 如果字段类型为值类型，则装箱为 object 类型
        if (fieldInfo.FieldType.IsValueType)
        {
            ilGenerator.Emit(OpCodes.Box, fieldInfo.FieldType);
        }

        // 从动态方法返回
        ilGenerator.Emit(OpCodes.Ret);

        // 创建一个委托并将其转换为适当的 Func 类型
        return (Func<object, object?>) dynamicMethod.CreateDelegate(typeof(Func<object, object>));
    }

    /// <summary>
    /// 获取类型自定义特性
    /// </summary>
    /// <typeparam name="TAttribute">特性类型</typeparam>
    /// <param name="type">类类型</param>
    /// <param name="inherit">是否继承查找</param>
    /// <returns>特性对象</returns>
    public static TAttribute? GetTypeAttribute<TAttribute>(this Type type, bool inherit = false) where TAttribute : Attribute
    {
        // 空检查
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        // 检查特性并获取特性对象
        return type.IsDefined(typeof(TAttribute), inherit) ? type.GetCustomAttribute<TAttribute>(inherit) : default;
    }
}