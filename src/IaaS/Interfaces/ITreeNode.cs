// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System;
using System.Collections;

namespace Fast.IaaS;

/// <summary>
/// 树基类
/// </summary>
/// <typeparam name="TProperty">属性值类型</typeparam>
public interface ITreeNode<out TProperty> where TProperty : struct, IComparable, IConvertible, IFormattable
{
    /// <summary>
    /// 获取节点 id
    /// </summary>
    /// <returns>获取到的节点 id</returns>
    TProperty GetId();

    /// <summary>
    /// 获取节点父 id
    /// </summary>
    /// <returns>获取到的节点父 id</returns>
    TProperty GetPid();

    /// <summary>
    /// 获取排序字段
    /// </summary>
    /// <returns>获取到的排序字段</returns>
    TProperty GetSort();

    /// <summary>
    /// 设置 Children
    /// </summary>
    /// <param name="children">当前节点的直接子节点集合</param>
    void SetChildren(IList children);
}
