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

using System.Collections;
using System.Data;
using System.Reflection;
using SqlSugar;

// ReSharper disable once CheckNamespace
namespace Fast.SqlSugar;

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
        var sugarTable = type.GetCustomAttribute<SugarTable>(true);
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
        return type.GetCustomAttribute<SugarTable>(true);
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
            type = list[0]
                .GetType();
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
                var sugarColumn = property.GetCustomAttribute<SugarColumn>(true);

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