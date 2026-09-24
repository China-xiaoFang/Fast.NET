// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

namespace Fast.Consul;

/// <summary>
/// Key/Value 服务
/// </summary>
public interface IKeyValueService
{
    /// <summary>
    /// 读取 Consul 配置
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="settingPath"/> 或 <paramref name="dcName"/> 为空</exception>
    /// <exception cref="KeyNotFoundException">Consul 中不存在指定路径的配置</exception>
    /// <exception cref="FormatException">Consul 返回的配置值不是有效的 Base64 文本</exception>
    /// <exception cref="System.Text.Json.JsonException">配置内容无法反序列化为 <typeparamref name="T"/></exception>
    /// <param name="settingPath">Consul 中保存配置的键路径</param>
    /// <param name="dcName">Consul 数据中心名称</param>
    /// <typeparam name="T">配置值反序列化后的类型</typeparam>
    /// <returns>表示异步读取 Consul 配置的任务，任务结果为读取到的 Consul 配置</returns>
    Task<T> GetKeyValue<T>(string settingPath, string dcName);

    /// <summary>
    /// 读取 Consul 配置
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="settingPath"/> 或 <paramref name="dcName"/> 为空</exception>
    /// <exception cref="KeyNotFoundException">Consul 中不存在指定路径的配置</exception>
    /// <exception cref="FormatException">Consul 返回的配置值不是有效的 Base64 文本</exception>
    /// <param name="settingPath">Consul 中保存配置的键路径</param>
    /// <param name="dcName">Consul 数据中心名称</param>
    /// <returns>表示异步读取 Consul 配置的任务，任务结果为读取到的 Consul 配置</returns>
    Task<string> GetKeyValue(string settingPath, string dcName);

    /// <summary>
    /// 编辑 Consul 配置
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="settingPath"/> 或 <paramref name="dcName"/> 为空</exception>
    /// <param name="settingPath">Consul 中保存配置的键路径</param>
    /// <param name="dcName">Consul 数据中心名称</param>
    /// <param name="data">要写入 Consul 的配置文本</param>
    /// <returns>Consul 确认写入成功时返回 <see langword="true"/>；否则返回 <see langword="false"/></returns>
    Task<bool> EditKeyValue(string settingPath, string dcName, string data);
}
