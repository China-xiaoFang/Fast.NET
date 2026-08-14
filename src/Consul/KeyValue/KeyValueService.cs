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

namespace Fast.Consul;

/// <summary>
/// Consul Key/Value 响应 DTO
/// </summary>
internal sealed class ConsulKeyValueResponseDto
{
    public int LockIndex { get; set; }

    public string Key { get; set; }

    public int Flags { get; set; }

    public string Value { get; set; }

    public long CreateIndex { get; set; }

    public long ModifyIndex { get; set; }
}

/// <inheritdoc cref="IKeyValueService" />
public class KeyValueService : IKeyValueService
{
    /// <summary>
    /// Key/Value 请求共用的 HTTP 客户端
    /// </summary>
    private static readonly HttpClient _httpClient = new() {Timeout = TimeSpan.FromSeconds(60)};

    /// <summary>
    /// Consul Key/Value 响应的 JSON 反序列化配置
    /// </summary>
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new() {PropertyNameCaseInsensitive = true};

    /// <summary>
    /// 向 Consul 发送 GET 请求并反序列化响应正文
    /// </summary>
    /// <param name="requestUri">Consul Key/Value 请求地址</param>
    /// <typeparam name="T">响应正文反序列化后的类型</typeparam>
    /// <returns>反序列化后的响应内容</returns>
    private static async Task<T> Get<T>(string requestUri)
    {
        using var response = await _httpClient.GetAsync(requestUri)
            .ConfigureAwait(false);
        var responseContent = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        // 优先读取响应正文，使异常能够保留 Consul 返回的具体错误信息
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(responseContent, null, response.StatusCode);

        return JsonSerializer.Deserialize<T>(responseContent, _jsonSerializerOptions);
    }

    /// <summary>
    /// 向 Consul 发送 PUT 请求并返回响应正文
    /// </summary>
    /// <param name="requestUri">Consul Key/Value 请求地址</param>
    /// <param name="data">要写入 Consul 的原始 UTF-8 文本</param>
    /// <returns>Consul 返回的响应正文</returns>
    private static async Task<string> Put(string requestUri, string data)
    {
        using var content = data == null ? null : new StringContent(data, Encoding.UTF8, "application/json");
        using var response = await _httpClient.PutAsync(requestUri, content)
            .ConfigureAwait(false);
        var responseContent = await response.Content.ReadAsStringAsync()
            .ConfigureAwait(false);

        // 与 GET 保持一致，失败响应直接携带 Consul 返回的正文
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(responseContent, null, response.StatusCode);

        return responseContent;
    }

    /// <inheritdoc />
    public async Task<T> GetKeyValue<T>(string settingPath, string dcName)
    {
        ValidatePath(settingPath, dcName);
        var result = await Get<List<ConsulKeyValueResponseDto>>(BuildKeyValueUrl(settingPath, dcName));

        if (result == null || result.Count == 0)
            throw new KeyNotFoundException("未找到指定 Consul 配置！");

        var value = result[0].Value;

        return JsonSerializer.Deserialize<T>(Encoding.UTF8.GetString(Convert.FromBase64String(value)));
    }

    /// <inheritdoc />
    public async Task<string> GetKeyValue(string settingPath, string dcName)
    {
        ValidatePath(settingPath, dcName);
        var result = await Get<List<ConsulKeyValueResponseDto>>(BuildKeyValueUrl(settingPath, dcName));

        if (result == null || result.Count == 0)
            throw new KeyNotFoundException("未找到指定 Consul 配置！");

        var value = result[0].Value;

        return Encoding.UTF8.GetString(Convert.FromBase64String(value));
    }

    /// <inheritdoc />
    public async Task<bool> EditKeyValue(string settingPath, string dcName, string data)
    {
        ValidatePath(settingPath, dcName);
        var responseContent = await Put($"{BuildKeyValueUrl(settingPath, dcName)}&flags=0", data);

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
