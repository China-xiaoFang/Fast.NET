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
using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="Expression"/> 拓展类
/// </summary>
[SuppressSniffer]
public static class LinqExpressionExtension
{
    /// <summary>
    /// 解析表达式属性名称
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <typeparam name="TProperty">属性类型</typeparam>
    /// <param name="propertySelector"><see cref="Expression{TDelegate}"/></param>
    /// <returns><see cref="string"/></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string GetPropertyName<T, TProperty>(this Expression<Func<T, TProperty?>> propertySelector)
    {
        return propertySelector.Body switch
        {
            // 检查 Lambda 表达式的主体是否是 MemberExpression 类型
            MemberExpression memberExpression => GetPropertyName<T>(memberExpression),

            // 如果主体是 UnaryExpression 类型，则继续解析
            UnaryExpression {Operand: MemberExpression nestedMemberExpression} => GetPropertyName<T>(nestedMemberExpression),

            _ => throw new ArgumentException("Expression is not valid for property selection.")
        };
    }

    /// <summary>
    /// 解析表达式属性名称
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="memberExpression"><see cref="MemberExpression"/></param>
    /// <returns><see cref="string"/></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string GetPropertyName<T>(MemberExpression memberExpression)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(memberExpression);

        // 获取属性声明类型
        var propertyType = memberExpression.Member.DeclaringType;

        // 检查是否越界访问属性
        if (propertyType != typeof(T))
        {
            throw new ArgumentException("Invalid property selection.");
        }

        // 返回属性名称
        return memberExpression.Member.Name;
    }
}