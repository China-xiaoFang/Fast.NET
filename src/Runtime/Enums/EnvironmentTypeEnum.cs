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

using System.ComponentModel;

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>
/// <see cref="EnvironmentTypeEnum"/> 环境类型枚举
/// </summary>
[Flags]
[FastEnum("环境类型枚举")]
public enum EnvironmentTypeEnum
{
    /// <summary>
    /// Production 生产环境
    /// </summary>
    /// <remarks>生产，正式环境</remarks>
    [Description("生产环境")]
    Production = 1,

    /// <summary>
    /// Development 开发环境
    /// </summary>
    /// <remarks>本地，开发环境</remarks>
    [Description("开发环境")]
    Development = 2,

    /// <summary>
    /// Test 测试环境
    /// </summary>
    /// <remarks>测试环境，功能测试，验证新功能或修复问题</remarks>
    [Description("测试环境")]
    Test = 4,

    /// <summary>
    /// UAT 测试验收环境
    /// </summary>
    /// <remarks>测试验收环境，用于测试是否符合需求和预期</remarks>
    [Description("测试验收环境")]
    UAT = 8,

    /// <summary>
    /// PreProduction 预生产环境
    /// </summary>
    /// <remarks>预生产环境，用于最后的测试和验证</remarks>
    [Description("预生产环境")]
    PreProduction = 16,

    /// <summary>
    /// GrayDeployment 灰度环境
    /// </summary>
    /// <remarks>灰度环境，用于部署新版本到生产环境之前进行有限范围的测试和验收的环境</remarks>
    [Description("灰度环境")]
    GrayDeployment = 32,

    /// <summary>
    /// StressTest 压测环境
    /// </summary>
    /// <remarks>压测环境，用于压力测试的环境</remarks>
    [Description("压测环境")]
    StressTest = 64
}