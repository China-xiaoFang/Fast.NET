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

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlSugar;

namespace Fast.SqlSugar;

/// <summary>
/// SqlSugar 仓储实现
/// </summary>
internal sealed partial class SqlSugarRepository<TEntity> : SqlSugarClient, ISqlSugarRepository<TEntity>
    where TEntity : class, new()
{
    /// <summary>
    /// 服务提供器
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// SqlSugar 仓储实现
    /// </summary>
    /// <param name="hostEnvironment">当前应用的宿主环境</param>
    /// <param name="serviceProvider">用于解析服务的服务提供器；为 <see langword="null"/> 时使用当前请求或根服务提供器</param>
    public SqlSugarRepository(IHostEnvironment hostEnvironment, IServiceProvider serviceProvider) : base(
        SqlSugarContext.GetConnectionConfig(SqlSugarContext.ConnectionSettings))
    {
        _serviceProvider = serviceProvider;

        var entityType = typeof(TEntity);

        // 是否支持逻辑删除
        SupportsLogicDelete = typeof(IDeletedEntity).IsAssignableFrom(entityType);
        // 是否支持行版本控制
        SupportsRowVersion = typeof(IUpdateVersion).IsAssignableFrom(entityType);

        // 是否分表，判断是否存在 SplitTableAttribute 特性
        IsSplitTable = entityType.GetCustomAttribute<SplitTableAttribute>(true) != null;

        // 获取当前实体类头部的 SugarDbTypeAttribute 特性
        var sugarDbTypeAttribute = entityType.GetCustomAttribute<SugarDbTypeAttribute>(true);

        // 根据 TEntity 加载对应的数据库连接字符串
        var sqlSugarEntityHandler = _serviceProvider.GetService<ISqlSugarEntityHandler>();

        // 获取新的连接字符串
        // DI 构造函数不能声明为 async；这里显式解包异步结果，避免 .Result 将真实异常包装成 AggregateException
        var connectionSettings = sqlSugarEntityHandler?.GetConnectionSettings<TEntity>(Context, sugarDbTypeAttribute, entityType)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        // 数据库信息
        DatabaseInfo = connectionSettings ?? SqlSugarContext.ConnectionSettings;

        // 重新初始化 Context
        InitContext(SqlSugarContext.GetConnectionConfig(DatabaseInfo));

        Context.Ado.CommandTimeOut = DatabaseInfo.CommandTimeOut!.Value;

        SugarEntityFilter.LoadSugarAop(hostEnvironment.IsDevelopment(), Context, DatabaseInfo.SugarSqlExecMaxSeconds!.Value,
            DatabaseInfo.DiffLog!.Value, DatabaseInfo.DisableAop!.Value, sqlSugarEntityHandler);

        SugarEntityFilter.LoadSugarFilter(Context, sqlSugarEntityHandler);
    }

    /// <inheritdoc />
    public bool SupportsLogicDelete { get; }

    /// <inheritdoc />
    public bool SupportsRowVersion { get; }

    /// <inheritdoc />
    public bool IsSplitTable { get; }

    /// <summary>
    /// 实体集合
    /// </summary>
    public ISugarQueryable<TEntity> Entities => Queryable<TEntity>();

    /// <inheritdoc />
    public ConnectionSettingsOptions DatabaseInfo { get; set; }

    /// <inheritdoc />
    public ISqlSugarRepository<TChangeEntity> Change<TChangeEntity>() where TChangeEntity : class, new()
    {
        return _serviceProvider.GetService(typeof(ISqlSugarRepository<TChangeEntity>)) as ISqlSugarRepository<TChangeEntity>;
    }
}
