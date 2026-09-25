// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

namespace Fast.Runtime;

/// <summary>
/// .deps.json 文件中 libraries 节点的 Model
/// </summary>
public class DependencyLibrary
{
    /// <summary>
    /// 初始化依赖库元数据
    /// </summary>
    /// <param name="type">依赖来源类型，通常为 <c>project</c> 或 <c>package</c></param>
    /// <param name="name">依赖名称</param>
    /// <param name="version">依赖版本</param>
    /// <param name="fileName">不含扩展名的运行时程序集文件名</param>
    /// <param name="serviceable">指示依赖是否可通过运行时服务更新</param>
    internal DependencyLibrary(string type, string name, string version, string fileName, bool serviceable)
    {
        Type = type;
        Name = name;
        Version = version;
        FileName = fileName;
        Serviceable = serviceable;
    }

    /// <summary>
    /// 类型
    /// </summary>
    /// <remarks>"package"是引用的包，"project"是本地引用的项目</remarks>
    public string Type { get; }

    /// <summary>
    /// 程序集名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 程序集 dll 文件名称（不带后缀）
    /// </summary>
    /// <remarks>
    /// <para>从 "runtime" 节点获取，如果为空，则默认为 Name</para>
    /// <para>注意：部分时候可能和 Name 不一致</para>
    /// </remarks>
    public string FileName { get; }

    /// <summary>
    /// 程序集版本
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// 是否可服务
    /// </summary>
    public bool Serviceable { get; }
}
