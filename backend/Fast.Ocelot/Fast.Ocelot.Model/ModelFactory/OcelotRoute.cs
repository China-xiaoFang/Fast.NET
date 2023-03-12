using Fast.Admin.Model.Enum;
using Fast.SqlSugar.Tenant.AttributeFilter;
using Fast.SqlSugar.Tenant.BaseModel;
using SqlSugar;

namespace Fast.Ocelot.Model.ModelFactory;

/// <summary>
/// 网关路由表
/// </summary>
[SugarTable("Ocelot_Route", "网关路由表")]
[SugarDbType]
public class OcelotRoute : BaseEntity
{
    /// <summary>
    /// 项目Id
    /// </summary>
    [SugarColumn(ColumnDescription = "项目Id", IsNullable = false)]
    public long ProjectId { get; set; }

    /// <summary>
    /// 下游的路由模板，即真实处理请求的路径模板 
    ///</summary>
    [SugarColumn(ColumnDescription = "下游的路由模板，即真实处理请求的路径模板", ColumnDataType = "Nvarchar(max)", IsNullable = false)]
    public string DownstreamPathTemplate { get; set; }

    /// <summary>
    /// 上游请求的模板，即用户真实请求的链接 
    ///</summary>
    [SugarColumn(ColumnDescription = "上游请求的模板，即用户真实请求的链接", ColumnDataType = "Nvarchar(max)", IsNullable = false)]
    public string UpstreamPathTemplate { get; set; }

    /// <summary>
    /// 上游请求的http方法（数组：GET、POST、PUT） 
    ///</summary>
    [SugarColumn(ColumnDescription = "上游请求的http方法（数组：GET、POST、PUT）", ColumnDataType = "Nvarchar(max)", IsNullable = false,
        IsJson = true)]
    public List<string> UpstreamHttpMethod { get; set; }

    /// <summary>
    /// 下游请求的http方法（数组：GET、POST、PUT） 
    ///</summary>
    [SugarColumn(ColumnDescription = "下游请求的http方法（数组：GET、POST、PUT）", ColumnDataType = "Nvarchar(max)", IsNullable = false,
        IsJson = true)]
    public List<string> DownstreamHttpMethod { get; set; }

    /// <summary>
    /// 下游Http版本 
    ///</summary>
    [SugarColumn(ColumnDescription = "下游Http版本", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string DownstreamHttpVersion { get; set; }

    /// <summary>
    /// 请求Id 
    ///</summary>
    [SugarColumn(ColumnDescription = "请求Id", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string RequestIdKey { get; set; }

    /// <summary>
    /// 开启上下游路由模板大小写匹配 
    ///</summary>
    [SugarColumn(ColumnDescription = "开启上下游路由模板大小写匹配", IsNullable = false)]
    public bool RouteIsCaseSensitive { get; set; }

    /// <summary>
    /// 服务名 
    ///</summary>
    [SugarColumn(ColumnDescription = "服务名", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string ServiceName { get; set; }

    /// <summary>
    /// 服务命名空间 
    ///</summary>
    [SugarColumn(ColumnDescription = "服务命名空间", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string ServiceNamespace { get; set; }

    /// <summary>
    /// 请求的方式，如：http,https 
    ///</summary>
    [SugarColumn(ColumnDescription = "请求的方式，如：http,https", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string DownstreamScheme { get; set; }

    /// <summary>
    /// 请求缓存过期时间（需使用Ocelot.Cache.CacheManager） 
    ///</summary>
    [SugarColumn(ColumnDescription = "请求缓存过期时间", IsNullable = false)]
    public int CacheTtlSeconds { get; set; }

    /// <summary>
    /// 缓存区域（需使用Ocelot.Cache.CacheManager） 
    ///</summary>
    [SugarColumn(ColumnDescription = "缓存区域", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string CacheRegion { get; set; }

    /// <summary>
    /// 中断前允许Qos异常
    ///</summary>
    [SugarColumn(ColumnDescription = "中断前允许Qos异常", IsNullable = false)]
    public int? QosExceptionsAllowedBeforeBreaking { get; set; }

    /// <summary>
    /// 中断Qos持续时间
    ///</summary>
    [SugarColumn(ColumnDescription = "中断Qos持续时间", IsNullable = false)]
    public int QosDurationOfBreak { get; set; }

    /// <summary>
    /// Qos超时值
    ///</summary>
    [SugarColumn(ColumnDescription = "Qos超时值", IsNullable = false)]
    public int QosTimeoutValue { get; set; }

    /// <summary>
    /// 负载平衡类型
    ///</summary>
    [SugarColumn(ColumnDescription = "负载平衡类型", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string LoadBalancerType { get; set; }

    /// <summary>
    /// 负载平衡键
    ///</summary>
    [SugarColumn(ColumnDescription = "负载平衡键", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string LoadBalancerKey { get; set; }

    /// <summary>
    /// 负载平衡器失效
    ///</summary>
    [SugarColumn(ColumnDescription = "负载平衡器失效", IsNullable = false)]
    public int LoadBalancerExpiry { get; set; }

    /// <summary>
    /// 是否启用流量限制
    ///</summary>
    [SugarColumn(ColumnDescription = "是否启用流量限制", IsNullable = false)]
    public bool RateLimitEnableRateLimiting { get; set; }

    /// <summary>
    /// 限费率期
    ///</summary>
    [SugarColumn(ColumnDescription = "速率限制", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string RateLimitPeriod { get; set; }

    /// <summary>
    /// 限速时间段时间段
    ///</summary>
    [SugarColumn(ColumnDescription = "速率限制", IsNullable = false)]
    public decimal RateLimitPeriodTimespan { get; set; }

    /// <summary>
    /// 速率限制
    ///</summary>
    [SugarColumn(ColumnDescription = "速率限制", IsNullable = false)]
    public long RateLimitLimit { get; set; }

    /// <summary>
    /// 速率限制白名单
    ///</summary>
    [SugarColumn(ColumnDescription = "速率限制白名单", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string RateLimitWhitelist { get; set; }

    /// <summary>
    /// 身份验证身份验证提供者密钥
    ///</summary>
    [SugarColumn(ColumnDescription = "身份验证身份验证提供者密钥", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string AuthenticationAuthenticationProviderKey { get; set; }

    /// <summary>
    /// 允许身份验证的范围
    ///</summary>
    [SugarColumn(ColumnDescription = "允许身份验证的范围", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string AuthenticationAllowedScopes { get; set; }

    /// <summary>
    /// Http处理程序允许自动重定向
    ///</summary>
    [SugarColumn(ColumnDescription = "Http处理程序允许自动重定向", IsNullable = false)]
    public bool HttpHandlerAllowAutoRedirect { get; set; }

    /// <summary>
    /// Http处理程序使用Cookie容器
    ///</summary>
    [SugarColumn(ColumnDescription = "Http处理程序使用Cookie容器", IsNullable = false)]
    public bool HttpHandlerUseCookieContainer { get; set; }

    /// <summary>
    /// Http处理程序使用跟踪
    ///</summary>
    [SugarColumn(ColumnDescription = "Http处理程序使用跟踪", IsNullable = false)]
    public bool HttpHandlerUseTracing { get; set; }

    /// <summary>
    /// Http处理程序使用代理
    ///</summary>
    [SugarColumn(ColumnDescription = "Http处理程序使用代理", IsNullable = false)]
    public bool HttpHandlerUseProxy { get; set; }

    /// <summary>
    /// Http处理最大连接数
    ///</summary>
    [SugarColumn(ColumnDescription = "Http处理最大连接数", IsNullable = false)]
    public int HttpHandlerMaxConnectionsPerServer { get; set; }

    /// <summary>
    /// 上游主机
    ///</summary>
    [SugarColumn(ColumnDescription = "上游主机", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string UpstreamHost { get; set; }

    /// <summary>
    /// Key
    ///</summary>
    [SugarColumn(ColumnDescription = "Key", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string Key { get; set; }

    /// <summary>
    /// 授权处理程序
    ///</summary>
    [SugarColumn(ColumnDescription = "授权处理程序", ColumnDataType = "Nvarchar(50)", IsNullable = false)]
    public string DelegatingHandlers { get; set; }

    /// <summary>
    /// 优先级
    ///</summary>
    [SugarColumn(ColumnDescription = "优先级", IsNullable = false)]
    public int Priority { get; set; }

    /// <summary>
    /// 超时时间
    ///</summary>
    [SugarColumn(ColumnDescription = "超时时间", IsNullable = false)]
    public int Timeout { get; set; }

    /// <summary>
    /// 评估危险服务验证 
    ///</summary>
    [SugarColumn(ColumnDescription = "评估危险服务验证", IsNullable = false)]
    public bool DangerousAcceptAnyServerCertificateValidator { get; set; }

    /// <summary>
    /// Ip允许列表
    ///</summary>
    [SugarColumn(ColumnDescription = "Ip允许列表", ColumnDataType = "Nvarchar(max)", IsNullable = false, IsJson = true)]
    public string SecurityIpAllowedList { get; set; }

    /// <summary>
    /// Ip屏蔽列表
    ///</summary>
    [SugarColumn(ColumnDescription = "Ip屏蔽列表", ColumnDataType = "Nvarchar(max)", IsNullable = false, IsJson = true)]
    public string SecurityIpBlockedList { get; set; }

    /// <summary>
    /// 顺序
    /// </summary>
    [SugarColumn(ColumnDescription = "顺序", IsNullable = false)]
    public int Sort { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [SugarColumn(ColumnDescription = "状态", IsNullable = false)]
    public CommonStatusEnum Status { get; set; } = CommonStatusEnum.Enable;
}