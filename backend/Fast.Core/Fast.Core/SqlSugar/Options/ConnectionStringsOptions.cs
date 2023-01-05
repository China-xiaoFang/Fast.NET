namespace Fast.Core.SqlSugar.Options;

/// <summary>
/// 数据库配置
/// </summary>
public class ConnectionStringsOptions
{
    /// <summary>
    /// 连接Id
    /// </summary>
    public string DefaultConnectionId { get; set; }

    /// <summary>
    /// 服务器Ip地址
    /// </summary>
    public string DefaultServiceIp { get; set; }

    /// <summary>
    /// 端口号
    /// </summary>
    public string DefaultPort { get; set; }

    /// <summary>
    /// 数据库名称
    /// </summary>
    public string DefaultDbName { get; set; }

    /// <summary>
    /// 数据库用户
    /// </summary>
    public string DefaultDbUser { get; set; }

    /// <summary>
    /// 数据库密码
    /// </summary>
    public string DefaultDbPwd { get; set; }

    /// <summary>
    /// 数据库类型
    /// </summary>
    public DbType DefaultDbType { get; set; }
}