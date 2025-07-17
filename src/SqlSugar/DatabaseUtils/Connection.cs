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

using SqlSugar;

// ReSharper disable once CheckNamespace
namespace Fast.SqlSugar;

/// <summary>
/// <see cref="DatabaseUtil"/> 连接字符串工具类
/// </summary>
internal partial class DatabaseUtil
{
    /// <summary>
    /// 得到数据库连接字符串
    /// </summary>
    /// <param name="dbType"><see cref="DbType"/> 数据库类型</param>
    /// <param name="dbInfo"><see cref="DbConnectionInfo"/> 数据库连接信息</param>
    /// <returns></returns>
    internal static string GetConnectionStr(DbType dbType, DbConnectionInfo dbInfo)
    {
        string connectionStr;
        if (!string.IsNullOrWhiteSpace(dbInfo.CustomConnectionStr))
        {
            connectionStr = dbInfo.CustomConnectionStr;
        }
        else
        {
            connectionStr = dbType switch
            {
                DbType.MySql or DbType.MySqlConnector =>
                    $"Server={dbInfo.ServiceIp};Port={dbInfo.Port ?? 3306};Database={dbInfo.DbName};User ID={dbInfo.DbUser};Password={dbInfo.DbPwd};CharSet=utf8;SslMode=none;Pooling=true;Convert Zero Datetime=True;Allow Zero Datetime=True;Max Pool Size=100;",
                DbType.SqlServer =>
                    $"Server={dbInfo.ServiceIp},{dbInfo.Port ?? 1433};Database={dbInfo.DbName};User={dbInfo.DbUser};Password={dbInfo.DbPwd};MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;Max Pool Size=100;",
                DbType.Sqlite => $"Data Source={dbInfo.DbName};",
                DbType.Oracle =>
                    $"Data Source={dbInfo.ServiceIp}:{dbInfo.Port ?? 1521}/{dbInfo.DbName};User ID={dbInfo.DbUser};Password={dbInfo.DbPwd};",
                DbType.PostgreSQL =>
                    $"Host={dbInfo.ServiceIp};Port={dbInfo.Port ?? 5432};Database={dbInfo.DbName};Username={dbInfo.DbUser};Password={dbInfo.DbPwd};Pooling=true;MaxPoolSize=100;",
                DbType.Access => $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={dbInfo.DbName};Persist Security Info=False;",
                DbType.MongoDb =>
                    $"mongodb://{dbInfo.DbUser}:{dbInfo.DbPwd}@{dbInfo.ServiceIp}:{dbInfo.Port ?? 27017}/{dbInfo.DbName}",
                _ => throw new SqlSugarException("数据库类型配置异常！")
            };
        }

        return connectionStr;
    }
}