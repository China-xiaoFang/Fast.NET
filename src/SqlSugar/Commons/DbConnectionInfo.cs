// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using Fast.Runtime;
using SqlSugar;

namespace Fast.SqlSugar;

/// <summary>
/// 数据库连接信息
/// </summary>
[SuppressSniffer]
public class DbConnectionInfo : IPostConfigure
{
    /// <summary>
    /// 服务器 Ip 地址
    /// </summary>
    [SugarColumn(ColumnDescription = "服务器Ip地址", Length = 15, IsNullable = true)]
    public virtual string ServiceIp { get; set; }

    /// <summary>
    /// 端口号
    /// </summary>
    /// <remarks>
    /// <para>MySql：3306</para>
    /// <para>SqlServer：1433</para>
    /// <para>Oracle：1521</para>
    /// <para>PostgreSQL：5432</para>
    /// <para>MongoDb：27017</para>
    /// </remarks>
    [SugarColumn(ColumnDescription = "端口号", IsNullable = true)]
    public virtual int? Port { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库名称", Length = 50, IsNullable = false)]
    public virtual string DbName { get; set; }

    /// <summary>
    /// 数据库用户
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库用户", Length = 10, IsNullable = true)]
    public virtual string DbUser { get; set; }

    /// <summary>
    /// 数据库密码
    /// </summary>
    [SugarColumn(ColumnDescription = "数据库密码", Length = 20, IsNullable = true)]
    public virtual string DbPwd { get; set; }

    /// <summary>
    /// 自定义连接字符串
    /// </summary>
    [SugarColumn(ColumnDescription = "自定义连接字符串", Length = 100, IsNullable = true)]
    public virtual string CustomConnectionStr { get; set; }

    /// <inheritdoc />
    public virtual void PostConfigure()
    {
        ServiceIp ??= "127.0.0.1";
        Port ??= 1433;
        DbUser ??= "sa";
    }
}
