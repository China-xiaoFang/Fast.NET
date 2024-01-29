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

using Fast.CorsAccessor.Internal;
using Fast.CorsAccessor.Options;
using Fast.IaaS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.CorsAccessor.Injections;

/// <summary>
/// <see cref="CorsAccessorInjection"/> 跨域服务注册
/// <remarks>默认解决跨域请求问题，默认允许所有来源</remarks>
/// </summary>
public class CorsAccessorInjection : IHostInjection
{
    /// <summary>
    /// 排序
    /// <remarks>
    /// <para>顺序越大，越优先注册</para>
    /// <para>建议最大不超过9999</para>
    /// </remarks>
    /// </summary>
    public int Order => 69966;

    /// <summary>
    /// 配置
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((hostContext, services) =>
        {
            Debugging.Info("Registering cors accessor......");

            // 配置验证
            services.AddConfigurableOptions<CorsAccessorSettingsOptions>("CorsAccessorSettings");

            // 获取跨域配置选项
            var corsAccessorSettings = hostContext.Configuration.GetSection("CorsAccessorSettings")
                .Get<CorsAccessorSettingsOptions>().LoadPostConfigure();

            // 添加跨域服务
            services.AddCors(options =>
            {
                // 添加策略跨域
                options.AddPolicy(corsAccessorSettings.PolicyName, configurePolicy =>
                {
                    // 设置跨域策略
                    Penetrates.SetCorsPolicy(configurePolicy, corsAccessorSettings);
                });
            });
        });
    }
}