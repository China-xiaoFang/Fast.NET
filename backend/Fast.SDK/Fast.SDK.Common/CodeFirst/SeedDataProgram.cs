using SqlSugar;

namespace Fast.SDK.Common.CodeFirst;

/// <summary>
/// 种子数据入口
/// </summary>
public class SeedDataProgram
{
    /// <summary>
    /// 获取所有继承了接口的种子数据
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<Type> GetSeedDataType(Type type)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(sl => sl.GetTypes().Where(wh => wh.GetInterfaces().Contains(type)));

        return types;
    }

    /// <summary>
    /// 执行种子数据
    /// </summary>
    /// <param name="_db"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    public static void ExecSeedData(SqlSugarProvider _db, IEnumerable<Type> types)
    {
        foreach (var typeItem in types)
        {
            var instance = Activator.CreateInstance(typeItem);

            var hasDataMethod = typeItem.GetMethod("SeedData");
            hasDataMethod?.Invoke(instance, new object[] {_db});
        }
    }
}