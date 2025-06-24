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
using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="Expression"/> 拓展类
/// </summary>
public static class LinqExpressionExtension
{
    /// <summary>
    /// 解析表达式属性名称
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <typeparam name="TProperty">属性类型</typeparam>
    /// <param name="propertySelector"><see cref="Expression{TDelegate}"/></param>
    /// <returns><see cref="string"/></returns>
    /// <exception cref="ArgumentException">Expression is not valid for property selection.</exception>
    public static string GetPropertyName<T, TProperty>(this Expression<Func<T, TProperty>> propertySelector)
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
    /// <exception cref="ArgumentException">Invalid property selection.</exception>
    public static string GetPropertyName<T>(MemberExpression memberExpression)
    {
        // 空检查
        if (memberExpression is null)
        {
            throw new ArgumentNullException(nameof(memberExpression));
        }

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