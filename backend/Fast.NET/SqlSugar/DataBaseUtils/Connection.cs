// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Fast.SqlSugar.Commons;
using SqlSugar;

namespace Fast.SqlSugar.DataBaseUtils;

/// <summary>
/// <see cref="DataBaseUtil"/> 连接字符串工具类
/// </summary>
internal partial class DataBaseUtil
{
    /// <summary>
    /// 得到数据库连接字符串
    /// </summary>
    /// <param name="dbType"><see cref="DbType"/> 数据库类型</param>
    /// <param name="dbInfo"><see cref="DbConnectionInfo"/> 数据库连接信息</param>
    /// <remarks>目前只验证了Sql Server 和 MySql</remarks>
    /// <returns></returns>
    internal static string GetConnectionStr(DbType dbType, DbConnectionInfo dbInfo)
    {
        var connectionStr = "";
        switch (dbType)
        {
            case DbType.MySql:
                connectionStr =
                    $"Data Source={dbInfo.ServiceIp};Database={dbInfo.DbName};User ID={dbInfo.DbUser};Password={dbInfo.DbPwd};pooling=true;port={dbInfo.Port};sslmode=none;CharSet=utf8;Convert Zero Datetime=True;Allow Zero Datetime=True;";
                break;
            case DbType.SqlServer:
                connectionStr =
                    $"Data Source={dbInfo.ServiceIp};Initial Catalog={dbInfo.DbName};User ID={dbInfo.DbUser};Password={dbInfo.DbPwd};MultipleActiveResultSets=true;max pool size=100;";
                break;
            case DbType.Sqlite:
                connectionStr = $"Data Source={dbInfo.DbName};Version=3;";
                break;
            case DbType.Oracle:
                connectionStr = $"Data Source={dbInfo.ServiceIp};User ID={dbInfo.DbUser};Password={dbInfo.DbPwd};";
                break;
            case DbType.PostgreSQL:
                connectionStr =
                    $"Host={dbInfo.ServiceIp};Port={dbInfo.Port};Database={dbInfo.DbName};Username={dbInfo.DbUser};Password={dbInfo.DbPwd};";
                break;
            case DbType.Dm:
                connectionStr = $"Data Source={dbInfo.ServiceIp}, {dbInfo.Port};User Id={dbInfo.DbUser};Password={dbInfo.DbPwd};";
                break;
            case DbType.Kdbndp:
                connectionStr = $"Data Source={dbInfo.ServiceIp}:{dbInfo.Port};User ID={dbInfo.DbUser};Password={dbInfo.DbPwd};";
                break;
            case DbType.Oscar:
                connectionStr =
                    $"Data Source={dbInfo.ServiceIp}:{dbInfo.Port};Initial Catalog={dbInfo.DbName};User ID={dbInfo.DbUser};Password={dbInfo.DbPwd};";
                break;
            case DbType.MySqlConnector:
                connectionStr =
                    $"Data Source={dbInfo.ServiceIp};Database={dbInfo.DbName};User ID={dbInfo.DbUser};Password={dbInfo.DbPwd};";
                break;
            case DbType.Access:
                connectionStr = $"Data Source={dbInfo.DbName};Provider=Microsoft.ACE.OLEDB.12.0;Persist Security Info=False;";
                break;
            case DbType.OpenGauss:
                connectionStr =
                    $"Host={dbInfo.ServiceIp};Port={dbInfo.Port};Username={dbInfo.DbUser};Password={dbInfo.DbPwd};Database={dbInfo.DbName};";
                break;
            case DbType.Custom:
                // 自定义数据库类型的连接字符串配置
                //connectionStr = dbInfo.CustomConnectionStr;
                break;
            case DbType.QuestDB:
                connectionStr =
                    $"Server={dbInfo.ServiceIp}:{dbInfo.Port};Database={dbInfo.DbName};User Id={dbInfo.DbUser};Password={dbInfo.DbPwd};";
                break;
            case DbType.HG:
                connectionStr =
                    $"Server={dbInfo.ServiceIp}:{dbInfo.Port};Database={dbInfo.DbName};User Id={dbInfo.DbUser};Password={dbInfo.DbPwd};";
                break;
            case DbType.ClickHouse:
                connectionStr =
                    $"Host={dbInfo.ServiceIp};Port={dbInfo.Port};Username={dbInfo.DbUser};Password={dbInfo.DbPwd};Database={dbInfo.DbName};";
                break;
            case DbType.GBase:
                connectionStr =
                    $"Server={dbInfo.ServiceIp};Port={dbInfo.Port};User ID={dbInfo.DbUser};Password={dbInfo.DbPwd};Database={dbInfo.DbName};";
                break;
            case DbType.Odbc:
                // ODBC 数据库连接字符串配置
                //connectionStr = dbInfo.CustomConnectionStr;
                break;
            case DbType.OceanBaseForOracle:
            case DbType.TDengine:
            case DbType.GaussDB:
            case DbType.OceanBase:
            case DbType.Tidb:
            case DbType.Vastbase:
            default:
                throw new SqlSugarException("数据库类型配置异常！");
        }

        return connectionStr;
    }
}