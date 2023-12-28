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


using Fast.Admin.Core.Enums;

namespace Fast.Admin.Core.Authentication;

/// <summary>
/// <see cref="IUser"/> 授权用户信息
/// <remarks>作用域注册，保证当前请求管道中是唯一的，并且只会加载一次</remarks>
/// </summary>
public interface IUser
{
    /// <summary>
    /// 租户Id
    /// </summary>
    long TenantId { get; }

    /// <summary>
    /// 租户编号
    /// </summary>
    string TenantNo { get; }

    /// <summary>
    /// 用户Id
    /// </summary>
    long UserId { get; }

    /// <summary>
    /// 用户账号
    /// </summary>
    string UserAccount { get; }

    /// <summary>
    /// 用户工号
    /// </summary>
    string UserJobNo { get; }

    /// <summary>
    /// 用户名称
    /// </summary>
    string UserName { get; }

    /// <summary>
    /// 部门Id
    /// </summary>
    long DepartmentId { get; }

    /// <summary>
    /// 部门名称
    /// </summary>
    string DepartmentName { get; }

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    bool IsSuperAdmin { get; }

    /// <summary>
    /// 是否系统管理员
    /// </summary>
    bool IsSystemAdmin { get; }

    /// <summary>
    /// 是否租户管理员
    /// </summary>
    bool IsTenantAdmin { get; }

    /// <summary>
    /// App 运行环境
    /// </summary>
    AppEnvironmentEnum AppEnvironment { get; }

    /// <summary>
    /// 统一登录
    /// </summary>
    /// <param name="authUserInfo"></param>
    /// <returns></returns>
    Task Login(AuthUserInfo authUserInfo);

    /// <summary>
    /// 统一退出登录
    /// </summary>
    /// <returns></returns>
    Task Logout();
}