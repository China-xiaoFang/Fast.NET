namespace Hello.NET.Core.AttributeFilter;

/// <summary>
/// 任务调度服务属性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class ScheduledServer : Attribute
{
    /// <summary>
    /// 服务名称
    /// </summary>
    public string ServerName { get; set; }

    /// <summary>
    /// 分组名称
    /// </summary>
    public string GroupName { get; set; }
}