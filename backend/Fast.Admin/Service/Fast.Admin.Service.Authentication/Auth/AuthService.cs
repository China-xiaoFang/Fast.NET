// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
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

using Fast.Admin.Core.Authentication;
using Fast.Admin.Core.Entity.System.Account;
using Fast.Admin.Entity.Tenant.Organization;
using Fast.Admin.Service.Authentication.Auth.Dto;
using Fast.DependencyInjection;
using Fast.IaaS;
using Fast.SqlSugar.Repository;
using SqlSugar;

namespace Fast.Admin.Service.Authentication.Auth;

/// <summary>
/// <see cref="AuthService"/> 授权服务
/// </summary>
public class AuthService : IAuthService, ITransientDependency
{
    private readonly IUser _user;
    private readonly ISqlSugarClient _repository;
    private readonly ISqlSugarRepository<TenUserModel> _tenRepository;

    public AuthService(IUser user, ISqlSugarClient repository, ISqlSugarRepository<TenUserModel> tenRepository)
    {
        _user = user;
        _repository = repository;
        _tenRepository = tenRepository;
    }

    /// <summary>
    /// 获取登录用户信息
    /// </summary>
    /// <returns></returns>
    /// <exception cref="UserFriendlyException"></exception>
    public async Task<GetLoginUserInfoOutput> GetLoginUserInfo()
    {
        // 登录用户Id
        var userId = _user.UserId;
        var tenantId = _user.TenantId;

        var tenantAccount = await _repository.Queryable<SysTenantAccountModel>().Includes(e => e.SysAccount)
            .Where(wh => wh.TenantId == tenantId && wh.UserId == userId).FirstAsync();

        if (tenantAccount == null)
        {
            throw new UserFriendlyException("系统账号信息不存在！");
        }

        var user = await _tenRepository.FirstOrDefaultAsync(wh => wh.Id == userId);

        if (user == null)
        {
            throw new UserFriendlyException("租户用户信息不存在！");
        }

        // TODO：其余的信息查询

        var result = new GetLoginUserInfoOutput
        {
            Account = tenantAccount.SysAccount.Account,
            JobNumber = user.JobNumber,
            UserName = tenantAccount.SysAccount.UserName,
            NickName = user.NickName,
            Avatar = user.Avatar,
            Birthday = tenantAccount.SysAccount.Birthday,
            Sex = tenantAccount.SysAccount.Sex,
            Email = tenantAccount.SysAccount.Email,
            Mobile = tenantAccount.SysAccount.Mobile,
            Tel = tenantAccount.SysAccount.Tel,
            DepartmentId = user.DepartmentId,
            DepartmentName = user.DepartmentName,
            AdminType = user.AdminType,
            LastLoginDevice = user.LastLoginDevice,
            LastLoginOS = user.LastLoginOS,
            LastLoginBrowser = user.LastLoginBrowser,
            LastLoginProvince = user.LastLoginProvince,
            LastLoginCity = user.LastLoginCity,
            LastLoginIp = user.LastLoginIp,
            LastLoginTime = user.LastLoginTime,
        };

        return result;
    }
}