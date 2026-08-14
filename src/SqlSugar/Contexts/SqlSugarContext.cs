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
using SqlSugar;

namespace Fast.SqlSugar;

/// <summary>
/// SqlSugar 上下文。
/// </summary>
[SuppressSniffer]
public sealed class SqlSugarContext
{
    /// <summary>
    /// 连接字符串配置。
    /// </summary>
    public static ConnectionSettingsOptions ConnectionSettings { get; internal set; }

    /// <summary>
    /// 雪花Id配置。
    /// </summary>
    public static SnowflakeSettingsOptions SnowflakeSettings { get; internal set; }

    /// <summary>
    /// 最大不分页大小限制。
    /// </summary>
    public static int MaxNotPageSize { get; set; }

    /// <summary>
    /// SqlSugar 实体集合。
    /// </summary>
    public static readonly List<SqlSugarEntityInfo> SqlSugarEntityList;

    static SqlSugarContext()
    {
        MaxNotPageSize = 10000;

        var dataBaseEntityType = typeof(IDatabaseEntity);

        SqlSugarEntityList = MAppContext.EffectiveTypes.Where(wh => dataBaseEntityType.IsAssignableFrom(wh) && !wh.IsInterface)
            .Select(sl =>
            {
                var sqlSugarTableAttribute = sl.GetSugarTableAttribute();

                var splitTableAttribute = sl.GetCustomAttribute<SplitTableAttribute>(true);

                var sugarDbTypeAttribute = sl.GetCustomAttribute<SugarDbTypeAttribute>(true);

                return new SqlSugarEntityInfo
                {
                    TableName = sqlSugarTableAttribute?.TableName ?? sl.Name,
                    TableDescription = sqlSugarTableAttribute?.TableDescription,
                    EntityType = sl,
                    IsSplitTable = splitTableAttribute != null,
                    SugarDbType = sugarDbTypeAttribute?.Type
                };
            })
            .ToList();
    }

    /// <summary>
    /// 获取连接配置。
    /// </summary>
    /// <param name="connectionSettings">connection Settings 配置。</param>
    /// <returns>获取到的连接配置。</returns>
    public static ConnectionConfig GetConnectionConfig(ConnectionSettingsOptions connectionSettings)
    {
        // 得到连接字符串
        var connectionStr = SqlSugarDatabaseUtil.GetConnectionStr(connectionSettings.DbType!.Value, connectionSettings);

        var slaveConnectionList = new List<SlaveConnectionConfig>();

        // 判断是否存在从库信息
        if (connectionSettings.SlaveConnectionList is {Count: > 0})
        {
            foreach (var slaveConnectionInfo in connectionSettings.SlaveConnectionList)
            {
                var slaveConnectionStr =
                    SqlSugarDatabaseUtil.GetConnectionStr(connectionSettings.DbType!.Value, slaveConnectionInfo);

                slaveConnectionList.Add(new SlaveConnectionConfig
                {
                    HitRate = slaveConnectionInfo.HitRate, ConnectionString = slaveConnectionStr
                });
            }
        }

        return new ConnectionConfig
        {
            // 此链接标志，用以后面切库使用
            ConfigId = connectionSettings.ConnectionId,
            ConnectionString = connectionStr,
            DbType = connectionSettings.DbType!.Value,
            // 每次操作完成后释放连接，避免作用域仓储长期占用连接。
            IsAutoCloseConnection = true,
            // 从特性读取主键和自增列信息
            InitKeyType = InitKeyType.Attribute,
            MoreSettings = SqlSugarDatabaseUtil.GetSugarMoreSettings(),
            ConfigureExternalServices = SqlSugarDatabaseUtil.GetSugarExternalServices(connectionSettings.DbType!.Value),
            SlaveConnectionConfigs = slaveConnectionList
        };
    }

    /// <summary>
    /// Entity Value 检测。
    /// </summary>
    /// <param name="emptyList">空对象检测集合。</param>
    /// <param name="entityInfo">实体信息。</param>
    /// <returns>检查通过时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
    internal static bool EntityValueCheck(ICollection<object> emptyList, DataFilterModel entityInfo)
    {
        try
        {
            var propertyValue = entityInfo.EntityColumnInfo.PropertyInfo.GetValue(entityInfo.EntityValue);
            return emptyList == null || emptyList.Any(empty => Equals(empty, propertyValue));
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 设置 Entity Value。
    /// </summary>
    /// <param name="fieldName">字段名称。</param>
    /// <param name="emptyList">空对象检测集合。</param>
    /// <param name="setValue">要设置的值。</param>
    /// <param name="entityInfo">实体信息。</param>
    internal static void SetEntityValue(string fieldName, ICollection<object> emptyList, object setValue,
        DataFilterModel entityInfo)
    {
        // 判断属性名称是否等于传入的字段名称
        if (entityInfo.PropertyName == fieldName)
        {
            if (EntityValueCheck(emptyList, entityInfo))
            {
                entityInfo.SetValue(setValue);
            }
        }
    }
}
