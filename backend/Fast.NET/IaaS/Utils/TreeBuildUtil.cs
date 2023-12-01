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


// ReSharper disable once CheckNamespace

namespace Fast.IaaS;

/// <summary>
/// <see cref="TreeBuildUtil{TEntity, TProperty}"/> 递归工具类，用于遍历有父子关系的节点，例如菜单树，字典树等等
/// </summary>
/// <typeparam name="TEntity">模型</typeparam>
/// <typeparam name="TProperty">Id属性类型</typeparam>
[SuppressSniffer]
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
        var result = nodes.Where(i => i.GetPid().Equals(_rootParentId)).OrderBy(ob => ob.Sort()).ToList();
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
        var nodeSubList = totalNodes.Where(i => i.GetPid().Equals(node.GetId())).OrderBy(ob => ob.Sort()).ToList();
        nodeSubList.ForEach(u => BuildChildNodes(totalNodes, u));
        node.SetChildren(nodeSubList);
    }
}