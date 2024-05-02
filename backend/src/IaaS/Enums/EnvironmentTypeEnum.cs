// Apache开源许可证
//
// 版权所有 © 2018-Now 小方
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

using System.ComponentModel;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="EnvironmentTypeEnum"/> 环境类型枚举
/// </summary>
[FastEnum("环境类型枚举")]
public enum EnvironmentTypeEnum
{
    /// <summary>
    /// Production 生产环境
    /// <remarks>生产，正式环境</remarks>
    /// </summary>
    [Description("生产环境")]
    Production = 1,

    /// <summary>
    /// Development 开发环境
    /// <remarks>本地，开发环境</remarks>
    /// </summary>
    [Description("开发环境")]
    Development = 2,

    /// <summary>
    /// UAT 测试验收环境
    /// <remarks>测试验收环境，用于测试是否符合需求和预期</remarks>
    /// </summary>
    [Description("测试验收环境")]
    UAT = 4,

    /// <summary>
    /// PreProduction 预生产环境
    /// <remarks>预生产环境，用于最后的测试和验证</remarks>
    /// </summary>
    [Description("预生产环境")]
    PreProduction = 8,

    /// <summary>
    /// GrayDeployment 灰度环境
    /// <remarks>灰度环境，用于部署新版本到生产环境之前进行有限范围的测试和验收的环境</remarks>
    /// </summary>
    [Description("灰度环境")]
    GrayDeployment = 16,

    /// <summary>
    /// StressTest 压测环境
    /// <remarks>压测环境，用于压力测试的环境</remarks>
    /// </summary>
    [Description("压测环境")]
    StressTest = 32,
}