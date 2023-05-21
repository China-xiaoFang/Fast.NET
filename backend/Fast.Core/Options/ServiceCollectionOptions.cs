namespace Fast.Core.Options;

/// <summary>
/// 服务集合配置
/// </summary>
public class ServiceCollectionOptions
{
    /// <summary>
    /// 控制台日志时间格式
    /// </summary>
    public string ConsoleDataTimeFormatter { get; set; } = "yyyy-MM-dd HH:mm:ss(zzz) dddd";

    /// <summary>
    /// 添加上传文件大小限制
    /// </summary>
    public bool LimitUploadedFile { get; set; } = true;

    /// <summary>
    /// 添加Gzip Brotli 压缩
    /// </summary>
    public bool GzipBrotliCompression { get; set; } = true;

    /// <summary>
    /// 启用JWT
    /// </summary>
    public bool JWT { get; set; }

    /// <summary>
    /// 多语言
    /// </summary>
    public bool AppLocalization { get; set; } = true;

    /// <summary>
    /// 接口限流
    /// </summary>
    public bool RequestLimit { get; set; } = true;

    /// <summary>
    /// 请求AES解密
    /// </summary>
    public bool RequestAESDecrypt { get; set; } = true;

    /// <summary>
    /// 演示环境请求判断
    /// </summary>
    public bool DemoEnvironmentRequest { get; set; } = true;

    /// <summary>
    /// JSON序列化
    /// </summary>
    public bool JsonOptions { get; set; } = true;

    /// <summary>
    /// JSON序列化时间格式
    /// </summary>
    public string JsonOptionDateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

    /// <summary>
    /// 雪花Id WorkerId 
    /// </summary>
    public string SnowIdWorkerId { get; set; } = "1";

    /// <summary>
    /// 即时通讯
    /// </summary>
    public bool SignalR { get; set; }

    /// <summary>
    /// 日志文件
    /// </summary>
    public bool Log { get; set; } = true;

    /// <summary>
    /// 日志文件格式
    /// </summary>
    public string LogFileFormat { get; set; } = "{0:yyyy}-{0:MM}-{0:dd}";

    /// <summary>
    /// 日志文件大小 控制每一个日志文件最大存储大小，默认无限制，单位是 B，也就是 1024 才等于 1KB
    /// </summary>
    public long LogFileSizeLimitBytes { get; set; } = 10 * 1024 * 1024;

    /// <summary>
    /// 事件总线
    /// </summary>
    public bool EventBusService { get; set; } = true;

    /// <summary>
    /// 任务调度
    /// </summary>
    public bool Scheduler { get; set; } = true;
}