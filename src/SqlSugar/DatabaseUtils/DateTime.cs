﻿// ------------------------------------------------------------------------
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
/// <see cref="DatabaseUtil"/> Database Nvarchar 类型工具类
/// </summary>
internal partial class DatabaseUtil
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
            case DbType.MySqlConnector:
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
            default:
                throw new SqlSugarException("数据库类型配置异常！");
        }
    }
}