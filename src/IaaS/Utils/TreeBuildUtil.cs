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

using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="TreeBuildUtil{TEntity, TProperty}"/> 递归工具类，用于遍历有父子关系的节点，例如菜单树，字典树等等
/// </summary>
/// <typeparam name="TEntity">模型</typeparam>
/// <typeparam name="TProperty">Id属性类型</typeparam>
public class TreeBuildUtil<TEntity, TProperty> where TEntity : ITreeNode<TProperty>
    where TProperty : struct, IComparable, IConvertible, IFormattable
{
    /// <summary>
    /// 顶级节点的父节点Id(默认0)
    /// </summary>
    // ReSharper disable once RedundantDefaultMemberInitializer
    private TProperty _rootParentId = default;

    /// <summary>
    /// 设置根节点方法
    /// 查询数据可以设置其他节点为根节点，避免父节点永远是0，查询不到数据的问题
    /// </summary>
    public void SetRootParentId(TProperty rootParentId)
    {
        _rootParentId = rootParentId;
    }

    /// <summary>
    /// 构造树节点
    /// </summary>
    /// <param name="nodes"></param>
    /// <returns></returns>
    public List<TEntity> Build(List<TEntity> nodes)
    {
        var result = nodes.Where(i => i.GetPid()
                .Equals(_rootParentId))
            .OrderBy(ob => ob.GetSort())
            .ToList();
        result.ForEach(u => BuildChildNodes(nodes, u));
        return result;
    }

    /// <summary>
    /// 构造子节点集合
    /// </summary>
    /// <param name="totalNodes"></param>
    /// <param name="node"></param>
    private void BuildChildNodes(List<TEntity> totalNodes, TEntity node)
    {
        var nodeSubList = totalNodes.Where(i => i.GetPid()
                .Equals(node.GetId()))
            .OrderBy(ob => ob.GetSort())
            .ToList();
        nodeSubList.ForEach(u => BuildChildNodes(totalNodes, u));
        node.SetChildren(nodeSubList);
    }
}