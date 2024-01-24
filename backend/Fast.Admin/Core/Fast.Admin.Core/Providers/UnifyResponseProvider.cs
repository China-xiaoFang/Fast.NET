// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
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

using System.Text;
using Fast.UnifyResult;
using Fast.UnifyResult.Metadatas;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Fast.Admin.Core.Providers;

/// <summary>
/// <see cref="UnifyResponseProvider"/> 规范化响应数据提供器
/// </summary>
public class UnifyResponseProvider : IUnifyResponseProvider
{
    private readonly ILogger<IUnifyResponseProvider> _logger;

    public UnifyResponseProvider(ILogger<IUnifyResponseProvider> logger)
    {
        _logger = logger;
    }

    /// <summary>响应异常处理</summary>
    /// <param name="context"><see cref="T:Microsoft.AspNetCore.Mvc.Filters.ExceptionContext" /></param>
    /// <param name="metadata"><see cref="T:Fast.UnifyResult.Metadatas.ExceptionMetadata" /> 异常元数据</param>
    /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" /> 请求上下文</param>
    /// <returns></returns>
    public async Task<(int statusCode, string message)> ResponseExceptionAsync(ExceptionContext context,
        ExceptionMetadata metadata, HttpContext httpContext)
    {
        // 默认 400 错误
        var statusCode = StatusCodes.Status400BadRequest;

        var message = context.Exception.Message;

        switch (context.Exception)
        {
            // 友好异常处理
            case UserFriendlyException userFriendlyException:
            {
                if (userFriendlyException.OriginErrorCode != null)
                {
                    statusCode = userFriendlyException.OriginErrorCode.ToString().ParseToInt();
                }
                else if (userFriendlyException.ErrorCode != null)
                {
                    statusCode = userFriendlyException.ErrorCode.ToString().ParseToInt();
                }
                else
                {
                    statusCode = userFriendlyException.StatusCode;
                }

                // 处理可能为0的情况
                statusCode = statusCode == 0 ? StatusCodes.Status400BadRequest : statusCode;
                break;
            }
            // SqlSugar 并发处理
            case VersionExceptions versionExceptions:
                statusCode = StatusCodes.Status400BadRequest;
                message = "数据已更改，请刷新后重试！\r\n" + versionExceptions.Message;
                break;
        }

        // 记录 Http 请求错误数据
        RecordHttpRequestData(context.HttpContext, context.Exception);

        return await Task.FromResult((statusCode, message));
    }

    /// <summary>
    /// 响应数据处理
    /// <remarks>只有响应成功且为正常返回才会调用</remarks>
    /// </summary>
    /// <param name="timestamp"><see cref="T:System.Int64" /> 响应时间戳</param>
    /// <param name="data"><see cref="T:System.Object" /> 数据</param>
    /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" /> 请求上下文</param>
    /// <returns></returns>
    public async Task<object> ResponseDataAsync(long timestamp, object data, HttpContext httpContext)
    {
        // 这里可以做返回数据加密处理
        return await Task.FromResult(data);
    }

    /// <summary>
    /// 记录 Http 请求错误数据
    /// </summary>
    /// <param name="httpContent"></param>
    /// <param name="exception"></param>
    /// <param name="message"></param>
    private void RecordHttpRequestData(HttpContext httpContent, Exception exception = null, string message = null)
    {
        try
        {
            // 不管是 400 异常还是 500 异常，都记录一下请求数据
            switch (httpContent.Request.Method)
            {
                case "GET":
                case "DELETE":
                    if (httpContent.Request.Path.HasValue)
                    {
                        var queryParam = httpContent.Request.QueryString;
                        _logger.LogError(exception,
                            $"请求异常时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n请求URL：{httpContent.Request.Scheme}://{httpContent.Request.Host}{httpContent.Request.Path}\r\n请求Method：{httpContent.Request.Method}\r\n请求参数：{queryParam}\r\n异常信息：{exception?.Message ?? message}");
                    }

                    break;
                case "POST":
                case "PUT":
                    // 允许读取请求的Body
                    httpContent.Request.EnableBuffering();

                    // 重置指针
                    httpContent.Request.Body.Position = 0;

                    if (httpContent.Request.Path.HasValue)
                    {
                        // 请求参数
                        using var streamReader = new StreamReader(httpContent.Request.Body, Encoding.UTF8);
                        var requestParam = streamReader.ReadToEnd();

                        _logger.LogError(exception,
                            $"请求异常时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n请求URL：{httpContent.Request.Scheme}://{httpContent.Request.Host}{httpContent.Request.Path}\r\n请求Method：{httpContent.Request.Method}\r\n请求参数：{requestParam}\r\n异常信息：{exception?.Message ?? message}");

                        // 重置指针
                        httpContent.Request.Body.Position = 0;
                    }

                    break;
            }
        }
        catch
        {
            // ignored
        }
    }
}