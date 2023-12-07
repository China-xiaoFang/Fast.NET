// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Fast.IaaS;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Fast.Logging.Implantation.Console;

/// <summary>
/// 控制台默认格式化选项拓展
/// </summary>
[SuppressSniffer]
public sealed class ConsoleFormatterExtendOptions : ConsoleFormatterOptions
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public ConsoleFormatterExtendOptions()
    {
        // 默认启用控制台日志上下文功能
        IncludeScopes = true;
    }

    /// <summary>
    /// 控制是否启用颜色
    /// </summary>
    public LoggerColorBehavior ColorBehavior { get; set; }

    /// <summary>
    /// 自定义日志消息格式化程序
    /// </summary>
    public Func<LogMessage, string> MessageFormat { get; set; }

    /// <summary>
    /// 日期格式化
    /// </summary>
    public string DateFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fffffff zzz dddd";

    /// <summary>
    /// 自定义格式化日志处理程序
    /// </summary>
    public Action<LogMessage, IExternalScopeProvider, TextWriter, string, ConsoleFormatterExtendOptions> WriteHandler
    {
        get;
        set;
    }

    /// <summary>
    /// 显示跟踪/请求 Id
    /// </summary>
    public bool WithTraceId { get; set; } = false;

    /// <summary>
    /// 显示堆栈框架（程序集和方法签名）
    /// </summary>
    public bool WithStackFrame { get; set; } = false;
}