using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Fast.Core.Extensions;

/// <summary>
/// <see cref="IApplicationBuilder"/> 拓展类
/// </summary>
public static class IApplicationBuilderExtension
{
    /// <summary>
    /// 启用 Body 重复读功能
    /// </summary>
    /// <remarks>须在 app.UseRouting() 之前注册</remarks>
    /// <param name="app"><see cref="IApplicationBuilder"/></param>
    /// <returns><see cref="IApplicationBuilder"/></returns>
    public static IApplicationBuilder EnableBuffering(this IApplicationBuilder app)
    {
        return app.Use(next => context =>
        {
            context.Request.EnableBuffering();
            return next(context);
        });
    }

    /// <summary>
    /// 添加应用中间件
    /// </summary>
    /// <param name="app"><see cref="IApplicationBuilder"/>应用构建器</param>
    /// <param name="configure">应用配置</param>
    /// <returns><see cref="IApplicationBuilder"/>应用构建器</returns>
    internal static IApplicationBuilder UseApp(this IApplicationBuilder app, Action<IApplicationBuilder> configure = null)
    {
        // 调用自定义服务
        configure?.Invoke(app);
        return app;
    }
}