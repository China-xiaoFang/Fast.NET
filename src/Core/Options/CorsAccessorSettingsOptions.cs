// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using System.ComponentModel.DataAnnotations;
using Fast.Runtime;

namespace Fast.NET.Core;

/// <summary>
/// 跨域配置选项
/// </summary>
[SuppressSniffer]
public sealed class CorsAccessorSettingsOptions : IPostConfigure
{
    /// <summary>
    /// 策略名称
    /// </summary>
    [Required]
    public string PolicyName { get; set; }

    /// <summary>
    /// 允许来源域名，没有配置则允许所有来源
    /// </summary>
    public string[] WithOrigins { get; set; }

    /// <summary>
    /// 请求表头，没有配置则允许所有表头
    /// </summary>
    public string[] WithHeaders { get; set; }

    /// <summary>
    /// 设置客户端可获取的响应标头
    /// </summary>
    public string[] WithExposedHeaders { get; set; }

    /// <summary>
    /// 设置跨域允许请求谓词，没有配置则允许所有
    /// </summary>
    public string[] WithMethods { get; set; }

    /// <summary>
    /// 是否允许跨域请求中的凭据
    /// </summary>
    public bool? AllowCredentials { get; set; }

    /// <summary>
    /// 设置预检过期时间
    /// </summary>
    public int? SetPreflightMaxAge { get; set; }

    /// <summary>
    /// 修正前端无法获取 Token 问题
    /// </summary>
    public bool? FixedClientToken { get; set; }

    /// <summary>
    /// 启用 SignalR 跨域支持
    /// </summary>
    public bool? SignalRSupport { get; set; }

    /// <inheritdoc />
    public void PostConfigure()
    {
        PolicyName ??= "App.Cors.Policy";
        WithOrigins ??= Array.Empty<string>();
        AllowCredentials ??= true;
        FixedClientToken ??= true;
        SignalRSupport ??= false;
        // 默认支持前端非代理情况，使用 Axios 的情况
        WithExposedHeaders ??= new[] {"Content-Disposition"};
    }
}
