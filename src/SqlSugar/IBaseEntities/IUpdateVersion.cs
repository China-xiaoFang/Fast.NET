// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using SqlSugar;

namespace Fast.SqlSugar;

/// <summary>
/// 行版本实体接口
/// </summary>
/// <remarks>仅单条实体更新会触发行版本检查，冲突时抛出 <see cref="VersionExceptions"/></remarks>
[SuppressSniffer]
public interface IUpdateVersion : IDatabaseEntity
{
    /// <summary>
    /// 更新版本控制字段
    /// </summary>
    long RowVersion { get; set; }
}
