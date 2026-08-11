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

using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;

namespace Fast.Runtime;

/// <summary>
/// 为 <see cref="Assembly"/> 提供扩展方法。
/// </summary>
public static class AssemblyExtension
{
    /// <summary>
    /// 获取入口运行库。
    /// </summary>
    /// <remarks>暂不支持独立/单文件发布。</remarks>
    /// <param name="assembly">目标程序集。</param>
    /// <returns>获取到的入口运行库集合。</returns>
    public static List<DependencyLibrary> GetEntryRuntimeLibraries(this Assembly assembly)
    {
        var depsJsonFilePath = ResolveDependencyContextPath(assembly);
        if (!string.IsNullOrWhiteSpace(depsJsonFilePath))
        {
            // 读取文件
            var depsJsonContent = File.ReadAllText(depsJsonFilePath);

            // 解析 JSON 字符串
            var depsJsonRoot = JsonDocument.Parse(depsJsonContent)
                .RootElement;

            var targetsMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // 获取 "targets" 节点的值
            var targetsContent = depsJsonRoot.GetProperty("targets")
                .EnumerateObject();
            foreach (var targetsArr in targetsContent)
            {
                // "targets" 节点下通常有一个节点，例如 ".NETCoreApp,Version=v8.0"
                foreach (var targets in targetsArr.Value.EnumerateObject())
                {
                    if (targets.Value.TryGetProperty("runtime", out var runtimeElement))
                    {
                        // 直接默认获取第一个（大多数包只有一个主程序集）
                        var runtimeObj = runtimeElement.EnumerateObject()
                            .FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(runtimeObj.Name))
                        {
                            var fileName = Path.GetFileNameWithoutExtension(runtimeObj.Name);
                            if (!string.IsNullOrWhiteSpace(fileName))
                            {
                                targetsMap.TryAdd(targets.Name, fileName);
                            }
                        }
                    }
                }
            }

            // 获取 "libraries" 节点的值
            var librariesContent = depsJsonRoot.GetProperty("libraries")
                .EnumerateObject();

            var dependencyLibraryList = new List<DependencyLibrary>();

            // 处理 "libraries" 节点的值
            foreach (var library in librariesContent)
            {
                // "Azure.Core/1.25.0"
                var libraryName = library.Name;
                var libraryNameArr = libraryName.Split("/");

                // 根据 Key，获取 Name 和 Version
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

                var fileName = targetsMap.GetValueOrDefault(library.Name, name);

                // 放入集合中
                dependencyLibraryList.Add(new DependencyLibrary(type, name, version, fileName, serviceable));
            }

            return dependencyLibraryList;
        }

        return [];
    }

    /// <summary>
    /// 解析当前宿主实际使用的依赖上下文文件。
    /// </summary>
    /// <param name="assembly">目标程序集。</param>
    /// <returns>解析后的当前宿主实际使用的依赖上下文文件。</returns>
    private static string ResolveDependencyContextPath(Assembly assembly)
    {
        if (!string.IsNullOrWhiteSpace(assembly?.Location))
        {
            var assemblyDepsFile = Path.ChangeExtension(assembly.Location, ".deps.json");
            if (File.Exists(assemblyDepsFile))
                return assemblyDepsFile;
        }

        // 测试宿主和插件宿主的入口程序集可能位于 SDK 目录，实际应用的 deps 文件由宿主上下文提供。
        var contextDepsFiles = AppContext.GetData("APP_CONTEXT_DEPS_FILES") as string;
        if (!string.IsNullOrWhiteSpace(contextDepsFiles))
        {
            var baseDirectory = Path.GetFullPath(AppContext.BaseDirectory)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var applicationDepsFile = contextDepsFiles.Split(Path.PathSeparator,
                    StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(File.Exists)
                .FirstOrDefault(file => string.Equals(Path.GetDirectoryName(Path.GetFullPath(file))
                        ?.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar), baseDirectory,
                    StringComparison.OrdinalIgnoreCase));
            if (applicationDepsFile != null)
                return applicationDepsFile;
        }

        return Directory.Exists(AppContext.BaseDirectory)
            ? Directory.EnumerateFiles(AppContext.BaseDirectory, "*.deps.json", SearchOption.TopDirectoryOnly)
                .OrderBy(file => file, StringComparer.OrdinalIgnoreCase)
                .FirstOrDefault()
            : null;
    }

    /// <summary>
    /// 获取入口引用程序集。
    /// </summary>
    /// <remarks>暂不支持独立/单文件发布。</remarks>
    /// <param name="assembly">目标程序集。</param>
    /// <param name="dependencyLibraryList">应用依赖库集合。</param>
    /// <returns>获取到的入口引用程序集集合。</returns>
    public static List<Assembly> GetEntryReferencedAssembly(this Assembly assembly,
        List<DependencyLibrary> dependencyLibraryList = null)
    {
        if (assembly == null)
            return [];

        dependencyLibraryList ??= assembly.GetEntryRuntimeLibraries();

        if (!dependencyLibraryList.Any())
        {
            return [assembly];
        }

        // 已经加载的程序集
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        // 需排除的程序集后缀
        // 这里的 Microsoft.Data.SqlClient 排除是为了解决这个错误 https://github.com/dotnet/SqlClient/issues/1930
        var excludeAssemblyNames = new[] {"Database.Migrations", "Microsoft.Data.SqlClient"};

        // 读取项目引用程序集
        return dependencyLibraryList.Where(wh =>
                (wh.Type == "project" && !excludeAssemblyNames.Any(a => wh.Name.EndsWith(a))) || wh.Type == "package")
            .Select(sl =>
            {
                // .deps.json 同时包含应用依赖和仅供运行时使用的库；不可加载的条目会被跳过。
                var loadedAssembly = loadedAssemblies.FirstOrDefault(f => f.GetName()
                                                                              ?.Name?.Equals(sl.FileName,
                                                                                  StringComparison.OrdinalIgnoreCase)
                                                                          == true);
                if (loadedAssembly != null)
                {
                    return loadedAssembly;
                }

                try
                {
                    return AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(sl.Name));
                }
                catch
                {
                    try
                    {
                        return Assembly.Load(sl.FileName);
                    }
                    catch
                    {
                        return null;
                    }
                }
            })
            .Where(wh => wh != null)
            .ToList();
    }

    /// <summary>
    /// 获取程序集中所有类型。
    /// </summary>
    /// <remarks>默认仅返回公开声明的类型；可通过筛选器进一步限制结果。</remarks>
    /// <param name="assembly">目标程序集。</param>
    /// <param name="typeFilter">用于筛选程序集类型的委托。</param>
    /// <returns>获取到的程序集中所有类型集合。</returns>
    public static IEnumerable<Type> GetAssemblyTypes(this Assembly assembly, Func<Type, bool> typeFilter = null)
    {
        var types = Array.Empty<Type>();

        try
        {
            types = assembly.GetTypes();
        }
        catch
        {
            var useColor = !Console.IsOutputRedirected;
            var logSb = new StringBuilder();
            if (useColor)
                logSb.Append("\u001b[41m\u001b[30m");
            logSb.Append("fail");
            if (useColor)
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            logSb.Append(": ");
            logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
            logSb.Append(Environment.NewLine);
            if (useColor)
                logSb.Append("\u001b[41m\u001b[30m");
            logSb.Append("      ");
            logSb.Append($"Error load `{assembly.FullName}` assembly.");
            if (useColor)
                logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            Console.WriteLine(logSb.ToString());
        }

        return types.Where(wh => wh.IsPublic && (typeFilter == null || typeFilter(wh)));
    }
}
