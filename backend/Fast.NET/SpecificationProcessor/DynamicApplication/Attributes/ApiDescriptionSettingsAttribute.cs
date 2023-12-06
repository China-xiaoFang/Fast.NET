// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
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

using Fast.IaaS;
using Fast.SpecificationProcessor.Internal;
using Microsoft.AspNetCore.Mvc;

// ReSharper disable once CheckNamespace
namespace Fast.SpecificationProcessor.DynamicApplication;

/// <summary>
/// 接口描述设置
/// </summary>
[SuppressSniffer, AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ApiDescriptionSettingsAttribute : ApiExplorerSettingsAttribute
{
    /// <summary>
    /// 构造函数
    /// </summary>
    public ApiDescriptionSettingsAttribute()
    {
        Order = 0;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="enabled">是否启用</param>
    public ApiDescriptionSettingsAttribute(bool enabled)
    {
        IgnoreApi = !enabled;
        Order = 0;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="groups">分组列表</param>
    public ApiDescriptionSettingsAttribute(params string[] groups)
    {
        GroupName = string.Join(Penetrates.GroupSeparator, groups);
        Groups = groups;
        Order = 0;
    }

    /// <summary>
    /// 自定义名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 模块名
    /// </summary>
    public string Module { get; set; }

    /// <summary>
    /// 版本号
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// 分组
    /// </summary>
    public string[] Groups { get; set; }

    /// <summary>
    /// 标签
    /// </summary>
    public string Tag { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// 额外描述，支持 HTML
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 强制携带路由前缀，即使使用 [Route] 重写，仅对 Class/Controller 有效
    /// </summary>
    public object ForceWithRoutePrefix { get; set; }
}