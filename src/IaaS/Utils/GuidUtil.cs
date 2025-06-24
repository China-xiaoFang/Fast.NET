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

using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="GuidUtil"/> Guid 工具类
/// </summary>
public static class GuidUtil
{
    /// <summary>
    /// 生成一个Guid
    /// </summary>
    /// <remarks>
    /// <para>只支持 N D B P</para>
    /// <para>N ece4f4a60b764339b94a07c84e338a27</para>
    /// <para>D 5bf99df1-dc49-4023-a34a-7bd80a42d6bb</para>
    /// <para>B 2280f8d7-fd18-4c72-a9ab-405de3fcfbc9</para>
    /// <para>P 25e6e09f-fb66-4cab-b4cd-bfb429566549</para>
    /// </remarks>
    /// <param name="format"><see cref="string"/>格式化方式</param>
    /// <returns><see cref="string"/></returns>
    public static string GetGuid(string format = "N")
    {
        return Guid.NewGuid()
            .ToString(format);
    }

    /// <summary>
    /// 生成一个短的Guid
    /// </summary>
    /// <returns><see cref="string"/></returns>
    public static string GetShortGuid()
    {
        var i = Guid.NewGuid()
            .ToByteArray()
            .Aggregate<byte, long>(1, (current, b) => current * (b + 1));

        return $"{i - DateTime.Now.Ticks:x}";
    }
}