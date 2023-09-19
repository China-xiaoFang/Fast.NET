namespace Fast.Core.UnifyResult.Attributes;

/// <summary>
/// 规范化序列化配置
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class UnifySerializerSettingAttribute : Attribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="name"></param>
    public UnifySerializerSettingAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// 序列化名称
    /// </summary>
    public string Name { get; set; }
}