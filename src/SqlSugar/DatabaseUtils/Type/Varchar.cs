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

using System.Text.RegularExpressions;
using SqlSugar;

// ReSharper disable once CheckNamespace
namespace Fast.SqlSugar;

/// <summary>
/// <see cref="DatabaseUtil"/> Database Varchar 类型工具类
/// </summary>
internal partial class DatabaseUtil
{
    /// <summary>
    /// 设置 Varchar 类型
    /// </summary>
    /// <param name="dbType"></param>
    /// <param name="columnInfo"></param>
    internal static void SetDbTypeVarchar(DbType dbType, EntityColumnInfo columnInfo)
    {
        // 读取长度
        var match = Regex.Match(columnInfo.DataType, @"varchar\(\s*(\d+)\s*\)", RegexOptions.IgnoreCase);
        var length = match.Success ? int.Parse(match?.Groups[1].Value) : 50;

        switch (dbType)
        {
            // MySQL 系列
            case DbType.MySql:
            case DbType.MySqlConnector:
            case DbType.Tidb:
            case DbType.PolarDB:
            case DbType.GBase:
            case DbType.HG:
                columnInfo.DataType = $"varchar({length})";
                break;

            // SQL Server
            case DbType.SqlServer:
                columnInfo.DataType = $"varchar({length})";
                break;

            // PostgreSQL 系列
            case DbType.PostgreSQL:
            case DbType.OpenGauss:
            case DbType.TDSQLForPGODBC:
            case DbType.TDSQL:
            case DbType.GaussDB:
            case DbType.GaussDBNative:
            case DbType.Vastbase:
            case DbType.Xugu:
            case DbType.Doris:
            case DbType.GoldenDB:
            case DbType.DuckDB:
                columnInfo.DataType = $"varchar({length})";
                break;

            // Oracle 系列
            case DbType.Oracle:
            case DbType.OceanBaseForOracle:
                columnInfo.DataType = $"varchar2({length})";
                break;

            // SQLite
            case DbType.Sqlite:
                columnInfo.DataType = "text";
                break;

            // ClickHouse
            case DbType.ClickHouse:
                columnInfo.DataType = "string";
                break;

            // Access
            case DbType.Access:
                columnInfo.DataType = "text";
                break;

            // MongoDB
            case DbType.MongoDb:
                columnInfo.DataType = "string";
                break;

            // 默认
            case DbType.Dm:
            case DbType.Oscar:
            case DbType.Kdbndp:
            case DbType.DB2:
            case DbType.HANA:
            case DbType.Odbc:
            case DbType.QuestDB:
            case DbType.TDengine:
            case DbType.OceanBase:
            case DbType.Custom:
            default:
                columnInfo.DataType = $"varchar({length})";
                break;
        }
    }
}