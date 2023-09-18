using System;

namespace Fast.Logging.Implantations.Monitors;

/// <summary>
/// 控制跳过日志监视
/// </summary>
/// <remarks>作用于全局 <see cref="LoggingMonitorAttribute"/></remarks>
[ AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Parameter, Inherited = true, AllowMultiple = false)]
public sealed class SuppressMonitorAttribute : Attribute
{
}