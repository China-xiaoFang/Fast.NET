// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using SqlSugar;

namespace Fast.SqlSugar;

/// <summary>
/// 为 <see cref="DbType"/> 提供扩展方法
/// </summary>
[SuppressSniffer]
public static class DbTypeExtension
{
    /// <summary>
    /// 将 <see cref="SugarDbType"/> 转换为 <see cref="DbType"/>
    /// </summary>
    /// <param name="sugarDbType">sugar Db 类型</param>
    /// <returns>将 SugarDbType 转换为 DbType</returns>
    public static DbType ToDbType(this SugarDbType sugarDbType)
    {
        return sugarDbType switch
        {
            SugarDbType.MySql => DbType.MySql,
            SugarDbType.SqlServer => DbType.SqlServer,
            SugarDbType.Sqlite => DbType.Sqlite,
            SugarDbType.Oracle => DbType.Oracle,
            SugarDbType.PostgreSQL => DbType.PostgreSQL,
            SugarDbType.Dm => DbType.Dm,
            SugarDbType.Kdbndp => DbType.Kdbndp,
            SugarDbType.Oscar => DbType.Oscar,
            SugarDbType.MySqlConnector => DbType.MySqlConnector,
            SugarDbType.Access => DbType.Access,
            SugarDbType.OpenGauss => DbType.OpenGauss,
            SugarDbType.QuestDB => DbType.QuestDB,
            SugarDbType.HG => DbType.HG,
            SugarDbType.ClickHouse => DbType.ClickHouse,
            SugarDbType.GBase => DbType.GBase,
            SugarDbType.Odbc => DbType.Odbc,
            SugarDbType.OceanBaseForOracle => DbType.OceanBaseForOracle,
            SugarDbType.TDengine => DbType.TDengine,
            SugarDbType.GaussDB => DbType.GaussDB,
            SugarDbType.OceanBase => DbType.OceanBase,
            SugarDbType.Tidb => DbType.Tidb,
            SugarDbType.Vastbase => DbType.Vastbase,
            SugarDbType.PolarDB => DbType.PolarDB,
            SugarDbType.Doris => DbType.Doris,
            SugarDbType.Xugu => DbType.Xugu,
            SugarDbType.GoldenDB => DbType.GoldenDB,
            SugarDbType.TDSQLForPGODBC => DbType.TDSQLForPGODBC,
            SugarDbType.TDSQL => DbType.TDSQL,
            SugarDbType.HANA => DbType.HANA,
            SugarDbType.DB2 => DbType.DB2,
            SugarDbType.GaussDBNative => DbType.GaussDBNative,
            SugarDbType.DuckDB => DbType.DuckDB,
            SugarDbType.MongoDb => DbType.MongoDb,
            SugarDbType.Custom => DbType.Custom,
            _ => throw new ArgumentOutOfRangeException(nameof(sugarDbType), sugarDbType, "不支持的数据库类型")
        };
    }

    /// <summary>
    /// 将 <see cref="DbType"/> 转换为 <see cref="SugarDbType"/>
    /// </summary>
    /// <param name="dbType">数据库类型</param>
    /// <returns>将 DbType 转换为 SugarDbType</returns>
    public static SugarDbType ToSugarDbType(this DbType dbType)
    {
        return dbType switch
        {
            DbType.MySql => SugarDbType.MySql,
            DbType.SqlServer => SugarDbType.SqlServer,
            DbType.Sqlite => SugarDbType.Sqlite,
            DbType.Oracle => SugarDbType.Oracle,
            DbType.PostgreSQL => SugarDbType.PostgreSQL,
            DbType.Dm => SugarDbType.Dm,
            DbType.Kdbndp => SugarDbType.Kdbndp,
            DbType.Oscar => SugarDbType.Oscar,
            DbType.MySqlConnector => SugarDbType.MySqlConnector,
            DbType.Access => SugarDbType.Access,
            DbType.OpenGauss => SugarDbType.OpenGauss,
            DbType.QuestDB => SugarDbType.QuestDB,
            DbType.HG => SugarDbType.HG,
            DbType.ClickHouse => SugarDbType.ClickHouse,
            DbType.GBase => SugarDbType.GBase,
            DbType.Odbc => SugarDbType.Odbc,
            DbType.OceanBaseForOracle => SugarDbType.OceanBaseForOracle,
            DbType.TDengine => SugarDbType.TDengine,
            DbType.GaussDB => SugarDbType.GaussDB,
            DbType.OceanBase => SugarDbType.OceanBase,
            DbType.Tidb => SugarDbType.Tidb,
            DbType.Vastbase => SugarDbType.Vastbase,
            DbType.PolarDB => SugarDbType.PolarDB,
            DbType.Doris => SugarDbType.Doris,
            DbType.Xugu => SugarDbType.Xugu,
            DbType.GoldenDB => SugarDbType.GoldenDB,
            DbType.TDSQLForPGODBC => SugarDbType.TDSQLForPGODBC,
            DbType.TDSQL => SugarDbType.TDSQL,
            DbType.HANA => SugarDbType.HANA,
            DbType.DB2 => SugarDbType.DB2,
            DbType.GaussDBNative => SugarDbType.GaussDBNative,
            DbType.DuckDB => SugarDbType.DuckDB,
            DbType.MongoDb => SugarDbType.MongoDb,
            DbType.Custom => SugarDbType.Custom,
            _ => throw new ArgumentOutOfRangeException(nameof(dbType), dbType, "不支持的数据库类型")
        };
    }
}
