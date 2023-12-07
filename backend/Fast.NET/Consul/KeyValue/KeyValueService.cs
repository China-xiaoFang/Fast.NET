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

using System.Text;
using System.Text.Json;
using Fast.Consul.Internal;
using Fast.Consul.KeyValue.Dto;
using Fast.IaaS;

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
        var (result, _) = await RemoteRequestUtil.GetAsync<List<ConsulKeyValueResponseDto>>(
            $"{Penetrates.ConsulSettings.Address.TrimEnd('/')}/v1/kv/{settingPath}?dc={dcName}");

        if (!result.Any())
            throw new Exception("未找到指定 Consul 配置！");

        var value = result.First().Value;

        return JsonSerializer.Deserialize<T>(Encoding.Default.GetString(Convert.FromBase64String(value)));
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
        var (result, _) = await RemoteRequestUtil.GetAsync<List<ConsulKeyValueResponseDto>>(
            $"{Penetrates.ConsulSettings.Address.TrimEnd('/')}/v1/kv/{settingPath}?dc={dcName}");

        if (!result.Any())
            throw new Exception("未找到指定 Consul 配置！");

        var value = result.First().Value;

        return Encoding.Default.GetString(Convert.FromBase64String(value));
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
        var (responseContent, _) = await RemoteRequestUtil.PutAsync(
            $"{Penetrates.ConsulSettings.Address.TrimEnd('/')}/v1/kv/{settingPath}?dc={dcName}&flags=0", data);

        return bool.TryParse(responseContent, out var result) && result;
    }
}