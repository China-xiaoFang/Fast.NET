using System.Reflection;
using Fast.Core.FriendlyException.Attributes;

namespace Fast.Core.FriendlyException.Internal;

/// <summary>
/// 方法异常类
/// </summary>
internal sealed class MethodIfException
{
    /// <summary>
    /// 出异常的方法
    /// </summary>
    public MethodBase ErrorMethod { get; set; }

    /// <summary>
    /// 异常特性
    /// </summary>
    public IEnumerable<IfExceptionAttribute> IfExceptionAttributes { get; set; }
}