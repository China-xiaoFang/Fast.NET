using System;

namespace Fast.Core.DataValidation.Attributes;

/// <summary>
/// 验证类型特性
/// </summary>
[AttributeUsage(AttributeTargets.Enum)]
public sealed class ValidationTypeAttribute : Attribute
{
}