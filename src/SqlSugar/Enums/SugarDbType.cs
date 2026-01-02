// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using System.ComponentModel;

namespace Fast.SqlSugar;

/// <summary>
/// <see cref="SugarDbType"/> Sugar数据库类型枚举
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
    /// QuestDB时序数据库
    /// </summary>
    [Description("QuestDB时序数据库")]
    QuestDB = 11,

    /// <summary>
    /// HG
    /// </summary>
    [Description("HG")]
    HG = 12,

    /// <summary>
    /// ClickHouse列式数据库
    /// </summary>
    [Description("ClickHouse列式数据库")]
    ClickHouse = 13,

    /// <summary>
    /// 南大通用GBase
    /// </summary>
    [Description("南大通用GBase")]
    GBase = 14,

    /// <summary>
    /// ODBC
    /// </summary>
    [Description("ODBC")]
    Odbc = 15,

    /// <summary>
    /// 蚂蚁OceanBase(Oracle兼容模式)
    /// </summary>
    [Description("蚂蚁OceanBase")]
    OceanBaseForOracle = 16,

    /// <summary>
    /// TDengine时序数据库
    /// </summary>
    [Description("TDengine时序数据库")]
    TDengine = 17,

    /// <summary>
    /// 华为GaussDB
    /// </summary>
    [Description("华为GaussDB")]
    GaussDB = 18,

    /// <summary>
    /// 蚂蚁OceanBase
    /// </summary>
    [Description("蚂蚁OceanBase")]
    OceanBase = 19,

    /// <summary>
    /// PingCAP TiDB分布式数据库
    /// </summary>
    [Description("PingCAP TiDB分布式数据库")]
    Tidb = 20,

    /// <summary>
    /// 海量数据Vastbase
    /// </summary>
    [Description("海量数据Vastbase")]
    Vastbase = 21,

    /// <summary>
    /// 阿里云PolarDB
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
    /// 中兴通讯GoldenDB
    /// </summary>
    [Description("中兴通讯GoldenDB")]
    GoldenDB = 25,

    /// <summary>
    /// 腾讯云TDSQL PostgreSQL版ODBC
    /// </summary>
    [Description("腾讯云TDSQL PostgreSQL版ODBC")]
    TDSQLForPGODBC = 26,

    /// <summary>
    /// 腾讯云TDSQL
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
    /// 华为GaussDB
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