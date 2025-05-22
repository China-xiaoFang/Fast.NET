﻿// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using Fast.Runtime;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    /// <param name="hostEnvironment"><see cref="IHostEnvironment"/></param>
    /// <param name="connectionSection">
    /// <see cref="string"/>
    /// <para>Json配置文件节点的Key</para>
    /// <remarks>默认值：ConnectionSettings</remarks>
    /// </param>
    /// <param name="snowflakeSection">
    /// <see cref="string"/>
    /// <para>Json配置文件节点的Key</para>
    /// <remarks>默认值：SnowflakeSettings</remarks>
    /// </param>
    /// <returns><see cref="IServiceCollection"/></returns>
    public static IServiceCollection AddSqlSugar(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment hostEnvironment, string connectionSection = "ConnectionSettings",
        string snowflakeSection = "SnowflakeSettings")
    {
        Debugging.Info("Registering sql sugar......");

        // 配置验证
        services.AddConfigurableOptions<ConnectionSettingsOptions>(connectionSection);
        services.AddConfigurableOptions<SnowflakeSettingsOptions>(snowflakeSection);

        // 获取配置选项
        SqlSugarContext.ConnectionSettings = configuration.GetSection(connectionSection).Get<ConnectionSettingsOptions>();
        SqlSugarContext.SnowflakeSettings = configuration.GetSection(snowflakeSection)
            .Get<SnowflakeSettingsOptions>().LoadPostConfigure();

        // Add Snowflakes Id.
        // 设置雪花Id的workerId，确保每个实例workerId都应不同
        YitIdHelper.SetIdGenerator(new IdGeneratorOptions {WorkerId = SqlSugarContext.SnowflakeSettings.WorkerId!.Value});

        SqlSugarContext.DefaultConnectionConfig = SqlSugarContext.GetConnectionConfig(SqlSugarContext.ConnectionSettings);
        SqlSugarContext.DefaultConnectionConfigNoAop = SqlSugarContext.GetConnectionConfig(SqlSugarContext.ConnectionSettings);

        // 查找Sugar实体处理程序提供者
        var iSqlSugarEntityHandlerType =
            MAppContext.EffectiveTypes.FirstOrDefault(f => typeof(ISqlSugarEntityHandler).IsAssignableFrom(f) && !f.IsInterface);
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
                SugarEntityFilter.LoadSugarAop(hostEnvironment.IsDevelopment(), sqlSugarClient,
                    SqlSugarContext.ConnectionSettings.SugarSqlExecMaxSeconds, SqlSugarContext.ConnectionSettings.DiffLog,
                    sqlSugarEntityHandler);
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