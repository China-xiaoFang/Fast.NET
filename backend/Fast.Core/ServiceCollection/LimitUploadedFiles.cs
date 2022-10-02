using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace Fast.Core.ServiceCollection;

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
    /// <param name="isRun"></param>
    public static void AddLimitUploadedFile(this IServiceCollection service, bool isRun = true)
    {
        if (!isRun)
            return;
        var sysSetOptions = GlobalContext.SystemSettings;
        if (sysSetOptions == null || sysSetOptions.MaxRequestBodySize.IsNullOrZero())
            return;
        var maxRequestBodySize = sysSetOptions.MaxRequestBodySize;
        service.Configure<KestrelServerOptions>(options => { options.Limits.MaxRequestBodySize = maxRequestBodySize; });
        service.Configure<IISServerOptions>(options => { options.MaxRequestBodySize = maxRequestBodySize; });
        service.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = maxRequestBodySize; });
    }
}