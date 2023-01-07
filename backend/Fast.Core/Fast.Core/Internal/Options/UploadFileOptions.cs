namespace Fast.Core.Internal.Options;

/// <summary>
/// 上传文件配置
/// </summary>
public class UploadFileOptions
{
    /// <summary>
    /// 头像
    /// </summary>
    public FileDescription Avatar { get; set; }

    /// <summary>
    /// 编辑器
    /// </summary>
    public FileDescription Editor { get; set; }

    /// <summary>
    /// 默认
    /// </summary>
    public FileDescription Default { get; set; }

    /// <summary>
    /// 文件参数
    /// </summary>
    public class FileDescription
    {
        /// <summary>
        /// 路径
        /// </summary>
        public string path { get; set; }

        /// <summary>
        /// 大小
        /// </summary>
        public long maxSize { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string[] contentType { get; set; }
    }
}