namespace Fast.Authentication.Infrastructures;

/// <summary>
/// 授权用户信息
/// </summary>
public class AuthUserInfo
{
    public AuthUserInfo()
    {
    }

    public AuthUserInfo(long tenantId, long userId, string userAccount, string userJobNum, string userName, bool isSuperAdmin,
        bool isSystemAdmin, bool isTenantAdmin)
    {
        TenantId = tenantId;
        UserId = userId;
        UserAccount = userAccount;
        UserJobNum = userJobNum;
        UserName = userName;
        IsSuperAdmin = isSuperAdmin;
        IsSystemAdmin = isSystemAdmin;
        IsTenantAdmin = isTenantAdmin;
    }

    /// <summary>
    /// 当前租户Id
    /// </summary>
    public virtual long TenantId { get; }

    /// <summary>
    /// 当前用户Id
    /// </summary>
    public virtual long UserId { get; }

    /// <summary>
    /// 当前用户账号
    /// </summary>
    public virtual string UserAccount { get; }

    /// <summary>
    /// 当前用户工号
    /// </summary>
    public virtual string UserJobNum { get; }

    /// <summary>
    /// 当前用户名称
    /// </summary>
    public virtual string UserName { get; }

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    public virtual bool IsSuperAdmin { get; }

    /// <summary>
    /// 是否系统管理员
    /// </summary>
    public virtual bool IsSystemAdmin { get; }

    /// <summary>
    /// 是否租户管理员
    /// </summary>
    public virtual bool IsTenantAdmin { get; }
}