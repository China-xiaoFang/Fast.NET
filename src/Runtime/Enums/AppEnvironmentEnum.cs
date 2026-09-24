// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.ComponentModel;


// ReSharper disable once CheckNamespace
namespace System;

/// <summary>
/// App 运行环境枚举
/// </summary>
[Flags]
[FastEnum("App运行环境枚举")]
public enum AppEnvironmentEnum : long
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
    /// 微信公众号
    /// </summary>
    [Description("微信公众号")]
    WeChatOfficialAccount = 512,

    /// <summary>
    /// 微信服务号
    /// </summary>
    [Description("微信服务号")]
    WeChatServiceAccount = 1024,

    /// <summary>
    /// 微信开放平台
    /// </summary>
    [Description("微信开放平台")]
    WeChatOpenPlatform = 2048,

    /// <summary>
    /// 企业微信
    /// </summary>
    [Description("企业微信")]
    WorkWeChat = 4096,

    /// <summary>
    /// 支付宝小程序
    /// </summary>
    [Description("支付宝小程序")]
    AlipayMiniProgram = 8192,

    /// <summary>
    /// 抖音小程序
    /// </summary>
    [Description("抖音小程序")]
    TiktokMiniProgram = 16384,

    /// <summary>
    /// 钉钉小程序
    /// </summary>
    [Description("钉钉小程序")]
    DingTalkMiniProgram = 32768,

    /// <summary>
    /// 飞书小程序
    /// </summary>
    [Description("飞书小程序")]
    FeiShuMiniProgram = 65536,

    /// <summary>
    /// QQ 小程序
    /// </summary>
    [Description("QQ小程序")]
    QQMiniProgram = 131072,

    /// <summary>
    /// 百度小程序
    /// </summary>
    [Description("百度小程序")]
    BaiduMiniProgram = 262144,

    /// <summary>
    /// 快手小程序
    /// </summary>
    [Description("快手小程序")]
    KuaiShouMiniProgram = 524288,

    /// <summary>
    /// 小红书小程序
    /// </summary>
    [Description("小红书小程序")]
    XiaoHongShuMiniProgram = 1048576,

    /// <summary>
    /// 京东小程序
    /// </summary>
    [Description("京东小程序")]
    JDMiniProgram = 2097152,

    /// <summary>
    /// API
    /// </summary>
    [Description("Api")]
    Api = 2147483648,

    /// <summary>
    /// 其他
    /// </summary>
    [Description("其他")]
    Other = 4294967296,

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
                  | AlipayMiniProgram
                  | TiktokMiniProgram
                  | DingTalkMiniProgram
                  | FeiShuMiniProgram
                  | QQMiniProgram
                  | BaiduMiniProgram
                  | KuaiShouMiniProgram
                  | XiaoHongShuMiniProgram
                  | JDMiniProgram
}
