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

using System.Data;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="DataTable"/> 拓展类
/// </summary>
[SuppressSniffer]
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
                values[i] = properties[i].GetValue(item, null);
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