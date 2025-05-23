﻿// ------------------------------------------------------------------------
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

// ReSharper disable once CheckNamespace

namespace Fast.Logging;

/// <summary>
/// 文件日志写入器
/// </summary>
internal class FileLoggingWriter
{
    /// <summary>
    /// 文件日志记录器提供程序
    /// </summary>
    private readonly FileLoggerProvider _fileLoggerProvider;

    /// <summary>
    /// 日志配置选项
    /// </summary>
    private readonly FileLoggerOptions _options;

    /// <summary>
    /// 日志文件名
    /// </summary>
    private string _fileName;

    /// <summary>
    /// 文件流
    /// </summary>
    private Stream _fileStream;

    /// <summary>
    /// 文本写入器
    /// </summary>
    private TextWriter _textWriter;

    /// <summary>
    /// 缓存上次返回的基本日志文件名，避免重复解析
    /// </summary>
    private string __LastBaseFileName;

    /// <summary>
    /// 判断是否启动滚动日志功能
    /// </summary>
    private readonly bool _isEnabledRollingFiles;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="fileLoggerProvider">文件日志记录器提供程序</param>
    internal FileLoggingWriter(FileLoggerProvider fileLoggerProvider)
    {
        _fileLoggerProvider = fileLoggerProvider;
        _options = fileLoggerProvider.LoggerOptions;
        _isEnabledRollingFiles = _options.MaxRollingFiles > 0 && _options.FileSizeLimitBytes > 0;

        // 解析当前写入日志的文件名
        GetCurrentFileName();

        // 打开文件并持续写入
        OpenFile(true);
    }

    /// <summary>
    /// 获取日志基础文件名
    /// </summary>
    /// <returns>日志文件名</returns>
    private string GetBaseFileName()
    {
        var fileName = _fileLoggerProvider.FileName;

        // 如果配置了日志文件名格式化程序，则先处理再返回
        if (_options.FileNameRule != null)
            fileName = _options.FileNameRule(fileName);

        return fileName;
    }

    /// <summary>
    /// 解析当前写入日志的文件名
    /// </summary>
    private void GetCurrentFileName()
    {
        // 获取日志基础文件名并将其缓存
        var baseFileName = GetBaseFileName();
        __LastBaseFileName = baseFileName;

        // 是否配置了日志文件最大存储大小
        if (_options.FileSizeLimitBytes > 0)
        {
            // 定义文件查找通配符
            var logFileMask = Path.GetFileNameWithoutExtension(baseFileName) + "*" + Path.GetExtension(baseFileName);

            // 获取文件路径
            var logDirName = Path.GetDirectoryName(baseFileName);

            // 如果没有配置文件路径则默认放置根目录
            if (string.IsNullOrEmpty(logDirName))
                logDirName = Directory.GetCurrentDirectory();

            // 在当前目录下根据文件通配符查找所有匹配的文件
            var logFiles = Directory.Exists(logDirName)
                ? Directory.GetFiles(logDirName, logFileMask, SearchOption.TopDirectoryOnly)
                : Array.Empty<string>();

            // 处理已有日志文件存在情况
            if (logFiles.Length > 0)
            {
                // 根据文件名和最后更新时间获取最近操作的文件
                var lastFileInfo = logFiles.Select(fName => new FileInfo(fName)).OrderByDescending(fInfo => fInfo.Name)
                    .ThenByDescending(fInfo => fInfo.LastWriteTime).First();

                _fileName = lastFileInfo.FullName;
            }
            // 没有任何匹配的日志文件直接使用当前基础文件名
            else
                _fileName = baseFileName;
        }
        else
            _fileName = baseFileName;
    }

    /// <summary>
    /// 获取下一个匹配的日志文件名
    /// </summary>
    /// <remarks>只有配置了 <see cref="FileLoggerOptions.FileSizeLimitBytes"/> 或 <see cref="FileLoggerOptions.FileNameRule"/> 或 <see cref="FileLoggerOptions.MaxRollingFiles"/> 有效</remarks>
    /// <returns>新的文件名</returns>
    private string GetNextFileName()
    {
        // 获取日志基础文件名
        var baseFileName = GetBaseFileName();

        // 如果文件不存在或没有达到 FileSizeLimitBytes 限制大小，则返回基础文件名
        if (!File.Exists(baseFileName) || _options.FileSizeLimitBytes <= 0 ||
            new FileInfo(baseFileName).Length < _options.FileSizeLimitBytes)
            return baseFileName;

        // 获取日志基础文件名和当前日志文件名
        var currentFileIndex = 0;
        var baseFileNameOnly = Path.GetFileNameWithoutExtension(baseFileName);
        var currentFileNameOnly = Path.GetFileNameWithoutExtension(_fileName);

        // 解析日志文件名【递增】部分
        var suffix = currentFileNameOnly?[baseFileNameOnly.Length..];
        if (suffix?.Length > 0 && int.TryParse(suffix, out var parsedIndex))
        {
            currentFileIndex = parsedIndex;
        }

        // 【递增】部分 +1
        var nextFileIndex = currentFileIndex + 1;

        // 如果配置了最大【递增】数，则超出自动从头开始（覆盖写入）
        if (_options.MaxRollingFiles > 0)
        {
            nextFileIndex %= _options.MaxRollingFiles;
        }

        // 返回下一个匹配的日志文件名（完整路径）
        var nextFileName = baseFileNameOnly + (nextFileIndex > 0 ? nextFileIndex.ToString() : "") +
                           Path.GetExtension(baseFileName);
        return Path.Combine(Path.GetDirectoryName(baseFileName), nextFileName);
    }

    /// <summary>
    /// 打开文件
    /// </summary>
    /// <param name="append"><see cref="bool"/>追加还是覆盖</param>
    private void OpenFile(bool append)
    {
        try
        {
            CreateFileStream();
        }
        catch (Exception ex)
        {
            // 处理文件写入错误
            if (_options.HandleWriteError != null)
            {
                var fileWriteError = new FileWriteError(_fileName, ex);
                _options.HandleWriteError(fileWriteError);

                // 如果配置了备用文件名，则重新写入
                if (fileWriteError.RollbackFileName != null)
                {
                    _fileLoggerProvider.FileName = fileWriteError.RollbackFileName;

                    // 递归操作，直到应用程序停止
                    GetCurrentFileName();
                    CreateFileStream();
                }
            }
            // 其他直接抛出异常
            else
                throw;
        }

        // 初始化文本写入器
        _textWriter = new StreamWriter(_fileStream);

        // 创建文件流
        void CreateFileStream()
        {
            var fileInfo = new FileInfo(_fileName);

            // 判断文件目录是否存在，不存在则自动创建
            fileInfo.Directory?.Create();

            // 创建文件流，采用共享锁方式
            _fileStream = new FileStream(_fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite, 4096,
                FileOptions.WriteThrough);

            // 删除超出滚动日志限制的文件
            DropFilesIfOverLimit(fileInfo);

            // 判断是否追加还是覆盖
            if (append)
            {
                _fileStream.Seek(0, SeekOrigin.End);
            }
            else
            {
                _fileStream.SetLength(0);
            }
        }
    }

    /// <summary>
    /// 判断是否需要创建新文件写入
    /// </summary>
    private void CheckForNewLogFile()
    {
        var openNewFile = false;
        if (isMaxFileSizeThresholdReached() || isBaseFileNameChanged())
            openNewFile = true;

        // 重新创建新文件并写入
        if (openNewFile)
        {
            Close();

            // 计算新文件名
            _fileName = GetNextFileName();

            // 打开新文件并写入
            OpenFile(false);
        }

        // 是否超出限制的最大大小
        bool isMaxFileSizeThresholdReached() =>
            _options.FileSizeLimitBytes > 0 && _fileStream.Length > _options.FileSizeLimitBytes;

        // 是否重新自定义了文件名
        bool isBaseFileNameChanged()
        {
            if (_options.FileNameRule != null)
            {
                var baseFileName = GetBaseFileName();

                if (baseFileName != __LastBaseFileName)
                {
                    __LastBaseFileName = baseFileName;
                    return true;
                }

                return false;
            }

            return false;
        }
    }

    /// <summary>
    /// 删除超出滚动日志限制的文件
    /// </summary>
    /// <param name="fileInfo"></param>
    private void DropFilesIfOverLimit(FileInfo fileInfo)
    {
        // 判断是否启用滚动文件功能
        if (!_isEnabledRollingFiles)
            return;

        // 处理 Windows 和 Linux 路径分隔符不一致问题
        var fName = fileInfo.FullName.Replace('\\', '/');

        // 将当前文件名存储到集合中
        var succeed = _fileLoggerProvider._rollingFileNames.TryAdd(fName, fileInfo);

        // 判断超出限制的文件自动删除
        if (succeed && _fileLoggerProvider._rollingFileNames.Count > _options.MaxRollingFiles)
        {
            // 根据最后写入时间删除过时日志
            var dropFiles = _fileLoggerProvider._rollingFileNames.OrderBy(u => u.Value.LastWriteTimeUtc)
                .Take(_fileLoggerProvider._rollingFileNames.Count - _options.MaxRollingFiles);

            // 遍历所有需要删除的文件
            foreach (var rollingFile in dropFiles)
            {
                var removeSucceed = _fileLoggerProvider._rollingFileNames.TryRemove(rollingFile.Key, out _);
                if (!removeSucceed)
                    continue;

                // 执行删除
                Task.Run(() =>
                {
                    if (File.Exists(rollingFile.Key))
                        File.Delete(rollingFile.Key);
                });
            }
        }
    }

    /// <summary>
    /// 写入文件
    /// </summary>
    /// <param name="logMsg">日志消息</param>
    /// <param name="flush"></param>
    internal void Write(LogMessage logMsg, bool flush)
    {
        if (_textWriter == null)
            return;

        CheckForNewLogFile();
        _textWriter.WriteLine(logMsg.Message);

        if (flush)
            _textWriter.Flush();
    }

    /// <summary>
    /// 关闭文本写入器并释放
    /// </summary>
    internal void Close()
    {
        if (_textWriter == null)
            return;

        var textloWriter = _textWriter;
        _textWriter = null;

        textloWriter.Dispose();
        _fileStream.Dispose();

        _fileStream = null;
    }
}