// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
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
using System.Text.RegularExpressions;
using Fast.DynamicApplication.Extensions;
using Fast.DynamicApplication.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.DynamicApplication.Conventions;

/// <summary>
/// 动态接口控制器应用模型转换器
/// </summary>
internal sealed class DynamicApiControllerApplicationModelConvention : IApplicationModelConvention
{
    /// <summary>
    /// 带版本的名称正则表达式
    /// </summary>
    private readonly Regex _nameVersionRegex;

    /// <summary>
    /// 服务集合
    /// </summary>
    private readonly IServiceCollection _services;

    /// <summary>
    /// 模板正则表达式
    /// </summary>
    private const string commonTemplatePattern = @"\{(?<p>.+?)\}";

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="services">服务集合</param>
    public DynamicApiControllerApplicationModelConvention(IServiceCollection services)
    {
        _services = services;
        _nameVersionRegex = new Regex(@"V(?<version>[0-9_]+$)");
    }

    /// <summary>
    /// 配置应用模型信息
    /// </summary>
    /// <param name="application">引用模型</param>
    public void Apply(ApplicationModel application)
    {
        var controllers = application.Controllers.Where(u => Penetrates.IsApiController(u.ControllerType));
        foreach (var controller in controllers)
        {
            var controllerType = controller.ControllerType;

            // 解析 [ApiDescriptionSettings] 特性
            var controllerApiDescriptionSettings = controllerType.IsDefined(typeof(ApiDescriptionSettingsAttribute), true)
                ? controllerType.GetCustomAttribute<ApiDescriptionSettingsAttribute>(true)
                : default;

            // 判断是否处理 Mvc控制器
            if (typeof(ControllerBase).IsAssignableFrom(controllerType))
            {
                // 存储排序给 Swagger 使用
                Penetrates.ControllerOrderCollection.TryAdd(controller.ControllerName,
                    (controllerApiDescriptionSettings?.Tag ?? controller.ControllerName,
                        controllerApiDescriptionSettings?.Order ?? 0, controller.ControllerType));

                continue;
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
        // 配置区域
        ConfigureControllerArea(controller, controllerApiDescriptionSettings);

        // 配置控制器名称
        ConfigureControllerName(controller, controllerApiDescriptionSettings);

        // 存储排序给 Swagger 使用
        Penetrates.ControllerOrderCollection.TryAdd(controller.ControllerName,
            (controllerApiDescriptionSettings?.Tag ?? controller.ControllerName, controllerApiDescriptionSettings?.Order ?? 0,
                controller.ControllerType));

        var actions = controller.Actions;

        // 查找所有重复的方法签名
        var repeats = actions.GroupBy(u => new {u.ActionMethod.ReflectedType.Name, Signature = u.ActionMethod.ToString()})
            .Where(u => u.Count() > 1)
            .SelectMany(u => u.Where(u => u.ActionMethod.ReflectedType.Name != u.ActionMethod.DeclaringType.Name));

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
                : default;
            ConfigureAction(action, actionApiDescriptionSettings, controllerApiDescriptionSettings, hasApiControllerAttribute);
        }
    }

    /// <summary>
    /// 配置控制器区域
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="controllerApiDescriptionSettings"></param>
    private void ConfigureControllerArea(ControllerModel controller,
        ApiDescriptionSettingsAttribute controllerApiDescriptionSettings)
    {
        // 如果配置了区域，则跳过
        if (controller.RouteValues.ContainsKey("area"))
            return;

        // 如果没有配置区域，则跳过
        var area = controllerApiDescriptionSettings?.Area;
        if (string.IsNullOrWhiteSpace(area))
            return;

        controller.RouteValues["area"] = area;
    }

    /// <summary>
    /// 配置控制器名称
    /// </summary>
    /// <param name="controller">控制器模型</param>
    /// <param name="controllerApiDescriptionSettings">接口描述配置</param>
    private void ConfigureControllerName(ControllerModel controller,
        ApiDescriptionSettingsAttribute controllerApiDescriptionSettings)
    {
        var (Name, _, _, _) =
            ConfigureControllerAndActionName(controllerApiDescriptionSettings, controller.ControllerType.Name, _ => _);
        controller.ControllerName = Name;
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
        var (isLowercaseRoute, isKeepName, isLowerCamelCase) =
            ConfigureActionName(action, apiDescriptionSettings, controllerApiDescriptionSettings);

        // 配置动作方法请求谓词特性
        ConfigureActionHttpMethodAttribute(action);

        // 配置引用类型参数
        ConfigureClassTypeParameter(action);

        // 配置动作方法路由特性
        ConfigureActionRouteAttribute(action, apiDescriptionSettings, controllerApiDescriptionSettings, isLowercaseRoute,
            isKeepName, isLowerCamelCase, hasApiControllerAttribute);
    }

    /// <summary>
    /// 配置动作方法接口可见性
    /// </summary>
    /// <param name="action">动作方法模型</param>
    private static void ConfigureActionApiExplorer(ActionModel action)
    {
        if (!action.ApiExplorer.IsVisible.HasValue)
            action.ApiExplorer.IsVisible = true;
    }

    /// <summary>
    /// 配置动作方法名称
    /// </summary>
    /// <param name="action">动作方法模型</param>
    /// <param name="apiDescriptionSettings">接口描述配置</param>
    /// <param name="controllerApiDescriptionSettings"></param>
    /// <returns></returns>
    private (bool IsLowercaseRoute, bool IsKeepName, bool IsLowerCamelCase) ConfigureActionName(ActionModel action,
        ApiDescriptionSettingsAttribute apiDescriptionSettings, ApiDescriptionSettingsAttribute controllerApiDescriptionSettings)
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

        var (Name, IsLowercaseRoute, IsKeepName, IsLowerCamelCase) = ConfigureControllerAndActionName(apiDescriptionSettings,
            action.ActionMethod.Name, tempName => tempName, controllerApiDescriptionSettings, actionName);
        action.ActionName = Name;

        return (IsLowercaseRoute, IsKeepName, IsLowerCamelCase);
    }

    /// <summary>
    /// 配置动作方法请求谓词特性
    /// </summary>
    /// <param name="action">动作方法模型</param>
    private void ConfigureActionHttpMethodAttribute(ActionModel action)
    {
        var selectorModel = action.Selectors[0];
        // 跳过已配置请求谓词特性的配置
        if (selectorModel.ActionConstraints.Any(u => u is HttpMethodActionConstraint))
            return;

        throw new NotSupportedException($"{action.ActionMethod.Name}");
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

        // 如果动作方法请求谓词只有GET和HEAD，则将类转查询参数
        var httpMethods = action.Selectors.SelectMany(u =>
            u.ActionConstraints.Where(metadata => metadata is HttpMethodActionConstraint)
                .SelectMany(metadata => (metadata as HttpMethodActionConstraint)?.HttpMethods));

        if (httpMethods.All(u => u.Equals("GET") || u.Equals("HEAD")))
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
            if (parameterType.IsInterface &&
                !parameterModel.Attributes.Any(u => typeof(IBindingSourceMetadata).IsAssignableFrom(u.GetType())) &&
                _services.Any(s => s.ServiceType.Name == parameterType.Name))
            {
                parameterModel.BindingInfo = BindingInfo.GetBindingInfo(new[] {new FromServicesAttribute()});
                continue;
            }

            parameterModel.BindingInfo = BindingInfo.GetBindingInfo(new[] {new FromBodyAttribute()});
        }
    }

    /// <summary>
    /// 配置动作方法路由特性
    /// </summary>
    /// <param name="action">动作方法模型</param>
    /// <param name="apiDescriptionSettings">接口描述配置</param>
    /// <param name="controllerApiDescriptionSettings">控制器接口描述配置</param>
    /// <param name="isLowercaseRoute"></param>
    /// <param name="isKeepName"></param>
    /// <param name="isLowerCamelCase"></param>
    /// <param name="hasApiControllerAttribute"></param>
    private void ConfigureActionRouteAttribute(ActionModel action, ApiDescriptionSettingsAttribute apiDescriptionSettings,
        ApiDescriptionSettingsAttribute controllerApiDescriptionSettings, bool isLowercaseRoute, bool isKeepName,
        bool isLowerCamelCase, bool hasApiControllerAttribute)
    {
        foreach (var selectorModel in action.Selectors)
        {
            // 跳过已配置路由特性的配置
            if (selectorModel.AttributeRouteModel != null)
            {
                // 1. 如果控制器自定义了 [Route] 特性，则跳过
                if (action.ActionMethod.DeclaringType.IsDefined(typeof(RouteAttribute), true) ||
                    action.Controller.ControllerType.IsDefined(typeof(RouteAttribute), true))
                {
                    if (string.IsNullOrWhiteSpace(selectorModel.AttributeRouteModel.Template) &&
                        !string.IsNullOrWhiteSpace(selectorModel.AttributeRouteModel.Name))
                    {
                        selectorModel.AttributeRouteModel.Template = selectorModel.AttributeRouteModel.Name;
                    }

                    continue;
                }

                // 2. 如果方法自定义路由模板且以 `/` 开头，则跳过
                if (!string.IsNullOrWhiteSpace(selectorModel.AttributeRouteModel.Template) &&
                    selectorModel.AttributeRouteModel.Template.StartsWith("/"))
                    continue;
            }

            // 读取模块
            var module = apiDescriptionSettings?.Module;

            string template;
            // 如果动作方法名称为空、参数值为空，且无需保留谓词，则只生成控制器路由模板
            if (action.ActionName.Length == 0 && !isKeepName && action.Parameters.Count == 0)
            {
                template = GenerateControllerRouteTemplate(action.Controller, controllerApiDescriptionSettings);
                if (!string.IsNullOrWhiteSpace(selectorModel.AttributeRouteModel?.Template))
                {
                    template = $"{template}/{selectorModel.AttributeRouteModel?.Template}";
                }
            }
            else
            {
                // 判断是否定义了控制器路由，如果定义，则不拼接控制器路由
                var actionRouteTemplate =
                    string.IsNullOrWhiteSpace(action.ActionName) ||
                    (action.Controller.Selectors[0].AttributeRouteModel?.Template?.Contains("[action]") ?? false)
                        ? null
                        : (selectorModel?.AttributeRouteModel?.Template ??
                           selectorModel?.AttributeRouteModel?.Name ?? "[action]");

                if (actionRouteTemplate == null && !string.IsNullOrWhiteSpace(selectorModel.AttributeRouteModel?.Template))
                {
                    actionRouteTemplate = $"{actionRouteTemplate}/{selectorModel.AttributeRouteModel?.Template}";
                }

                template = $"{(string.IsNullOrWhiteSpace(module) ? "/" : $"{module}/")}{actionRouteTemplate}";
            }

            AttributeRouteModel actionAttributeRouteModel = null;
            if (!string.IsNullOrWhiteSpace(template))
            {
                // 处理多个斜杆问题
                template = Regex.Replace(
                    isLowercaseRoute ? template.ToLower() : isLowerCamelCase ? template.FirstCharToLower() : template, @"\/{2,}",
                    "/");
                template = HandleRouteTemplateRepeat(template);

                // 生成路由
                actionAttributeRouteModel = string.IsNullOrWhiteSpace(template)
                    ? null
                    : new AttributeRouteModel(new RouteAttribute(template));
            }

            // 拼接路由
            selectorModel.AttributeRouteModel = (actionAttributeRouteModel == null
                ? null
                : AttributeRouteModel.CombineAttributeRouteModel(action.Controller.Selectors[0].AttributeRouteModel,
                    actionAttributeRouteModel));
        }
    }

    /// <summary>
    /// 生成控制器路由模板
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="apiDescriptionSettings"></param>
    /// <returns></returns>
    private string GenerateControllerRouteTemplate(ControllerModel controller,
        ApiDescriptionSettingsAttribute apiDescriptionSettings)
    {
        var selectorModel = controller.Selectors[0];
        // 跳过已配置路由特性的配置
        if (selectorModel.AttributeRouteModel != null)
            return default;

        // 读取模块
        var module = apiDescriptionSettings?.Module;

        return $"{(string.IsNullOrWhiteSpace(module) ? null : $"{module}/")}[controller]";
    }

    /// <summary>
    /// 配置控制器和动作方法名称
    /// </summary>
    /// <param name="apiDescriptionSettings"></param>
    /// <param name="orignalName"></param>
    /// <param name="configure"></param>
    /// <param name="controllerApiDescriptionSettings"></param>
    /// <param name="actionName">针对 [ActionName] 特性和 [HttpMethod] 特性处理</param>
    /// <returns></returns>
    private (string Name, bool IsLowercaseRoute, bool IsKeepName, bool IsLowerCamelCase) ConfigureControllerAndActionName(
        ApiDescriptionSettingsAttribute apiDescriptionSettings, string orignalName, Func<string, string> configure,
        ApiDescriptionSettingsAttribute controllerApiDescriptionSettings = default, string actionName = default)
    {
        // 获取版本号
        var apiVersion = apiDescriptionSettings?.Version;
        var isKeepName = false;

        // 判断是否有自定义名称
        var tempName = actionName ?? apiDescriptionSettings?.Name;
        if (string.IsNullOrWhiteSpace(tempName))
        {
            // 处理版本号
            var (name, version) = ResolveNameVersion(orignalName);
            tempName = name;
            apiVersion ??= version;

            isKeepName = CheckIsKeepName(controllerApiDescriptionSettings == null ? null : apiDescriptionSettings,
                controllerApiDescriptionSettings ?? apiDescriptionSettings);

            // 判断是否保留原有名称
            if (!isKeepName)
            {
                // 自定义配置
                tempName = configure.Invoke(tempName);

                // 处理骆驼命名
                if (CheckIsSplitCamelCase(controllerApiDescriptionSettings == null ? null : apiDescriptionSettings,
                        controllerApiDescriptionSettings ?? apiDescriptionSettings))
                {
                    tempName = string.Join("-", tempName.SplitCamelCase());
                }
            }
        }

        // 拼接名称和版本号
        var newName = $"{tempName}{apiVersion}";

        var isLowercaseRoute = CheckIsLowercaseRoute(controllerApiDescriptionSettings == null ? null : apiDescriptionSettings,
            controllerApiDescriptionSettings ?? apiDescriptionSettings);
        var isLowerCamelCase = CheckIsLowerCamelCase(controllerApiDescriptionSettings == null ? null : apiDescriptionSettings,
            controllerApiDescriptionSettings ?? apiDescriptionSettings);

        return (isLowercaseRoute ? newName.ToLower() : isLowerCamelCase ? newName.FirstCharToLower() : newName, isLowercaseRoute,
            isKeepName, isLowerCamelCase);
    }

    /// <summary>
    /// 检查是否设置了 KeepName参数
    /// </summary>
    /// <param name="apiDescriptionSettings"></param>
    /// <param name="controllerApiDescriptionSettings"></param>
    /// <returns></returns>
    private bool CheckIsKeepName(ApiDescriptionSettingsAttribute apiDescriptionSettings,
        ApiDescriptionSettingsAttribute controllerApiDescriptionSettings)
    {
        bool isKeepName;

        // 判断 Action 是否配置了 KeepName 属性
        if (apiDescriptionSettings?.KeepName != null)
        {
            var canParse = bool.TryParse(apiDescriptionSettings.KeepName.ToString(), out var value);
            isKeepName = canParse && value;
        }
        // 判断 Controller 是否配置了 KeepName 属性
        else if (controllerApiDescriptionSettings?.KeepName != null)
        {
            var canParse = bool.TryParse(controllerApiDescriptionSettings.KeepName.ToString(), out var value);
            isKeepName = canParse && value;
        }
        else
            isKeepName = false;

        return isKeepName;
    }

    /// <summary>
    /// 检查是否设置了 AsLowerCamelCase 参数
    /// </summary>
    /// <param name="apiDescriptionSettings"></param>
    /// <param name="controllerApiDescriptionSettings"></param>
    /// <returns></returns>
    private bool CheckIsLowerCamelCase(ApiDescriptionSettingsAttribute apiDescriptionSettings,
        ApiDescriptionSettingsAttribute controllerApiDescriptionSettings)
    {
        bool isLowerCamelCase;

        // 判断 Action 是否配置了 AsLowerCamelCase 属性
        if (apiDescriptionSettings?.AsLowerCamelCase != null)
        {
            var canParse = bool.TryParse(apiDescriptionSettings.AsLowerCamelCase.ToString(), out var value);
            isLowerCamelCase = canParse && value;
        }
        // 判断 Controller 是否配置了 AsLowerCamelCase 属性
        else if (controllerApiDescriptionSettings?.AsLowerCamelCase != null)
        {
            var canParse = bool.TryParse(controllerApiDescriptionSettings.AsLowerCamelCase.ToString(), out var value);
            isLowerCamelCase = canParse && value;
        }
        else
            isLowerCamelCase = false;

        return isLowerCamelCase;
    }

    /// <summary>
    /// 判断切割命名参数是否配置
    /// </summary>
    /// <param name="apiDescriptionSettings"></param>
    /// <param name="controllerApiDescriptionSettings"></param>
    /// <returns></returns>
    private static bool CheckIsSplitCamelCase(ApiDescriptionSettingsAttribute apiDescriptionSettings,
        ApiDescriptionSettingsAttribute controllerApiDescriptionSettings)
    {
        bool isSplitCamelCase;

        // 判断 Action 是否配置了 SplitCamelCase 属性
        if (apiDescriptionSettings?.SplitCamelCase != null)
        {
            var canParse = bool.TryParse(apiDescriptionSettings.SplitCamelCase.ToString(), out var value);
            isSplitCamelCase = !canParse || value;
        }
        // 判断 Controller 是否配置了 SplitCamelCase 属性
        else if (controllerApiDescriptionSettings?.SplitCamelCase != null)
        {
            var canParse = bool.TryParse(controllerApiDescriptionSettings.SplitCamelCase.ToString(), out var value);
            isSplitCamelCase = !canParse || value;
        }
        // 取全局配置
        else
            isSplitCamelCase = true;

        return isSplitCamelCase;
    }

    /// <summary>
    /// 检查是否启用小写路由
    /// </summary>
    /// <param name="apiDescriptionSettings"></param>
    /// <param name="controllerApiDescriptionSettings"></param>
    /// <returns></returns>
    private bool CheckIsLowercaseRoute(ApiDescriptionSettingsAttribute apiDescriptionSettings,
        ApiDescriptionSettingsAttribute controllerApiDescriptionSettings)
    {
        bool isLowercaseRoute;

        // 判断 Action 是否配置了 LowercaseRoute 属性
        if (apiDescriptionSettings?.LowercaseRoute != null)
        {
            var canParse = bool.TryParse(apiDescriptionSettings.LowercaseRoute.ToString(), out var value);
            isLowercaseRoute = !canParse || value;
        }
        // 判断 Controller 是否配置了 LowercaseRoute 属性
        else if (controllerApiDescriptionSettings?.LowercaseRoute != null)
        {
            var canParse = bool.TryParse(controllerApiDescriptionSettings.LowercaseRoute.ToString(), out var value);
            isLowercaseRoute = !canParse || value;
        }
        // 取全局配置
        else
            isLowercaseRoute = true;

        return isLowercaseRoute;
    }

    /// <summary>
    /// 解析名称中的版本号
    /// </summary>
    /// <param name="name">名称</param>
    /// <returns>名称和版本号</returns>
    private (string name, string version) ResolveNameVersion(string name)
    {
        if (!_nameVersionRegex.IsMatch(name))
            return (name, default);

        var version = _nameVersionRegex.Match(name).Groups["version"].Value.Replace("_", ".");
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

            var templates = Regex.Matches(part, commonTemplatePattern).Select(t => t.Value);
            foreach (var temp in templates)
            {
                // 处理带路由约束的路由参数模板 https://gitee.com/zuohuaijun/Admin.NET/issues/I736XJ
                var t = !temp.Contains("?", StringComparison.CurrentCulture)
                    ? (!temp.Contains(":", StringComparison.CurrentCulture)
                        ? temp
                        : temp[..temp.IndexOf(":", StringComparison.Ordinal)] + "}")
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