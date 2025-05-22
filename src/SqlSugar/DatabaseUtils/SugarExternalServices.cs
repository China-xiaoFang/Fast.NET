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

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using SqlSugar;

// ReSharper disable once CheckNamespace
namespace Fast.SqlSugar;

/// <summary>
/// <see cref="DatabaseUtil"/> SugarExternalServices工具类
/// </summary>
internal partial class DatabaseUtil
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