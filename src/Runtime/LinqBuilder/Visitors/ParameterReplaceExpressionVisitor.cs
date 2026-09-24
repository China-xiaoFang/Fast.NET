// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
namespace System.Linq;

/// <summary>
/// 处理 Lambda 参数不一致问题
/// </summary>
internal sealed class ParameterReplaceExpressionVisitor : ExpressionVisitor
{
    /// <summary>
    /// 参数表达式映射集合
    /// </summary>
    private readonly Dictionary<ParameterExpression, ParameterExpression> parameterExpressionSetter;

    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    /// <param name="parameterExpressionSetter">参数表达式映射集合</param>
    public ParameterReplaceExpressionVisitor(Dictionary<ParameterExpression, ParameterExpression> parameterExpressionSetter)
    {
        this.parameterExpressionSetter = parameterExpressionSetter ?? new Dictionary<ParameterExpression, ParameterExpression>();
    }

    /// <summary>
    /// 替换表达式参数
    /// </summary>
    /// <param name="parameterExpressionSetter">参数表达式映射集合</param>
    /// <param name="expression">表达式</param>
    /// <returns>新的表达式</returns>
    public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> parameterExpressionSetter,
        Expression expression)
    {
        return new ParameterReplaceExpressionVisitor(parameterExpressionSetter).Visit(expression);
    }

    /// <inheritdoc />
    protected override Expression VisitParameter(ParameterExpression parameterExpression)
    {
        if (parameterExpressionSetter.TryGetValue(parameterExpression, out ParameterExpression replacement))
        {
            parameterExpression = replacement;
        }

        return base.VisitParameter(parameterExpression);
    }
}
