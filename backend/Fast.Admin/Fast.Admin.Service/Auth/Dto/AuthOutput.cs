using System.Collections;

namespace Fast.Admin.Service.Auth.Dto;

/// <summary>
/// 获取登录用户信息输出
/// </summary>
public class GetLoginUserInfoOutput
{
    /// <summary>
    /// 账号
    /// </summary>
    public string Account { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    public string NickName { get; set; }

    /// <summary>
    /// 头像
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 手机
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    public string Tel { get; set; }

    /// <summary>
    /// 最后登录Ip
    /// </summary>
    public string LastLoginIp { get; set; }

    /// <summary>
    /// 最后登录时间
    /// </summary>
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// 管理员类型
    /// </summary>
    public AdminTypeEnum AdminType { get; set; }

    /// <summary>
    /// 工号
    /// </summary>
    public string JobNum { get; set; }

    /// <summary>
    /// 机构名称
    /// </summary>
    public string OrgName { get; set; }

    /// <summary>
    /// 机构名称集合
    /// </summary>
    public List<string> OrgNameList { get; set; }

    /// <summary>
    /// 职位名称
    /// </summary>
    public string PositionName { get; set; }

    /// <summary>
    /// 职位名称集合
    /// </summary>
    public List<string> PositionNameList { get; set; }

    /// <summary>
    /// 职级名称
    /// </summary>
    public string RankName { get; set; }

    /// <summary>
    /// 角色名称集合
    /// </summary>
    public List<string> RoleNameList { get; set; }

    /// <summary>
    /// 按钮编码集合
    /// </summary>
    public List<string> ButtonCodeList { get; set; }

    /// <summary>
    /// 菜单集合
    /// </summary>
    public List<AntDesignRouterOutput> MenuList { get; set; }
}

/// <summary>
/// AntDV路由信息输出
/// </summary>
public class AntDesignRouterOutput : ITreeNode
{
    /// <summary>
    /// 获取节点id
    /// </summary>
    /// <returns></returns>
    public long GetId()
    {
        return Id;
    }

    /// <summary>
    /// 获取节点父id
    /// </summary>
    /// <returns></returns>
    public long GetPid()
    {
        return ParentId;
    }

    /// <summary>
    /// 获取排序字段
    /// </summary>
    /// <returns></returns>
    int ITreeNode.Sort()
    {
        return Sort;
    }

    /// <summary>
    /// 设置Children
    /// </summary>
    /// <param name="children"></param>
    public void SetChildren(IList children)
    {
        Children = (List<AntDesignRouterOutput>) children;
    }

    public long Id { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 父级Id
    /// </summary>
    public long ParentId { get; set; }

    /// <summary>
    /// 模块Id
    /// </summary>
    public long ModuleId { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 路由地址
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// 组件地址
    /// </summary>
    public string Component { get; set; }

    /// <summary>
    /// AntDV路由元信息
    /// </summary>
    public AntDesignRouterMetaOutput Meta { get; set; }

    /// <summary>
    /// 子节点
    /// </summary>
    public List<AntDesignRouterOutput> Children { get; set; }
}

/// <summary>
/// AntDV路由元信息类
/// </summary>
public class AntDesignRouterMetaOutput
{
    /// <summary>
    /// 路由标题, 用于显示面包屑, 页面标题 *推荐设置
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// 是否可见
    /// </summary>
    public bool Show { get; set; }

    /// <summary>
    /// 如需外部打开，增加：_blank
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// 内链打开http链接
    /// </summary>
    public string Link { get; set; }
}