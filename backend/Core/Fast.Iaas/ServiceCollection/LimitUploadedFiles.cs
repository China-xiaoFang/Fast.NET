using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.ServiceCollection.ServiceCollection;

/// <summary>
/// 限制上传文件
/// </summary>
public static class LimitUploadedFiles
{
    /// <summary>
    /// 添加上传文件大小限制
    /// 需要配置JSON文件，如果没有则不会添加
    /// </summary>
    /// <param name="service"></param>
    /// <param name="maxRequestBodySize">最大请求Body Size</param>
    public static void AddLimitUploadedFile(this IServiceCollection service, long? maxRequestBodySize = null)
    {
        service.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = maxRequestBodySize; });
        service.Configure<IISServerOptions>(options => { options.MaxRequestBodySize = maxRequestBodySize; });
        if (maxRequestBodySize != null)
        {
            service.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = maxRequestBodySize.Value; });
        }
    }
}