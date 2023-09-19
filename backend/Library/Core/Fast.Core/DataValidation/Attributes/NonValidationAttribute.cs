namespace Fast.Core.DataValidation.Attributes;

/// <summary>
/// 跳过验证
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class NonValidationAttribute : Attribute
{
}