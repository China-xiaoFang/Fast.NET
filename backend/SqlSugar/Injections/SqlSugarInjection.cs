﻿// Apache开源许可证
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

using Fast.IaaS;
using Fast.SqlSugar.Filters;
using Fast.SqlSugar.Handlers;
using Fast.SqlSugar.Options;
using Fast.SqlSugar.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.SqlSugar.Injections;

/// <summary>
/// <see cref="SqlSugarInjection"/> SqlSugar服务注入
/// </summary>
public class SqlSugarInjection : IHostInjection
{
    /// <summary>
    /// 排序
    /// <remarks>
    /// <para>顺序越大，越优先注册</para>
    /// <para>建议最大不超过9999</para>
    /// </remarks>
    /// </summary>
    public int Order => 69888;

    /// <summary>
    /// 配置
    /// </summary>
    /// <param name="builder"></param>
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices((hostContext, services) =>
        {
            Debugging.Info("Registering sql sugar......");

            // 配置验证
            services.AddConfigurableOptions<ConnectionSettingsOptions>("ConnectionSettings");
            services.AddConfigurableOptions<SnowflakeSettingsOptions>("SnowflakeSettings");

            // 获取配置选项
            SqlSugarContext.ConnectionSettings =
                hostContext.Configuration.GetSection("ConnectionSettings").Get<ConnectionSettingsOptions>();
            SqlSugarContext.SnowflakeSettings = hostContext.Configuration.GetSection("SnowflakeSettings")
                .Get<SnowflakeSettingsOptions>().LoadPostConfigure();

            // Add Snowflakes Id.
            // 设置雪花Id的workerId，确保每个实例workerId都应不同
            YitIdHelper.SetIdGenerator(new IdGeneratorOptions {WorkerId = SqlSugarContext.SnowflakeSettings.WorkerId!.Value});

            SqlSugarContext.DefaultConnectionConfig = SqlSugarContext.GetConnectionConfig(SqlSugarContext.ConnectionSettings);
            SqlSugarContext.DefaultConnectionConfigNoAop =
                SqlSugarContext.GetConnectionConfig(SqlSugarContext.ConnectionSettings);

            // 查找Sugar实体处理程序提供者
            var iSqlSugarEntityHandlerType =
                IaaSContext.EffectiveTypes.FirstOrDefault(f =>
                    typeof(ISqlSugarEntityHandler).IsAssignableFrom(f) && !f.IsInterface);
            if (iSqlSugarEntityHandlerType != null)
            {
                // 注册Sugar实体处理程序
                services.AddScoped(typeof(ISqlSugarEntityHandler), iSqlSugarEntityHandlerType);
            }

            // 注册 SqlSugarClient，这里注册一遍是因为防止直接使用 ISqlSugarClient
            services.AddScoped<ISqlSugarClient>(serviceProvider =>
            {
                // 获取 Sugar实体处理 接口的实现类
                var sqlSugarEntityHandler = serviceProvider.GetRequiredService<ISqlSugarEntityHandler>();

                var sqlSugarClient = new SqlSugarClient(SqlSugarContext.DefaultConnectionConfig);

                // 执行超时时间
                sqlSugarClient.Ado.CommandTimeOut = SqlSugarContext.ConnectionSettings.CommandTimeOut;

                // 判断是否禁用 Aop
                if (!SqlSugarContext.ConnectionSettings.DisableAop)
                {
                    // Aop
                    SugarEntityFilter.LoadSugarAop(sqlSugarClient, SqlSugarContext.ConnectionSettings.SugarSqlExecMaxSeconds,
                        SqlSugarContext.ConnectionSettings.DiffLog, sqlSugarEntityHandler);
                }

                // 过滤器
                SugarEntityFilter.LoadSugarFilter(sqlSugarClient, sqlSugarEntityHandler);

                return sqlSugarClient;
            });

            // 注册泛型仓储
            services.AddScoped(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));
        });
    }
}