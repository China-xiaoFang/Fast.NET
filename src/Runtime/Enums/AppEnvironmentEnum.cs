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

using System.ComponentModel;

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>
/// <see cref="AppEnvironmentEnum"/> App运行环境枚举
/// </summary>
[Flags]
[FastEnum("App运行环境枚举")]
public enum AppEnvironmentEnum
{
    /// <summary>
    /// Web
    /// </summary>
    [Description("Web")]
    Web = 1,

    /// <summary>
    /// Windows
    /// </summary>
    [Description("Windows")]
    Windows = 2,

    /// <summary>
    /// Mac
    /// </summary>
    [Description("Mac")]
    Mac = 4,

    /// <summary>
    /// Linux
    /// </summary>
    [Description("Linux")]
    Linux = 8,

    /// <summary>
    /// Android
    /// </summary>
    [Description("Android")]
    Android = 32,

    /// <summary>
    /// IOS
    /// </summary>
    [Description("IOS")]
    IOS = 64,

    /// <summary>
    /// 快应用
    /// </summary>
    [Description("快应用")]
    QuickApp = 128,

    /// <summary>
    /// 微信小程序
    /// </summary>
    [Description("微信小程序")]
    WeChatMiniProgram = 256,

    /// <summary>
    /// QQ小程序
    /// </summary>
    [Description("QQ小程序")]
    QQMiniProgram = 512,

    /// <summary>
    /// 抖音小程序
    /// </summary>
    [Description("抖音小程序")]
    TiktokMiniProgram = 1024,

    /// <summary>
    /// 百度小程序
    /// </summary>
    [Description("百度小程序")]
    BaiduMiniProgram = 2048,

    /// <summary>
    /// 支付宝小程序
    /// </summary>
    [Description("支付宝小程序")]
    AlipayMiniProgram = 4096,

    /// <summary>
    /// 快手小程序
    /// </summary>
    [Description("快手小程序")]
    KuaishouMiniProgram = 8192,

    /// <summary>
    /// 飞书小程序
    /// </summary>
    [Description("飞书小程序")]
    FeishuMiniProgram = 16384,

    /// <summary>
    /// 钉钉小程序
    /// </summary>
    [Description("钉钉小程序")]
    DingTalkMiniProgram = 32768,

    /// <summary>
    /// 京东小程序
    /// </summary>
    [Description("京东小程序")]
    JDMiniProgram = 65536,

    /// <summary>
    /// 小红书小程序
    /// </summary>
    [Description("小红书小程序")]
    XiaohongshuMiniProgram = 131072,

    /// <summary>
    /// Api
    /// </summary>
    [Description("Api")]
    Api = 16777216,

    /// <summary>
    /// 其他
    /// </summary>
    [Description("其他")]
    Other = 1073741824,

    /// <summary>
    /// 桌面端
    /// </summary>
    [Description("桌面端")]
    Desktop = Windows | Mac | Linux,

    /// <summary>
    /// 移动端
    /// </summary>
    [Description("移动端")]
    Mobile = Android | IOS,

    /// <summary>
    /// 移动端（三端）
    /// </summary>
    [Description("移动端（三端）")]
    MobileThree = Android | IOS | MiniProgram,

    /// <summary>
    /// 小程序
    /// </summary>
    [Description("小程序")]
    MiniProgram = QuickApp
                  | WeChatMiniProgram
                  | QQMiniProgram
                  | TiktokMiniProgram
                  | BaiduMiniProgram
                  | AlipayMiniProgram
                  | KuaishouMiniProgram
                  | FeishuMiniProgram
                  | DingTalkMiniProgram
                  | JDMiniProgram
                  | XiaohongshuMiniProgram
}