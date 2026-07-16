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

using System.Text;
using System.Text.Json;
using Fast.Consul.Internal;
using Fast.Consul.KeyValue.Dto;
using Fast.NET.Core;

namespace Fast.Consul.KeyValue;

/// <summary>
/// <see cref="KeyValueService"/> Key/Value 服务
/// </summary>
public class KeyValueService : IKeyValueService
{
    /// <summary>
    /// 读取 Consul 配置
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="settingPath"><see cref="string"/> 路径</param>
    /// <param name="dcName"><see cref="string"/> 数据中心名称</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<T> GetKeyValue<T>(string settingPath, string dcName)
    {
        ValidatePath(settingPath, dcName);
        var (result, _) =
            await RemoteRequestUtil.GetAsync<List<ConsulKeyValueResponseDto>>(BuildKeyValueUrl(settingPath, dcName));

        if (result == null || result.Count == 0)
            throw new KeyNotFoundException("未找到指定 Consul 配置！");

        var value = result[0].Value;

        return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(Convert.FromBase64String(value)));
    }

    /// <summary>
    /// 读取 Consul 配置
    /// </summary>
    /// <param name="settingPath"><see cref="string"/> 路径</param>
    /// <param name="dcName"><see cref="string"/> 数据中心名称</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> GetKeyValue(string settingPath, string dcName)
    {
        ValidatePath(settingPath, dcName);
        var (result, _) =
            await RemoteRequestUtil.GetAsync<List<ConsulKeyValueResponseDto>>(BuildKeyValueUrl(settingPath, dcName));

        if (result == null || result.Count == 0)
            throw new KeyNotFoundException("未找到指定 Consul 配置！");

        var value = result[0].Value;

        return Encoding.UTF8.GetString(Convert.FromBase64String(value));
    }

    /// <summary>
    /// 编辑 Consul 配置
    /// </summary>
    /// <param name="settingPath"><see cref="string"/> 路径</param>
    /// <param name="dcName"><see cref="string"/> 数据中心名称</param>
    /// <param name="data"><see cref="string"/> JSON 格式字符串</param>
    /// <returns><see cref="bool"/> 是否成功</returns>
    public async Task<bool> EditKeyValue(string settingPath, string dcName, string data)
    {
        ValidatePath(settingPath, dcName);
        var (responseContent, _) = await RemoteRequestUtil.PutAsync($"{BuildKeyValueUrl(settingPath, dcName)}&flags=0", data);

        return bool.TryParse(responseContent, out var result) && result;
    }

    private static string BuildKeyValueUrl(string settingPath, string dcName)
    {
        return $"{Penetrates.ConsulSettings.Address.TrimEnd('/')}/v1/kv/{Uri.EscapeDataString(settingPath)}"
               + $"?dc={Uri.EscapeDataString(dcName)}";
    }

    private static void ValidatePath(string settingPath, string dcName)
    {
        if (string.IsNullOrWhiteSpace(settingPath))
            throw new ArgumentException("Consul 配置路径不能为空。", nameof(settingPath));
        if (string.IsNullOrWhiteSpace(dcName))
            throw new ArgumentException("Consul 数据中心名称不能为空。", nameof(dcName));
    }
}