namespace Fast.ServiceCollection.Internal;

/// <summary>
/// App运行环境枚举
/// </summary>
[FastEnum("App运行环境枚举")]
[Flags]
public enum AppEnvironmentEnum
{
    /// <summary>
    /// PC
    /// </summary>
    [Description("PC")]
    PC = 1,

    /// <summary>
    /// Windows端
    /// </summary>
    [Description("Windows")]
    Windows = 2,

    /// <summary>
    /// App端
    /// </summary>
    [Description("App")]
    App = 4,

    /// <summary>
    /// H5
    /// </summary>
    [Description("H5")]
    H5 = 8,

    /// <summary>
    /// 微信小程序
    /// </summary>
    [Description("微信小程序")]
    WeChatMiniProgram = 16,
}