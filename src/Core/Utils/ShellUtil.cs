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

using System.Diagnostics;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Fast.NET.Core;

/// <summary>
/// <see cref="ShellUtil"/> 系统Shell工具类
/// </summary>
[SuppressSniffer]
public static class ShellUtil
{
    /// <summary>
    /// Linux Bash 命令
    /// </summary>
    /// <param name="command"><see cref="string"/> 执行的命令</param>
    /// <param name="timeout"><see cref="int"/> 执行名称超时时间，单位毫秒</param>
    /// <returns></returns>
    public static string Bash(string command, int timeout = 0)
    {
        var escapedArgs = command.Replace("\"", "\\\"");
        // 创建进程对象
        using var process = new Process();
        // 创建新的进程启动信息
        process.StartInfo = new ProcessStartInfo
        {
            // 执行的命令
            FileName = "/bin/bash",
            // 命令参数
            Arguments = $"-c \"{escapedArgs}\"",
            // 重定向标准输出
            RedirectStandardOutput = true,
            // 重定向标准错误
            RedirectStandardError = true,
            // 不使用操作系统外壳程序来启动进程
            UseShellExecute = false,
            // 不创建窗口
            CreateNoWindow = true,
            // 输出编码
            StandardOutputEncoding = Encoding.UTF8,
            // 错误输出编码
            StandardErrorEncoding = Encoding.UTF8
        };

        // 启动进程
        process.Start();

        // 是否超时判断
        if (timeout > 0)
        {
            // 等待命令执行完成
            var exited = process.WaitForExit(timeout);

            // 检查是否超时或失败
            if (!exited)
            {
                throw new TimeoutException("命令执行超时");
            }
        }

        // 获取输出内容
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        if (!string.IsNullOrEmpty(error))
        {
            throw new InvalidOperationException($"命令执行出错: {error}");
        }

        return output;
    }

    /// <summary>
    /// Windows Cmd 命令
    /// </summary>
    /// <param name="command"><see cref="string"/> 执行的命令</param>
    /// <param name="args"><see cref="string"/> 执行的命令参数</param>
    /// <param name="timeout"><see cref="int"/> 执行名称超时时间，单位毫秒</param>
    /// <returns></returns>
    public static string Cmd(string command, string args = null, int timeout = 0)
    {
        // 创建进程对象
        using var process = new Process();
        // 创建新的进程启动信息
        process.StartInfo = new ProcessStartInfo
        {
            // 执行的命令
            FileName = command,
            // 命令参数
            Arguments = args,
            // 重定向标准输出
            RedirectStandardOutput = true,
            // 重定向标准错误
            RedirectStandardError = true,
            // 不使用操作系统外壳程序来启动进程
            UseShellExecute = false,
            // 不创建窗口
            CreateNoWindow = true,
            // 输出编码
            StandardOutputEncoding = Encoding.UTF8,
            // 错误输出编码
            StandardErrorEncoding = Encoding.UTF8
        };

        // 启动进程
        process.Start();

        // 是否超时判断
        if (timeout > 0)
        {
            // 等待命令执行完成
            var exited = process.WaitForExit(timeout);

            // 检查是否超时或失败
            if (!exited)
            {
                throw new TimeoutException("命令执行超时");
            }
        }

        // 获取输出内容
        var output = process.StandardOutput.ReadToEnd();
        var error = process.StandardError.ReadToEnd();

        if (!string.IsNullOrEmpty(error))
        {
            throw new InvalidOperationException($"命令执行出错: {error}");
        }

        return output;
    }
}