// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using Fast.Runtime;

namespace Fast.Cache;

/// <summary>
/// Redis 连接字符串配置
/// </summary>
[SuppressSniffer]
public class RedisSettingsOptions : IPostConfigure
{
    /// <summary>
    /// 服务器 Ip 地址
    /// </summary>
    public string ServiceIp { get; set; }

    /// <summary>
    /// 端口号
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// 默认库
    /// </summary>
    public int? DbName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string DbPwd { get; set; }

    /// <summary>
    /// 前缀
    /// </summary>
    public string Prefix { get; set; }

    /// <summary>
    /// 连接池大小
    /// </summary>
    public int? Poolsize { get; set; }

    /// <summary>
    /// SSL 加密连接
    /// </summary>
    public bool? SSL { get; set; }

    /// <summary>
    /// 服务
    /// </summary>
    public List<RedisServiceSettingsOptions> Services { get; set; }

    /// <inheritdoc />
    public void PostConfigure()
    {
        ServiceIp ??= "127.0.0.1";
        Port ??= 6379;
        DbName ??= 0;
        Poolsize ??= 100;
        SSL ??= false;
        Services ??= [];
    }
}

/// <summary>
/// Redis 连接字符串服务配置
/// </summary>
[SuppressSniffer]
public class RedisServiceSettingsOptions : RedisSettingsOptions
{
    /// <summary>
    /// 服务名称
    /// </summary>
    public string ServiceName { get; set; }
}
