using Fast.Core.App;
using Microsoft.AspNetCore.Builder;

namespace Fast.Core.Extensions;

/// <summary>
/// WebApplicationBuilder 扩展
/// </summary>
// ReSharper disable once PartialTypeWithSinglePart
public static partial class Extensions
{
    public static WebApplicationBuilder FastInitialize(this WebApplicationBuilder builder)
    {
        InternalApp.WebHostEnvironment = builder.Environment;

        // 初始化配置
        InternalApp.ConfigureApplication(builder.WebHost, builder.Host);

        return builder;
    }
}