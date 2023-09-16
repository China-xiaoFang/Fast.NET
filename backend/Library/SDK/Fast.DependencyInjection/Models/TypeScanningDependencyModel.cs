
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Furion.DependencyInjection;

/// <summary>
/// 类型扫描依赖关系模型
/// </summary>
public sealed class TypeScanningDependencyModel
{
    /// <summary>
    /// <inheritdoc cref="TypeScanningDependencyModel"/>
    /// </summary>
    /// <param name="serviceType">服务类型</param>
    /// <param name="implementationType">实现服务类型</param>
    /// <param name="serviceLifetime"><see cref="ServiceLifetime"/></param>
    /// <param name="registrationType"><see cref="RegistrationType"/></param>
    internal TypeScanningDependencyModel(Type serviceType
        , Type implementationType
        , ServiceLifetime serviceLifetime
        , RegistrationTypeEnum registrationType)
    {
        // 空检查
        ArgumentNullException.ThrowIfNull(serviceType);
        ArgumentNullException.ThrowIfNull(implementationType);

        Descriptor = ServiceDescriptor.Describe(serviceType, implementationType, serviceLifetime);
        RegistrationType = registrationType;
    }

    /// <inheritdoc cref="ServiceDescriptor"/>
    public ServiceDescriptor Descriptor { get; init; }

    /// <inheritdoc cref="RegistrationType"/>
    public RegistrationTypeEnum RegistrationType { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    /// <remarks>值越大则注册越晚</remarks>
    public int Order { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Registration = {RegistrationType}, Order = {Order}, ServiceType = {Descriptor.ServiceType}, Lifetime = {Descriptor.Lifetime}, ImplementationType = {Descriptor.ImplementationType}";
    }
}