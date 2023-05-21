using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Fast.SqlSugar.Model;

namespace Fast.SqlSugar.Helper;

/// <summary>
/// 数据库帮助类
/// 各种类型数据库的兼容
/// </summary>
static class DataBaseHelper
{
    /// <summary>
    /// 得到数据库连接字符串
    /// </summary>
    /// <param name="dbInfo"></param>
    /// <returns></returns>
    public static string GetConnectionStr(SysTenantDataBaseModel dbInfo)
    {
        var connectionStr = "";
        // 目前暂时支持Sql Server 和 MySql
        switch (dbInfo.DbType)
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
                break;
            case DbType.Oracle:
                break;
            case DbType.PostgreSQL:
                break;
            case DbType.Dm:
                break;
            case DbType.Kdbndp:
                break;
            case DbType.Oscar:
                break;
            case DbType.MySqlConnector:
                break;
            case DbType.Access:
                break;
            case DbType.OpenGauss:
                break;
            case DbType.Custom:
                break;
            case DbType.QuestDB:
            case DbType.HG:
            case DbType.ClickHouse:
            default:
                throw new SqlSugarException("数据库类型配置异常！");
        }

        return connectionStr;
    }

    /// <summary>
    /// 目前暂时支持Sql Server 和 MySql
    /// </summary>
    /// <param name="dbType"></param>
    /// <returns></returns>
    public static ConfigureExternalServices GetSugarExternalServices(DbType dbType)
    {
        var externalServices = new ConfigureExternalServices
        {
            EntityNameService = (type, entityInfo) =>
            {
                // 全局开启创建表按照字段排序，避免重复代码
                entityInfo.IsCreateTableFiledSort = true;

                // Table Name 配置，如果使用SqlSugar的规范，其实这里是不会走的
                var tableAttribute = type.GetCustomAttribute<TableAttribute>();
                if (tableAttribute != null)
                {
                    entityInfo.DbTableName = tableAttribute.Name;
                }
            },
            EntityService = (propertyInfo, columnInfo) =>
            {
                // 主键配置，如果使用SqlSugar的规范，其实这里是不会走的
                var keyAttribute = propertyInfo.GetCustomAttribute<KeyAttribute>();
                if (keyAttribute != null)
                {
                    columnInfo.IsPrimarykey = true;
                }

                // 列名配置，如果使用SqlSugar的规范，其实这里是不会走的
                var columnAttribute = propertyInfo.GetCustomAttribute<ColumnAttribute>();
                if (columnAttribute != null)
                {
                    columnInfo.DbColumnName = columnAttribute.Name;
                }

                // 可空类型配置
                if (propertyInfo.PropertyType.IsGenericType &&
                    propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    columnInfo.IsNullable = true;
                }

                // 这里的所有数据库类型，默认是根据SqlServer配置的
                var columnDbType = columnInfo.DataType?.ToUpper();
                if (columnDbType == null)
                    return;
                // String
                if (columnDbType.StartsWith("NVARCHAR"))
                {
                    var length = columnDbType.Substring("NVARCHAR(".Length, columnDbType.Length - "NVARCHAR(".Length - 1);
                    SetDbTypeNvarchar(dbType, length, ref columnInfo);
                }

                // DateTime
                if (columnDbType == "DATETIMEOFFSET")
                {
                    SetDbTypeDateTime(dbType, ref columnInfo);
                }
            }
        };
        return externalServices;
    }

    /// <summary>
    /// 设置Nvarchar类型
    /// </summary>
    /// <param name="dbType"></param>
    /// <param name="length"></param>
    /// <param name="columnInfo"></param>
    private static void SetDbTypeNvarchar(DbType dbType, string length, ref EntityColumnInfo columnInfo)
    {
        switch (dbType)
        {
            case DbType.MySql:
                columnInfo.DataType = length.ToLower() == "max" ? "longtext" : $"varchar({length})";
                break;
            case DbType.SqlServer:
                columnInfo.DataType = $"Nvarchar({length})";
                break;
            case DbType.Sqlite:
                break;
            case DbType.Oracle:
                break;
            case DbType.PostgreSQL:
                break;
            case DbType.Dm:
                break;
            case DbType.Kdbndp:
                break;
            case DbType.Oscar:
                break;
            case DbType.MySqlConnector:
                break;
            case DbType.Access:
                break;
            case DbType.OpenGauss:
                break;
            case DbType.QuestDB:
                break;
            case DbType.HG:
                break;
            case DbType.ClickHouse:
                break;
            case DbType.Custom:
                break;
            default:
                throw new SqlSugarException("数据库类型配置异常！");
        }
    }

    /// <summary>
    /// 设置DateTime类型
    /// </summary>
    /// <param name="dbType"></param>
    /// <param name="columnInfo"></param>
    private static void SetDbTypeDateTime(DbType dbType, ref EntityColumnInfo columnInfo)
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
                break;
            case DbType.Oracle:
                break;
            case DbType.PostgreSQL:
                break;
            case DbType.Dm:
                break;
            case DbType.Kdbndp:
                break;
            case DbType.Oscar:
                break;
            case DbType.MySqlConnector:
                break;
            case DbType.Access:
                break;
            case DbType.OpenGauss:
                break;
            case DbType.QuestDB:
                break;
            case DbType.HG:
                break;
            case DbType.ClickHouse:
                break;
            case DbType.Custom:
                break;
            default:
                throw new SqlSugarException("数据库类型配置异常！");
        }
    }
}