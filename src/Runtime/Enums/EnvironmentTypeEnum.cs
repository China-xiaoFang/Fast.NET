// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.ComponentModel;


// ReSharper disable once CheckNamespace
namespace System;

/// <summary>
/// 环境类型枚举
/// </summary>
[Flags]
[FastEnum("环境类型枚举")]
public enum EnvironmentTypeEnum : byte
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
