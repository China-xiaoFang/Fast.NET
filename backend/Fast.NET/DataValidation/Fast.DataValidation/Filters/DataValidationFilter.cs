using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Fast.DataValidation.Filters;

/// <summary>
/// 数据验证拦截器
/// </summary>
public sealed class DataValidationFilter : IAsyncActionFilter, IOrderedFilter
{
    /// <summary>
    /// Api 行为配置选项
    /// </summary>
    private readonly ApiBehaviorOptions _apiBehaviorOptions;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options"></param>
    public DataValidationFilter(IOptions<ApiBehaviorOptions> options)
    {
        _apiBehaviorOptions = options.Value;
    }

    /// <summary>
    /// 过滤器排序
    /// </summary>
    private const int FilterOrder = -1000;

    /// <summary>
    /// 排序属性
    /// </summary>
    public int Order => FilterOrder;

    /// <summary>
    /// 是否是可重复使用的
    /// </summary>
    public static bool IsReusable => true;

    /// <summary>
    /// 拦截请求
    /// </summary>
    /// <param name="context">动作方法上下文</param>
    /// <param name="next">中间件委托</param>
    /// <returns></returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // 排除 WebSocket 请求处理
        if (context.HttpContext.WebSockets.IsWebSocketRequest || context.HttpContext.Request.Path == "/ws")
        {
            await next();
            return;
        }

        // 获取控制器/方法信息
        var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

        // 跳过验证类型
        var method = actionDescriptor?.MethodInfo;

        // 获取验证状态
        var modelState = context.ModelState;

        // 如果参数数量为 0或验证成功或已经设置了结果，则跳过验证
        if (actionDescriptor?.Parameters?.Count == 0 || modelState.IsValid ||
            method?.DeclaringType?.Assembly?.GetName()?.Name?.StartsWith("Microsoft.AspNetCore.OData") == true ||
            context.Result != null)
        {
            // 处理执行后验证信息
            var resultContext = await next();

            // 如果异常不为空
            if (resultContext.Exception != null)
            {
                // 返回 JsonResult
                resultContext.Result =
                    new JsonResult(resultContext.Exception.Message) {StatusCode = StatusCodes.Status400BadRequest};
                return;
            }
        }

        // 返回 JsonResult
        context.Result = new JsonResult(modelState) {StatusCode = StatusCodes.Status400BadRequest};
    }
}