﻿// Apache开源许可证
//
// 版权所有 © 2018-Now 小方
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using Yitter.IdGenerator;

// ReSharper disable once CheckNamespace
namespace Fast.SqlSugar;

/// <summary>
/// <see cref="IServiceCollection"/> 动态Api 拓展类
/// </summary>
[SuppressSniffer]
public static class IServiceCollectionExtension
{
    /// <summary>
    /// 注册SqlSugar服务
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="configuration"><see cref="IConfiguration"/></param>
    /// <param name="connectionSection">
    /// <see cref="string"/>
    /// <para>Json配置文件节点的Key</para>
    /// <remarks>默认值：ConnectionSettings</remarks>
    /// </param>
    /// <param name="snowflakeSetion">
    /// <see cref="string"/>
    /// <para>Json配置文件节点的Key</para>
    /// <remarks>默认值：SnowflakeSettings</remarks>
    /// </param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddSqlSugar(this IServiceCollection services, IConfiguration configuration,
        string connectionSection = "ConnectionSettings", string snowflakeSetion = "SnowflakeSettings")
    {
        Debugging.Info("Registering sql sugar......");

        // 配置验证
        services.AddConfigurableOptions<ConnectionSettingsOptions>(connectionSection);
        services.AddConfigurableOptions<SnowflakeSettingsOptions>(snowflakeSetion);

        // 获取配置选项
        SqlSugarContext.ConnectionSettings = configuration.GetSection(connectionSection).Get<ConnectionSettingsOptions>();
        SqlSugarContext.SnowflakeSettings = configuration.GetSection(snowflakeSetion)
            .Get<SnowflakeSettingsOptions>().LoadPostConfigure();

        // Add Snowflakes Id.
        // 设置雪花Id的workerId，确保每个实例workerId都应不同
        YitIdHelper.SetIdGenerator(new IdGeneratorOptions {WorkerId = SqlSugarContext.SnowflakeSettings.WorkerId!.Value});

        SqlSugarContext.DefaultConnectionConfig = SqlSugarContext.GetConnectionConfig(SqlSugarContext.ConnectionSettings);
        SqlSugarContext.DefaultConnectionConfigNoAop = SqlSugarContext.GetConnectionConfig(SqlSugarContext.ConnectionSettings);

        // 查找Sugar实体处理程序提供者
        var iSqlSugarEntityHandlerType =
            IaaSContext.EffectiveTypes.FirstOrDefault(f => typeof(ISqlSugarEntityHandler).IsAssignableFrom(f) && !f.IsInterface);
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

        return services;
    }
}