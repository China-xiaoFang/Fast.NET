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

using Microsoft.AspNetCore.Http;

namespace Fast.UnifyResult;

/// <summary>
/// 为 <see cref="HttpContext"/> 提供扩展方法
/// </summary>
internal static class HttpContextExtension
{
    /// <summary>
    /// 设置规范化响应时间戳
    /// </summary>
    /// <param name="httpContext">当前请求上下文</param>
    /// <param name="timestamp">要写入响应头的毫秒时间戳</param>
    public static void UnifyResponseTimestamp(this HttpContext httpContext, long timestamp)
    {
        httpContext?.Response.Headers.TryAdd(nameof(Fast) + "-NET-Timestamp", $"{timestamp}");
    }

    /// <summary>
    /// 获取规范化响应时间戳
    /// </summary>
    /// <param name="httpContext">当前请求上下文</param>
    /// <returns>响应时间戳</returns>
    public static long UnifyResponseTimestamp(this HttpContext httpContext)
    {
        var timestampStr = httpContext?.Response.Headers[nameof(Fast) + "-NET-Timestamp"];

        if (string.IsNullOrEmpty(timestampStr))
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // 将请求开始时间写入响应头，供耗时统计复用
            httpContext.UnifyResponseTimestamp(timestamp);

            return timestamp;
        }

        return long.Parse(timestampStr);
    }

    /// <summary>
    /// 获取终结点元数据中的指定特性
    /// </summary>
    /// <param name="metadata">终结点元数据集合</param>
    /// <param name="attributeType">要读取的特性类型</param>
    /// <returns>匹配的特性实例；不存在时返回 <see langword="null"/></returns>
    public static object GetMetadata(this EndpointMetadataCollection metadata, Type attributeType)
    {
        return metadata?.GetType()
            .GetMethod(nameof(EndpointMetadataCollection.GetMetadata))
            ?.MakeGenericMethod(attributeType)
            .Invoke(metadata, null);
    }

    /// <summary>
    /// 获取终结点元数据中的指定特性
    /// </summary>
    /// <param name="httpContext">当前请求上下文</param>
    /// <param name="attributeType">要读取的特性类型</param>
    /// <returns>当前终结点上匹配的特性实例；不存在时返回 <see langword="null"/></returns>
    public static object GetMetadata(this HttpContext httpContext, Type attributeType)
    {
        return httpContext?.GetEndpoint()
            ?.Metadata.GetMetadata(attributeType);
    }
}
