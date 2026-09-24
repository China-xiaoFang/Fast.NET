// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

// ReSharper disable once CheckNamespace

namespace System;

/// <summary>
/// 不被扫描和发现的特性
/// </summary>
/// <remarks>用于程序集扫描类型或方法时候</remarks>
[SuppressSniffer]
[AttributeUsage(AttributeTargets.Class
                | AttributeTargets.Interface
                | AttributeTargets.Method
                | AttributeTargets.Enum
                | AttributeTargets.Struct)]
public class SuppressSnifferAttribute : Attribute
{
}
