// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

namespace Fast.Admin.Core.Enum.Http;

/// <summary>
/// <see cref="HttpRequestMethodEnum"/> Http请求方式枚举
/// </summary>
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
    Put = 3,

    /// <summary>
    /// Delete请求
    /// <remarks>用于从服务器删除指定资源。DELETE 请求通过指定的 URL 删除服务器上的资源。</remarks>
    /// </summary>
    [Description("Delete请求")]
    Delete = 4,

    /// <summary>
    /// Patch请求
    /// <remarks>用于对服务器上的资源进行部分更新。PATCH 请求将请求体中的数据应用到指定的 URL 上，只更新部分字段。</remarks>
    /// </summary>
    [Description("Patch请求")]
    Patch = 4,

    /// <summary>
    /// Patch请求
    /// <remarks>与 GET 请求类似，只是服务器返回的响应中不包含实体内容，主要用于获取资源的元数据（例如，响应头信息）。</remarks>
    /// </summary>
    [Description("Patch请求")]
    Head = 5,

    /// <summary>
    /// Options请求
    /// <remarks>用于获取指定资源所支持的通信选项，也就是说，当客户端想知道服务器支持的请求方式、响应头等信息时，可以发送 OPTIONS 请求。</remarks>
    /// </summary>
    [Description("Options请求")]
    Options = 6,

    /// <summary>
    /// Connect请求
    /// <remarks>用于建立与目标资源的网络链接，通常用于 HTTPS 中的隧道ing，将流量转发给真正的 HTTPS 服务器。</remarks>
    /// </summary>
    [Description("Connect请求")]
    Connect = 7,

    /// <summary>
    /// Trace请求
    /// <remarks>用于追踪请求-响应的传输路径，主要用于故障诊断。</remarks>
    /// </summary>
    [Description("Trace请求")]
    Trace = 8
}