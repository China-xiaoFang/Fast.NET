// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using Microsoft.AspNetCore.Mvc;

namespace Fast.DynamicApplication;

/// <summary>
/// 接口描述设置
/// </summary>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ApiDescriptionSettingsAttribute : ApiExplorerSettingsAttribute
{
    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    public ApiDescriptionSettingsAttribute()
    {
        Order = 0;
    }

    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    /// <param name="enabled">是否启用该功能</param>
    public ApiDescriptionSettingsAttribute(bool enabled)
    {
        IgnoreApi = !enabled;
        Order = 0;
    }

    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    /// <param name="groups">文档分组集合</param>
    public ApiDescriptionSettingsAttribute(params string[] groups)
    {
        GroupName = string.Join("##", groups);
        Groups = groups;
        Order = 0;
    }

    /// <summary>
    /// 自定义名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 模块名
    /// </summary>
    public string Module { get; set; }

    /// <summary>
    /// 版本号
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// 分组
    /// </summary>
    public string[] Groups { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// 额外描述，支持 HTML
    /// </summary>
    public string Description { get; set; }
}
