// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
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

using System.Reflection;
using System.Runtime.Loader;
using System.Text.Json;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="Assembly"/> 工具类
/// </summary>
public static class AssemblyUtil
{
    /// <summary>
    /// 获取入口程序集
    /// </summary>
    /// <param name="referenced"><see cref="bool"/> 是否包含引用的，默认为 true</param>
    /// <returns><see cref="IEnumerable{Assembly}"/></returns>
    public static IEnumerable<Assembly> GetEntryAssembly(bool referenced = true)
    {
        // 获取入口程序集
        var entryAssembly = Assembly.GetEntryAssembly();

        // 判断是否包含引用的，不包含直接返回
        if (!referenced)
        {
            return new List<Assembly> {entryAssembly};
        }

        // 需排除的程序集后缀
        var excludeAssemblyNames = new[] {"Database.Migrations"};

        // 非独立发布/非单文件发布
        if (!string.IsNullOrWhiteSpace(entryAssembly?.Location))
        {
            // 获取程序入口文件的 .deps.json 文件
            var depsJsonFilePath = $"{entryAssembly.Location[..^".dll".Length]}.deps.json";

            // 判断文件是否存在
            if (!File.Exists(depsJsonFilePath))
            {
                throw new Exception($"Cannot find {entryAssembly.GetName().Name}.deps.json file.");
            }

            // 读取文件
            var depsJsonContent = File.ReadAllText(depsJsonFilePath);

            // 解析 JSON字符串，并获取 "libraries" 节点的值
            var depsJsonRoot = JsonDocument.Parse(depsJsonContent).RootElement;
            var librariesContent = depsJsonRoot.GetProperty("libraries").EnumerateObject();

            var depsLibraryList = new List<DepsLibrary>();

            // 处理 "libraries" 节点的值
            foreach (var library in librariesContent)
            {
                // "Azure.Core/1.25.0"
                var libraryName = library.Name;
                var libraryNameArr = libraryName.Split("/");

                // 根据Key，获取Name 和 Version
                var name = libraryNameArr.Length >= 1 ? libraryNameArr[0] : null;
                var version = libraryNameArr.Length >= 2 ? libraryNameArr[1] : null;

                string type = null;
                if (library.Value.TryGetProperty("type", out var typeObj))
                {
                    type = typeObj.ToString();
                }

                var serviceable = false;
                if (library.Value.TryGetProperty("serviceable", out var serviceableObj))
                {
                    serviceable = serviceableObj.GetBoolean();
                }

                // 放入集合中
                depsLibraryList.Add(new DepsLibrary(type, name, version, serviceable));
            }

            // 读取项目程序集 或 第三方引用的包，或手动添加引用的dll，或配置特定的包前缀
            return depsLibraryList.Where(wh => (wh.Type == "project" && !excludeAssemblyNames.Any(a => wh.Name.EndsWith(a))) ||
                                               wh.Type == "package").Select(sl =>
            {
                // 这里由于一些dll文件是运行时文件，但是却也包含了在 .deps.json 文件的 "libraries" 节点中，所以采用极限1换100操作，报错的不处理
                try
                {
                    return GetAssembly(sl.Name);
                }
                catch
                {
                    return null;
                }
            }).Where(wh => wh != null);
        }

        // 独立发布/单文件发布
        throw new Exception("暂时不支持单文件或独立发布！");
    }

    /// <summary>
    /// 根据程序集名称获取运行时程序集
    /// </summary>
    /// <param name="assemblyName"><see cref="string"/> 程序集名称</param>
    /// <returns><see cref="Assembly"/></returns>
    public static Assembly GetAssembly(string assemblyName)
    {
        // 加载程序集
        return AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(assemblyName));
    }

    /// <summary>
    /// 根据路径加载程序集
    /// </summary>
    /// <param name="path"><see cref="string"/> 绝对路径</param>
    /// <returns><see cref="Assembly"/></returns>
    public static Assembly LoadAssembly(string path)
    {
        if (!File.Exists(path))
            return default;
        return Assembly.LoadFrom(path);
    }

    /// <summary>
    /// 通过流加载程序集
    /// </summary>
    /// <param name="assembly"><see cref="MemoryStream"/> 内存流</param>
    /// <returns><see cref="Assembly"/></returns>
    public static Assembly LoadAssembly(MemoryStream assembly)
    {
        return Assembly.Load(assembly.ToArray());
    }

    /// <summary>
    /// 根据程序集名称、类型完整限定名获取运行时类型
    /// </summary>
    /// <param name="assemblyName"><see cref="string"/> 程序集名称</param>
    /// <param name="typeFullName"><see cref="string"/> 类型完整限定名称</param>
    /// <returns><see cref="Type"/></returns>
    public static Type GetType(string assemblyName, string typeFullName)
    {
        return GetAssembly(assemblyName).GetType(typeFullName);
    }

    /// <summary>
    /// 根据程序集和类型完全限定名获取运行时类型
    /// </summary>
    /// <param name="assembly"><see cref="MemoryStream"/> 内存流</param>
    /// <param name="typeFullName"><see cref="string"/> 类型完整限定名称</param>
    /// <returns><see cref="Type"/></returns>
    public static Type GetType(MemoryStream assembly, string typeFullName)
    {
        return LoadAssembly(assembly).GetType(typeFullName);
    }
}