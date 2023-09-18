using System.Collections.Generic;

namespace Fast.Extension;

/// <summary>
/// <see cref="ICollection{T}"/> 拓展类
/// </summary>
public static class ICollectionExtensions
{
    /// <summary>
    /// 判断集合是否为空
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="collection"><see cref="ICollection{T}"/></param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsNullOrEmpty<T>(this ICollection<T>? collection)
    {
        return collection is null || collection.Count == 0;
    }
}