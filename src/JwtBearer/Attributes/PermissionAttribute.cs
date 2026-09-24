// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

// ReSharper disable once CheckNamespace

namespace Microsoft.AspNetCore.Authorization;

/// <summary>
/// 权限
/// </summary>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class PermissionAttribute : Attribute
{
    /// <summary>
    /// 权限标识集合
    /// </summary>
    public List<string> TagList { get; set; }

    /// <summary>
    /// 权限
    /// </summary>
    public PermissionAttribute()
    {
        TagList = null;
    }

    /// <summary>
    /// 权限
    /// </summary>
    /// <param name="tagList">权限匹配使用的标签集合</param>
    public PermissionAttribute(params string[] tagList)
    {
        TagList = tagList.ToList();
    }
}
