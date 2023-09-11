using System;
using System.IO.Compression;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Cache
{
    /// <summary>
    /// 缓存扩展类
    /// </summary>
    // ReSharper disable once PartialTypeWithSinglePart
    public static partial class Extension
    {
        /// <summary>
        /// 添加缓存服务（单例）
        /// </summary>
        /// <param name="services"></param>
        /// <param name="connectionString"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void AddCache(this IServiceCollection services, string connectionString)
        {
            // 判断连接字符串是否为空
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "AddCache is connectionString empty.");
            }

            // 添加为单例服务
            services.AddSingleton<ICache>(provider =>
            {
                // 放入连接字符串
                var cache = new Cache(connectionString);

                return cache;
            });
        }
    }
}