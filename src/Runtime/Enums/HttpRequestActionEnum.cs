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
/// HTTP 请求行为枚举
/// </summary>
[FastEnum("Http请求行为枚举")]
public enum HttpRequestActionEnum
{
    /// <summary>
    /// 未知
    /// </summary>
    [Description("未知")]
    None = 0,

    /// <summary>
    /// 鉴权
    /// </summary>
    [Description("鉴权")]
    Auth = 1,

    /// <summary>
    /// 分页
    /// </summary>
    [Description("分页")]
    Paged = 11,

    /// <summary>
    /// 查询
    /// </summary>
    [Description("查询")]
    Query = 12,

    /// <summary>
    /// 添加
    /// </summary>
    [Description("添加")]
    Add = 21,

    /// <summary>
    /// 编辑
    /// </summary>
    [Description("编辑")]
    Edit = 31,

    /// <summary>
    /// 删除
    /// </summary>
    [Description("删除")]
    Delete = 41,

    /// <summary>
    /// 提交
    /// </summary>
    [Description("提交")]
    Submit = 51,

    /// <summary>
    /// 上传
    /// </summary>
    [Description("上传")]
    Upload = 61,

    /// <summary>
    /// 下载
    /// </summary>
    [Description("下载")]
    Download = 62,

    /// <summary>
    /// 导入
    /// </summary>
    [Description("导入")]
    Import = 71,

    /// <summary>
    /// 导出
    /// </summary>
    [Description("导出")]
    Export = 72,

    /// <summary>
    /// 通知
    /// </summary>
    [Description("通知")]
    Notify = 253,

    /// <summary>
    /// 回调
    /// </summary>
    [Description("回调")]
    Callback = 254,

    /// <summary>
    /// 其他
    /// </summary>
    [Description("其他")]
    Other = 255
}
