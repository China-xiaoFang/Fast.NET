using System;
using System.Linq.Expressions;

namespace Fast.Extensions;

/// <summary>
/// <see cref="Expression"/> 拓展类
/// </summary>
public static class LinqExpressionExtensions
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