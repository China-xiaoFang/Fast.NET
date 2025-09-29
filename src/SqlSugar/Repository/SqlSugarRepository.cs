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

// ReSharper disable once CheckNamespace
namespace Fast.SqlSugar;

/// <summary>
/// <see cref="SqlSugarRepository{TEntity}"/> SqlSugar 仓储实现
/// </summary>
internal sealed partial class SqlSugarRepository<TEntity> : SqlSugarClient, ISqlSugarRepository<TEntity>
    where TEntity : class, new()
{
    /// <summary>
    /// 服务提供器
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// <see cref="SqlSugarRepository{TEntity}"/> SqlSugar 仓储实现
    /// </summary>
    /// <param name="hostEnvironment"></param>
    /// <param name="serviceProvider"></param>
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
        var connectionSettings = sqlSugarEntityHandler?.GetConnectionSettings<TEntity>(Context, sugarDbTypeAttribute, entityType)
            .Result;

        // 数据库信息
        DatabaseInfo = connectionSettings ?? SqlSugarContext.ConnectionSettings;

        // 重新初始化Context
        InitContext(SqlSugarContext.GetConnectionConfig(DatabaseInfo));

        // 执行超时时间
        Context.Ado.CommandTimeOut = DatabaseInfo.CommandTimeOut!.Value;

        // Aop
        SugarEntityFilter.LoadSugarAop(hostEnvironment.IsDevelopment(), Context, DatabaseInfo.SugarSqlExecMaxSeconds!.Value,
            DatabaseInfo.DiffLog!.Value, DatabaseInfo.DisableAop!.Value, sqlSugarEntityHandler);

        // 过滤器
        SugarEntityFilter.LoadSugarFilter(Context, sqlSugarEntityHandler);
    }

    /// <summary>
    /// 是否支持逻辑删除
    /// </summary>
    /// <remarks><typeparamref name="TEntity"/> 继承了 <see cref="IDeletedEntity"/> 才有用</remarks>
    public bool SupportsLogicDelete { get; }

    /// <summary>
    /// 是否支持行版本控制（乐观锁）
    /// </summary>
    /// <remarks><typeparamref name="TEntity"/> 继承了 <see cref="IUpdateVersion"/> 才有用</remarks>
    public bool SupportsRowVersion { get; }

    /// <summary>
    /// 是否分表
    /// </summary>
    /// <remarks><typeparamref name="TEntity"/> 头部标记 <see cref="SplitTableAttribute"/> 特性才有用</remarks>
    public bool IsSplitTable { get; }

    /// <summary>
    /// 实体集合
    /// </summary>
    public ISugarQueryable<TEntity> Entities => Queryable<TEntity>();

    /// <summary>
    /// 当前仓储的数据库信息
    /// </summary>
    public ConnectionSettingsOptions DatabaseInfo { get; set; }

    /// <summary>
    /// 切换仓储/切换租户仓储
    /// </summary>
    /// <typeparam name="TChangeEntity">实体类型</typeparam>
    /// <returns>仓储</returns>
    public ISqlSugarRepository<TChangeEntity> Change<TChangeEntity>() where TChangeEntity : class, new()
    {
        return _serviceProvider.GetService(typeof(ISqlSugarRepository<TChangeEntity>)) as ISqlSugarRepository<TChangeEntity>;
    }
}