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

// ReSharper disable once CheckNamespace
namespace Fast.Runtime;

/// <summary>
/// <see cref="Assembly"/> 拓展类
/// </summary>
public static class AssemblyExtension
{
    /// <summary>
    /// 获取入口引用程序集
    /// </summary>
    /// <remarks>暂不支持独立/单文件发布</remarks>
    /// <param name="assembly"><see cref="Assembly"/> 入口程序集</param>
    /// <returns></returns>
    public static IEnumerable<Assembly> GetEntryReferencedAssembly(this Assembly assembly)
    {
        // 判断是否为独立/单文件发布
        if (!string.IsNullOrWhiteSpace(assembly?.Location))
        {
            // 需排除的程序集后缀
            // 这里的 Microsoft.Data.SqlClient 排除是为了解决这个错误 https://github.com/dotnet/SqlClient/issues/1930
            var excludeAssemblyNames = new[] {"Database.Migrations", "Microsoft.Data.SqlClient"};

            // 获取程序入口文件的 .deps.json 文件
            var depsJsonFilePath = $"{assembly.Location[..^".dll".Length]}.deps.json";

            // 判断文件是否存在
            if (!File.Exists(depsJsonFilePath))
            {
                throw new Exception($"Cannot find {assembly.GetName().Name}.deps.json file.");
            }

            // 读取文件
            var depsJsonContent = File.ReadAllText(depsJsonFilePath);

            // 解析 JSON字符串，并获取 "libraries" 节点的值
            var depsJsonRoot = JsonDocument.Parse(depsJsonContent)
                .RootElement;
            var librariesContent = depsJsonRoot.GetProperty("libraries")
                .EnumerateObject();

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
            return depsLibraryList.Where(wh =>
                    (wh.Type == "project" && !excludeAssemblyNames.Any(a => wh.Name.EndsWith(a))) || wh.Type == "package")
                .Select(sl =>
                {
                    // 这里由于一些dll文件是运行时文件，但是却也包含了在 .deps.json 文件的 "libraries" 节点中，所以采用极限1换100操作，报错的不处理
                    try
                    {
                        return AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName(sl.Name));
                    }
                    catch
                    {
                        return null;
                    }
                })
                .Where(wh => wh != null)
                .ToList();
        }

        return new[] {assembly};
    }

    /// <summary>
    /// 获取程序集中所有类型
    /// </summary>
    /// <remarks>这里默认获取所有 Public 声明的</remarks>
    /// <param name="assembly"><see cref="Assembly"/> 程序集</param>
    /// <param name="typeFilter"><see cref="Func{TResult}"/> 类型过滤条件</param>
    /// <returns></returns>
    public static IEnumerable<Type> GetAssemblyTypes(this Assembly assembly, Func<Type, bool> typeFilter = null)
    {
        var types = Array.Empty<Type>();

        try
        {
            types = assembly.GetTypes();
        }
        catch
        {
            var logSb = new StringBuilder();
            logSb.Append("\u001b[41m\u001b[30m");
            logSb.Append("fail");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            logSb.Append(": ");
            logSb.Append($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fffffff zzz dddd}");
            logSb.Append(Environment.NewLine);
            logSb.Append("\u001b[41m\u001b[30m");
            logSb.Append("      ");
            logSb.Append($"Error load `{assembly.FullName}` assembly.");
            logSb.Append("\u001b[39m\u001b[22m\u001b[49m");
            Console.WriteLine(logSb.ToString());
        }

        return types.Where(wh => wh.IsPublic && (typeFilter == null || typeFilter(wh)));
    }
}