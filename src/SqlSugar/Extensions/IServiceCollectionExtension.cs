// ------------------------------------------------------------------------
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

namespace Fast.SqlSugar;

/// <summary>
/// 为 <see cref="IServiceCollection"/> 提供动态 API 扩展方法。
/// </summary>
[SuppressSniffer]
public static class IServiceCollectionExtension
{
    /// <summary>
    /// 添加雪花Id。
    /// </summary>
    /// <param name="services">要添加服务的服务集合。</param>
    /// <param name="configuration">用于读取模块设置的配置。</param>
    /// <param name="section">JSON 配置文件节点的 Key 默认值：SnowflakeSettings。</param>
    /// <returns>返回 <paramref name="services"/>，便于链式调用。</returns>
    public static IServiceCollection AddSnowflake(this IServiceCollection services, IConfiguration configuration,
        string section = "SnowflakeSettings")
    {
        Debugging.Info("Registering snowflake......");

        services.AddConfigurableOptions<SnowflakeSettingsOptions>(section);

        SqlSugarContext.SnowflakeSettings = configuration.GetSection(section)
            .Get<SnowflakeSettingsOptions>()
            .LoadPostConfigure();

        // 每个实例必须使用不同的雪花算法 WorkerId，避免生成重复Id。
        YitIdHelper.SetIdGenerator(new IdGeneratorOptions {WorkerId = SqlSugarContext.SnowflakeSettings.WorkerId ?? 1});

        return services;
    }

    /// <summary>
    /// 添加雪花Id。
    /// </summary>
    /// <param name="services">要添加服务的服务集合。</param>
    /// <param name="configuration">用于读取模块设置的配置。</param>
    /// <param name="optionAction">雪花Id配置操作。</param>
    /// <returns>返回 <paramref name="services"/>，便于链式调用。</returns>
    public static IServiceCollection AddSnowflake(this IServiceCollection services, IConfiguration configuration,
        Action<SnowflakeSettingsOptions> optionAction)
    {
        Debugging.Info("Registering snowflake......");

        services.Configure(optionAction);

        var snowflakeSettings = new SnowflakeSettingsOptions();
        optionAction.Invoke(snowflakeSettings);

        // 每个实例必须使用不同的雪花算法 WorkerId，避免生成重复Id。
        YitIdHelper.SetIdGenerator(new IdGeneratorOptions {WorkerId = SqlSugarContext.SnowflakeSettings.WorkerId ?? 1});

        return services;
    }

    /// <summary>
    /// 注册 SqlSugar 服务。
    /// </summary>
    /// <param name="services">要添加服务的服务集合。</param>
    /// <param name="configuration">用于读取模块设置的配置。</param>
    /// <param name="hostEnvironment">当前应用的宿主环境。</param>
    /// <param name="connectionSection">JSON 配置文件节点的 Key 默认值：ConnectionSettings。</param>
    /// <returns>返回 <paramref name="services"/>，便于链式调用。</returns>
    public static IServiceCollection AddSqlSugar(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment hostEnvironment, string connectionSection = "ConnectionSettings")
    {
        Debugging.Info("Registering sql sugar......");

        services.AddConfigurableOptions<ConnectionSettingsOptions>(connectionSection);

        var connectionSettings = configuration.GetSection(connectionSection)
            .Get<ConnectionSettingsOptions>();

        SqlSugarContext.ConnectionSettings = connectionSettings;

        services.AddSqlSugar(hostEnvironment);

        return services;
    }

    /// <summary>
    /// 注册 SqlSugar 服务。
    /// </summary>
    /// <param name="services">要添加服务的服务集合。</param>
    /// <param name="configuration">用于读取模块设置的配置。</param>
    /// <param name="hostEnvironment">当前应用的宿主环境。</param>
    /// <param name="optionAction">数据库连接配置操作。</param>
    /// <returns>返回 <paramref name="services"/>，便于链式调用。</returns>
    public static IServiceCollection AddSqlSugar(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment hostEnvironment, Action<ConnectionSettingsOptions> optionAction)
    {
        Debugging.Info("Registering sql sugar......");

        services.Configure(optionAction);

        var connectionSettings = new ConnectionSettingsOptions();
        optionAction.Invoke(connectionSettings);

        SqlSugarContext.ConnectionSettings = connectionSettings;

        services.AddSqlSugar(hostEnvironment);

        return services;
    }

    /// <summary>
    /// 注册 SqlSugar 服务。
    /// </summary>
    /// <param name="services">服务注册集合。</param>
    /// <param name="hostEnvironment">当前应用的宿主环境。</param>
    private static void AddSqlSugar(this IServiceCollection services, IHostEnvironment hostEnvironment)
    {
        // 查找 Sugar 实体处理程序提供者
        var iSqlSugarEntityHandlerType =
            MAppContext.EffectiveTypes.FirstOrDefault(f => typeof(ISqlSugarEntityHandler).IsAssignableFrom(f) && !f.IsInterface);
        if (iSqlSugarEntityHandlerType != null)
        {
            // 注册 Sugar 实体处理程序
            services.AddScoped(typeof(ISqlSugarEntityHandler), iSqlSugarEntityHandlerType);
        }

        // 同时注册具体类型，支持调用方不经 ISqlSugarClient 接口直接解析客户端。
        services.AddScoped<ISqlSugarClient>(serviceProvider =>
        {
            // 获取 Sugar 实体处理 接口的实现类
            var sqlSugarEntityHandler = serviceProvider.GetRequiredService<ISqlSugarEntityHandler>();

            var sqlSugarClient = new SqlSugarClient(SqlSugarContext.GetConnectionConfig(SqlSugarContext.ConnectionSettings));

            sqlSugarClient.Ado.CommandTimeOut = SqlSugarContext.ConnectionSettings.CommandTimeOut!.Value;

            SugarEntityFilter.LoadSugarAop(hostEnvironment.IsDevelopment(), sqlSugarClient,
                SqlSugarContext.ConnectionSettings.SugarSqlExecMaxSeconds!.Value,
                SqlSugarContext.ConnectionSettings.DiffLog!.Value, SqlSugarContext.ConnectionSettings.DisableAop!.Value,
                sqlSugarEntityHandler);

            SugarEntityFilter.LoadSugarFilter(sqlSugarClient, sqlSugarEntityHandler);

            return sqlSugarClient;
        });

        // 注册泛型仓储
        services.AddScoped(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));
    }
}
