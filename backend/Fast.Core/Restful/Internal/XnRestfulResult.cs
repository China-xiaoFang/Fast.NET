﻿using Furion.DependencyInjection;

namespace Fast.Core.Restful.Internal;

/// <summary>
/// RESTful风格---小诺返回格式
/// </summary>
/// <typeparam name="T"></typeparam>
[SuppressSniffer]
public class XnRestfulResult<T>
{
    /// <summary>
    /// 执行成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 状态码
    /// </summary>
    public int? Code { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public object Message { get; set; }

    /// <summary>
    /// 数据
    /// </summary>
    public T Data { get; set; }

    /// <summary>
    /// 附加数据
    /// </summary>
    public object Extras { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public long Timestamp { get; set; }
}