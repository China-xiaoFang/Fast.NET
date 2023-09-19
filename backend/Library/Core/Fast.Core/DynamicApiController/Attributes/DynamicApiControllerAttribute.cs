using System;

namespace Fast.Core.DynamicApiController.Attributes;

/// <summary>
/// 动态 WebApi 特性
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class DynamicApiControllerAttribute : Attribute
{
}