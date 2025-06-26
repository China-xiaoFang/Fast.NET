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

using SqlSugar;

// ReSharper disable once CheckNamespace
namespace Fast.SqlSugar;

/// <summary>
/// <see cref="DatabaseUtil"/> Database DateTime 类型工具类
/// </summary>
internal partial class DatabaseUtil
{
    /// <summary>
    /// 设置 <see cref="int"/> 类型
    /// </summary>
    /// <param name="dbType"></param>
    /// <param name="columnInfo"></param>
    internal static void SetDbTypeInt(DbType dbType, EntityColumnInfo columnInfo)
    {
        switch (dbType)
        {
            // MySQL 系列
            case DbType.MySql:
            case DbType.MySqlConnector:
            case DbType.Tidb:
            case DbType.PolarDB:
            case DbType.GBase:
            case DbType.HG:
                columnInfo.DataType = "int";
                break;

            // SQL Server
            case DbType.SqlServer:
                columnInfo.DataType = "int";
                break;

            // PostgreSQL 系列
            case DbType.PostgreSQL:
            case DbType.OpenGauss:
            case DbType.GaussDB:
            case DbType.GaussDBNative:
            case DbType.TDSQL:
            case DbType.TDSQLForPGODBC:
            case DbType.Vastbase:
            case DbType.Xugu:
            case DbType.GoldenDB:
            case DbType.Doris:
            case DbType.DuckDB:
                columnInfo.DataType = "integer";
                break;

            // Oracle 系列
            case DbType.Oracle:
            case DbType.OceanBaseForOracle:
                columnInfo.DataType = "number(10,0)";
                break;

            // SQLite
            case DbType.Sqlite:
                columnInfo.DataType = "integer";
                break;

            // ClickHouse
            case DbType.ClickHouse:
                columnInfo.DataType = "int32";
                break;

            // QuestDB
            case DbType.QuestDB:
                columnInfo.DataType = "int";
                break;

            // MongoDB
            case DbType.MongoDb:
                columnInfo.DataType = "int32";
                break;

            // Access
            case DbType.Access:
                columnInfo.DataType = "integer";
                break;

            // Dm、Oscar、Kdbndp、DB2、HANA、Odbc
            case DbType.Dm:
            case DbType.Oscar:
            case DbType.Kdbndp:
            case DbType.DB2:
            case DbType.HANA:
            case DbType.Odbc:
                columnInfo.DataType = "int";
                break;

            // OceanBase（MySQL 模式）
            case DbType.OceanBase:
                columnInfo.DataType = "int";
                break;

            // TDengine：原生不支持 int，建议不使用
            case DbType.TDengine:
                throw new SqlSugarException("TDengine 不支持 int 类型，请使用 long 或 float 替代。");

            // 默认
            case DbType.Custom:
            default:
                columnInfo.DataType = "int";
                break;
        }
    }
}