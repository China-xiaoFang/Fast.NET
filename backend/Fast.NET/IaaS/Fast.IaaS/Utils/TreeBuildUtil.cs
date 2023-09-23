using System.Collections;

namespace Fast.IaaS.Utils;

/// <summary>
/// 树基类
/// </summary>
/// <typeparam name="TProperty"></typeparam>
public interface ITreeNode<out TProperty> where TProperty : struct, IComparable, IConvertible
{
    /// <summary>
    /// 获取节点id
    /// </summary>
    /// <returns></returns>
    TProperty GetId();

    /// <summary>
    /// 获取节点父id
    /// </summary>
    /// <returns></returns>
    TProperty GetPid();

    /// <summary>
    /// 获取排序字段
    /// </summary>
    /// <returns></returns>
    int Sort();

    /// <summary>
    /// 设置Children
    /// </summary>
    /// <param name="children"></param>
    void SetChildren(IList children);
}

/// <summary>
/// 递归工具类，用于遍历有父子关系的节点，例如菜单树，字典树等等
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TProperty"></typeparam>
public class TreeBuildUtil<TEntity, TProperty> where TEntity : ITreeNode<TProperty>
    where TProperty : struct, IComparable, IConvertible
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