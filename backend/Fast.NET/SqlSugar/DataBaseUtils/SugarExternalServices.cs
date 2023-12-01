// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using SqlSugar;

namespace Fast.SqlSugar.DataBaseUtils;

/// <summary>
/// <see cref="DataBaseUtil"/> SugarExternalServices工具类
/// </summary>
internal partial class DataBaseUtil
{
    /// <summary>
    /// 目前只验证了Sql Server 和 MySql
    /// </summary>
    /// <param name="dbType"></param>
    /// <returns></returns>
    internal static ConfigureExternalServices GetSugarExternalServices(DbType dbType)
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
                if (columnDbType.ToUpper().StartsWith("NVARCHAR"))
                {
                    var length = columnDbType.ToUpper()
                        .Substring("NVARCHAR(".Length, columnDbType.Length - "NVARCHAR(".Length - 1);
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
}