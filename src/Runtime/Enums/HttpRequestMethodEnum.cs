// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using System.ComponentModel;


// ReSharper disable once CheckNamespace
namespace System;

/// <summary>
/// HTTP 请求方式枚举
/// </summary>
[FastEnum("Http请求方式枚举")]
public enum HttpRequestMethodEnum
{
    /// <summary>
    /// 未知请求
    /// </summary>
    /// <remarks>当前不在 HTTP 请求上下文中，或请求方式无法识别</remarks>
    [Description("未知请求")]
    Unknown = 0,

    /// <summary>
    /// Get 请求
    /// </summary>
    /// <remarks>用于从服务器获取资源。GET 请求将参数附加在 URL 后面，通过查询字符串传递给服务器。GET 请求是幂等的，即多次相同的 GET 请求应该返回相同的结果</remarks>
    [Description("Get请求")]
    Get = 1,

    /// <summary>
    /// Post 请求
    /// </summary>
    /// <remarks>用于向服务器提交数据并处理。POST 请求将参数包含在请求体中发送给服务器。POST 请求不是幂等的，即多次相同的 POST 请求可能会导致不同的结果</remarks>
    [Description("Post请求")]
    Post = 2,

    /// <summary>
    /// Put 请求
    /// </summary>
    /// <remarks>用于向服务器更新指定资源。PUT 请求将请求体中的数据保存到指定的 URL 上</remarks>
    [Description("Put请求")]
    Put = 4,

    /// <summary>
    /// Delete 请求
    /// </summary>
    /// <remarks>用于从服务器删除指定资源。DELETE 请求通过指定的 URL 删除服务器上的资源</remarks>
    [Description("Delete请求")]
    Delete = 8,

    /// <summary>
    /// Patch 请求
    /// </summary>
    /// <remarks>用于对服务器上的资源进行部分更新。PATCH 请求将请求体中的数据应用到指定的 URL 上，只更新部分字段</remarks>
    [Description("Patch请求")]
    Patch = 16,

    /// <summary>
    /// Head 请求
    /// </summary>
    /// <remarks>与 GET 请求类似，只是服务器返回的响应中不包含实体内容，主要用于获取资源的元数据（例如，响应头信息）</remarks>
    [Description("Head请求")]
    Head = 32,

    /// <summary>
    /// Options 请求
    /// </summary>
    /// <remarks>用于获取指定资源所支持的通信选项，也就是说，当客户端想知道服务器支持的请求方式、响应头等信息时，可以发送 OPTIONS 请求</remarks>
    [Description("Options请求")]
    Options = 64,

    /// <summary>
    /// Connect 请求
    /// </summary>
    /// <remarks>用于建立与目标资源的网络链接，通常用于 HTTPS 中的隧道 ing，将流量转发给真正的 HTTPS 服务器</remarks>
    [Description("Connect请求")]
    Connect = 128,

    /// <summary>
    /// Trace 请求
    /// </summary>
    /// <remarks>用于追踪请求-响应的传输路径，主要用于故障诊断</remarks>
    [Description("Trace请求")]
    Trace = 256
}
