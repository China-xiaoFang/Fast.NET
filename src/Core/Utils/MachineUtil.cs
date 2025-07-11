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
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

// ReSharper disable once CheckNamespace
namespace Fast.NET.Core;

/// <summary>
/// <see cref="MachineUtil"/> 系统机器工具类
/// </summary>
[SuppressSniffer]
public static class MachineUtil
{
    /// <summary>
    /// 是否为 Unix/Linux 操作系统
    /// </summary>
    /// <returns></returns>
    public static bool IsUnix()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }

    /// <summary>
    /// 是否为 MacOS 操作系统
    /// </summary>
    /// <returns></returns>
    public static bool IsMacOS()
    {
        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
    }

    /// <summary>
    /// 获取操作系统版本
    /// <para>Linux获取发行版本</para>
    /// </summary>
    /// <returns></returns>
    public static string GetOSDescription()
    {
        // 判断是否为 MacOS
        if (IsMacOS())
        {
            // 使用 sw_vers 命令获取 MacOS 的版本信息，并提取操作系统版本号
            var output = ShellUtil.Bash("sw_vers | awk 'NR<=2{printf \"%s \", $NF}'");
            if (output != null)
            {
                // 去除百分号并返回版本信息
                return output.Replace("%", string.Empty);
            }

            return string.Empty;
        }

        // 判断是否为 Unix/Linux
        if (IsUnix())
        {
            // 使用 /etc/os-release 文件中的 VERSION_ID 获取 Linux 发行版的版本号
            var output = ShellUtil.Bash("awk -F= '/^VERSION_ID/ {print $2}' /etc/os-release | tr -d '\"'");
            return output ?? string.Empty;
        }

        // Windows
        return RuntimeInformation.OSDescription;
    }

    /// <summary>
    /// 获取系统启动时间
    /// </summary>
    /// <returns></returns>
    public static DateTime GetSystemStartTime()
    {
        // 判断是否为 MacOS
        if (IsMacOS())
        {
            // MacOS 获取系统启动时间：sysctl -n kern.boottime | awk '{print $4}' | tr -d ','
            // 返回：1705379131
            var output = ShellUtil
                .Bash("date -r $(sysctl -n kern.boottime | awk '{print $4}' | tr -d ',') +\"%Y-%m-%d %H:%M:%S\"")
                .Trim();
            return DateTime.Parse(output);
        }

        // 判断是否为 Unix/Linux
        if (IsUnix())
        {
            // 使用 awk 命令来获取 Linux 系统的 uptime 信息
            var output = ShellUtil.Bash("date -d \"$(awk -F. '{print $1}' /proc/uptime) second ago\" +\"%Y-%m-%d %H:%M:%S\"")
                .Trim();
            return DateTime.Parse(output);
        }
        // Windows
        else
        {
            // 使用 wmic 获取系统启动时间
            var output = ShellUtil.Cmd("wmic", "OS get LastBootUpTime/Value");
            var timeValue = output.Replace("LastBootUpTime=", string.Empty)
                .Trim()
                .Split('.', StringSplitOptions.RemoveEmptyEntries)[0];

            return DateTime.ParseExact(timeValue, "yyyyMMddHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None);
        }
    }

    /// <summary>
    /// 获取系统运行时间描述
    /// </summary>
    /// <param name="format"><see cref="string"/> 输出格式化，默认：“00 天 00 时 00 分 00 秒”</param>
    /// <returns></returns>
    public static string GetSystemRunTimes(string format = "dd\\ \\天\\ hh\\ \\时\\ mm\\ \\分\\ ss\\ \\秒")
    {
        // 当前时间
        var dateTime = DateTime.Now;

        // 系统启动时间
        var startTime = GetSystemStartTime();

        // 当前时间 - 系统启动时间
        var diffTime = dateTime - startTime;

        return diffTime.ToString(format);
    }

    /// <summary>
    /// 获取当前进程启动时间
    /// </summary>
    /// <returns></returns>
    public static DateTime GetProgramStartTime()
    {
        return Process.GetCurrentProcess()
            .StartTime;
    }

    /// <summary>
    /// 获取当前进程运行时间描述
    /// </summary>
    /// <param name="format"><see cref="string"/> 输出格式化，默认：“00 天 00 时 00 分 00 秒”</param>
    /// <returns></returns>
    public static string GetProgramRunTimes(string format = "dd\\ \\天\\ hh\\ \\时\\ mm\\ \\分\\ ss\\ \\秒")
    {
        // 当前时间
        var dateTime = DateTime.Now;

        // 线程启动时间
        var startTime = GetProgramStartTime();

        // 当前时间 - 线程启动时间
        var diffTime = dateTime - startTime;

        return diffTime.ToString(format);
    }

    /// <summary>
    /// 获取操作系统 CPU 使用率
    /// </summary>
    /// <returns></returns>
    public static List<decimal> GetSystemCpuRate()
    {
        var rates = new List<decimal>();

        // 判断是否为 MacOS
        if (IsMacOS())
        {
            // 使用 top 命令获取获取 CPU 使用率（用户和系统占用总和）
            var output = ShellUtil.Bash("top -l 1 | grep \"CPU usage\" | awk '{print $3 + $5}'");
            rates.Add(decimal.Parse(output));
        }

        // 判断是否为 Unix/Linux
        if (IsUnix())
        {
            // 通过解析 '/proc/stat' 文件来计算 CPU 使用率
            var output = ShellUtil.Bash(
                "awk '{u=$2+$4; t=$2+$4+$5; if (NR==1){u1=u; t1=t;} else print ($2+$4-u1) * 100 / (t-t1); }' <(grep 'cpu ' /proc/stat) <(sleep 1;grep 'cpu ' /proc/stat)");
            rates.Add(decimal.Parse(output));
        }
        // Windows
        else
        {
            // 使用 wmic 获取 CPU 使用率
            var output = ShellUtil.Cmd("wmic", "cpu get LoadPercentage");
            rates.AddRange(output.Replace("LoadPercentage", string.Empty)
                .Trim()
                .Split("\r\r\n", StringSplitOptions.RemoveEmptyEntries)
                .Select(sl =>
                {
                    if (string.IsNullOrWhiteSpace(sl.Trim()))
                    {
                        return 0;
                    }

                    return decimal.Parse(sl.Trim());
                }));
        }

        return rates;
    }

    /// <summary>
    /// 获取当前进程 CPU 使用率
    /// <para>注：这里存在线程等待</para>
    /// </summary>
    /// <param name="sleep"><see cref="int"/> 线程等待时间，单位毫秒。建议不要超过3秒，0.5秒 1秒为性能最优</param>
    /// <returns></returns>
    public static async Task<decimal> GetProgramCpuUsage(int sleep = 500)
    {
        // 获取当前进程对象
        var process = Process.GetCurrentProcess();

        // 启动时间
        var startTime = DateTime.UtcNow;
        var startUsage = process.TotalProcessorTime;

        // 线程等待
        await Task.Delay(sleep);

        // 刷新进程状态
        process.Refresh();

        // 结束时间
        var endTime = DateTime.UtcNow;
        var endUsage = process.TotalProcessorTime;

        // 计算在延迟期间 CPU 使用的时间（单位：微秒）
        var usedMs = (endUsage - startUsage).TotalMilliseconds;
        var totalMs = (endTime - startTime).TotalMilliseconds;

        if (totalMs <= 0 || usedMs <= 0)
        {
            return 0;
        }

        // 考虑多核，计算总 CPU 时间的比例
        var usageTotal = (decimal) (usedMs / (Environment.ProcessorCount * totalMs) * 100);

        // 四舍五入保留两位小数
        return Math.Round(usageTotal, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// 获取操作系统内存信息，单位(MB)
    /// </summary>
    /// <returns>
    /// <para>total：总内存</para>
    /// <para>used：已用内存</para>
    /// <para>free：可用内存</para>
    /// </returns>
    public static (decimal total, decimal used, decimal free) GetSystemRamInfo()
    {
        // 总内存
        decimal total = 0;
        // 已用内存
        decimal used = 0;
        // 可用内存
        decimal free = 0;

        // 判断是否为 MacOS
        if (IsMacOS())
        {
            // 获取总内存：sysctl 命令返回的值为字节，转换为 MB
            var output1 = ShellUtil.Bash("sysctl -n hw.memsize | awk '{printf \"%.2f\", $1/1024/1024}'");
            total = decimal.Parse(output1.Replace("%", string.Empty));

            // 获取已用内存：top 命令中显示物理内存的使用情况，PhysMem 返回可用内存和已用内存的合计，单位为 KB
            var output2 = ShellUtil.Bash("top -l 1 -s 0 | awk '/PhysMem/ {print $6+$8}'");
            free = decimal.Parse(output2);

            // 计算已用内存
            used = total - free;
        }

        // 判断是否为 Unix/Linux
        if (IsUnix())
        {
            // 使用 `awk` 命令从 `/proc/meminfo` 获取总内存和可用内存，单位为 KB
            var output = ShellUtil.Bash(
                "awk '/MemTotal/ {total=$2} /MemAvailable/ {available=$2} END {print total,available}' /proc/meminfo");
            var memory = output.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (memory.Length == 2)
            {
                // 解析总内存，已用内存，可用内存
                total = decimal.Parse(memory[0]) / 1024;
                free = decimal.Parse(memory[1]) / 1024;

                // 计算已用内存
                used = total - free;
            }
        }
        // Windows
        else
        {
            // 使用 `wmic` 命令获取内存信息
            var output = ShellUtil.Cmd("wmic", "OS get FreePhysicalMemory,TotalVisibleMemorySize /Value");
            var lines = output.Trim()
                .Split("\n", StringSplitOptions.RemoveEmptyEntries);

            // 提取并解析内存信息：总内存和可用内存（单位：KB）
            var freeMemoryParts = lines[0]
                .Split("=", StringSplitOptions.RemoveEmptyEntries);
            var totalMemoryParts = lines[1]
                .Split("=", StringSplitOptions.RemoveEmptyEntries);

            // 将内存值转换为 MB
            total = decimal.Parse(totalMemoryParts[1]) / 1024;
            free = decimal.Parse(freeMemoryParts[1]) / 1024;

            // 计算已用内存
            used = total - free;
        }

        return (total, used, free);
    }

    /// <summary>
    /// 获取当前进程内存信息，单位(MB)
    /// </summary>
    /// <returns>
    /// <para>working：RAM 物理内存</para>
    /// <para>peakWorking：最大 RAM 物理内存</para>
    /// <para>virtualMemory：虚拟内存</para>
    /// <para>peakVirtualMemory：最大虚拟内存</para>
    /// <para>pagedMemory：分页内存</para>
    /// <para>peakPagedMemory：最大分页内存</para>
    /// </returns>
    public static (decimal working, decimal peakWorking, decimal virtualMemory, decimal peakVirtualMemory, decimal pagedMemory,
        decimal peakPagedMemory) GetProgramMemoryInfo()
    {
        // RAM 物理内存
        decimal working = 0;
        // 最大 RAM 物理内存
        decimal peakWorking = 0;
        // 虚拟内存
        decimal virtualMemory = 0;
        // 最大虚拟内存
        decimal peakVirtualMemory = 0;
        // 分页内存
        decimal pagedMemory = 0;
        // 最大分页内存
        decimal peakPagedMemory = 0;

        // 判断是否为 MacOS
        if (IsMacOS())
        {
            // 本地获取内存的方法可能需要额外的库或调用系统 API
        }

        // 判断是否为 Unix/Linux
        if (IsUnix())
        {
            decimal ByteToMB(string line)
            {
                // 解析文件中的内存值，VmRSS: 123456 kB
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2)
                {
                    return 0;
                }

                var value = Convert.ToDecimal(parts[1]);
                var unit = parts[2]
                    .ToLower();

                // 将内存值转换为 MB
                return unit switch
                {
                    "kb" => value / 1024,
                    "mb" => value,
                    "gb" => value * 1024,
                    _ => value
                };
            }

            // 过读取 /proc/self/status 文件获取 Linux 系统内存信息
            var lines = File.ReadAllLines("/proc/self/status");

            foreach (var line in lines)
            {
                // 物理内存
                if (line.StartsWith("VmRSS:"))
                {
                    working = ByteToMB(line);
                }
                // 最大物理内存
                else if (line.StartsWith("VmPeak:"))
                {
                    peakWorking = ByteToMB(line);
                }
                // 虚拟内存
                else if (line.StartsWith("VmSize:"))
                {
                    virtualMemory = ByteToMB(line);
                }
                // 最大虚拟内存
                else if (line.StartsWith("VmHWM:"))
                {
                    peakVirtualMemory = ByteToMB(line);
                }
                // 分页内存（交换内存）
                else if (line.StartsWith("VmSwap:"))
                {
                    pagedMemory = ByteToMB(line);
                    // Linux 中没有 类似 PeakPagedMemory 的字段，所以这里直接返回交换内存
                    peakPagedMemory = pagedMemory;
                }
            }
        }
        // Windows
        else
        {
            // 获取当前进程对象
            var process = Process.GetCurrentProcess();

            const decimal relation = 1024 * 1024;

            // 将内存值转换为 MB
            working = process.WorkingSet64 / relation;
            peakWorking = process.PeakWorkingSet64 / relation;
            virtualMemory = process.VirtualMemorySize64 / relation;
            peakVirtualMemory = process.PeakVirtualMemorySize64 / relation;
            pagedMemory = process.PagedMemorySize64 / relation;
            peakPagedMemory = process.PeakPagedMemorySize64 / relation;
        }

        return (working, peakWorking, virtualMemory, peakVirtualMemory, pagedMemory, peakPagedMemory);
    }

    /// <summary>
    /// 获取硬盘信息
    /// </summary>
    /// <returns></returns>
    public static List<DiskInfo> GetDiskInfos()
    {
        var diskInfos = new List<DiskInfo>();

        // 判断是否为 MacOS
        if (IsMacOS())
        {
            var output = ShellUtil.Bash(@"df -m | awk '/^\/dev\/disk/ {print $1,$2,$3,$4,$5}'");
            var disks = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (disks.Length < 1)
                return diskInfos;
            foreach (var item in disks)
            {
                var disk = item.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (disk.Length >= 5)
                {
                    var diskInfo = new DiskInfo
                    {
                        DiskName = disk[0],
                        TypeName = ShellUtil.Bash("diskutil info " + disk[0] + " | awk '/File System Personality/ {print $4}'")
                            .Replace("\n", string.Empty),
                        TotalSize = Math.Round(long.Parse(disk[1]) / 1024M, 2, MidpointRounding.AwayFromZero),
                        Used = Math.Round(long.Parse(disk[2]) / 1024M, 2, MidpointRounding.AwayFromZero),
                        AvailableFreeSpace = Math.Round(long.Parse(disk[3]) / 1024M, 2, MidpointRounding.AwayFromZero),
                        AvailablePercent = decimal.Parse(disk[4]
                            .Replace("%", ""))
                    };
                    diskInfos.Add(diskInfo);
                }
            }
        }

        // 判断是否为 Unix/Linux
        if (IsUnix())
        {
            var output = ShellUtil.Bash(@"df -mT | awk '/^\/dev\/(sd|vd|xvd|nvme|sda|vda|mapper)/ {print $1,$2,$3,$4,$5,$6}'");
            var disks = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            if (disks.Length >= 1)
            {
                foreach (var item in disks)
                {
                    var disk = item.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (disk.Length < 6)
                        continue;

                    var diskInfo = new DiskInfo
                    {
                        DiskName = disk[0],
                        TypeName = disk[1],
                        TotalSize = Math.Round(long.Parse(disk[2]) / 1024M, 2, MidpointRounding.AwayFromZero),
                        Used = Math.Round(long.Parse(disk[3]) / 1024M, 2, MidpointRounding.AwayFromZero),
                        AvailableFreeSpace = Math.Round(long.Parse(disk[4]) / 1024M, 2, MidpointRounding.AwayFromZero),
                        AvailablePercent = decimal.Parse(disk[5]
                            .Replace("%", ""))
                    };
                    diskInfos.Add(diskInfo);
                }
            }
        }
        // Windows
        else
        {
            var driveList = DriveInfo.GetDrives()
                .Where(u => u.IsReady);

            const decimal relation = 1024 * 1024 * 1024;

            foreach (var item in driveList)
            {
                if (item.DriveType == DriveType.CDRom)
                    continue;
                var diskInfo = new DiskInfo
                {
                    DiskName = item.Name,
                    TypeName = item.DriveType.ToString(),
                    TotalSize = Math.Round(item.TotalSize / relation, 2, MidpointRounding.AwayFromZero),
                    AvailableFreeSpace = Math.Round(item.AvailableFreeSpace / relation, 2, MidpointRounding.AwayFromZero)
                };
                diskInfo.Used = diskInfo.TotalSize - diskInfo.AvailableFreeSpace;
                diskInfo.AvailablePercent =
                    Math.Round(diskInfo.Used / diskInfo.TotalSize * 100, 2, MidpointRounding.AwayFromZero);
                diskInfos.Add(diskInfo);
            }
        }

        return diskInfos;
    }

    /// <summary>
    /// 获取服务器IP地址
    /// </summary>
    /// <returns></returns>
    public static async Task<(string ip, string address)> GetServerIp()
    {
        // IP地址
        var ip = "unknow";
        var address = "中国";

        // 发送 Http 请求
        using var httpClient = new HttpClient();

        // 设置请求超时时间
        httpClient.Timeout = TimeSpan.FromSeconds(10);

        using var request = new HttpRequestMessage();

        // 设置请求 Url
        request.RequestUri = new Uri("https://www.ip.cn/api/index?ip&type=0");
        // 设置请求方式
        request.Method = HttpMethod.Get;
        // 设置请求头部
        request.Headers.Add("Accept", "application/json, text/plain, */*");

        // 添加默认 User-Agent
        request.Headers.TryAddWithoutValidation("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.5112.81 Safari/537.36 Edg/104.0.1293.47");

        try
        {
            // 发送请求
            using var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseContent = Encoding.UTF8.GetString(response.Content.ReadAsByteArrayAsync()
                .Result);

            var ipInfoDictionary = JsonSerializer.Deserialize<IDictionary<string, object>>(responseContent);

            if (ipInfoDictionary.TryGetValue("ip", out var resIp))
            {
                ip = resIp.ToString();
            }

            if (ipInfoDictionary.TryGetValue("address", out var resAddress))
            {
                address = resAddress.ToString();
            }
        }
        catch (HttpRequestException ex)
        {
            throw new Exception("远程请求错误：" + ex.Message, ex);
        }
        catch (TaskCanceledException ex)
        {
            throw new Exception("远程请求超时：" + ex.Message, ex);
        }

        return (ip, address);
    }
}