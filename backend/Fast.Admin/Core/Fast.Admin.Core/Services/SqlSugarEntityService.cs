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

using Fast.Admin.Core.Constants;
using Fast.Admin.Core.Entity.System.DataBase;
using Fast.Admin.Core.Enum.Common;
using Fast.Admin.Core.Enum.Db;
using Fast.SqlSugar.Commons;
using Fast.SqlSugar.Options;
using Mapster;
using Microsoft.Extensions.Logging;
using SqlSugar;

namespace Fast.Admin.Core.Services;

/// <summary>
/// <see cref="SqlSugarEntityService"/> SqlSugar 实体服务
/// </summary>
public class SqlSugarEntityService : ISqlSugarEntityService, ITransientDependency
{
    private readonly ICache _cache;

    /// <summary>
    /// 这里为了防止死循环 Aop 的发生，直接注入 ISqlSugarClient，并且禁用 Aop 处理
    /// </summary>
    private readonly ISqlSugarClient _sqlSugarClient;

    private readonly HttpContext _httpContext;
    private readonly ILogger<ISqlSugarEntityService> _logger;

    public SqlSugarEntityService(ICache cache, ISqlSugarClient sqlSugarClient, IHttpContextAccessor httpContextAccessor,
        ILogger<ISqlSugarEntityService> logger)
    {
        _cache = cache;
        // 禁用当前SugarClient的AOP，不然会存在死循环的问题
        sqlSugarClient.Ado.IsEnableLogEvent = true;
        _sqlSugarClient = sqlSugarClient;
        _httpContext = httpContextAccessor.HttpContext;
        _logger = logger;
    }

    /// <summary>
    /// 根据类型获取连接字符串
    /// </summary>
    /// <param name="fastDbType"></param>
    /// <param name="tenantId"></param>
    /// <param name="isSystem"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<ConnectionSettingsOptions> GetConnectionSettings(FastDbTypeEnum fastDbType, long tenantId,
        YesOrNotEnum isSystem = YesOrNotEnum.N)
    {
        // 禁用当前SugarClient的AOP，不然会存在死循环的问题
        _sqlSugarClient.Ado.IsEnableLogEvent = true;

        // 获取缓存Key
        var cacheKey = CacheConst.GetCacheKey(CacheConst.TenantDataBaseInfo, tenantId, System.Enum.GetName(fastDbType));

        // 优先从 HttpContext.Items 中获取
        var connectionSettingsObj =
            _httpContext.Items[nameof(Fast) + nameof(ConnectionSettingsOptions) + System.Enum.GetName(fastDbType)];

        if (connectionSettingsObj is ConnectionSettingsOptions connectionSettings)
        {
            return connectionSettings;
        }

        return await _cache.GetAndSetAsync(cacheKey, async () =>
        {
            var sysTenantDataBaseModel = await _sqlSugarClient.Queryable<SysTenantMainDataBaseModel>().Where(wh =>
                    wh.IsSystem == isSystem && wh.FastDbType == fastDbType && wh.TenantId == tenantId && wh.IsDeleted == false)
                .SingleAsync();

            if (sysTenantDataBaseModel == null)
            {
                var errorMessage = $"未能找到对应类型【{System.Enum.GetName(fastDbType)}】所存在的DataBase信息！";
                // 写入错误日志
                _logger.LogError($"TenantId：{tenantId}；${errorMessage}");
                throw new ArgumentNullException(errorMessage);
            }

            // 查询从库
            var sysTenantDataBaseSlaveList = await _sqlSugarClient.Queryable<SysTenantSlaveDataBaseModel>()
                .Where(wh => wh.TenantId == tenantId && wh.MainId == sysTenantDataBaseModel.Id && wh.IsDeleted == false)
                .ToListAsync();

            // 组装返回数据
            var result = sysTenantDataBaseModel.Adapt<ConnectionSettingsOptions>();

            result.SlaveConnectionList = sysTenantDataBaseSlaveList?.Adapt<List<SlaveConnectionInfo>>();

            // 放入 HttpContext.Items 中
            _httpContext.Items[nameof(Fast) + nameof(ConnectionSettingsOptions) + System.Enum.GetName(fastDbType)] = result;

            return result;
        });
    }

    /// <summary>
    /// 获取日志库连接配置
    /// </summary>
    /// <returns></returns>
    public async Task<ConnectionConfig> GetLogSqlSugarClient()
    {
        // 获取系统日志库连接字符串配置
        var logConnectionSettings = await GetConnectionSettings(
            FastDbTypeEnum.SysCoreLog, SystemConst.DefaultSystemTenantId, YesOrNotEnum.Y);

        return SqlSugarContext.GetConnectionConfig(logConnectionSettings);
    }

    /// <summary>
    /// 获取系统Admin核心库连接配置
    /// </summary>
    /// <param name="tenantId"><see cref="long"/> 租户Id</param>
    /// <returns></returns>
    public async Task<ConnectionConfig> GetAdminCoreSqlSugarClient(long tenantId)
    {
        // 获取系统日志库连接字符串配置
        var logConnectionSettings = await GetConnectionSettings(FastDbTypeEnum.SysAdminCore, tenantId);

        return SqlSugarContext.GetConnectionConfig(logConnectionSettings);
    }
}