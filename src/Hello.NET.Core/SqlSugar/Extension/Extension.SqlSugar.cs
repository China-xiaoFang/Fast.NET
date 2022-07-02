namespace Hello.NET.Core.SqlSugar.Extension;

/// <summary>
/// SqlSugar 拓展类
/// </summary>
public static class SqlSugarServiceCollectionExtensions
{
    /// <summary>
    /// 添加 SqlSugar 拓展
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <param name="buildAction"></param>
    /// <returns></returns>
    public static IServiceCollection AddSqlSugar(this IServiceCollection services, ConnectionConfig config,
        Action<SqlSugarClient> buildAction = default)
    {
        return services.AddSqlSugar(new List<ConnectionConfig> {config}, buildAction);
    }

    /// <summary>
    /// 添加 SqlSugar 拓展
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configs"></param>
    /// <param name="buildAction"></param>
    /// <returns></returns>
    public static IServiceCollection AddSqlSugar(this IServiceCollection services, List<ConnectionConfig> configs,
        Action<SqlSugarClient> buildAction = default)
    {
        // 注册 SqlSugar 客户端
        services.AddScoped<ISqlSugarClient>(u =>
        {
            var sqlSugarClient = new SqlSugarClient(configs.ToList());
            buildAction?.Invoke(sqlSugarClient);

            return sqlSugarClient;
        });

        // 添加多库事务
        services.AddScoped<ITenant>(u =>
        {
            var tenant = new SqlSugarClient(configs.ToList());
            buildAction?.Invoke(tenant);
            return tenant;
        });

        // 注册非泛型仓储
        services.AddScoped<ISqlSugarRepository, SqlSugarRepository>();

        // 注册 SqlSugar 仓储
        services.AddScoped(typeof(ISqlSugarRepository<>), typeof(SqlSugarRepository<>));

        return services;
    }
}