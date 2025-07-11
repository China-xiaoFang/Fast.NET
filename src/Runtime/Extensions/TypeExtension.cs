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

using System.Reflection;
using System.Reflection.Emit;

// ReSharper disable once CheckNamespace
namespace Fast.Runtime;

/// <summary>
/// <see cref="Type"/> 拓展类
/// </summary>
public static class TypeExtension
{
    /// <summary>
    /// 创建属性值设置器
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="propertyInfo"><see cref="PropertyInfo"/></param>
    /// <returns><see cref="Action{T1, T2}"/></returns>
    public static Action<object, object> CreatePropertySetter(this Type type, PropertyInfo propertyInfo)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(propertyInfo);

        // 创建一个新的动态方法，并为其命名，命名格式为类型全名_设置_属性名
        var setterMethod = new DynamicMethod($"{type.FullName}_Set_{propertyInfo.Name}", null,
            new[] {typeof(object), typeof(object)}, typeof(TypeExtensions).Module);

        // 获取动态方法的 IL 生成器
        var ilGenerator = setterMethod.GetILGenerator();

        // 获取属性的设置方法，并允许非公开访问
        var setMethod = propertyInfo.GetSetMethod(true);

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
        return (Action<object, object>) setterMethod.CreateDelegate(typeof(Action<object, object>));
    }

    /// <summary>
    /// 创建字段值设置器
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="fieldInfo"><see cref="FieldInfo"/></param>
    /// <returns><see cref="Action{T1, T2}"/></returns>
    public static Action<object, object> CreateFieldSetter(this Type type, FieldInfo fieldInfo)
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
        return (Action<object, object>) setterMethod.CreateDelegate(typeof(Action<object, object>));
    }

    /// <summary>
    /// 创建属性值访问器
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="propertyInfo"><see cref="PropertyInfo"/></param>
    /// <returns><see cref="Func{T1, T2}"/></returns>
    public static Func<object, object> CreatePropertyGetter(this Type type, PropertyInfo propertyInfo)
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
        var getMethod = propertyInfo.GetGetMethod(true);

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
        return (Func<object, object>) dynamicMethod.CreateDelegate(typeof(Func<object, object>));
    }

    /// <summary>
    /// 创建字段值访问器
    /// </summary>
    /// <param name="type"><see cref="Type"/></param>
    /// <param name="fieldInfo"><see cref="FieldInfo"/></param>
    /// <returns><see cref="Func{T1, T2}"/></returns>
    public static Func<object, object> CreateFieldGetter(this Type type, FieldInfo fieldInfo)
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
        return (Func<object, object>) dynamicMethod.CreateDelegate(typeof(Func<object, object>));
    }
}