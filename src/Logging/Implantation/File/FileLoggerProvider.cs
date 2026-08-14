// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Fast.Logging;

/// <summary>
/// 文件日志记录器提供程序
/// </summary>
/// <remarks>实现遵循 Microsoft.Extensions.Logging 自定义日志提供器约定</remarks>
[ProviderAlias("File")]
internal sealed class FileLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    /// <summary>
    /// 存储多日志分类日志记录器
    /// </summary>
    private readonly ConcurrentDictionary<string, FileLogger> _fileLoggers = new();

    /// <summary>
    /// 日志消息队列（线程安全）
    /// </summary>
    private readonly BlockingCollection<LogMessage> _logMessageQueue = new(1024);

    /// <summary>
    /// 日志作用域提供器
    /// </summary>
    private IExternalScopeProvider _scopeProvider;

    /// <summary>
    /// 记录日志所有滚动文件名
    /// </summary>
    /// <remarks>只有 MaxRollingFiles 和 FileSizeLimitBytes 大于 0 有效</remarks>
    internal readonly ConcurrentDictionary<string, FileInfo> _rollingFileNames = new();

    /// <summary>
    /// 文件日志写入器
    /// </summary>
    private readonly FileLoggingWriter _fileLoggingWriter;

    /// <summary>
    /// 长时间运行的后台任务
    /// </summary>
    /// <remarks>实现不间断写入</remarks>
    private readonly Task _processQueueTask;

    /// <summary>
    /// 是否已经释放
    /// </summary>
    private int _disposed;

    /// <summary>
    /// 当前队列饱和周期是否已输出警告
    /// </summary>
    private int _queueFullWarningEmitted;

    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    /// <param name="fileName">日志文件名</param>
    /// <param name="fileLoggerOptions">文件日志记录器配置选项</param>
    public FileLoggerProvider(string fileName, FileLoggerOptions fileLoggerOptions)
    {
        // 支持文件名嵌入系统环境变量，格式为：%SystemDrive%，%SystemRoot%，处理 Windows 和 Linux 路径分隔符不一致问题
        // 使用 Path.GetFullPath 将相对路径转换为绝对路径，避免在 Linux 下因工作目录与应用程序目录不一致导致日志文件无法写入
        FileName = Path.GetFullPath(Environment.ExpandEnvironmentVariables(fileName)
            .Replace('\\', '/'));
        LoggerOptions = fileLoggerOptions;

        // 创建文件日志写入器
        _fileLoggingWriter = new FileLoggingWriter(this);

        // 创建长时间运行的后台任务，并将日志消息队列中数据写入文件中
        _processQueueTask = Task.Factory.StartNew(state => ((FileLoggerProvider) state).ProcessQueue(), this,
            CancellationToken.None, TaskCreationOptions.LongRunning | TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
    }

    /// <summary>
    /// 文件名
    /// </summary>
    internal string FileName;

    /// <summary>
    /// 文件日志记录器配置选项
    /// </summary>
    internal FileLoggerOptions LoggerOptions { get; private set; }

    /// <summary>
    /// 日志作用域提供器
    /// </summary>
    internal IExternalScopeProvider ScopeProvider
    {
        get
        {
            _scopeProvider ??= new LoggerExternalScopeProvider();
            return _scopeProvider;
        }
    }

    /// <inheritdoc />
    public ILogger CreateLogger(string categoryName)
    {
        return _fileLoggers.GetOrAdd(categoryName, name => new FileLogger(name, this));
    }

    /// <inheritdoc />
    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        _scopeProvider = scopeProvider;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        // 标记日志消息队列停止写入
        _logMessageQueue.CompleteAdding();

        try
        {
            // CompleteAdding 会让消费循环在排空队列后自然结束；等待完成可避免关闭文件时仍有后台写入
            _processQueueTask.GetAwaiter()
                .GetResult();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[Fast.Logging] Failed to drain the log queue: {ex.Message}");
        }

        // 清空文件日志记录器
        _fileLoggers.Clear();

        // 清空滚动文件名记录器
        _rollingFileNames.Clear();

        // 释放内部文件写入器
        _fileLoggingWriter.Dispose();
        _logMessageQueue.Dispose();
        _processQueueTask.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 将日志消息写入队列中等待后台任务出队写入文件
    /// </summary>
    /// <param name="logMsg">日志消息</param>
    internal void WriteToQueue(LogMessage logMsg)
    {
        if (Volatile.Read(ref _disposed) != 0)
            return;

        try
        {
            // 只有队列可持续入队才写入
            if (_logMessageQueue.IsAddingCompleted)
                return;

            // 使用 TryAdd 非阻塞写入，避免后台任务异常退出时队列满导致调用方线程永久阻塞
            if (_logMessageQueue.TryAdd(logMsg))
            {
                Volatile.Write(ref _queueFullWarningEmitted, 0);
                return;
            }

            // 每个连续饱和周期只输出一次，既暴露日志丢弃，又避免持续写满标准错误流
            if (Interlocked.Exchange(ref _queueFullWarningEmitted, 1) == 0)
                Console.Error.WriteLine("[Fast.Logging] Log queue is full; new log messages are being dropped.");
        }
        catch (ObjectDisposedException)
        {
            // Dispose 与生产者并发时属于正常关闭流程
        }
        catch (InvalidOperationException)
        {
            // CompleteAdding 与生产者并发时属于正常关闭流程
        }
    }

    /// <summary>
    /// 将日志消息写入文件中
    /// </summary>
    private void ProcessQueue()
    {
        foreach (var logMsg in _logMessageQueue.GetConsumingEnumerable())
        {
            try
            {
                _fileLoggingWriter.Write(logMsg, _logMessageQueue.Count == 0);
            }
            catch (Exception ex)
            {
                // 保持消费线程继续运行，同时把无法写入文件的事实暴露给宿主诊断通道
                Console.Error.WriteLine($"[Fast.Logging] Failed to write log file '{FileName}': {ex.Message}");
            }
        }
    }
}
