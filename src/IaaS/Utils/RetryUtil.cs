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
using System.Threading;
using System.Threading.Tasks;

namespace Fast.IaaS;

/// <summary>
/// 重试静态类
/// </summary>
public sealed class RetryUtil
{
    /// <summary>
    /// 重试有异常的方法，还可以指定特定异常
    /// </summary>
    /// <param name="action">要执行的操作委托</param>
    /// <param name="numRetries">最大重试次数</param>
    /// <param name="retryTimeout">两次重试之间的等待时间</param>
    /// <param name="finalThrow">重试耗尽后要抛出的异常工厂</param>
    /// <param name="exceptionTypes">允许触发重试的异常类型集合</param>
    /// <param name="fallbackPolicy">重试耗尽后使用的降级策略</param>
    /// <param name="retryAction">每次失败后执行的重试回调</param>
    public static void Invoke(Action action, int numRetries, int retryTimeout = 1000, bool finalThrow = true,
        Type[] exceptionTypes = null, Action<Exception> fallbackPolicy = null, Action<int, int> retryAction = null)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        InvokeAsync(async () =>
            {
                action();
                await Task.CompletedTask;
            }, numRetries, retryTimeout, finalThrow, exceptionTypes, async ex =>
            {
                fallbackPolicy?.Invoke(ex);
                await Task.CompletedTask;
            }, async (total, times) =>
            {
                retryAction?.Invoke(total, times);
                await Task.CompletedTask;
            })
            .GetAwaiter()
            .GetResult();
    }

    /// <summary>
    /// 重试有异常的方法，还可以指定特定异常
    /// </summary>
    /// <param name="action">要执行的操作委托</param>
    /// <param name="numRetries">最大重试次数</param>
    /// <param name="retryTimeout">两次重试之间的等待时间</param>
    /// <param name="finalThrow">重试耗尽后要抛出的异常工厂</param>
    /// <param name="exceptionTypes">允许触发重试的异常类型集合</param>
    /// <param name="fallbackPolicy">重试耗尽后使用的降级策略</param>
    /// <param name="retryAction">每次失败后执行的重试回调</param>
    /// <param name="cancellationToken">用于取消异步操作的令牌</param>
    /// <returns>表示异步“重试有异常的方法，还可以指定特定异常”操作的任务</returns>
    public static async Task InvokeAsync(Func<Task> action, int numRetries, int retryTimeout = 1000, bool finalThrow = true,
        Type[] exceptionTypes = null, Func<Exception, Task> fallbackPolicy = null, Func<int, int, Task> retryAction = null,
        CancellationToken cancellationToken = default)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        // 未配置重试次数时只执行一次，不进入重试循环
        if (numRetries <= 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await action()
                .ConfigureAwait(false);
            return;
        }

        // 存储总的重试次数
        var totalNumRetries = numRetries;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await action()
                    .ConfigureAwait(false);
                break;
            }
            catch (Exception ex)
            {
                var retriesExhausted = --numRetries < 0;
                var cannotRetryException = exceptionTypes != null
                                           && exceptionTypes.Length > 0
                                           && !exceptionTypes.Where(u => u != null)
                                               .Any(u => u.IsAssignableFrom(ex.GetType()));

                // 重试耗尽或异常类型不匹配时统一执行失败回调
                if (retriesExhausted || cannotRetryException)
                {
                    if (fallbackPolicy != null)
                    {
                        await fallbackPolicy.Invoke(ex)
                            .ConfigureAwait(false);
                    }

                    if (finalThrow)
                        throw;

                    return;
                }

                // 重试调用委托
                if (retryAction != null)
                {
                    await retryAction.Invoke(totalNumRetries, totalNumRetries - numRetries)
                        .ConfigureAwait(false);
                }

                // 仅对允许重试的异常等待指定间隔后重试
                if (retryTimeout > 0)
                {
                    await Task.Delay(retryTimeout, cancellationToken)
                        .ConfigureAwait(false);
                }
            }
        }
    }
}
