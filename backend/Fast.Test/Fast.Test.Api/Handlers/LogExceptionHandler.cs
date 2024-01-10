using Fast.Logging;
using Fast.UnifyResult;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Fast.Test.Api.Handlers
{
    public class LogExceptionHandler :IGlobalExceptionHandler
    {
        private readonly ILogger<IGlobalExceptionHandler> _logger;

        public LogExceptionHandler(ILogger<IGlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 异常拦截
        /// </summary>
        /// <param name="context"><see cref="ExceptionContext"/></param>
        /// <param name="isUserFriendlyException"><see cref="bool"/> 是否友好异常</param>
        /// <param name="isValidationException"><see cref="bool"/> 是否验证异常</param>
        /// <returns></returns>
        public async Task OnExceptionAsync(ExceptionContext context, bool isUserFriendlyException, bool isValidationException)
        {
            // 判断Code是否为400，400为业务错误，非系统异常不记录
            if (!isValidationException)
            {
                // 写日志文件
                Log.Error(context.Exception.ToString());
                _logger.LogCritical(context.Exception.Message, context.Exception);
            }

            // 记录 HTTP 请求数据
            //RestfulResultProvider.RecordHttpRequestData(context.HttpContext, context.Exception);

            await Task.CompletedTask;
        }
    }
}
