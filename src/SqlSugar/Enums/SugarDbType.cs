// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.ComponentModel;

namespace Fast.SqlSugar;

/// <summary>
/// Sugar 数据库类型枚举
/// </summary>
[FastEnum("Sugar数据库类型枚举")]
public enum SugarDbType
{
    /// <summary>
    /// MySql
    /// </summary>
    [Description("MySql")]
    MySql = 0,

    /// <summary>
    /// SqlServer
    /// </summary>
    [Description("SqlServer")]
    SqlServer = 1,

    /// <summary>
    /// SqLite
    /// </summary>
    [Description("SqLite")]
    Sqlite = 2,

    /// <summary>
    /// Oracle
    /// </summary>
    [Description("Oracle")]
    Oracle = 3,

    /// <summary>
    /// PostgreSQL
    /// </summary>
    [Description("PostgreSQL")]
    PostgreSQL = 4,

    /// <summary>
    /// 达梦
    /// </summary>
    [Description("达梦")]
    Dm = 5,

    /// <summary>
    /// Kdbndp
    /// </summary>
    [Description("Kdbndp")]
    Kdbndp = 6,

    /// <summary>
    /// Oscar
    /// </summary>
    [Description("Oscar")]
    Oscar = 7,

    /// <summary>
    /// MySql Connector
    /// </summary>
    [Description("MySql Connector")]
    MySqlConnector = 8,

    /// <summary>
    /// Microsoft Access
    /// </summary>
    [Description("Microsoft Access")]
    Access = 9,

    /// <summary>
    /// OpenGauss
    /// </summary>
    [Description("OpenGauss")]
    OpenGauss = 10,

    /// <summary>
    /// QuestDB 时序数据库
    /// </summary>
    [Description("QuestDB时序数据库")]
    QuestDB = 11,

    /// <summary>
    /// HG
    /// </summary>
    [Description("HG")]
    HG = 12,

    /// <summary>
    /// ClickHouse 列式数据库
    /// </summary>
    [Description("ClickHouse列式数据库")]
    ClickHouse = 13,

    /// <summary>
    /// 南大通用 GBase
    /// </summary>
    [Description("南大通用GBase")]
    GBase = 14,

    /// <summary>
    /// ODBC
    /// </summary>
    [Description("ODBC")]
    Odbc = 15,

    /// <summary>
    /// 蚂蚁 OceanBase(Oracle 兼容模式)
    /// </summary>
    [Description("蚂蚁OceanBase")]
    OceanBaseForOracle = 16,

    /// <summary>
    /// TDengine 时序数据库
    /// </summary>
    [Description("TDengine时序数据库")]
    TDengine = 17,

    /// <summary>
    /// 华为 GaussDB
    /// </summary>
    [Description("华为GaussDB")]
    GaussDB = 18,

    /// <summary>
    /// 蚂蚁 OceanBase
    /// </summary>
    [Description("蚂蚁OceanBase")]
    OceanBase = 19,

    /// <summary>
    /// PingCAP TiDB 分布式数据库
    /// </summary>
    [Description("PingCAP TiDB分布式数据库")]
    Tidb = 20,

    /// <summary>
    /// 海量数据 Vastbase
    /// </summary>
    [Description("海量数据Vastbase")]
    Vastbase = 21,

    /// <summary>
    /// 阿里云 PolarDB
    /// </summary>
    [Description("阿里云PolarDB")]
    PolarDB = 22,

    /// <summary>
    /// Apache Doris
    /// </summary>
    [Description("Apache Doris")]
    Doris = 23,

    /// <summary>
    /// 虚谷数据库
    /// </summary>
    [Description("虚谷数据库")]
    Xugu = 24,

    /// <summary>
    /// 中兴通讯 GoldenDB
    /// </summary>
    [Description("中兴通讯GoldenDB")]
    GoldenDB = 25,

    /// <summary>
    /// 腾讯云 TDSQL PostgreSQL 版 ODBC
    /// </summary>
    [Description("腾讯云TDSQL PostgreSQL版ODBC")]
    TDSQLForPGODBC = 26,

    /// <summary>
    /// 腾讯云 TDSQL
    /// </summary>
    [Description("腾讯云TDSQL")]
    TDSQL = 27,

    /// <summary>
    /// SAP HANA
    /// </summary>
    [Description("SAP HANA")]
    HANA = 28,

    /// <summary>
    /// IBM DB2
    /// </summary>
    [Description("IBM DB2")]
    DB2 = 29,

    /// <summary>
    /// 华为 GaussDB
    /// </summary>
    [Description("华为GaussDB")]
    GaussDBNative = 30,

    /// <summary>
    /// DuckDB
    /// </summary>
    [Description("DuckDB")]
    DuckDB = 31,

    /// <summary>
    /// MongoDB
    /// </summary>
    [Description("MongoDB")]
    MongoDb = 32,

    /// <summary>
    /// 自定义
    /// </summary>
    [Description("自定义")]
    Custom = 900
}
