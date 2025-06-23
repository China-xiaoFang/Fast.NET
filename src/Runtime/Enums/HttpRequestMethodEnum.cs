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
/// <see cref="HttpRequestMethodEnum"/> Http请求方式枚举
/// </summary>
[FastEnum("Http请求方式枚举")]
public enum HttpRequestMethodEnum
{
    /// <summary>
    /// Get请求
    /// <remarks>用于从服务器获取资源。GET 请求将参数附加在 URL 后面，通过查询字符串传递给服务器。GET 请求是幂等的，即多次相同的 GET 请求应该返回相同的结果。</remarks>
    /// </summary>
    [Description("Get请求")]
    Get = 1,

    /// <summary>
    /// Post请求
    /// <remarks>用于向服务器提交数据并处理。POST 请求将参数包含在请求体中发送给服务器。POST 请求不是幂等的，即多次相同的 POST 请求可能会导致不同的结果。</remarks>
    /// </summary>
    [Description("Post请求")]
    Post = 2,

    /// <summary>
    /// Put请求
    /// <remarks>用于向服务器更新指定资源。PUT 请求将请求体中的数据保存到指定的 URL 上。</remarks>
    /// </summary>
    [Description("Put请求")]
    Put = 4,

    /// <summary>
    /// Delete请求
    /// <remarks>用于从服务器删除指定资源。DELETE 请求通过指定的 URL 删除服务器上的资源。</remarks>
    /// </summary>
    [Description("Delete请求")]
    Delete = 8,

    /// <summary>
    /// Patch请求
    /// <remarks>用于对服务器上的资源进行部分更新。PATCH 请求将请求体中的数据应用到指定的 URL 上，只更新部分字段。</remarks>
    /// </summary>
    [Description("Patch请求")]
    Patch = 16,

    /// <summary>
    /// Head请求
    /// <remarks>与 GET 请求类似，只是服务器返回的响应中不包含实体内容，主要用于获取资源的元数据（例如，响应头信息）。</remarks>
    /// </summary>
    [Description("Head请求")]
    Head = 32,

    /// <summary>
    /// Options请求
    /// <remarks>用于获取指定资源所支持的通信选项，也就是说，当客户端想知道服务器支持的请求方式、响应头等信息时，可以发送 OPTIONS 请求。</remarks>
    /// </summary>
    [Description("Options请求")]
    Options = 64,

    /// <summary>
    /// Connect请求
    /// <remarks>用于建立与目标资源的网络链接，通常用于 HTTPS 中的隧道ing，将流量转发给真正的 HTTPS 服务器。</remarks>
    /// </summary>
    [Description("Connect请求")]
    Connect = 128,

    /// <summary>
    /// Trace请求
    /// <remarks>用于追踪请求-响应的传输路径，主要用于故障诊断。</remarks>
    /// </summary>
    [Description("Trace请求")]
    Trace = 256
}