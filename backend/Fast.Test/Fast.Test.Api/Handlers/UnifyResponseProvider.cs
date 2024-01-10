using System.Text;
using Fast.IaaS;
using Fast.Logging;
using Fast.Serialization.Extensions;
using Fast.UnifyResult;
using Fast.UnifyResult.Metadatas;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fast.Test.Api.Handlers
{
   
public class UnifyResponseProvider : IUnifyResponseProvider
{
    private readonly ILogger<IUnifyResponseProvider> _logger;

    public UnifyResponseProvider(ILogger<IUnifyResponseProvider> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// JSON 类型处理白名单
    /// </summary>
    private static List<Type> JsonTypeWhiteList =>
        new List<Type>
        {
            typeof(int),
            typeof(decimal),
            typeof(double),
            typeof(long),
            typeof(bool),
            typeof(float),
            typeof(int?),
            typeof(decimal?),
            typeof(double?),
            typeof(long?),
            typeof(bool?),
            typeof(float?),
            typeof(string),
        };

    /// <summary>响应异常处理</summary>
    /// <param name="context"><see cref="T:Microsoft.AspNetCore.Mvc.Filters.ExceptionContext" /></param>
    /// <param name="metadata"><see cref="T:Gejia.UnifyResult.Metadatas.ExceptionMetadata" /> 异常元数据</param>
    /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" /> 请求上下文</param>
    /// <returns></returns>
    public Task<(int statusCode, string message)> ResponseExceptionAsync(ExceptionContext context, ExceptionMetadata metadata,
        HttpContext httpContext)
    {
        // 默认 500 错误
        var statusCode = StatusCodes.Status500InternalServerError;

        var message = context.Exception.Message;

        if (context.Exception is UserFriendlyException appFriendlyException)
        {
            if (appFriendlyException.OriginErrorCode != null)
            {
                statusCode = appFriendlyException.OriginErrorCode.ToString().ParseToInt();
            }
            else if (appFriendlyException.ErrorCode != null)
            {
                statusCode = appFriendlyException.ErrorCode.ToString().ParseToInt();
            }
            else
            {
                statusCode = appFriendlyException.StatusCode;
            }

            // 处理可能为0的情况
            statusCode = statusCode == 0 ? StatusCodes.Status500InternalServerError : statusCode;
        }

        context.HttpContext.Response.StatusCode = statusCode;

        // 记录 HTTP 请求数据
        RecordHttpRequestData(context.HttpContext, context.Exception);

        return Task.FromResult((statusCode, message));
    }

    /// <summary>
    /// 响应数据处理
    /// <remarks>只有响应成功且为正常返回才会调用</remarks>
    /// </summary>
    /// <param name="timestamp"><see cref="T:System.Int64" /> 响应时间戳</param>
    /// <param name="data"><see cref="T:System.Object" /> 数据</param>
    /// <param name="httpContext"><see cref="T:Microsoft.AspNetCore.Http.HttpContext" /> 请求上下文</param>
    /// <returns></returns>
    public Task<object> ResponseDataAsync(long timestamp, object data, HttpContext httpContext)
    {
        var resultData = data;

        // 判断前置中间件中是否写入了加密的标识
        if (httpContext.Response.Headers["AES-Encrypt"] == "true")
        {
            var dataType = data.GetType();

            string dataJsonStr;

            // 处理类型为基类的错误
            if (JsonTypeWhiteList.Contains(dataType))
            {
                dataJsonStr = data.ToString();
            }
            else
            {
                dataJsonStr = data.ToJsonString();
            }

            resultData = CryptoUtil.AESEncrypt(dataJsonStr, $"Gejia.WMS.EnResult.{timestamp}", $"GIV{timestamp}");

            // 写日志文件
            Log.Debug($"HTTP Request AES 加密详情：\r\n\t源数据：{dataJsonStr}\r\n\tAES加密：{resultData}");
        }

        return Task.FromResult(resultData);
    }

    private static void RecordHttpRequestData(HttpContext httpContent, Exception exception = null, string message = null)
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
                        Log.Error(
                            $"请求异常时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n请求URL：{httpContent.Request.Scheme}://{httpContent.Request.Host}{httpContent.Request.Path}\r\n请求Method：{httpContent.Request.Method}\r\n请求参数：{queryParam}\r\n异常信息：{exception?.Message ?? message}",
                            exception);
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

                        Log.Error(
                            $"请求异常时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n请求URL：{httpContent.Request.Scheme}://{httpContent.Request.Host}{httpContent.Request.Path}\r\n请求Method：{httpContent.Request.Method}\r\n请求参数：{requestParam}异常信息：{exception?.Message ?? message}",
                            exception);

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
}
