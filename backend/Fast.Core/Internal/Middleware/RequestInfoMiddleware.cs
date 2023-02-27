using Fast.Core.Const;
using Microsoft.AspNetCore.Http;

namespace Fast.Core.Internal.Middleware;

/// <summary>
/// 请求信息中间件
/// </summary>
public class RequestInfoMiddleware
{
    private readonly RequestDelegate _next;

    public RequestInfoMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 这里可以处理一些没有任何逻辑的业务

        // 响应报文头增加环境变量
        context.Response.Headers[ClaimConst.EnvironmentCode] = $"{GlobalContext.SystemSettingsOptions.Environment.ParseToInt()}";
        context.Response.Headers[ClaimConst.EnvironmentName] = GlobalContext.SystemSettingsOptions.Environment.ParseToString();

        // 响应报文头增加接口版本
        context.Response.Headers[ClaimConst.ApiVersion] = GlobalContext.SystemSettingsOptions.ApiVersion;

        // 抛给下一个过滤器
        await _next(context);
    }
}