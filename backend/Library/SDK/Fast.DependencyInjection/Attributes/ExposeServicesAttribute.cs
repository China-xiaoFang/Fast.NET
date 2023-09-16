
using System;

namespace Furion.DependencyInjection;

/// <summary>
/// 服务导出配置特性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class ExposeServicesAttribute : Attribute
{
    /// <summary>
    /// <inheritdoc cref="ExposeServicesAttribute"/>
    /// </summary>
    /// <param name="serviceTypes"><see cref="ServiceTypes"/></param>
    public ExposeServicesAttribute(params Type[] serviceTypes)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(serviceTypes);

        ServiceTypes = serviceTypes;
    }

    /// <summary>
    /// 类型服务集合
    /// </summary>
    public Type[] ServiceTypes { get; init; }
}

/// <summary>
/// 服务导出配置特性
/// </summary>
/// <typeparam name="TService">服务类型</typeparam>
public sealed class ExposeServicesAttribute<TService> : ExposeServicesAttribute
    where TService : class
{
    /// <summary>
    /// <inheritdoc cref="ExposeServicesAttribute{TService}"/>
    /// </summary>
    public ExposeServicesAttribute()
        : base(typeof(TService))
    {
    }
}

/// <summary>
/// 服务导出配置特性
/// </summary>
/// <typeparam name="TService1">服务类型</typeparam>
/// <typeparam name="TService2">服务类型</typeparam>
public sealed class ExposeServicesAttribute<TService1, TService2> : ExposeServicesAttribute
    where TService1 : class
    where TService2 : class
{
    /// <summary>
    /// <inheritdoc cref="ExposeServicesAttribute{TService1, TService2}"/>
    /// </summary>
    public ExposeServicesAttribute()
        : base(typeof(TService1)
            , typeof(TService2))
    {
    }
}

/// <summary>
/// 服务导出配置特性
/// </summary>
/// <typeparam name="TService1">服务类型</typeparam>
/// <typeparam name="TService2">服务类型</typeparam>
/// <typeparam name="TService3">服务类型</typeparam>
public sealed class ExposeServicesAttribute<TService1, TService2, TService3> : ExposeServicesAttribute
    where TService1 : class
    where TService2 : class
    where TService3 : class
{
    /// <summary>
    /// <inheritdoc cref="ExposeServicesAttribute{TService1, TService2, TService3}"/>
    /// </summary>
    public ExposeServicesAttribute()
        : base(typeof(TService1)
            , typeof(TService2)
            , typeof(TService3))
    {
    }
}

/// <summary>
/// 服务导出配置特性
/// </summary>
/// <typeparam name="TService1">服务类型</typeparam>
/// <typeparam name="TService2">服务类型</typeparam>
/// <typeparam name="TService3">服务类型</typeparam>
/// <typeparam name="TService4">服务类型</typeparam>
public sealed class ExposeServicesAttribute<TService1, TService2, TService3, TService4> : ExposeServicesAttribute
    where TService1 : class
    where TService2 : class
    where TService3 : class
    where TService4 : class
{
    /// <summary>
    /// <inheritdoc cref="ExposeServicesAttribute{TService1, TService2, TService3, TService4}"/>
    /// </summary>
    public ExposeServicesAttribute()
        : base(typeof(TService1)
            , typeof(TService2)
            , typeof(TService3)
            , typeof(TService4))
    {
    }
}

/// <summary>
/// 服务导出配置特性
/// </summary>
/// <typeparam name="TService1">服务类型</typeparam>
/// <typeparam name="TService2">服务类型</typeparam>
/// <typeparam name="TService3">服务类型</typeparam>
/// <typeparam name="TService4">服务类型</typeparam>
/// <typeparam name="TService5">服务类型</typeparam>
public sealed class ExposeServicesAttribute<TService1, TService2, TService3, TService4, TService5> : ExposeServicesAttribute
    where TService1 : class
    where TService2 : class
    where TService3 : class
    where TService4 : class
    where TService5 : class
{
    /// <summary>
    /// <inheritdoc cref="ExposeServicesAttribute{TService1, TService2, TService3, TService4, TService5}"/>
    /// </summary>
    public ExposeServicesAttribute()
        : base(typeof(TService1)
            , typeof(TService2)
            , typeof(TService3)
            , typeof(TService4)
            , typeof(TService5))
    {
    }
}

/// <summary>
/// 服务导出配置特性
/// </summary>
/// <typeparam name="TService1">服务类型</typeparam>
/// <typeparam name="TService2">服务类型</typeparam>
/// <typeparam name="TService3">服务类型</typeparam>
/// <typeparam name="TService4">服务类型</typeparam>
/// <typeparam name="TService5">服务类型</typeparam>
/// <typeparam name="TService6">服务类型</typeparam>
public sealed class ExposeServicesAttribute<TService1, TService2, TService3, TService4, TService5, TService6> : ExposeServicesAttribute
    where TService1 : class
    where TService2 : class
    where TService3 : class
    where TService4 : class
    where TService5 : class
    where TService6 : class
{
    /// <summary>
    /// <inheritdoc cref="ExposeServicesAttribute{TService1, TService2, TService3, TService4, TService5, TService6}"/>
    /// </summary>
    public ExposeServicesAttribute()
        : base(typeof(TService1)
            , typeof(TService2)
            , typeof(TService3)
            , typeof(TService4)
            , typeof(TService5)
            , typeof(TService6))
    {
    }
}

/// <summary>
/// 服务导出配置特性
/// </summary>
/// <typeparam name="TService1">服务类型</typeparam>
/// <typeparam name="TService2">服务类型</typeparam>
/// <typeparam name="TService3">服务类型</typeparam>
/// <typeparam name="TService4">服务类型</typeparam>
/// <typeparam name="TService5">服务类型</typeparam>
/// <typeparam name="TService6">服务类型</typeparam>
/// <typeparam name="TService7">服务类型</typeparam>
public sealed class ExposeServicesAttribute<TService1, TService2, TService3, TService4, TService5, TService6, TService7> : ExposeServicesAttribute
    where TService1 : class
    where TService2 : class
    where TService3 : class
    where TService4 : class
    where TService5 : class
    where TService6 : class
    where TService7 : class
{
    /// <summary>
    /// <inheritdoc cref="ExposeServicesAttribute{TService1, TService2, TService3, TService4, TService5, TService6, TService7}"/>
    /// </summary>
    public ExposeServicesAttribute()
        : base(typeof(TService1)
            , typeof(TService2)
            , typeof(TService3)
            , typeof(TService4)
            , typeof(TService5)
            , typeof(TService6)
            , typeof(TService7))
    {
    }
}

/// <summary>
/// 服务导出配置特性
/// </summary>
/// <typeparam name="TService1">服务类型</typeparam>
/// <typeparam name="TService2">服务类型</typeparam>
/// <typeparam name="TService3">服务类型</typeparam>
/// <typeparam name="TService4">服务类型</typeparam>
/// <typeparam name="TService5">服务类型</typeparam>
/// <typeparam name="TService6">服务类型</typeparam>
/// <typeparam name="TService7">服务类型</typeparam>
/// <typeparam name="TService8">服务类型</typeparam>
public sealed class ExposeServicesAttribute<TService1, TService2, TService3, TService4, TService5, TService6, TService7, TService8> : ExposeServicesAttribute
    where TService1 : class
    where TService2 : class
    where TService3 : class
    where TService4 : class
    where TService5 : class
    where TService6 : class
    where TService7 : class
    where TService8 : class
{
    /// <summary>
    /// <inheritdoc cref="ExposeServicesAttribute{TService1, TService2, TService3, TService4, TService5, TService6, TService7, TService8}"/>
    /// </summary>
    public ExposeServicesAttribute()
        : base(typeof(TService1)
            , typeof(TService2)
            , typeof(TService3)
            , typeof(TService4)
            , typeof(TService5)
            , typeof(TService6)
            , typeof(TService7)
            , typeof(TService8))
    {
    }
}