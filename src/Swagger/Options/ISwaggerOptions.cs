// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Fast.Swagger;

/// <summary>
/// Swagger 配置
/// </summary>
[SuppressSniffer]
public interface ISwaggerOptions
{
    /// <summary>
    /// 配置 Swagger 生成器
    /// </summary>
    /// <returns>配置 Swagger 生成器</returns>
    Action<SwaggerGenOptions> SwaggerGen();

    /// <summary>
    /// 配置 Swagger 全局参数
    /// </summary>
    /// <returns>配置 Swagger 全局参数</returns>
    Action<SwaggerOptions> Swagger();

    /// <summary>
    /// 配置 Swagger UI 全局参数
    /// </summary>
    /// <returns>配置 Swagger UI 全局参数</returns>
    Action<SwaggerUIOptions> SwaggerUI();
}
