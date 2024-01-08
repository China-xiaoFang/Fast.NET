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

using SqlSugar;

namespace Fast.SqlSugar.DataBaseUtils;

/// <summary>
/// <see cref="DataBaseUtil"/> Database Nvarchar 类型工具类
/// </summary>
internal partial class DataBaseUtil
{
    /// <summary>
    /// 设置DateTime类型
    /// </summary>
    /// <param name="dbType"></param>
    /// <param name="columnInfo"></param>
    internal static void SetDbTypeDateTime(DbType dbType, ref EntityColumnInfo columnInfo)
    {
        switch (dbType)
        {
            case DbType.MySql:
                columnInfo.DataType = "datetime";
                break;
            case DbType.SqlServer:
                columnInfo.DataType = "datetimeoffset";
                break;
            case DbType.Sqlite:
                columnInfo.DataType = "TEXT";
                break;
            case DbType.Oracle:
                columnInfo.DataType = "TIMESTAMP WITH TIME ZONE";
                break;
            case DbType.PostgreSQL:
                columnInfo.DataType = "TIMESTAMP WITH TIME ZONE";
                break;
            case DbType.Dm:
                columnInfo.DataType = "TIMESTAMP";
                break;
            case DbType.Kdbndp:
                columnInfo.DataType = "DATETIME";
                break;
            case DbType.Oscar:
                columnInfo.DataType = "DATETIME";
                break;
            case DbType.MySqlConnector:
                columnInfo.DataType = "datetime";
                break;
            case DbType.Access:
                columnInfo.DataType = "DATETIME";
                break;
            case DbType.OpenGauss:
                columnInfo.DataType = "TIMESTAMP WITH TIME ZONE";
                break;
            case DbType.Custom:
                break;
            case DbType.QuestDB:
                columnInfo.DataType = "TIMESTAMP";
                break;
            case DbType.HG:
                columnInfo.DataType = "DATETIME";
                break;
            case DbType.ClickHouse:
                columnInfo.DataType = "DATETIME";
                break;
            case DbType.GBase:
                columnInfo.DataType = "DATETIME";
                break;
            case DbType.Odbc:
                // ODBC 数据库设置
                columnInfo.DataType = "DATETIME";
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
    }
}