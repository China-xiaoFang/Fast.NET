using System.Reflection;
using System.Text.RegularExpressions;
using Fast.Core.Json.Extension;
using Fast.Core.MiniExcel.AttributeFilter;
using Fast.Core.MiniExcel.Dto;
using MiniExcelLibs;

namespace Fast.Core.MiniExcel.Extension;

/// <summary>
/// MiniExcel扩展类
/// </summary>
public static class Extension
{
    /// <summary>
    /// 导出类型处理
    /// </summary>
    /// <param name="attr"></param>
    /// <param name="property"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static string ExportTypeDispose(MiniExcelColumn attr, PropertyInfo property, object value)
    {
        string result;

        // 是否为Json字符串
        if (attr.IsJson)
        {
            result = value.ToJsonString();
        }
        else if (property.PropertyType.IsEnum || value is Enum)
        {
            result = (value as Enum).GetDescription();
        }
        else
        {
            switch (property.PropertyType.Name)
            {
                // 判断是否为Boolean类型
                case "Boolean":
                    var boolVal = value.ParseToBool();
                    if (boolVal)
                        result = attr.TrueDesc ?? "是";
                    else
                        result = attr.FalseDesc ?? "否";

                    break;
                // 判断是否为DateTime类型
                case "DateTime":
                    result = value == null ? "" : (value as DateTime?)?.ToString(attr.ConvertDateTime);

                    break;
                default:
                    result = value.ParseToString();
                    break;
            }
        }

        return result;
    }

    /// <summary>
    /// 导出List处理
    /// </summary>
    /// <param name="attr"></param>
    /// <param name="property"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    private static string ExportListDispose(MiniExcelColumn attr, PropertyInfo property, IEnumerable<object> values)
    {
        var result = "";

        // 判断List是否需要深度处理
        if (attr.IsListDeep)
        {
            // 是否换行，拼接字符
            var newLineStr = attr.IsListLine ? "\r\n" : "，";

            foreach (var val in values)
            {
                if (val == null)
                    continue;

                var resultListVal = "";

                foreach (var listProperty in val.GetType().GetProperties())
                {
                    var listAttr = listProperty.GetCustomAttribute<MiniExcelColumn>(true);
                    if (listAttr == null)
                        continue;

                    // 判断导出是否忽略列
                    if (listAttr.IsIgnore || listAttr.IsExportIgnore)
                        continue;

                    var listVal = listProperty.GetValue(val, null);

                    resultListVal += $"{ExportTypeDispose(listAttr, listProperty, listVal)}，";
                }

                resultListVal = resultListVal.TrimEnd('，');
                result += $"{resultListVal}{newLineStr}";
            }

            result = result[..^newLineStr.Length];
        }
        else
        {
            result = values.Aggregate(result, (current, value) => current + $"{ExportTypeDispose(attr, property, value)}，");
            result = result.TrimEnd('，');
        }

        return result;
    }

    /// <summary>
    /// 得到MiniExcel列信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="isRetainColumn">是否保留列标题</param>
    /// <returns></returns>
    public static IEnumerable<IDictionary<string, object>> GetMiniExcelColumns<T>(this List<T> list, bool isRetainColumn = false)
    {
        var result = new List<IDictionary<string, object>>();

        var type = typeof(T);
        // 获取所有属性
        var properties = type.GetProperties();

        // 判断List集合是否为Null，如果为Null，保留列头
        if (isRetainColumn && list.Count == 0)
        {
            // 通过反射自动创建泛型对象
            list.Add(Activator.CreateInstance<T>());
        }

        foreach (var item in list)
        {
            var itemResult = new List<MiniExcelDictionaryDto>();
            // 遍历所有属性
            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<MiniExcelColumn>(true);
                if (attr == null)
                    continue;

                // 判断导出是否忽略列
                if (attr.IsIgnore || attr.IsExportIgnore)
                    continue;

                // 值
                var value = property.GetValue(item, null);
                // 判断是否为List集合
                if (property.PropertyType.IsGenericType && property.PropertyType.GetInterface("IEnumerable", false) != null)
                {
                    if (value is not IEnumerable<object> listVal)
                        continue; // 值为空不走下面

                    value = ExportListDispose(attr, property, listVal);
                }
                else
                {
                    value = ExportTypeDispose(attr, property, value);
                }

                itemResult.Add(new MiniExcelDictionaryDto
                {
                    Key = attr.ColumnDescription ?? property.Name,
                    Value = value,
                    Sort = attr.Sort > 0 ? attr.Sort - 1 : itemResult.Count
                });
            }

            result.Add(itemResult.OrderBy(ob => ob.Sort)
                .ToDictionary(dictionaryDto => dictionaryDto.Key, dictionaryDto => dictionaryDto.Value));
        }

        return result;
    }

    /// <summary>
    /// 导出Excel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="isRetainColumn">是否保留列标题</param>
    /// <param name="excelType">Excel类型</param>
    /// <returns></returns>
    public static MemoryStream ExportExcel<T>(this List<T> list, bool isRetainColumn = false,
        ExcelType excelType = ExcelType.XLSX)
    {
        var memoryStream = new MemoryStream();
        memoryStream.SaveAs(list.GetMiniExcelColumns(isRetainColumn), excelType: excelType);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    /// <summary>
    /// 导出Excel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="isRetainColumn">是否保留列标题</param>
    /// <param name="excelType">Excel类型</param>
    /// <returns></returns>
    public static async Task<MemoryStream> ExportExcelAsync<T>(this List<T> list, bool isRetainColumn = false,
        ExcelType excelType = ExcelType.XLSX)
    {
        var memoryStream = new MemoryStream();
        await memoryStream.SaveAsAsync(list.GetMiniExcelColumns(isRetainColumn), excelType: excelType);
        memoryStream.Seek(0, SeekOrigin.Begin);
        return memoryStream;
    }

    /// <summary>
    /// 得到MiniExcel导出数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="excelData"></param>
    /// <returns></returns>
    public static (List<T> data, List<MiniExcelRowInfoDto<T>> rowInfoList) GetMiniExcelImportData<T>(
        this IEnumerable<IDictionary<string, object>> excelData)
    {
        // 判断是否为Null
        // ReSharper disable once PossibleMultipleEnumeration
        if (!excelData.Any())
        {
            throw new Exception("Excel中不存在数据！");
        }

        var data = new List<T>();
        var rowInfoList = new List<MiniExcelRowInfoDto<T>>();

        var type = typeof(T);

        // 获取所有属性
        var properties = type.GetProperties();

        // 转为带Index的集合
        // ReSharper disable once PossibleMultipleEnumeration
        var excelList = excelData.Select((value, index) => (value, index));

        // 第一行为列头，Dic的Key为 A B C 就是列，Value为表头字段
        // ReSharper disable once PossibleMultipleEnumeration
        var columnInfo = excelList.First(f => f.index == 0).value;

        // ReSharper disable once PossibleMultipleEnumeration
        foreach (var (item, index) in excelList.Where(wh => wh.index != 0))
        {
            // 通过反射自动创建泛型对象
            var obj = Activator.CreateInstance<T>();

            // 遍历所有属性
            foreach (var property in properties)
            {
                // 获取两个特性的信息
                var columnAttr = property.GetCustomAttribute<MiniExcelColumn>(true);
                var requiredAttr = property.GetCustomAttribute<MiniExcelRequired>(true);

                // 判断导入是否忽略列
                if (columnAttr != null && (columnAttr.IsIgnore || columnAttr.IsImportIgnore))
                    continue;

                // 属性Name
                var proName = property.Name;

                // 查找当前属性的描述
                var proDesc = columnAttr == null
                    ? proName
                    : (string.IsNullOrEmpty(columnAttr.ColumnDescription) ? proName : columnAttr.ColumnDescription);

                // 查找当前属性所在的列Key
                var proKey = columnInfo.FirstOrDefault(f => f.Value.ToString() == proDesc).Key;

                // 查到当前属性的值
                var proVal = item.FirstOrDefault(f => f.Key == proKey).Value?.ToString();

                // 验证判断
                if (requiredAttr != null)
                {
                    var errMsg = string.IsNullOrEmpty(requiredAttr.ErrorMessage)
                        ? $"第 {index} 行，{proDesc}的值为空！"
                        : $"第 {index} 行，{requiredAttr.ErrorMessage}";

                    // 判断值是否为空
                    if (string.IsNullOrEmpty(proVal))
                        throw new Exception(errMsg);

                    //正则判断
                    if (!string.IsNullOrEmpty(requiredAttr.Regex) && !Regex.IsMatch(proVal, requiredAttr.Regex))
                        throw new Exception(errMsg);
                }

                // 处理特殊属性
                switch (property.PropertyType.Name)
                {
                    case "Boolean":
                        if (columnAttr != null && !string.IsNullOrEmpty(columnAttr.TrueDesc))
                        {
                            var boolStr = proVal == columnAttr.TrueDesc;
                            property.SetValue(obj, boolStr, null);
                        }
                        else
                        {
                            property.SetValue(obj, Convert.ChangeType(proVal, property.PropertyType), null);
                        }

                        break;
                    default:
                        property.SetValue(obj, Convert.ChangeType(proVal, property.PropertyType), null);
                        break;
                }
            }

            data.Add(obj);
            rowInfoList.Add(new MiniExcelRowInfoDto<T> {RowNumber = index, Value = obj});
        }

        return (data, rowInfoList);
    }

    /// <summary>
    /// 导入Excel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static (List<T> data, List<MiniExcelRowInfoDto<T>> rowInfoList) ImportExcel<T>(this Stream stream)
    {
        return GetMiniExcelImportData<T>(stream.Query().Cast<IDictionary<string, object>>());
    }

    /// <summary>
    /// 导入Excel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<(List<T> data, List<MiniExcelRowInfoDto<T>> rowInfoList)> ImportExcelAsync<T>(this Stream stream)
    {
        return GetMiniExcelImportData<T>((await stream.QueryAsync()).Cast<IDictionary<string, object>>());
    }
}