using System.Reflection;
using Fast.DynamicApplication.Internal;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Fast.DynamicApplication.Providers;

/// <summary>
/// 动态API引用特性提供器
/// </summary>
public sealed class DynamicApplicationFeatureProvider : ControllerFeatureProvider
{
    /// <summary>
    /// 扫描控制器
    /// </summary>
    /// <param name="typeInfo">类型</param>
    /// <returns>bool</returns>
    protected override bool IsController(TypeInfo typeInfo)
    {
        return Penetrates.IsApiController(typeInfo);
    }
}