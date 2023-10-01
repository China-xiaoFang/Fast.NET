using Fast.UnifyResult.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace Fast.UnifyResult.Extensions;

/// <summary>
/// <see cref="IApplicationBuilder"/> 拓展类
/// </summary>
public static class UnifyResultIApplicationBuilderExtension
{
    /// <summary>
    /// 添加状态码拦截中间件
    /// </summary>
    /// <param name="builder"><see cref="IApplicationBuilder"/></param>
    /// <returns><see cref="IApplicationBuilder"/></returns>
    public static IApplicationBuilder UseUnifyResultStatusCodes(this IApplicationBuilder builder)
    {
        // 注册中间件
        builder.UseMiddleware<UnifyResultStatusCodesMiddleware>();

        return builder;
    }
}