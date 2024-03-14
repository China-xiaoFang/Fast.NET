// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
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

namespace Fast.EventBus.Storers;

/// <summary>
/// <see cref="IEventSourceStorer"/> 事件源存储器
/// </summary>
/// <remarks>
/// <para>顾名思义，这里指的是事件消息存储中心，提供读写能力</para>
/// <para>默认实现为内存中的 <see cref="System.Threading.Channels.Channel"/></para>
/// </remarks>
internal interface IEventSourceStorer
{
    /// <summary>
    /// 将事件源写入存储器
    /// </summary>
    /// <param name="eventSource">事件源对象</param>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns><see cref="ValueTask"/></returns>
    ValueTask WriteAsync(IEventSource eventSource, CancellationToken cancellationToken);

    /// <summary>
    /// 从存储器中读取一条事件源
    /// </summary>
    /// <param name="cancellationToken">取消任务 Token</param>
    /// <returns>事件源对象</returns>
    ValueTask<IEventSource> ReadAsync(CancellationToken cancellationToken);
}