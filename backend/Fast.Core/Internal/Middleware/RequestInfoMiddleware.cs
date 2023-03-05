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
        // 响应报文头增加环境变量
        context.Response.Headers[ClaimConst.EnvironmentName] = GlobalContext.SystemSettingsOptions.Environment.ParseToString();

        // 响应报文头增加接口版本
        context.Response.Headers[ClaimConst.ApiVersion] = GlobalContext.SystemSettingsOptions.ApiVersion;

        // 响应报文头增加接口操作类型
        context.Response.Headers[ClaimConst.ApiActionName] = (context.Request.Method switch
        {
            "GET" => context.GetMetadata<HttpGetAttribute>()?.Action,
            "POST" => context.GetMetadata<HttpPostAttribute>()?.Action,
            "PUT" => context.GetMetadata<HttpPutAttribute>()?.Action,
            "DELETE" => context.GetMetadata<HttpDeleteAttribute>()?.Action,
            _ => null
        })?.ToString();

        // 抛给下一个过滤器
        await _next(context);
    }
}