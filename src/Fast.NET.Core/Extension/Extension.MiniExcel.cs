using System.Text.RegularExpressions;
using MiniExcelLibs;

namespace Fast.NET.Core.Extension;

/// <summary>
/// MiniExcel 列导出属性
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class MiniExcelColumn : Attribute
{
    public MiniExcelColumn()
    {
    }

    public MiniExcelColumn(string ColumnDescription)
    {
        this.ColumnDescription = ColumnDescription;
    }

    public MiniExcelColumn(string ColumnDescription, int Sort)
    {
        this.ColumnDescription = ColumnDescription;
        this.Sort = Sort;
    }

    /// <summary>
    /// 列描述
    /// </summary>
    public string ColumnDescription { get; set; }

    /// <summary>
    /// 导出导入是否忽略，优先级高于 IsExportIgnore 和 IsImportIgnore
    /// 默认False
    /// </summary>
    public bool IsIgnore { get; set; } = false;

    /// <summary>
    /// 导出是否忽略
    /// 默认False
    /// </summary>
    public bool IsExportIgnore { get; set; } = false;

    /// <summary>
    /// 导入是否忽略
    /// </summary>
    public bool IsImportIgnore { get; set; } = false;

    /// <summary>
    /// 是否深度处理List，只处理一层，如果为True，则循环List进行处理
    /// 默认False
    /// 注意：开启深度处理List如果为对象
    /// </summary>
    public bool IsListDeep { get; set; } = false;

    /// <summary>
    /// List集合是否换行
    /// 默认只支持 string int bool double decimal dateTime long
    /// 暂且不支持Object类型
    /// </summary>
    public bool IsListLine { get; set; } = false;

    /// <summary>
    /// 是否为Json字符串
    /// 默认False
    /// </summary>
    public bool IsJson { get; set; } = false;

    /// <summary>
    /// 排序
    /// 从1开始
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// True描述
    /// 默认是
    /// </summary>
    public string TrueDesc { get; set; }

    /// <summary>
    /// False描述
    /// 默认否
    /// </summary>
    public string FalseDesc { get; set; }

    /// <summary>
    /// DateTime转换
    /// 默认 yyyy-MM-dd HH:mm:ss
    /// </summary>
    public string ConvertDateTime { get; set; } = "yyyy-MM-dd HH:mm:ss";
}

/// <summary>
/// MiniExcel 列验证
/// 默认验证是否为空，支持正则表达式验证
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class MiniExcelRequired : Attribute
{
    public MiniExcelRequired()
    {
    }

    public MiniExcelRequired(string ErrorMessage)
    {
        this.ErrorMessage = ErrorMessage;
    }

    public MiniExcelRequired(string ErrorMessage, string Regex)
    {
        this.ErrorMessage = ErrorMessage;
        this.Regex = Regex;
    }

    /// <summary>
    /// 错误消息
    /// </summary>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// 正则表达式
    /// </summary>
    public string Regex { get; set; }
}

public class MiniExcelRowInfo<T>
{
    /// <summary>
    /// 行编号
    /// </summary>
    public int RowNumber { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public T Value { get; set; }
}

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
            var itemResult = new List<DictionaryDto>();
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

                itemResult.Add(new DictionaryDto
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
    public static (List<T> data, List<MiniExcelRowInfo<T>> rowInfoList) GetMiniExcelImportData<T>(
        this IEnumerable<IDictionary<string, object>> excelData)
    {
        // 判断是否为Null
        // ReSharper disable once PossibleMultipleEnumeration
        if (!excelData.Any())
        {
            throw new Exception("Excel中不存在数据！");
        }

        var data = new List<T>();
        var rowInfoList = new List<MiniExcelRowInfo<T>>();

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
            rowInfoList.Add(new MiniExcelRowInfo<T> {RowNumber = index, Value = obj});
        }

        return (data, rowInfoList);
    }

    /// <summary>
    /// 导入Excel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static (List<T> data, List<MiniExcelRowInfo<T>> rowInfoList) ImportExcel<T>(this Stream stream)
    {
        return GetMiniExcelImportData<T>(stream.Query().Cast<IDictionary<string, object>>());
    }

    /// <summary>
    /// 导入Excel
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="stream"></param>
    /// <returns></returns>
    public static async Task<(List<T> data, List<MiniExcelRowInfo<T>> rowInfoList)> ImportExcelAsync<T>(this Stream stream)
    {
        return GetMiniExcelImportData<T>((await stream.QueryAsync()).Cast<IDictionary<string, object>>());
    }
}

/// <summary>
/// 字典Dto
/// </summary>
public class DictionaryDto
{
    /// <summary>
    /// Key
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// Value
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// Sort
    /// </summary>
    public int Sort { get; set; }
}