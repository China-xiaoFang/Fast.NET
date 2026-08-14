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

namespace Fast.Consul;

/// <summary>
/// Key/Value 服务。
/// </summary>
public interface IKeyValueService
{
    /// <summary>
    /// 读取 Consul 配置。
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="settingPath"/> 或 <paramref name="dcName"/> 为空。</exception>
    /// <exception cref="KeyNotFoundException">Consul 中不存在指定路径的配置。</exception>
    /// <exception cref="FormatException">Consul 返回的配置值不是有效的 Base64 文本。</exception>
    /// <exception cref="System.Text.Json.JsonException">配置内容无法反序列化为 <typeparamref name="T"/>。</exception>
    /// <param name="settingPath">Consul 中保存配置的键路径。</param>
    /// <param name="dcName">Consul 数据中心名称。</param>
    /// <typeparam name="T">配置值反序列化后的类型。</typeparam>
    /// <returns>表示异步读取 Consul 配置的任务，任务结果为读取到的 Consul 配置。</returns>
    Task<T> GetKeyValue<T>(string settingPath, string dcName);

    /// <summary>
    /// 读取 Consul 配置。
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="settingPath"/> 或 <paramref name="dcName"/> 为空。</exception>
    /// <exception cref="KeyNotFoundException">Consul 中不存在指定路径的配置。</exception>
    /// <exception cref="FormatException">Consul 返回的配置值不是有效的 Base64 文本。</exception>
    /// <param name="settingPath">Consul 中保存配置的键路径。</param>
    /// <param name="dcName">Consul 数据中心名称。</param>
    /// <returns>表示异步读取 Consul 配置的任务，任务结果为读取到的 Consul 配置。</returns>
    Task<string> GetKeyValue(string settingPath, string dcName);

    /// <summary>
    /// 编辑 Consul 配置。
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="settingPath"/> 或 <paramref name="dcName"/> 为空。</exception>
    /// <param name="settingPath">Consul 中保存配置的键路径。</param>
    /// <param name="dcName">Consul 数据中心名称。</param>
    /// <param name="data">要写入 Consul 的配置文本。</param>
    /// <returns>Consul 确认写入成功时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    Task<bool> EditKeyValue(string settingPath, string dcName, string data);
}
