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

using System;
using System.Collections.Generic;
using System.Data;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="DataTable"/> 拓展类
/// </summary>
public static class DataTableExtension
{
    /// <summary>
    /// 转换为DataTable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"><see cref="IEnumerable{T}"/></param>
    /// <returns><see cref="DataTable"/></returns>
    public static DataTable ToDataTable<T>(this IEnumerable<T> data)
    {
        var dataTable = new DataTable();

        // 获取模型类型的属性列表
        var properties = typeof(T).GetProperties();

        // 创建 DataTable 的列
        foreach (var prop in properties)
        {
            dataTable.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        // 将数据添加到 DataTable
        foreach (var item in data)
        {
            var values = new object[properties.Length];
            for (var i = 0; i < properties.Length; i++)
            {
                values[i] = properties[i]
                    .GetValue(item, null);
            }

            dataTable.Rows.Add(values);
        }

        return dataTable;
    }

    /// <summary>
    /// DataTable To List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataTable"><see cref="DataTable"/></param>
    /// <returns><see cref="List{T}"/></returns>
    public static List<T> ToList<T>(this DataTable dataTable) where T : new()
    {
        var list = new List<T>();

        foreach (DataRow row in dataTable.Rows)
        {
            var item = new T();

            foreach (DataColumn column in dataTable.Columns)
            {
                var property = typeof(T).GetProperty(column.ColumnName);
                if (property != null && row[column] != DBNull.Value)
                {
                    property.SetValue(item, row[column]);
                }
            }

            list.Add(item);
        }

        return list;
    }
}