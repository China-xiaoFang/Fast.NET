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
using System.Text.RegularExpressions;
using Fast.UnifyResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Fast.DynamicApplication;

/// <summary>
/// <see cref="DynamicApiControllerApplicationModelConvention"/> 动态接口控制器应用模型转换器
/// </summary>
internal sealed class DynamicApiControllerApplicationModelConvention : IApplicationModelConvention
{
    /// <summary>
    /// 带版本的名称正则表达式
    /// </summary>
    private readonly Regex _nameVersionRegex = new("V(?<version>[0-9_]+$)");

    /// <summary>
    /// 服务集合
    /// </summary>
    private readonly IServiceCollection _services;

    /// <summary>
    /// 模板正则表达式
    /// </summary>
    private const string commonTemplatePattern = @"\{(?<p>.+?)\}";

    public DynamicApiControllerApplicationModelConvention(IServiceCollection services)
    {
        _services = services;
    }

    /// <summary>
    /// 配置应用模型信息
    /// </summary>
    /// <param name="application">引用模型</param>
    public void Apply(ApplicationModel application)
    {
        var controllers = application.Controllers.Where(u => DynamicApplicationContext.IsApiController(u.ControllerType));

        foreach (var controller in controllers)
        {
            var controllerType = controller.ControllerType;

            // 解析 [ApiDescriptionSettings] 特性
            var controllerApiDescriptionSettings = controllerType.IsDefined(typeof(ApiDescriptionSettingsAttribute), true)
                ? controllerType.GetCustomAttribute<ApiDescriptionSettingsAttribute>(true)
                : null;

            // 判断是否处理 Mvc控制器
            if (typeof(ControllerBase).IsAssignableFrom(controllerType))
            {
                if (controller.ApiExplorer.IsVisible == false)
                {
                    // 存储排序给 Swagger 使用
                    DynamicApplicationContext.ControllerOrderCollection.TryAdd(controller.ControllerName,
                        (controller.ControllerName, controllerApiDescriptionSettings?.Order ?? 0, controller.ControllerType));

                    // 控制器默认处理规范化结果
                    foreach (var action in controller.Actions)
                    {
                        // 配置动作方法规范化特性
                        ConfigureActionUnifyResultAttribute(action);
                    }

                    continue;
                }
            }

            ConfigureController(controller, controllerApiDescriptionSettings);
        }
    }

    /// <summary>
    /// 配置控制器
    /// </summary>
    /// <param name="controller">控制器模型</param>
    /// <param name="controllerApiDescriptionSettings">接口描述配置</param>
    private void ConfigureController(ControllerModel controller, ApiDescriptionSettingsAttribute controllerApiDescriptionSettings)
    {
        // 配置控制器名称
        ConfigureControllerName(controller, controllerApiDescriptionSettings);

        // 存储排序给 Swagger 使用
        DynamicApplicationContext.ControllerOrderCollection.TryAdd(controller.ControllerName,
            (controller.ControllerName, controllerApiDescriptionSettings?.Order ?? 0, controller.ControllerType));

        var actions = controller.Actions;

        // 查找所有重复的方法签名
        var repeats = actions.GroupBy(u => new {u.ActionMethod.ReflectedType?.Name, Signature = u.ActionMethod.ToString()})
            .Where(u => u.Count() > 1)
            .SelectMany(u => u.Where(i => i.ActionMethod.ReflectedType?.Name != i.ActionMethod.DeclaringType?.Name))
            .ToList();

        // 2021年04月01日 https://docs.microsoft.com/en-US/aspnet/core/web-api/?view=aspnetcore-5.0#binding-source-parameter-inference
        // 判断是否贴有 [ApiController] 特性
        var hasApiControllerAttribute = controller.Attributes.Any(u => u.GetType() == typeof(ApiControllerAttribute));

        foreach (var action in actions)
        {
            // 跳过相同方法签名
            if (repeats.Contains(action))
            {
                action.ApiExplorer.IsVisible = false;
                continue;
            }

            var actionMethod = action.ActionMethod;
            var actionApiDescriptionSettings = actionMethod.IsDefined(typeof(ApiDescriptionSettingsAttribute), true)
                ? actionMethod.GetCustomAttribute<ApiDescriptionSettingsAttribute>(true)
                : null;

            ConfigureAction(action, actionApiDescriptionSettings, controllerApiDescriptionSettings, hasApiControllerAttribute);
        }
    }

    /// <summary>
    /// 配置控制器名称
    /// </summary>
    /// <param name="controller">控制器模型</param>
    /// <param name="controllerApiDescriptionSettings">接口描述配置</param>
    private void ConfigureControllerName(ControllerModel controller,
        ApiDescriptionSettingsAttribute controllerApiDescriptionSettings)
    {
        controller.ControllerName =
            ConfigureControllerAndActionName(controllerApiDescriptionSettings, controller.ControllerType.Name);
    }

    /// <summary>
    /// 配置动作方法
    /// </summary>
    /// <param name="action">控制器模型</param>
    /// <param name="apiDescriptionSettings">接口描述配置</param>
    /// <param name="controllerApiDescriptionSettings">控制器接口描述配置</param>
    /// <param name="hasApiControllerAttribute">是否贴有 ApiController 特性</param>
    private void ConfigureAction(ActionModel action, ApiDescriptionSettingsAttribute apiDescriptionSettings,
        ApiDescriptionSettingsAttribute controllerApiDescriptionSettings, bool hasApiControllerAttribute)
    {
        // 配置动作方法接口可见性
        ConfigureActionApiExplorer(action);

        // 配置动作方法名称
        ConfigureActionName(action, apiDescriptionSettings);

        // 配置引用类型参数
        ConfigureClassTypeParameter(action);

        // 配置动作方法路由特性
        ConfigureActionRouteAttribute(action, apiDescriptionSettings, controllerApiDescriptionSettings,
            hasApiControllerAttribute);

        // 配置动作方法规范化特性
        ConfigureActionUnifyResultAttribute(action);
    }

    /// <summary>
    /// 配置动作方法接口可见性
    /// </summary>
    /// <param name="action">动作方法模型</param>
    private static void ConfigureActionApiExplorer(ActionModel action)
    {
        action.ApiExplorer.IsVisible ??= true;
    }

    /// <summary>
    /// 配置动作方法名称
    /// </summary>
    /// <param name="action">动作方法模型</param>
    /// <param name="apiDescriptionSettings">接口描述配置</param>
    /// <returns></returns>
    private void ConfigureActionName(ActionModel action, ApiDescriptionSettingsAttribute apiDescriptionSettings)
    {
        // 判断是否贴有 [ActionName]
        string actionName = null;

        // 判断是否贴有 [ActionName] 且 Name 不为 null
        var actionNameAttribute = action.ActionMethod.IsDefined(typeof(ActionNameAttribute), true)
            ? action.ActionMethod.GetCustomAttribute<ActionNameAttribute>(true)
            : null;

        if (actionNameAttribute?.Name != null)
        {
            actionName = actionNameAttribute.Name;
        }

        var Name = ConfigureControllerAndActionName(apiDescriptionSettings, action.ActionMethod.Name, actionName);
        action.ActionName = Name;
    }

    /// <summary>
    /// 处理类类型参数（添加[FromBody] 特性）
    /// </summary>
    /// <param name="action"></param>
    private void ConfigureClassTypeParameter(ActionModel action)
    {
        // 没有参数无需处理
        if (action.Parameters.Count == 0)
            return;

        var parameters = action.Parameters;
        foreach (var parameterModel in parameters)
        {
            // 如果参数已有绑定特性，则跳过
            if (parameterModel.BindingInfo != null)
                continue;

            var parameterType = parameterModel.ParameterType;
            // 如果是基元类型，则跳过
            if (parameterType.IsRichPrimitive())
                continue;

            // 如果是文件类型，则跳过
            if (typeof(IFormFile).IsAssignableFrom(parameterType) || typeof(IFormFileCollection).IsAssignableFrom(parameterType))
                continue;

            // 处理 .NET7 接口问题，同时支持 .NET5/6 无需贴 [FromServices] 操作
            if (parameterType.IsInterface
                && !parameterModel.Attributes.Any(u => u is IBindingSourceMetadata)
                && _services.Any(s => s.ServiceType.Name == parameterType.Name))
            {
                parameterModel.BindingInfo = BindingInfo.GetBindingInfo([new FromServicesAttribute()]);
                continue;
            }

            parameterModel.BindingInfo = BindingInfo.GetBindingInfo([new FromBodyAttribute()]);
        }
    }

    /// <summary>
    /// 配置动作方法路由特性
    /// </summary>
    /// <param name="action">动作方法模型</param>
    /// <param name="apiDescriptionSettings">接口描述配置</param>
    /// <param name="controllerApiDescriptionSettings">控制器接口描述配置</param>
    /// <param name="hasApiControllerAttribute"></param>
    private static void ConfigureActionRouteAttribute(ActionModel action, ApiDescriptionSettingsAttribute apiDescriptionSettings,
        ApiDescriptionSettingsAttribute controllerApiDescriptionSettings, bool hasApiControllerAttribute)
    {
        foreach (var selectorModel in action.Selectors)
        {
            // 读取模块
            var module = apiDescriptionSettings?.Module;

            // 跳过已配置路由特性的配置
            if (selectorModel.AttributeRouteModel != null)
            {
                // 1. 如果控制器自定义了 [Route] 特性，则跳过
                if (action.ActionMethod.DeclaringType.IsDefined(typeof(RouteAttribute), true)
                    || action.Controller.ControllerType.IsDefined(typeof(RouteAttribute), true))
                {
                    if (string.IsNullOrWhiteSpace(selectorModel.AttributeRouteModel.Template)
                        && !string.IsNullOrWhiteSpace(selectorModel.AttributeRouteModel.Name))
                    {
                        selectorModel.AttributeRouteModel.Template = selectorModel.AttributeRouteModel.Name;
                    }

                    var newTemplate =
                        $"{(selectorModel.AttributeRouteModel.Template?.StartsWith("/") == true ? "/" : null)}{(string.IsNullOrWhiteSpace(module) ? null : $"{module}/")}{selectorModel.AttributeRouteModel.Template}";
                    // 处理可能存在多斜杠问题
                    newTemplate = Regex.Replace(newTemplate, @"\/{2,}", "/");
                    selectorModel.AttributeRouteModel.Template = newTemplate;

                    continue;
                }

                // 2. 如果方法自定义路由模板且以 `/` 开头，则跳过
                if (!string.IsNullOrWhiteSpace(selectorModel.AttributeRouteModel.Template)
                    && selectorModel.AttributeRouteModel.Template.StartsWith("/"))
                    continue;
            }

            string template;
            string controllerRouteTemplate = null;
            // 如果动作方法名称为空、参数值为空，且无需保留谓词，则只生成控制器路由模板
            if (action.ActionName.Length == 0 && action.Parameters.Count == 0)
            {
                template = GenerateControllerRouteTemplate(action.Controller, controllerApiDescriptionSettings);
                if (!string.IsNullOrWhiteSpace(selectorModel.AttributeRouteModel?.Template))
                {
                    template = $"{template}/{selectorModel.AttributeRouteModel?.Template}";
                }
            }
            else
            {
                // 生成参数路由模板
                var parameterRouteTemplate = GenerateParameterRouteTemplates(action, hasApiControllerAttribute);

                // 生成控制器模板
                controllerRouteTemplate = GenerateControllerRouteTemplate(action.Controller, controllerApiDescriptionSettings);

                // 拼接动作方法路由模板
                var ActionEndTemplate = parameterRouteTemplate != null
                    ? parameterRouteTemplate.Count == 0 ? null : string.Join("/", parameterRouteTemplate)
                    : null;

                // 判断是否定义了控制器路由，如果定义，则不拼接控制器路由
                var actionRouteTemplate = string.IsNullOrWhiteSpace(action.ActionName)
                                          || (action.Controller.Selectors[0]
                                                  .AttributeRouteModel?.Template?.Contains("[action]")
                                              ?? false)
                    ? null
                    : selectorModel?.AttributeRouteModel?.Template ?? selectorModel?.AttributeRouteModel?.Name ?? "[action]";

                if (actionRouteTemplate == null && !string.IsNullOrWhiteSpace(selectorModel.AttributeRouteModel?.Template))
                {
                    actionRouteTemplate = $"{actionRouteTemplate}/{selectorModel.AttributeRouteModel?.Template}";
                }

                template = string.IsNullOrWhiteSpace(controllerRouteTemplate)
                    ? $"{(string.IsNullOrWhiteSpace(module) ? "/" : $"/{module}/")}/{actionRouteTemplate}/{ActionEndTemplate}"
                    : $"{controllerRouteTemplate}/{(string.IsNullOrWhiteSpace(module) ? null : $"/{module}/")}/{actionRouteTemplate}/{ActionEndTemplate}";
            }

            AttributeRouteModel actionAttributeRouteModel = null;
            if (!string.IsNullOrWhiteSpace(template))
            {
                // 处理多个斜杆问题
                template = HandleRouteTemplateRepeat(template);
                template = Regex.Replace(template, @"\/{2,}", "/");

                // 生成路由
                actionAttributeRouteModel = string.IsNullOrWhiteSpace(template)
                    ? null
                    : new AttributeRouteModel(new RouteAttribute(template));
            }

            // 拼接路由
            selectorModel.AttributeRouteModel = string.IsNullOrWhiteSpace(controllerRouteTemplate)
                ? actionAttributeRouteModel == null
                    ? null
                    : AttributeRouteModel.CombineAttributeRouteModel(action.Controller.Selectors[0].AttributeRouteModel,
                        actionAttributeRouteModel)
                : actionAttributeRouteModel;
        }
    }

    /// <summary>
    /// 生成控制器路由模板
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="apiDescriptionSettings"></param>
    /// <returns></returns>
    private static string GenerateControllerRouteTemplate(ControllerModel controller,
        ApiDescriptionSettingsAttribute apiDescriptionSettings)
    {
        var selectorModel = controller.Selectors[0];
        // 跳过已配置路由特性的配置
        if (selectorModel.AttributeRouteModel != null)
            return null;

        // 读取模块
        var module = apiDescriptionSettings?.Module;

        // 路由默认前缀
        var routePrefix = DynamicApplicationContext.RoutePrefix;

        return
            $"{(string.IsNullOrWhiteSpace(routePrefix) ? null : $"{routePrefix}/")}{(string.IsNullOrWhiteSpace(module) ? null : $"/{module}/")}[controller]";
    }

    /// <summary>
    /// 生成参数路由模板（非引用类型）
    /// </summary>
    /// <param name="action">动作方法模型</param>
    /// <param name="hasApiControllerAttribute"></param>
    private static IList<string> GenerateParameterRouteTemplates(ActionModel action, bool hasApiControllerAttribute)
    {
        // 如果没有参数，则跳过
        if (action.Parameters.Count == 0)
            return null;

        var result = new List<string>();
        var parameters = action.Parameters.Where(u =>
            !(u.BindingInfo is {BindingSource.DisplayName: "Special"} || u.Attributes.Any(c => c is BindNeverAttribute)));

        // 遍历所有参数
        foreach (var parameterModel in parameters)
        {
            var parameterType = parameterModel.ParameterType;
            // 如果非基元类型，则跳过
            if (!parameterType.IsRichPrimitive())
                continue;
            var parameterAttributes = parameterModel.Attributes;

            // 判断是否贴有任何 [FromXXX] 特性了
            var hasFromAttribute = parameterAttributes.Any(u => u is IBindingSourceMetadata);

            // 判断当前参数没有任何 [FromXXX] 特性，则添加 [FromQuery] 特性
            if (!hasFromAttribute)
            {
                parameterModel.BindingInfo = BindingInfo.GetBindingInfo([new FromQueryAttribute()]);
                continue;
            }

            // 如果没有贴 [FromRoute] 特性且不是基元类型，则跳过
            // 如果没有贴 [FromRoute] 特性且有任何绑定特性，则跳过
            if (!parameterAttributes.Any(u => u is FromRouteAttribute) && !parameterType.IsRichPrimitive())
                continue;

            // 处理基元数组数组类型，还有全局配置参数问题
            if (parameterType.IsArray)
            {
                parameterModel.BindingInfo = BindingInfo.GetBindingInfo(new[] {new FromQueryAttribute()});
                continue;
            }

            // 处理 [ApiController] 特性情况
            // https://docs.microsoft.com/en-US/aspnet/core/web-api/?view=aspnetcore-5.0#binding-source-parameter-inference
            if (hasApiControllerAttribute)
                continue;

            // 判断是否可以为null
            var canBeNull = parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Nullable<>);

            var template = $"{parameterModel.ParameterName}{(canBeNull ? "?" : string.Empty)}";

            // 动作方法名之后
            result.Add(template);
        }

        return result;
    }

    /// <summary>
    /// 配置控制器和动作方法名称
    /// </summary>
    /// <param name="apiDescriptionSettings"></param>
    /// <param name="originalName"></param>
    /// <param name="actionName">针对 [ActionName] 特性和 [HttpMethod] 特性处理</param>
    /// <returns></returns>
    private string ConfigureControllerAndActionName(ApiDescriptionSettingsAttribute apiDescriptionSettings, string originalName,
        string actionName = null)
    {
        // 获取版本号
        var apiVersion = apiDescriptionSettings?.Version;

        // 判断是否有自定义名称
        var tempName = actionName ?? apiDescriptionSettings?.Name;
        if (string.IsNullOrWhiteSpace(tempName))
        {
            // 处理版本号
            var (name, version) = ResolveNameVersion(originalName);
            tempName = name;
            apiVersion ??= version;
        }

        // 默认小驼峰命名
        tempName = tempName.FirstCharToLower();

        // 拼接名称和版本号
        var newName = $"{tempName}{(string.IsNullOrWhiteSpace(apiVersion) ? null : $"v{apiVersion}")}";

        return newName;
    }

    /// <summary>
    /// 配置规范化结果类型
    /// </summary>
    /// <param name="action"></param>
    private static void ConfigureActionUnifyResultAttribute(ActionModel action)
    {
        // 判断是否手动添加了标注或跳过规范化处理
        if (UnifyContext.CheckSucceededNonUnify(null, action.ActionMethod, out _, false))
            return;

        // 获取真实类型
        var returnType = action.ActionMethod.GetRealReturnType();
        if (returnType == typeof(void))
            return;

        // 判断是否启用规范化结果处理
        if (returnType != null && UnifyContext.EnabledUnifyHandler)
        {
            returnType = UnifyContext.UnifyResultType?.MakeGenericType(returnType) ?? returnType;
        }

        // 添加规范化结果特性
        action.Filters.Add(new ProducesResponseTypeAttribute(returnType, StatusCodes.Status200OK));
    }

    /// <summary>
    /// 解析名称中的版本号
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns>名称和版本号</returns>
    private (string name, string version) ResolveNameVersion(string name)
    {
        if (!_nameVersionRegex.IsMatch(name))
            return (name, null);

        var version = _nameVersionRegex.Match(name)
            .Groups["version"]
            .Value.Replace("_", ".");
        return (_nameVersionRegex.Replace(name, ""), version);
    }

    /// <summary>
    /// 处理路由模板重复参数
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    private static string HandleRouteTemplateRepeat(string template)
    {
        var isStartDiagonal = template.StartsWith("/");
        var paths = template.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var routeParts = new List<string>();

        // 参数模板
        var paramTemplates = new List<string>();
        foreach (var part in paths)
        {
            // 不包含 {} 模板的直接添加
            if (!Regex.IsMatch(part, commonTemplatePattern))
            {
                routeParts.Add(part);
                continue;
            }

            var templates = Regex.Matches(part, commonTemplatePattern)
                .Select(t => t.Value);
            foreach (var temp in templates)
            {
                // 处理带路由约束的路由参数模板 https://gitee.com/zuohuaijun/Admin.NET/issues/I736XJ
                var t = !temp.Contains('?', StringComparison.CurrentCulture)
                    ? !temp.Contains(':', StringComparison.CurrentCulture)
                        ? temp
                        : temp[..temp.IndexOf(":", StringComparison.Ordinal)] + "}"
                    : temp[..temp.IndexOf("?", StringComparison.Ordinal)] + "}";

                if (!paramTemplates.Contains(t, StringComparer.OrdinalIgnoreCase))
                {
                    routeParts.Add(part);
                    paramTemplates.Add(t);
                }
            }
        }

        var tmp = string.Join('/', routeParts);
        return isStartDiagonal ? "/" + tmp : tmp;
    }
}