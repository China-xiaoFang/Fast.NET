using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;

namespace Fast.DynamicApplication.Internal;

/// <summary>
/// 常量、公共方法配置类
/// </summary>
internal static class Penetrates
{
    /// <summary>
    /// 分组分隔符
    /// </summary>
    internal const string GroupSeparator = "##";

    /// <summary>
    /// 控制器排序集合
    /// </summary>
    internal static ConcurrentDictionary<string, (string, int, Type)> ControllerOrderCollection { get; set; }

    /// <summary>
    /// 构造函数
    /// </summary>
    static Penetrates()
    {
        ControllerOrderCollection = new ConcurrentDictionary<string, (string, int, Type)>();

        IsApiControllerCached = new ConcurrentDictionary<Type, bool>();
    }

    /// <summary>
    /// <see cref="IsApiController(Type)"/> 缓存集合
    /// </summary>
    private static readonly ConcurrentDictionary<Type, bool> IsApiControllerCached;

    /// <summary>
    /// 是否是Api控制器
    /// </summary>
    /// <param name="type">type</param>
    /// <returns></returns>
    internal static bool IsApiController(Type type)
    {
        return IsApiControllerCached.GetOrAdd(type, Function);

        // 本地静态方法
        static bool Function(Type type)
        {
            if (type == null)
                return false;

            // 排除 OData 控制器
            if (type.Assembly.GetName().Name?.StartsWith("Microsoft.AspNetCore.OData") == true)
                return false;

            // 不能是非公开、基元类型、值类型、抽象类、接口、泛型类
            if (!type.IsPublic || type.IsPrimitive || type.IsValueType || type.IsAbstract || type.IsInterface ||
                type.IsGenericType)
                return false;

            // 继承 ControllerBase 或 实现 IDynamicApiController 的类型
            if ((!typeof(Controller).IsAssignableFrom(type) && typeof(ControllerBase).IsAssignableFrom(type)) ||
                typeof(IDynamicApplication).IsAssignableFrom(type))
                return true;

            return false;
        }
    }
}