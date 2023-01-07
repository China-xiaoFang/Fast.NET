using System.ComponentModel;
using Fast.Core.Internal.AttributeFilter;

namespace Fast.Ocelot.Model.EnumFactory;

/// <summary>
/// 路由Prop类型枚举
/// </summary>
[FastEnum("路由Prop类型枚举")]
public enum RoutePropsTypeEnum
{
    /// <summary>
    /// 头部
    /// </summary>
    [Description("头部")]
    Header = 1,

    /// <summary>
    /// 头部向上变换
    /// </summary>
    [Description("头部向上变换")]
    HeaderUpTransforms = 2,

    /// <summary>
    /// 头部向下变换
    /// </summary>
    [Description("头部向下变换")]
    HeaderDownTransform = 3,

    /// <summary>
    /// 授权
    /// </summary>
    [Description("授权")]
    Claims = 4,

    /// <summary>
    /// 路由授权
    /// </summary>
    [Description("路由授权")]
    RouteClaims = 5,

    /// <summary>
    /// 查询
    /// </summary>
    [Description("查询")]
    Queries = 6,

    /// <summary>
    /// 下载模板
    /// </summary>
    [Description("下载模板")]
    DownloadTemplate = 7
}