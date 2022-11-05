using System.IO.Compression;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Core.ServiceCollection;

/// <summary>
/// 配置Gzip Brotli 压缩
/// </summary>
public static class GzipBrotliCompression
{
    /// <summary>
    /// 添加Gzip Brotli 压缩
    /// </summary>
    /// <param name="service"></param>
    /// <param name="isRun"></param>
    public static void AddGzipBrotliCompression(this IServiceCollection service, bool isRun = true)
    {
        if (!isRun)
            return;
        service.Configure<BrotliCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
        service.Configure<GzipCompressionProviderOptions>(options => { options.Level = CompressionLevel.Optimal; });
        service.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
            {
                "text/html; charset=utf-8", "application/xhtml+xml", "application/atom+xml", "image/svg+xml"
            });
        });
    }
}