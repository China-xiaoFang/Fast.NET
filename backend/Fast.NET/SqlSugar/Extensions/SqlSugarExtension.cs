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

using System.Collections;
using System.Data;
using System.Reflection;
using Fast.IaaS;
using SqlSugar;

namespace Fast.SqlSugar.Extensions;

/// <summary>
/// <see cref="ISqlSugarClient"/> SqlSugar 拓展类
/// </summary>
[SuppressSniffer]
public static class SqlSugarExtension
{
    /// <summary>
    /// 获取SugarTable特性中的TableName
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetSugarTableName(this Type type)
    {
        var sugarTable = type.GetCustomAttribute<SugarTable>();
        if (sugarTable != null && !string.IsNullOrEmpty(sugarTable.TableName))
        {
            return sugarTable.TableName;
        }

        return type.Name;
    }

    /// <summary>
    /// 获取SugarTable特性
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static SugarTable GetSugarTableAttribute(this Type type)
    {
        return type.GetCustomAttribute<SugarTable>();
    }

    /// <summary>
    /// 转为DataTable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<DataTable> ToDataTable<T>(this List<T> list)
    {
        var result = new List<DataTable>();

        // 判断是否为空
        if (list == null || !list.Any())
            return result;

        var type = typeof(T);
        if (type.Name == "Object")
        {
            type = list[0].GetType();
        }

        // 获取所有属性
        var properties = type.GetProperties();
        foreach (var item in list)
        {
            var dataTable = new DataTable();

            // 表名赋值
            dataTable.TableName = type.GetSugarTableName();

            var tempList = new ArrayList();

            foreach (var property in properties)
            {
                var colType = property.PropertyType;
                // 泛型
                if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    colType = colType.GetGenericArguments()[0];
                }

                // 获取Sugar列特性
                var sugarColumn = property.GetCustomAttribute<SugarColumn>(false);

                // 判断忽略列
                if (sugarColumn?.IsIgnore == true)
                {
                    continue;
                }

                var columnName = sugarColumn?.ColumnName ?? property.Name;

                dataTable.Columns.Add(columnName, colType);

                tempList.Add(property.GetValue(item, null));
            }

            dataTable.LoadDataRow(tempList.ToArray(), true);

            result.Add(dataTable);
        }

        return result;
    }
}