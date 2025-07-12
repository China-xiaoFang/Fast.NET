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

using System.Collections.Concurrent;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Fast.DynamicApplication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
#if NET8_0
using Microsoft.OpenApi;
#endif

// ReSharper disable once CheckNamespace
namespace Fast.Swagger;

/// <summary>
/// <see cref="SwaggerDocumentBuilder"/> 规范化文档构建器
/// </summary>
[SuppressSniffer]
public static class SwaggerDocumentBuilder
{
    /// <summary>
    /// 所有分组默认的组名 Key
    /// </summary>
    private const string AllGroupsKey = "All Groups";

    /// <summary>
    /// 分组信息
    /// </summary>
    private static readonly IEnumerable<GroupExtraInfo> DocumentGroupExtras;

    /// <summary>
    /// 带排序的分组名
    /// </summary>
    private static readonly Regex _groupOrderRegex;

    /// <summary>
    /// 文档分组列表
    /// </summary>
    public static readonly IEnumerable<string> DocumentGroups;

    /// <summary>
    /// 构造函数
    /// </summary>
    static SwaggerDocumentBuilder()
    {
        // 初始化常量
        _groupOrderRegex = new Regex(@"@(?<order>[0-9]+$)");
        GetActionGroupsCached = new ConcurrentDictionary<MethodInfo, IEnumerable<GroupExtraInfo>>();
        GetControllerGroupsCached = new ConcurrentDictionary<Type, IEnumerable<GroupExtraInfo>>();
        GetGroupOpenApiInfoCached = new ConcurrentDictionary<string, SwaggerOpenApiInfo>();
        GetControllerTagCached = new ConcurrentDictionary<ControllerActionDescriptor, string>();
        GetActionTagCached = new ConcurrentDictionary<ApiDescription, string>();

        // 默认分组，支持多个逗号分割
        DocumentGroupExtras = new List<GroupExtraInfo> {ResolveGroupExtraInfo(Penetrates.SwaggerSettings.DefaultGroupName)};

        // 加载所有分组
        DocumentGroups = ReadGroups();
    }

    /// <summary>
    /// 检查方法是否在分组中
    /// </summary>
    /// <param name="currentGroup"></param>
    /// <param name="apiDescription"></param>
    /// <returns></returns>
    public static bool CheckApiDescriptionInCurrentGroup(string currentGroup, ApiDescription apiDescription)
    {
        if (!apiDescription.TryGetMethodInfo(out var method))
            return false;

        // 处理 Mvc 和 WebAPI 混合项目路由问题
        if (typeof(Controller).IsAssignableFrom(method.DeclaringType)
            && apiDescription.ActionDescriptor.ActionConstraints == null)
        {
            return false;
        }

        // 处理贴有 [ApiExplorerSettings(IgnoreApi = true)] 或者 [ApiDescriptionSettings(false)] 特性的接口
        var apiExplorerSettings = method.GetFoundAttribute<ApiExplorerSettingsAttribute>(true);
        if (apiExplorerSettings?.IgnoreApi == true)
            return false;

        if (currentGroup == AllGroupsKey)
        {
            return true;
        }

        return GetActionGroups(method)
            .Any(u => u.Group == currentGroup);
    }

    /// <summary>
    /// 获取所有的规范化分组信息
    /// </summary>
    /// <returns></returns>
    public static List<SwaggerOpenApiInfo> GetOpenApiGroups()
    {
        var openApiGroups = new List<SwaggerOpenApiInfo>();
        foreach (var group in DocumentGroups)
        {
            openApiGroups.Add(GetGroupOpenApiInfo(group));
        }

        return openApiGroups;
    }

    /// <summary>
    /// 获取分组信息缓存集合
    /// </summary>
    private static readonly ConcurrentDictionary<string, SwaggerOpenApiInfo> GetGroupOpenApiInfoCached;

    /// <summary>
    /// 获取分组配置信息
    /// </summary>
    /// <param name="group"></param>
    /// <returns></returns>
    public static SwaggerOpenApiInfo GetGroupOpenApiInfo(string group)
    {
        return GetGroupOpenApiInfoCached.GetOrAdd(group, Function);

        // 本地函数
        static SwaggerOpenApiInfo Function(string group)
        {
            // 替换路由模板
            var routeTemplate = Penetrates.SwaggerSettings.RouteTemplate.Replace("{documentName}", Uri.EscapeDataString(group));
            if (!string.IsNullOrWhiteSpace(Penetrates.SwaggerSettings.ServerDir))
            {
                routeTemplate = Penetrates.SwaggerSettings.ServerDir + "/" + routeTemplate;
            }

            var template = $"/{routeTemplate}";

            var groupInfo = Penetrates.SwaggerSettings.GroupOpenApiInfos.FirstOrDefault(u => u.Group == group);
            if (groupInfo != null)
            {
                groupInfo.RouteTemplate = template;
                groupInfo.Title ??= group;
            }
            else
            {
                groupInfo = new SwaggerOpenApiInfo {Group = group, RouteTemplate = template};
            }

            return groupInfo;
        }
    }

    /// <summary>
    /// 构建Swagger全局配置
    /// </summary>
    /// <param name="swaggerOptions">Swagger 全局配置</param>
    /// <param name="configure"></param>
    internal static void Build(SwaggerOptions swaggerOptions, Action<SwaggerOptions> configure = null)
    {
        // 生成V2版本
#if NET6_0 || NET7_0
        swaggerOptions.SerializeAsV2 = Penetrates.SwaggerSettings.FormatAsV2 == true;
#elif NET8_0
        swaggerOptions.OpenApiVersion = Penetrates.SwaggerSettings.FormatAsV2 == true
            ? OpenApiSpecVersion.OpenApi2_0
            : OpenApiSpecVersion.OpenApi3_0;
#endif

        // 判断是否启用 Server
        if (Penetrates.SwaggerSettings.HideServers != true)
        {
            // 启动服务器 Servers
            swaggerOptions.PreSerializeFilters.Add((swagger, request) =>
            {
                // 默认 Server
                var servers = new List<OpenApiServer>
                {
                    new() {Url = $"{request.Scheme}://{request.Host.Value}", Description = "Default"}
                };
                servers.AddRange(Penetrates.SwaggerSettings.Servers);

                swagger.Servers = servers;
            });
        }

        // 配置路由模板
        swaggerOptions.RouteTemplate = Penetrates.SwaggerSettings.RouteTemplate;

        // 自定义配置
        configure?.Invoke(swaggerOptions);
    }

    /// <summary>
    /// Swagger 生成器构建
    /// </summary>
    /// <param name="swaggerGenOptions">Swagger 生成器配置</param>
    /// <param name="configure">自定义配置</param>
    internal static void BuildGen(SwaggerGenOptions swaggerGenOptions, Action<SwaggerGenOptions> configure = null)
    {
        // 创建分组文档
        CreateSwaggerDocs(swaggerGenOptions);

        // 加载分组控制器和动作方法列表
        LoadGroupControllerWithActions(swaggerGenOptions);

        // 配置 Swagger OperationIds
        ConfigureOperationIds(swaggerGenOptions);

        // 配置 Swagger SchemaId
        ConfigureSchemaIds(swaggerGenOptions);

        // 配置标签
        ConfigureTagsAction(swaggerGenOptions);

        // 配置 Action 排序
        ConfigureActionSequence(swaggerGenOptions);

        // 加载注释描述文件
        LoadXmlComments(swaggerGenOptions);

        // 配置授权
        ConfigureSecurities(swaggerGenOptions);

        //使得 Swagger 能够正确地显示 Enum 的对应关系
        if (Penetrates.SwaggerSettings.EnableEnumSchemaFilter == true)
            swaggerGenOptions.SchemaFilter<EnumSchemaFilter>();

        // 修复 editor.swagger.io 生成不能正常处理 C# object 类型问题
        swaggerGenOptions.SchemaFilter<AnySchemaFilter>();

        // 支持控制器排序操作
        if (Penetrates.SwaggerSettings.EnableTagsOrderDocumentFilter == true)
            swaggerGenOptions.DocumentFilter<TagsOrderDocumentFilter>();

        // 添加 Action 操作过滤器
        swaggerGenOptions.OperationFilter<ApiActionFilter>();

        // 自定义配置
        configure?.Invoke(swaggerGenOptions);
    }

    /// <summary>
    /// Swagger UI 构建
    /// </summary>
    /// <param name="swaggerUIOptions"></param>
    /// <param name="configure"></param>
    internal static void BuildUI(SwaggerUIOptions swaggerUIOptions, Action<SwaggerUIOptions> configure = null)
    {
        // 配置分组终点路由
        CreateGroupEndpoint(swaggerUIOptions);

        // 配置文档标题
        swaggerUIOptions.DocumentTitle = Penetrates.SwaggerSettings.DocumentTitle;

        // 配置UI地址（处理二级虚拟目录）
        swaggerUIOptions.RoutePrefix = Penetrates.SwaggerSettings.RoutePrefix ?? string.Empty;

        // 文档展开设置
        swaggerUIOptions.DocExpansion(Penetrates.SwaggerSettings.DocExpansionState!.Value);

        // 自定义 Swagger 首页
        CustomizeIndex(swaggerUIOptions);

        // 配置多语言和自动登录token
        AddDefaultInterceptor(swaggerUIOptions);

        // 自定义配置
        configure?.Invoke(swaggerUIOptions);
    }

    /// <summary>
    /// 创建分组文档
    /// </summary>
    /// <param name="swaggerGenOptions">Swagger生成器对象</param>
    private static void CreateSwaggerDocs(SwaggerGenOptions swaggerGenOptions)
    {
        foreach (var group in DocumentGroups)
        {
            var groupOpenApiInfo = GetGroupOpenApiInfo(group) as OpenApiInfo;
            swaggerGenOptions.SwaggerDoc(group, groupOpenApiInfo);
        }
    }

    /// <summary>
    /// 加载分组控制器和动作方法列表
    /// </summary>
    /// <param name="swaggerGenOptions">Swagger 生成器配置</param>
    private static void LoadGroupControllerWithActions(SwaggerGenOptions swaggerGenOptions)
    {
        swaggerGenOptions.DocInclusionPredicate(CheckApiDescriptionInCurrentGroup);
    }

    /// <summary>
    ///  配置标签
    /// </summary>
    /// <param name="swaggerGenOptions"></param>
    private static void ConfigureTagsAction(SwaggerGenOptions swaggerGenOptions)
    {
        swaggerGenOptions.TagActionsBy(apiDescription => { return new[] {GetActionTag(apiDescription)}; });
    }

    /// <summary>
    ///  配置 Action 排序
    /// </summary>
    /// <param name="swaggerGenOptions"></param>
    private static void ConfigureActionSequence(SwaggerGenOptions swaggerGenOptions)
    {
        swaggerGenOptions.OrderActionsBy(apiDesc =>
        {
            var apiDescriptionSettings = apiDesc.CustomAttributes()
                                             .FirstOrDefault(u => u is ApiDescriptionSettingsAttribute) as
                                         ApiDescriptionSettingsAttribute
                                         ?? new ApiDescriptionSettingsAttribute();

            return (int.MaxValue - apiDescriptionSettings.Order).ToString()
                .PadLeft(int.MaxValue.ToString()
                    .Length, '0');
        });
    }

    /// <summary>
    /// 配置 Swagger OperationIds
    /// </summary>
    /// <param name="swaggerGenOptions">Swagger 生成器配置</param>
    private static void ConfigureOperationIds(SwaggerGenOptions swaggerGenOptions)
    {
        swaggerGenOptions.CustomOperationIds(apiDescription =>
        {
            var isMethod = apiDescription.TryGetMethodInfo(out var method);

            // 判断是否自定义了 [OperationId] 特性
            if (isMethod && method.IsDefined(typeof(OperationIdAttribute), false))
            {
                return method.GetCustomAttribute<OperationIdAttribute>(false)
                    ?.OperationId;
            }

            var operationId = apiDescription.RelativePath?.Replace("/", "-")
                                  .Replace("{", "-")
                                  .Replace("}", "-")
                              + "-"
                              + apiDescription.HttpMethod?.ToLower()
                                  .FirstCharToUpper();

            return operationId.Replace("--", "-");
        });
    }

    /// <summary>
    /// 配置 Swagger SchemaIds
    /// </summary>
    /// <param name="swaggerGenOptions">Swagger 生成器配置</param>
    private static void ConfigureSchemaIds(SwaggerGenOptions swaggerGenOptions)
    {
        // 本地函数
        static string DefaultSchemaIdSelector(Type modelType)
        {
            var modelName = modelType.Name;

            // 处理泛型类型问题
            if (modelType.IsConstructedGenericType)
            {
                var prefix = modelType.GetGenericArguments()
                    .Select(DefaultSchemaIdSelector)
                    .Aggregate((previous, current) => previous + current);

                // 通过 _ 拼接多个泛型
                modelName = modelName.Split('`')
                                .First()
                            + "_"
                            + prefix;
            }

            // 判断是否自定义了 [SchemaId] 特性，解决模块化多个程序集命名冲突
            var isCustomize = modelType.IsDefined(typeof(SchemaIdAttribute));
            if (isCustomize)
            {
                var schemaIdAttribute = modelType.GetCustomAttribute<SchemaIdAttribute>();
                if (schemaIdAttribute is {Replace: false})
                    return schemaIdAttribute.SchemaId + modelName;
                return schemaIdAttribute?.SchemaId;
            }

            return modelName;
        }

        // 调用本地函数
        swaggerGenOptions.CustomSchemaIds(DefaultSchemaIdSelector);
    }

    /// <summary>
    /// 加载注释描述文件
    /// </summary>
    /// <param name="swaggerGenOptions">Swagger 生成器配置</param>
    private static void LoadXmlComments(SwaggerGenOptions swaggerGenOptions)
    {
        var xmlComments = Penetrates.SwaggerSettings.XmlComments;
        var members = new Dictionary<string, XElement>();

        // 显式继承的注释
        var regex = new Regex(@"[A-Z]:[a-zA-Z_@\.]+");
        // 隐式继承的注释
        var regex2 = new Regex(@"[A-Z]:[a-zA-Z_@\.]+\.");

        // 支持注释完整特性，包括 inheritdoc 注释语法
        foreach (var xmlComment in xmlComments)
        {
            var assemblyXmlName = xmlComment.EndsWith(".xml") ? xmlComment : $"{xmlComment}.xml";
            var assemblyXmlPath = Path.Combine(AppContext.BaseDirectory, assemblyXmlName);

            if (File.Exists(assemblyXmlPath))
            {
                var xmlDoc = XDocument.Load(assemblyXmlPath);

                // 查找所有 member[name] 节点，且不包含 <inheritdoc /> 和 <exclude /> 节点的注释
                var memberNotInheritdocElementList =
                    xmlDoc.XPathSelectElements("/doc/members/member[@name and not(inheritdoc) and not(exclude)]");

                foreach (var memberElement in memberNotInheritdocElementList)
                {
                    members.TryAdd(memberElement.Attribute("name")!.Value, memberElement);
                }

                // 查找所有 member[name] 含有 <inheritdoc /> 节点的注释
                var memberElementList = xmlDoc.XPathSelectElements("/doc/members/member[inheritdoc]");
                foreach (var memberElement in memberElementList)
                {
                    var inheritdocElement = memberElement.Element("inheritdoc");
                    var cref = inheritdocElement!.Attribute("cref");
                    var value = cref?.Value;

                    // 处理不带 cref 的 inheritdoc 注释
                    if (value == null)
                    {
                        var memberName = inheritdocElement.Parent!.Attribute("name")!.Value;

                        // 处理隐式实现接口的注释
                        // 注释格式：M:Fast.NET.Application.TestInheritdoc.Fast.NET#Application#ITestInheritdoc#Abc(System.String)
                        // 匹配格式：[A-Z]:[a-zA-Z_@\.]+\.
                        // 处理逻辑：直接替换匹配为空，然后讲 # 替换为 . 查找即可
                        if (memberName.Contains('#'))
                        {
                            value = $"{memberName[..2]}{regex2.Replace(memberName, "").Replace('#', '.')}";
                        }
                        // 处理带参数的注释
                        // 注释格式：M:Fast.NET.Application.TestInheritdoc.WithParams(System.String)
                        // 匹配格式：[A-Z]:[a-zA-Z_@\.]+
                        // 处理逻辑：匹配出不带参数的部分，然后获取类型命名空间，最后调用 GenerateInheritdocCref 进行生成
                        else if (memberName.Contains('('))
                        {
                            var noParamsClassName = regex.Match(memberName)
                                .Value;
                            var className =
                                noParamsClassName[
                                    noParamsClassName.IndexOf(":", StringComparison.Ordinal)..noParamsClassName.LastIndexOf(".",
                                        StringComparison.Ordinal)];
                            value = GenerateInheritdocCref(xmlDoc, memberName, className);
                        }
                        // 处理不带参数的注释
                        // 注释格式：M:Fast.NET.Application.TestInheritdoc.WithParams
                        // 匹配格式：无
                        // 处理逻辑：获取类型命名空间，最后调用 GenerateInheritdocCref 进行生成
                        else
                        {
                            var className =
                                memberName[
                                    memberName.IndexOf(":", StringComparison.Ordinal)..memberName.LastIndexOf(".",
                                        StringComparison.Ordinal)];
                            value = GenerateInheritdocCref(xmlDoc, memberName, className);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(value))
                        continue;

                    // 处理带 cref 的 inheritdoc 注释
                    if (members.TryGetValue(value, out var realDocMember))
                    {
                        memberElement.SetAttributeValue("_ref_", value);
                        inheritdocElement.Parent!.ReplaceNodes(realDocMember.Nodes());
                    }
                }

                swaggerGenOptions.IncludeXmlComments(() => new XPathDocument(xmlDoc.CreateReader()), true);
            }
        }
    }

    /// <summary>
    /// 生成 Inheritdoc cref 属性
    /// </summary>
    /// <param name="xmlDoc"></param>
    /// <param name="memberName"></param>
    /// <param name="className"></param>
    /// <returns></returns>
    private static string GenerateInheritdocCref(XDocument xmlDoc, string memberName, string className)
    {
        var classElement = xmlDoc.XPathSelectElements($"/doc/members/member[@name='{"T" + className}' and @_ref_]")
            .FirstOrDefault();
        if (classElement == null)
            return null;

        var _ref_value = classElement.Attribute("_ref_")
            ?.Value;
        if (_ref_value == null)
            return null;

        var classCrefValue = _ref_value[_ref_value.IndexOf(":", StringComparison.Ordinal)..];
        return memberName.Replace(className, classCrefValue);
    }

    /// <summary>
    /// 配置授权
    /// </summary>
    /// <param name="swaggerGenOptions">Swagger 生成器配置</param>
    private static void ConfigureSecurities(SwaggerGenOptions swaggerGenOptions)
    {
        // 判断是否启用了授权
        if (Penetrates.SwaggerSettings.EnableAuthorized != true || Penetrates.SwaggerSettings.SecurityDefinitions.Length == 0)
            return;

        var openApiSecurityRequirement = new OpenApiSecurityRequirement();

        // 生成安全定义
        foreach (var securityDefinition in Penetrates.SwaggerSettings.SecurityDefinitions)
        {
            // Id 必须定义
            if (string.IsNullOrWhiteSpace(securityDefinition.Id))
                continue;

            // 添加安全定义
            var openApiSecurityScheme = securityDefinition as OpenApiSecurityScheme;
            swaggerGenOptions.AddSecurityDefinition(securityDefinition.Id, openApiSecurityScheme);

            // 添加安全需求
            var securityRequirement = securityDefinition.Requirement;

            // C# 9.0 模式匹配新语法
            if (securityRequirement is {Scheme.Reference: not null})
            {
                securityRequirement.Scheme.Reference.Id ??= securityDefinition.Id;
                openApiSecurityRequirement.Add(securityRequirement.Scheme, securityRequirement.Accesses);
            }
        }

        // 添加安全需求
        if (openApiSecurityRequirement.Count > 0)
        {
            swaggerGenOptions.AddSecurityRequirement(openApiSecurityRequirement);
        }
    }

    /// <summary>
    /// 配置分组终点路由
    /// </summary>
    /// <param name="swaggerUIOptions"></param>
    private static void CreateGroupEndpoint(SwaggerUIOptions swaggerUIOptions)
    {
        foreach (var group in DocumentGroups)
        {
            var groupOpenApiInfo = GetGroupOpenApiInfo(group);

            swaggerUIOptions.SwaggerEndpoint(groupOpenApiInfo.RouteTemplate, groupOpenApiInfo.Title ?? group);
        }
    }

    /// <summary>
    /// 自定义 Swagger 首页
    /// </summary>
    /// <param name="swaggerUIOptions"></param>
    private static void CustomizeIndex(SwaggerUIOptions swaggerUIOptions)
    {
        var thisType = typeof(SwaggerDocumentBuilder);
        var thisAssembly = thisType.Assembly;

        var customIndex = $"{thisAssembly.GetName().Name}.Assets.index.html";
        swaggerUIOptions.IndexStream = () =>
        {
            StringBuilder htmlBuilder;

            // 读取文件内容
            using (var stream = thisAssembly.GetManifestResourceStream(customIndex))
            {
                using var reader = new StreamReader(stream);
                htmlBuilder = new StringBuilder(reader.ReadToEnd());
            }

            // 返回新的内存流
            var byteArray = Encoding.UTF8.GetBytes(htmlBuilder.ToString());
            return new MemoryStream(byteArray);
        };

        // 添加登录信息配置
        var additional = Penetrates.SwaggerSettings.LoginInfo;
        if (additional != null)
        {
            swaggerUIOptions.ConfigObject.AdditionalItems.Add(nameof(Penetrates.SwaggerSettings.LoginInfo), additional);
        }
    }

    /// <summary>
    /// 添加默认请求/响应拦截器
    /// </summary>
    /// <param name="swaggerUIOptions"></param>
    private static void AddDefaultInterceptor(SwaggerUIOptions swaggerUIOptions)
    {
        // 配置多语言和自动登录token
        swaggerUIOptions.UseRequestInterceptor("function(request) { return defaultRequestInterceptor(request); }");
        swaggerUIOptions.UseResponseInterceptor("function(response) { return defaultResponseInterceptor(response); }");
    }

    /// <summary>
    /// 读取所有分组信息
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<string> ReadGroups()
    {
        // 获取所有的控制器和动作方法
        var controllers = MAppContext.EffectiveTypes.Where(DynamicApplicationContext.IsApiController)
            .ToList();
        if (!controllers.Any())
        {
            var defaultGroups = new List<string> {Penetrates.SwaggerSettings.DefaultGroupName};

            // 启用总分组功能
            if (Penetrates.SwaggerSettings.EnableAllGroups == true)
            {
                defaultGroups.Add(AllGroupsKey);
            }

            return defaultGroups;
        }

        var actions = controllers.SelectMany(c => c.GetMethods()
            .Where(u => IsApiAction(u, c)));

        // 合并所有分组
        var groupOrders = controllers.SelectMany(GetControllerGroups)
            .Union(actions.SelectMany(GetActionGroups))
            .Where(u => u is {Visible: true})
            // 分组后取最大排序
            .GroupBy(u => u.Group)
            .Select(u => new GroupExtraInfo {Group = u.Key, Order = u.Max(x => x.Order), Visible = true});

        // 分组排序
        var groups = groupOrders.OrderByDescending(u => u.Order)
            .ThenBy(u => u.Group)
            .Select(u => u.Group)
            .Union(Penetrates.SwaggerSettings.PackagesGroups);

        // 启用总分组功能
        if (Penetrates.SwaggerSettings.EnableAllGroups == true)
        {
            groups = groups.Concat(new[] {AllGroupsKey});
        }

        return groups;
    }

    /// <summary>
    /// 获取控制器组缓存集合
    /// </summary>
    private static readonly ConcurrentDictionary<Type, IEnumerable<GroupExtraInfo>> GetControllerGroupsCached;

    /// <summary>
    /// 获取控制器分组列表
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static IEnumerable<GroupExtraInfo> GetControllerGroups(Type type)
    {
        return GetControllerGroupsCached.GetOrAdd(type, Function);

        // 本地函数
        static IEnumerable<GroupExtraInfo> Function(Type type)
        {
            // 如果控制器没有定义 [ApiDescriptionSettings] 特性，则返回默认分组
            if (!type.IsDefined(typeof(ApiDescriptionSettingsAttribute), true))
                return DocumentGroupExtras;

            // 读取分组
            var apiDescriptionSettings = type.GetCustomAttribute<ApiDescriptionSettingsAttribute>(true);
            if (apiDescriptionSettings?.Groups == null || apiDescriptionSettings.Groups.Length == 0)
                return DocumentGroupExtras;

            // 处理分组额外信息
            var groupExtras = new List<GroupExtraInfo>();
            foreach (var group in apiDescriptionSettings.Groups)
            {
                groupExtras.Add(ResolveGroupExtraInfo(group));
            }

            return groupExtras;
        }
    }

    /// <summary>
    /// <see cref="GetActionGroups(MethodInfo)"/> 缓存集合
    /// </summary>
    private static readonly ConcurrentDictionary<MethodInfo, IEnumerable<GroupExtraInfo>> GetActionGroupsCached;

    /// <summary>
    /// 获取动作方法分组列表
    /// </summary>
    /// <param name="method">方法</param>
    /// <returns></returns>
    public static IEnumerable<GroupExtraInfo> GetActionGroups(MethodInfo method)
    {
        return GetActionGroupsCached.GetOrAdd(method, Function);

        // 本地函数
        static IEnumerable<GroupExtraInfo> Function(MethodInfo method)
        {
            // 如果动作方法没有定义 [ApiDescriptionSettings] 特性，则返回所在控制器分组
            if (!method.IsDefined(typeof(ApiDescriptionSettingsAttribute), true))
                return GetControllerGroups(method.ReflectedType);

            // 读取分组
            var apiDescriptionSettings = method.GetCustomAttribute<ApiDescriptionSettingsAttribute>(true);
            if (apiDescriptionSettings?.Groups == null || apiDescriptionSettings.Groups.Length == 0)
                return GetControllerGroups(method.ReflectedType);

            // 处理排序
            var groupExtras = new List<GroupExtraInfo>();
            foreach (var group in apiDescriptionSettings.Groups)
            {
                groupExtras.Add(ResolveGroupExtraInfo(group));
            }

            return groupExtras;
        }
    }

    /// <summary>
    /// <see cref="GetActionTag(ApiDescription)"/> 缓存集合
    /// </summary>
    private static readonly ConcurrentDictionary<ControllerActionDescriptor, string> GetControllerTagCached;

    /// <summary>
    /// 获取控制器标签
    /// </summary>
    /// <param name="controllerActionDescriptor">控制器接口描述器</param>
    /// <returns></returns>
    public static string GetControllerTag(ControllerActionDescriptor controllerActionDescriptor)
    {
        return GetControllerTagCached.GetOrAdd(controllerActionDescriptor, Function);

        // 本地函数
        static string Function(ControllerActionDescriptor controllerActionDescriptor)
        {
            var type = controllerActionDescriptor.ControllerTypeInfo;
            // 如果动作方法没有定义 [ApiDescriptionSettings] 特性，则返回所在控制器名
            if (!type.IsDefined(typeof(ApiDescriptionSettingsAttribute), true))
                return controllerActionDescriptor.ControllerName;

            // 读取标签
            return controllerActionDescriptor.ControllerName;
        }
    }

    /// <summary>
    /// <see cref="GetActionTag(ApiDescription)"/> 缓存集合
    /// </summary>
    private static readonly ConcurrentDictionary<ApiDescription, string> GetActionTagCached;

    /// <summary>
    /// 获取动作方法标签
    /// </summary>
    /// <param name="apiDescription">接口描述器</param>
    /// <returns></returns>
    public static string GetActionTag(ApiDescription apiDescription)
    {
        return GetActionTagCached.GetOrAdd(apiDescription, Function);

        // 本地函数
        static string Function(ApiDescription apiDescription)
        {
            if (!apiDescription.TryGetMethodInfo(out var method)
                || apiDescription.ActionDescriptor is not ControllerActionDescriptor controllerActionDescriptor)
                return Assembly.GetEntryAssembly()
                    ?.GetName()
                    .Name;

            // 如果动作方法没有定义 [ApiDescriptionSettings] 特性，则返回所在控制器名
            if (!method.IsDefined(typeof(ApiDescriptionSettingsAttribute), true))
                return GetControllerTag(controllerActionDescriptor);

            // 读取标签
            return GetControllerTag(controllerActionDescriptor);
        }
    }

    /// <summary>
    /// 是否是动作方法
    /// </summary>
    /// <param name="method">方法</param>
    /// <param name="ReflectedType">声明类型</param>
    /// <returns></returns>
    public static bool IsApiAction(MethodInfo method, Type ReflectedType)
    {
        // 不是非公开、抽象、静态、泛型方法
        if (!method.IsPublic || method.IsAbstract || method.IsStatic || method.IsGenericMethod)
            return false;

        // 如果所在类型不是控制器，则该行为也被忽略
        if (method.ReflectedType != ReflectedType || method.DeclaringType == typeof(object))
            return false;

        return true;
    }

    /// <summary>
    /// 解析分组附加信息
    /// </summary>
    /// <param name="group">分组名</param>
    /// <returns></returns>
    private static GroupExtraInfo ResolveGroupExtraInfo(string group)
    {
        string realGroup;
        var order = 0;

        if (!_groupOrderRegex.IsMatch(group))
            realGroup = group;
        else
        {
            realGroup = _groupOrderRegex.Replace(group, "");
            order = int.Parse(_groupOrderRegex.Match(group)
                .Groups["order"].Value);
        }

        var groupOpenApiInfo = GetGroupOpenApiInfo(realGroup);
        return new GroupExtraInfo
        {
            Group = realGroup, Order = groupOpenApiInfo.Order ?? order, Visible = groupOpenApiInfo.Visible ?? true
        };
    }
}