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

// ReSharper disable once CheckNamespace

namespace Fast.NET;

/// <summary>
/// <see cref="InternalGuidUtil"/> Guid 工具类
/// </summary>
internal static class InternalGuidUtil
{
    /// <summary>
    /// 生成一个Guid
    /// <remarks>
    /// <para>只支持 N D B P</para>
    /// <para>N ece4f4a60b764339b94a07c84e338a27</para>
    /// <para>D 5bf99df1-dc49-4023-a34a-7bd80a42d6bb</para>
    /// <para>B 2280f8d7-fd18-4c72-a9ab-405de3fcfbc9</para>
    /// <para>P 25e6e09f-fb66-4cab-b4cd-bfb429566549</para>
    /// </remarks>
    /// </summary>
    /// <param name="format"><see cref="string"/>格式化方式</param>
    /// <returns><see cref="string"/></returns>
    internal static string GetGuid(string format = "N")
    {
        return Guid.NewGuid().ToString(format);
    }

    /// <summary>
    /// 生成一个短的Guid
    /// </summary>
    /// <returns><see cref="string"/></returns>
    internal static string GetShortGuid()
    {
        var i = Guid.NewGuid().ToByteArray().Aggregate<byte, long>(1, (current, b) => current * (b + 1));

        return $"{i - DateTime.Now.Ticks:x}";
    }
}