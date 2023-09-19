using System.Reflection;
using Fast.Core.DynamicApiController.Internal;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Fast.Core.DynamicApiController.Providers;

/// <summary>
/// 动态接口控制器特性提供器
/// </summary>
public sealed class DynamicApiControllerFeatureProvider : ControllerFeatureProvider
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