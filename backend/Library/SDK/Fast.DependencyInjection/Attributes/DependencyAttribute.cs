using System;

namespace Furion.DependencyInjection;

/// <summary>
/// 依赖关系配置特性
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class DependencyAttribute : Attribute
{
    /// <summary>
    /// <inheritdoc cref="DependencyAttribute"/>
    /// </summary>
    public DependencyAttribute()
        : this(RegistrationTypeEnum.Add)
    {
    }

    /// <summary>
    /// <inheritdoc cref="DependencyAttribute"/>
    /// </summary>
    /// <param name="addition"><see cref="RegistrationTypeEnum"/></param>
    public DependencyAttribute(RegistrationTypeEnum addition)
    {
        RegistrationType = addition;
    }

    /// <inheritdoc cref="RegistrationTypeEnum"/>
    public RegistrationTypeEnum RegistrationType { get; init; }

    /// <summary>
    /// 忽略注册
    /// </summary>
    public bool Ignore { get; init; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>值越大则注册越晚</remarks>
    public int Order { get; init; }

    /// <summary>
    /// 包含自身服务
    /// </summary>
    public bool IncludeSelf { get; init; }

    /// <summary>
    /// 包含基类服务
    /// </summary>
    public bool IncludeBase { get; init; }
}