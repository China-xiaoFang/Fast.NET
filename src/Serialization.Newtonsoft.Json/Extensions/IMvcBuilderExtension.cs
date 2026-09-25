// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using Microsoft.Extensions.DependencyInjection;

namespace Fast.Serialization;

/// <summary>
/// 为 <see cref="IMvcBuilder"/> 提供 Newtonsoft.Json 扩展方法
/// </summary>
public static class IMvcBuilderExtension
{
    /// <summary>
    /// 添加 Newtonsoft.Json 序列化服务
    /// </summary>
    /// <param name="builder">要配置的应用构建器</param>
    /// <returns>返回当前 MVC 构建器，便于链式调用</returns>
    public static IMvcBuilder AddSerialization(this IMvcBuilder builder)
    {
        // Web 环境配置
        builder.AddNewtonsoftJson();

        return builder;
    }
}
