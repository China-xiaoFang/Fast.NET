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

namespace Fast.SqlSugar;

/// <summary>
/// <see cref="DbType"/> 拓展类
/// </summary>
[SuppressSniffer]
public static class DbTypeExtension
{
    /// <summary>
    /// 将 <see cref="SugarDbType"/> 转换为 <see cref="DbType"/>
    /// </summary>
    /// <param name="sugarDbType"><see cref="SugarDbType"/></param>
    /// <returns><see cref="DbType"/></returns>
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
    /// <param name="dbType"><see cref="DbType"/></param>
    /// <returns><see cref="SugarDbType"/></returns>
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