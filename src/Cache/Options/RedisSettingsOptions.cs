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

// ReSharper disable once CheckNamespace
namespace Fast.Cache;

/// <summary>
/// <see cref="RedisSettingsOptions"/> Redis连接字符串配置
/// </summary>
[SuppressSniffer]
public class RedisSettingsOptions : IPostConfigure
{
    /// <summary>
    /// 服务器Ip地址
    /// </summary>
    public string ServiceIp { get; set; }

    /// <summary>
    /// 端口号
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// 默认库
    /// </summary>
    public int? DbName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string DbPwd { get; set; }

    /// <summary>
    /// 前缀
    /// </summary>
    public string Prefix { get; set; }

    /// <summary>
    /// 连接池大小
    /// </summary>
    public int? Poolsize { get; set; }

    /// <summary>
    /// SSL加密连接
    /// </summary>
    public bool? SSL { get; set; }

    /// <summary>
    /// 服务
    /// </summary>
    public List<RedisServiceSettingsOptions> Services { get; set; }

    /// <summary>
    /// 后期配置
    /// </summary>
    public void PostConfigure()
    {
        ServiceIp ??= "127.0.0.1";
        Port ??= 6379;
        DbName ??= 0;
        Poolsize ??= 100;
        SSL ??= false;
        Services ??= [];
    }
}

/// <summary>
/// <see cref="RedisServiceSettingsOptions"/> Redis连接字符串服务配置
/// </summary>
[SuppressSniffer]
public class RedisServiceSettingsOptions : RedisSettingsOptions
{
    /// <summary>
    /// 服务名称
    /// </summary>
    public string ServiceName { get; set; }
}