// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

namespace Fast.DynamicApplication;

/// <summary>
/// 参数路由模板
/// </summary>
internal sealed class ParameterRouteTemplate
{
    /// <summary>
    /// 初始化类的新实例
    /// </summary>
    internal ParameterRouteTemplate()
    {
        ControllerStartTemplates = new List<string>();
        ControllerEndTemplates = new List<string>();
        ActionStartTemplates = new List<string>();
        ActionEndTemplates = new List<string>();
    }

    /// <summary>
    /// 控制器之前的参数
    /// </summary>
    public IList<string> ControllerStartTemplates { get; set; }

    /// <summary>
    /// 控制器之后的参数
    /// </summary>
    public IList<string> ControllerEndTemplates { get; set; }

    /// <summary>
    /// 行为之前的参数
    /// </summary>
    public IList<string> ActionStartTemplates { get; set; }

    /// <summary>
    /// 行为之后的参数
    /// </summary>
    public IList<string> ActionEndTemplates { get; set; }
}
