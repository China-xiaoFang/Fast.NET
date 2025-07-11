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
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="RetryUtil"/> 重试静态类
/// </summary>
public sealed class RetryUtil
{
    /// <summary>
    /// 重试有异常的方法，还可以指定特定异常
    /// </summary>
    /// <param name="action"></param>
    /// <param name="numRetries">重试次数</param>
    /// <param name="retryTimeout">重试间隔时间</param>
    /// <param name="finalThrow">是否最终抛异常</param>
    /// <param name="exceptionTypes">异常类型,可多个</param>
    /// <param name="fallbackPolicy">重试失败回调</param>
    /// <param name="retryAction">重试时调用方法</param>
    public static void Invoke(Action action, int numRetries, int retryTimeout = 1000, bool finalThrow = true,
        Type[] exceptionTypes = null, Action<Exception> fallbackPolicy = null, Action<int, int> retryAction = null)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        if (fallbackPolicy == null)
        {
            InvokeAsync(async () =>
                {
                    action();
                    await Task.CompletedTask;
                }, numRetries, retryTimeout, finalThrow, exceptionTypes, null, retryAction)
                .GetAwaiter()
                .GetResult();
        }
        else
        {
            InvokeAsync(async () =>
                {
                    action();
                    await Task.CompletedTask;
                }, numRetries, retryTimeout, finalThrow, exceptionTypes, async ex =>
                {
                    fallbackPolicy.Invoke(ex);
                    await Task.CompletedTask;
                }, retryAction)
                .GetAwaiter()
                .GetResult();
        }
    }

    /// <summary>
    /// 重试有异常的方法，还可以指定特定异常
    /// </summary>
    /// <param name="action"></param>
    /// <param name="numRetries">重试次数</param>
    /// <param name="retryTimeout">重试间隔时间</param>
    /// <param name="finalThrow">是否最终抛异常</param>
    /// <param name="exceptionTypes">异常类型,可多个</param>
    /// <param name="fallbackPolicy">重试失败回调</param>
    /// <param name="retryAction">重试时调用方法</param>
    /// <returns><see cref="Task"/></returns>
    public static async Task InvokeAsync(Func<Task> action, int numRetries, int retryTimeout = 1000, bool finalThrow = true,
        Type[] exceptionTypes = null, Func<Exception, Task> fallbackPolicy = null, Action<int, int> retryAction = null)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        // 如果重试次数小于或等于 0，则直接调用
        if (numRetries <= 0)
        {
            await action();
            return;
        }

        // 存储总的重试次数
        var totalNumRetries = numRetries;

        // 不断重试
        while (true)
        {
            try
            {
                await action();
                break;
            }
            catch (Exception ex)
            {
                // 如果可重试次数小于或等于0，则终止重试
                if (--numRetries < 0)
                {
                    if (finalThrow)
                    {
                        if (fallbackPolicy != null)
                            await fallbackPolicy.Invoke(ex);
                        throw;
                    }

                    return;
                }

                // 如果填写了 exceptionTypes 且异常类型不在 exceptionTypes 之内，则终止重试
                if (exceptionTypes != null
                    && exceptionTypes.Length > 0
                    && !exceptionTypes.Any(u => u.IsAssignableFrom(ex.GetType())))
                {
                    if (finalThrow)
                    {
                        if (fallbackPolicy != null)
                            await fallbackPolicy.Invoke(ex);
                        throw;
                    }

                    return;
                }

                // 重试调用委托
                retryAction?.Invoke(totalNumRetries, totalNumRetries - numRetries);

                // 如果可重试异常数大于 0，则间隔指定时间后继续执行
                if (retryTimeout > 0)
                    await Task.Delay(retryTimeout);
            }
        }
    }
}