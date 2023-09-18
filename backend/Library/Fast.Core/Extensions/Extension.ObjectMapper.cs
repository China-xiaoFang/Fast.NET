using System.Linq;
using System.Reflection;
using Fast.Core.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core.Extensions;

/// <summary>
/// ObjectMapper 扩展
/// </summary>
// ReSharper disable once PartialTypeWithSinglePart
public static partial class Extensions
{
    /// <summary>
    /// 对象映射程序集名称
    /// </summary>
    private const string ASSEMBLY_NAME = "Fast.Mapster";

    /// <summary>
    /// 添加对象映射
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns></returns>
    public static IServiceCollection AddObjectMapper(this IServiceCollection services)
    {
        // 判断是否安装了 Mapster 程序集
        var objectMapperAssembly = App.App.Assemblies.FirstOrDefault(u => u.GetName().Name?.Equals(ASSEMBLY_NAME) == true);
        if (objectMapperAssembly != null)
        {
            // 加载 ObjectMapper 拓展类型和拓展方法
            var objectMapperServiceCollectionExtensionsType =
                Reflect.GetType(objectMapperAssembly, "Fast.Mapster.Extensions.Extensions");
            var addObjectMapperMethod = objectMapperServiceCollectionExtensionsType
                .GetMethods(BindingFlags.Public | BindingFlags.Static).First(u => u.Name == "AddObjectMapper");

            return addObjectMapperMethod.Invoke(null, new object[] {services, App.App.Assemblies.ToArray()}) as
                IServiceCollection;
        }

        return services;
    }
}