using System.Text.RegularExpressions;
using Fast.Admin.Model.Enum;
using Fast.Admin.Model.Model.Sys.Log;
using Fast.Admin.Model.Model.Sys.Menu;
using Fast.Admin.Model.Model.Tenant.Auth;
using Fast.Admin.Model.Model.Tenant.Organization;
using Fast.Admin.Model.Model.Tenant.Organization.User;
using Fast.Admin.Service.Auth.Dto;
using Fast.Admin.Service.SysMenu.Dto;
using Fast.Admin.Service.SysModule;
using Fast.Iaas.Cache;
using Fast.Iaas.Const;
using Fast.Iaas.Extension;
using Fast.Iaas.Internal;
using Fast.Iaas.Util;
using Fast.Iaas.Util.Http;
using Fast.SqlSugar.Extension;
using Fast.SqlSugar.Repository;
using Furion.DataEncryption;
using Furion.EventBus;

namespace Fast.Admin.Service.Auth;

/// <summary>
/// 租户授权服务
/// </summary>
public class TenAuthService : ITenAuthService, ITransient
{
    private readonly ISqlSugarClient _repository;
    private readonly ISqlSugarRepository<TenUserModel> _tenRepository;
    private readonly ICache _cache;
    private readonly IUser _user;
    private readonly IEventPublisher _eventPublisher;

    public TenAuthService(ISqlSugarClient repository, ISqlSugarRepository<TenUserModel> tenRepository, IUser user, ICache cache,
        IEventPublisher eventPublisher)
    {
        _repository = repository.AsTenant().GetConnection(Extension.GetDefaultDataBaseInfo().ConnectionId);
        _tenRepository = tenRepository;
        _user = user;
        _cache = cache;
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Web登录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task WebLogin(WebLoginInput input)
    {
        TenUserModel userInfo;

        // 判断登录方式
        switch (input.LoginMethod)
        {
            case LoginMethodEnum.Account:
                userInfo = await _tenRepository.Context.Queryable<TenUserModel>()
                    .LeftJoin<TenEmpModel>((t1, t2) => t1.Id == t2.SysUserId)
                    .Where((t1, t2) => t1.Account == input.Account && t1.Status != CommonStatusEnum.Delete)
                    .Select("t1.*, t2.JobNum").FirstAsync();
                break;
            case LoginMethodEnum.JobNum:
                userInfo = await _tenRepository.Context.Queryable<TenUserModel>()
                    .LeftJoin<TenEmpModel>((t1, t2) => t1.Id == t2.SysUserId)
                    .Where((t1, t2) => t2.JobNum == input.Account && t1.Status != CommonStatusEnum.Delete)
                    .Select("t1.*, t2.JobNum").FirstAsync();
                break;
            case LoginMethodEnum.Phone:
                userInfo = await _tenRepository.Context.Queryable<TenUserModel>()
                    .LeftJoin<TenEmpModel>((t1, t2) => t1.Id == t2.SysUserId)
                    .Where((t1, t2) => t1.Phone == input.Account && t1.Status != CommonStatusEnum.Delete)
                    .Select("t1.*, t2.JobNum").FirstAsync();
                break;
            case LoginMethodEnum.Email:
                // 判断是否为邮箱地址
                var emailRegex = new Regex(RegexConst.EmailAddress);
                if (!emailRegex.IsMatch(input.Account))
                {
                    throw Oops.Bah("不是一个有效的邮箱地址！");
                }

                userInfo = await _tenRepository.Context.Queryable<TenUserModel>()
                    .LeftJoin<TenEmpModel>((t1, t2) => t1.Id == t2.SysUserId)
                    .Where((t1, t2) => t1.Email == input.Account && t1.Status != CommonStatusEnum.Delete)
                    .Select("t1.*, t2.JobNum").FirstAsync();
                break;
            default:
                throw Oops.Bah("不是一个有效的登录方式！");
        }

        // 判断是否查询到了用户信息
        if (userInfo == null)
        {
            throw input.LoginMethod switch
            {
                LoginMethodEnum.Account => Oops.Bah("账号不存在！"),
                LoginMethodEnum.JobNum => Oops.Bah("工号不存在！"),
                LoginMethodEnum.Phone => Oops.Bah("手机号码不存在！"),
                LoginMethodEnum.Email => Oops.Bah("邮箱地址不存在！"),
                _ => Oops.Bah("账号不存在！")
            };
        }

        // 验证账号状态
        if (userInfo.Status == CommonStatusEnum.Disable)
        {
            throw Oops.Bah("账号已经被停用！");
        }

        /*
         * 连续错误5次，冻结1分钟
         * 连续错误10次，冻结5分钟
         * 连续错误15次，冻结30分钟
         * 连续错误20次，冻结60分钟
         * 连续错误25次，冻结120分钟
         * 连续错误30次，冻结账号
         * 登录成功后消除缓存
         */
        var errorPasswordCacheKey = $"{CacheConst.InputErrorPassword}{userInfo.Id}";
        // 判断密码是否正确
        if (userInfo.Password != MD5Encryption.Encrypt(input.Password))
        {
            // 记录密码错误次数
            var errorPasswordDto = await _cache.GetAsync<InputErrorPasswordDto>(errorPasswordCacheKey) ??
                                   new InputErrorPasswordDto {Count = 0, FreezeMinutes = 0, ThawingTime = null};

            // 错误次数+1
            errorPasswordDto.Count++;

            // 判断是否为5的倍数
            if (errorPasswordDto.Count == 30)
            {
                // 错误30次，直接冻结账号
                userInfo.Status = CommonStatusEnum.Disable;
                await _tenRepository.UpdateAsync(userInfo);
                await _cache.SetAsync(errorPasswordCacheKey, errorPasswordDto);
                throw Oops.Bah("密码连续输入错误30次，账号已被停用，请联系管理员！");
            }

            var time = DateTime.Now;

            // 判断是否存在解冻时间
            if (errorPasswordDto.ThawingTime != null && errorPasswordDto.ThawingTime.Value > time)
            {
                var thawingMinutes = (errorPasswordDto.ThawingTime.Value - time).TotalMinutes.ParseToInt();
                if (thawingMinutes == 0)
                {
                    thawingMinutes = 1;
                }

                throw Oops.Bah(
                    $"密码连续输入错误{errorPasswordDto.Count - 1}次，已被冻结{errorPasswordDto.FreezeMinutes}分钟，请{thawingMinutes}分钟后再重试！");
            }

            if (errorPasswordDto.Count % 5 != 0)
            {
                errorPasswordDto.ThawingTime = null;
                await _cache.SetAsync(errorPasswordCacheKey, errorPasswordDto);
                throw Oops.Bah("密码不正确！");
            }

            // 解冻分钟数
            var addMinutes = errorPasswordDto.Count switch
            {
                5 => 1,
                10 => 5,
                15 => 30,
                20 => 60,
                25 => 120,
                _ => 0
            };
            errorPasswordDto.FreezeMinutes = addMinutes;
            // 解冻时间
            errorPasswordDto.ThawingTime = DateTime.Now.AddMinutes(addMinutes);
            await _cache.SetAsync(errorPasswordCacheKey, errorPasswordDto);
            throw Oops.Bah($"密码连续输入错误{errorPasswordDto.Count}次，已被冻结{addMinutes}分钟，请{addMinutes}分钟后再重试！");
        }

        // 删除记录的错误密码次数
        await _cache.DelAsync(errorPasswordCacheKey);

        // 登录
        await _user.LoginAsync(new AuthUserInfo(_user.TenantId, userInfo.Id, userInfo.Account, userInfo.JobNum, userInfo.UserName,
            userInfo.AdminType == AdminTypeEnum.SuperAdmin, userInfo.AdminType == AdminTypeEnum.SystemAdmin,
            userInfo.AdminType == AdminTypeEnum.TenantAdmin));

        // 更新最后登录时间和Ip
        userInfo.LastLoginIp = HttpUtil.Ip;
        userInfo.LastLoginTime = DateTime.Now;
        await _eventPublisher.PublishAsync(new FastChannelEventSource("Update:UserLoginInfo", _user.TenantId, userInfo));
    }

    /// <summary>
    /// 获取登录用户信息
    /// </summary>
    /// <returns></returns>
    public async Task<GetLoginUserInfoOutput> GetLoginUserInfo()
    {
        // 登录用户Id
        var userId = _user.UserId;

        var result = await _tenRepository.Context.Queryable<TenUserModel>()
            .LeftJoin<TenEmpModel>((t1, t2) => t1.Id == t2.SysUserId).LeftJoin<TenOrgModel>((t1, t2, t3) => t2.OrgId == t3.Id)
            .LeftJoin<TenPositionModel>((t1, t2, t3, t4) => t2.PositionId == t4.Id)
            .LeftJoin<TenRankModel>((t1, t2, t3, t4, t5) => t2.RankId == t5.Id).Where(t1 => t1.Id == userId).Select(
                (t1, t2, t3, t4, t5) => new GetLoginUserInfoOutput
                {
                    Account = t1.Account,
                    Name = t1.UserName,
                    NickName = t1.NickName,
                    Avatar = t1.Avatar,
                    Email = t1.Email,
                    Phone = t1.Phone,
                    Tel = t1.Tel,
                    LastLoginIp = t1.LastLoginIp,
                    LastLoginTime = t1.LastLoginTime,
                    AdminType = t1.AdminType,
                    JobNum = t2.JobNum,
                    OrgName = t3.OrgName,
                    PositionName = t4.PositionName,
                    RankName = t5.RankName
                }).FirstAsync();

        // 查询其余机构和职位
        var otherOrgAndPositionList = await _tenRepository.Context.Queryable<TenUserOrgModel>()
            .LeftJoin<TenOrgModel>((t1, t2) => t1.OrgId == t2.Id)
            .LeftJoin<TenPositionModel>((t1, t2, t3) => t1.PositionId == t3.Id).Where(t1 => t1.SysUserId == userId)
            .Select((t1, t2, t3) => new GetLoginUserInfoOutput {OrgName = t2.OrgName, PositionName = t3.PositionName})
            .ToListAsync();
        result.OrgNameList = otherOrgAndPositionList.Select(sl => sl.OrgName).ToList();
        result.PositionNameList = otherOrgAndPositionList.Select(sl => sl.PositionName).ToList();

        // 查询角色名称
        result.RoleNameList = await _tenRepository.Context.Queryable<TenUserRoleModel>()
            .LeftJoin<TenRoleModel>((t1, t2) => t1.SysRoleId == t2.Id).Where(t1 => t1.SysUserId == userId)
            .Select((t1, t2) => t2.RoleName).ToListAsync();

        // 判断是否为超级管理员
        if (_user.IsSuperAdmin)
        {
            result.ButtonCodeList = null;
        }
        // 判断是否为系统管理员
        else if (_user.IsSystemAdmin)
        {
            result.ButtonCodeList = null;
        }
        // 判断是否为租户管理员或者普通用户
        else
        {
            // 查询角色授权按钮
            var roleButtonIdList = await _tenRepository.Context.Queryable<TenUserRoleModel>()
                .LeftJoin<TenRoleAuthButtonModel>((t1, t2) => t1.SysRoleId == t2.SysRoleId).Where(t1 => t1.SysUserId == userId)
                .Select((t1, t2) => t2.SysButtonId).ToListAsync();
            // 查询用户授权按钮
            var userButtonIdList = await _tenRepository.Context.Queryable<TenUserAuthButtonModel>()
                .Where(wh => wh.SysUserId == userId).Select(sl => sl.SysButtonId).ToListAsync();
            var buttonIdList = new List<long>();
            buttonIdList.AddRange(roleButtonIdList);
            buttonIdList.AddRange(userButtonIdList);
            // 查询授权按钮Code
            result.ButtonCodeList = await _repository.Queryable<SysButtonModel>()
                .LeftJoin<SysMenuModel>((t1, t2) => t1.MenuId == t2.Id)
                .Where((t1, t2) => !SqlFunc.IsNullOrEmpty(t2.Id) && buttonIdList.Contains(t1.Id))
                .Select((t1, t2) => $"{t2.MenuCode}:{t1.ButtonCode}").ToListAsync();
        }

        var visLog = new SysLogVisModel
        {
            Account = result.Account,
            UserName = result.Name,
            Location = HttpUtil.RequestUrlAddress,
            VisType = LoginTypeEnum.Login,
            VisTime = DateTime.Now
        };
        visLog.RecordCreate();
        // 访问日志
        await _eventPublisher.PublishAsync(new FastChannelEventSource("Create:VisLog", _user.TenantId, visLog));

        return result;
    }

    /// <summary>
    /// 获取登录菜单
    /// </summary>
    /// <returns></returns>
    public async Task<List<AntDesignRouterOutput>> GetLoginMenu()
    {
        var userId = _user.UserId;

        // 先从缓存中获取
        var result = await _cache.GetAsync<List<AntDesignRouterOutput>>($"{CacheConst.AuthSysMenu}{userId}");

        // 判断缓存中是否存在
        if (result != null && result.Any())
            return new TreeBuildUtil<AntDesignRouterOutput>().Build(result);

        // 查询模块
        var moduleList = await App.GetService<ISysModuleService>().QuerySysModuleSelector();
        var moduleIdList = new List<long>();
        List<SysMenuTreeOutput> menuList;

        // 判断是否为超级管理员
        if (_user.IsSuperAdmin)
        {
            // 查询所有的菜单
            menuList = await _repository.Queryable<SysMenuModel>().Where(wh => wh.Status == CommonStatusEnum.Enable)
                .Select<SysMenuTreeOutput>().ToListAsync();
        }
        // 判断是否为系统管理员
        else if (_user.IsSystemAdmin)
        {
            // 模块Id
            moduleIdList = moduleList.Select(sl => sl.Id).ToList();

            // 查询所有的菜单
            menuList = await _repository.Queryable<SysMenuModel>()
                .Where(wh => wh.Status == CommonStatusEnum.Enable && moduleIdList.Contains(wh.ModuleId))
                .Select<SysMenuTreeOutput>().ToListAsync();
        }
        else
        {
            // 查询角色授权菜单
            var roleMenuIdList = await _repository.Queryable<TenUserRoleModel>()
                .LeftJoin<TenRoleAuthMenuModel>((t1, t2) => t1.SysRoleId == t2.SysRoleId).Where(t1 => t1.SysUserId == userId)
                .Select((t1, t2) => t2.SysMenuId).ToListAsync();
            // 查询用户授权菜单
            var userMenuIdList = await _repository.Queryable<TenUserAuthMenuModel>().Where(wh => wh.SysUserId == userId)
                .Select(sl => sl.SysMenuId).ToListAsync();
            var menuIdList = new List<long>();
            menuIdList.AddRange(roleMenuIdList);
            menuIdList.AddRange(userMenuIdList);
            // 查询授权菜单
            menuList = await _repository.Queryable<SysMenuModel>()
                .Where(wh => wh.Status == CommonStatusEnum.Enable && moduleIdList.Contains(wh.ModuleId) &&
                             menuIdList.Contains(wh.Id)).Select<SysMenuTreeOutput>().ToListAsync();
        }

        result = new List<AntDesignRouterOutput>();

        // 循环所有模块
        foreach (var moduleInfo in moduleList)
        {
            // 循环菜单
            foreach (var menuInfo in menuList.Where(wh => wh.ModuleId == moduleInfo.Id))
            {
                // 顶级菜单
                if (menuInfo.ParentId == 0)
                {
                    // 顶级菜单父级Id为模块的Id
                    menuInfo.ParentId = moduleInfo.Id;
                }

                result.Add(new AntDesignRouterOutput
                {
                    Id = menuInfo.Id,
                    Name = menuInfo.MenuName,
                    ParentId = menuInfo.ParentId,
                    ModuleId = menuInfo.ModuleId,
                    Sort = menuInfo.Sort,
                    Path = menuInfo.MenuType switch
                    {
                        MenuTypeEnum.Outside => menuInfo.Link,
                        MenuTypeEnum.Catalog => $"/{menuInfo.Id}",
                        _ => $"{menuInfo.Router}?menuCode={menuInfo.MenuCode}"
                    },
                    Component = menuInfo.Component,
                    Type = menuInfo.MenuType,
                    Meta = new AntDesignRouterMetaOutput
                    {
                        Title = menuInfo.MenuTitle,
                        Icon = menuInfo.Icon,
                        Show = menuInfo.Visible == YesOrNotEnum.Y,
                        Target = menuInfo.MenuType == MenuTypeEnum.Outside ? "_blank" : "",
                        Link = menuInfo.Link
                    }
                });
            }

            result.Add(new AntDesignRouterOutput
            {
                Id = moduleInfo.Id,
                Name = moduleInfo.ModuleName,
                ParentId = 0,
                ModuleId = 0,
                Sort = moduleInfo.Sort,
                Path = $"/{moduleInfo.Id}",
                Component = "",
                Type = MenuTypeEnum.Catalog,
                Meta = new AntDesignRouterMetaOutput
                {
                    Title = moduleInfo.ModuleName,
                    Icon = moduleInfo.Icon,
                    Show = true,
                    Target = "",
                    Link = ""
                }
            });
        }

        // 放入缓存
        await _cache.SetAsync($"{CacheConst.AuthSysMenu}{userId}", result);

        // 构建树形菜单
        return new TreeBuildUtil<AntDesignRouterOutput>().Build(result);
    }
}