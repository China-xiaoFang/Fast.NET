using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Fast.Admin.Model.BaseModel.Interface;
using Fast.Admin.Model.Enum;
using Fast.Admin.Model.Model.Sys;
using Fast.Admin.Model.Model.Tenant.Auth.DataScope;
using Fast.Admin.Model.Model.Tenant.Organization;
using Fast.Admin.Model.Model.Tenant.Organization.User;
using Fast.Admin.Service.Tenant.Dto;
using Fast.Core.CodeFirst;
using Fast.Core.CodeFirst.Internal;
using Fast.Core.Restful.Extension;
using Fast.Core.Restful.Internal;
using Fast.Core.SqlSugar.Extension;
using Fast.Core.SqlSugar.Helper;
using Fast.Core.SqlSugar.Internal.Dto;
using Furion.DataEncryption;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Fast.Admin.Service.Tenant;

/// <summary>
/// 租户服务
/// </summary>
public class SysTenantService : ISysTenantService, ITransient
{
    private readonly ISqlSugarRepository<SysTenantModel> _repository;
    private readonly ICache _cache;
    private readonly ISqlSugarClient _tenant;

    public SysTenantService(ISqlSugarRepository<SysTenantModel> repository, ICache cache, ISqlSugarClient tenant)
    {
        _repository = repository;
        _cache = cache;
        _tenant = tenant;
    }

    /// <summary>
    /// 获取所有租户信息
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    [NonAction]
    public async Task<List<SysTenantModel>> GetAllTenantInfo(Expression<Func<SysTenantModel, bool>> predicate = null)
    {
        // 先从缓存获取
        var tenantList = await _cache.GetAsync<List<SysTenantModel>>(CacheConst.TenantInfo);

        if (tenantList != null && tenantList.Any())
            return predicate == null ? tenantList : tenantList.Where(predicate.Compile()).ToList();

        // 获取租户基本信息
        // 这里导航查询过滤器不能禁用，所以手动写
        //tenantList = await _tenant.Queryable<SysTenantModel>().ClearFilter().Where(wh => wh.IsDeleted == false)
        //    .Includes(app => app.AppList.Where(wh => wh.IsDeleted == false).ToList())
        //    .Includes(db => db.DataBaseList.Where(wh => wh.IsDeleted == false).ToList()).ToListAsync();
        tenantList = await _tenant.Queryable<SysTenantModel>().ToListAsync();
        var tenantIdList = tenantList.Select(sl => sl.Id).ToList();
        var appList = await _tenant.Queryable<SysTenantAppInfoModel>().ClearFilter<IBaseTenant>()
            .Where(wh => tenantIdList.Contains(wh.TenantId)).ToListAsync();
        var dbList = await _tenant.Queryable<SysTenantDataBaseModel>().ClearFilter<IBaseTenant>()
            .Where(wh => tenantIdList.Contains(wh.TenantId)).ToListAsync();
        foreach (var item in tenantList)
        {
            item.AppList = appList.Where(wh => wh.TenantId == item.Id).ToList();
            item.DataBaseList = dbList.Where(wh => wh.TenantId == item.Id).ToList();
        }

        // 获取租户两个管理员信息
        // 注：这里如果租户过多的话可能存在卡顿
        foreach (var tenant in tenantList)
        {
            // 加载租户数据库Db
            var (_db,_) = _tenant.LoadSqlSugar<TenUserModel>(_cache, tenant.Id);
            // 查询两个管理员信息
            var userList = await _db.Queryable<TenUserModel>().Where(wh =>
                (wh.AdminType == AdminTypeEnum.SystemAdmin || wh.AdminType == AdminTypeEnum.TenantAdmin)).ToListAsync();
            tenant.SystemAdminUser = userList.First(f => f.AdminType == AdminTypeEnum.SystemAdmin);
            tenant.TenantAdminUser = userList.First(f => f.AdminType == AdminTypeEnum.TenantAdmin);
        }

        await _cache.SetAsync(CacheConst.TenantInfo, tenantList);

        return predicate == null ? tenantList : tenantList.Where(predicate.Compile()).ToList();
    }

    /// <summary>
    /// Web站点初始化
    /// </summary>
    /// <returns></returns>
    public async Task<WebSiteInitOutput> WebSiteInit()
    {
        // webUel
        var webUrl = GlobalContext.OriginUrl;

        // 根据主机Host判断，是否存在该租户
        var tenantList = await GetAllTenantInfo(wh =>
            wh.AppList.Any(appWh => appWh.AppType == AppTypeEnum.WebAdmin && appWh.AppKey == webUrl));

        if (tenantList is not {Count: > 0})
            throw Oops.Bah("租户授权信息不存在！");

        // 获取第一个
        var tenantInfo = tenantList[0];

        // 判断授权是否过期或者还没有到使用的日期
        var appList = tenantInfo.AppList.Where(wh => wh.AppType == AppTypeEnum.WebAdmin && wh.AppKey == webUrl)
            .OrderBy(ob => ob.AuthStartTime).ToList();
        if (!appList.Any())
        {
            throw Oops.Bah($"{AppTypeEnum.WebAdmin.GetDescription()}未授权使用，请联系客服进行授权！");
        }

        var time = DateTime.Now;

        // 判断是否已经过期
        if (appList[^1].AuthEndTime < time)
        {
            throw Oops.Bah($"{AppTypeEnum.WebAdmin.GetDescription()}授权已过期，请联系客服进行续费！");
        }

        // 判断是否还没到使用日期
        if (appList.Any(wh =>
                (wh.AuthStartTime < time && wh.AuthEndTime < time) || (wh.AuthStartTime > time && wh.AuthEndTime > time)))
        {
            throw Oops.Bah(
                $"{AppTypeEnum.WebAdmin.GetDescription()}授权使用开始时间为：{appList.FirstOrDefault(f => f.AuthEndTime < time)?.AuthStartTime:yyyy-MM-dd HH:mm:ss}");
        }

        var result = tenantInfo.Adapt<WebSiteInitOutput>();
        result.TenantId = tenantInfo.Id.ToString().ToBase64();

        return result;
    }

    /// <summary>
    /// 分页查询租户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PageResult<TenantOutput>> QueryTenantPageList(QueryTenantInput input)
    {
        return await _repository.AsQueryable().Where(wh => wh.TenantType != TenantTypeEnum.System)
            .WhereIF(!input.Name.IsEmpty(), wh => wh.ChName.Contains(input.Name))
            .WhereIF(!input.ShortName.IsEmpty(), wh => wh.ChShortName.Contains(input.ShortName))
            .WhereIF(!input.AdminName.IsEmpty(), wh => wh.AdminName.Contains(input.AdminName))
            .WhereIF(!input.Phone.IsEmpty(), wh => wh.Phone.Contains(input.Phone)).OrderBy(ob => ob.CreatedTime)
            .OrderByIF(input.IsOrderBy, input.OrderByStr).Select<TenantOutput>().ToXnPagedListAsync(input.PageNo, input.PageSize);
    }

    #region 租户操作

    /// <summary>
    /// 添加租户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task AddTenant(AddTenantInput input)
    {
        // WebUrl，必须是Https
        if (!App.WebHostEnvironment.IsDevelopment())
        {
            if (input.WebUrl.Any(url => !Regex.IsMatch(url, RegexConst.HttpsUrl)))
            {
                throw Oops.Bah("租户WebUrl必须是Https协议！");
            }
        }

        // 判断租户信息是否存在
        if (await _repository.AnyAsync(wh =>
                wh.ChName == input.Name || wh.ChShortName == input.ShortName || wh.Email == input.Email))
            throw Oops.Bah("已存在同名租户信息！");

        var model = input.Adapt<SysTenantModel>();
        model.Secret = StringUtil.GetGuid();
        model.TenantType = TenantTypeEnum.Common;

        // 判断租户密钥是否重复
        while (true)
        {
            if (await _repository.AnyAsync(wh => wh.Secret == model.Secret))
            {
                model.Secret = StringUtil.GetGuid();
            }
            else
            {
                break;
            }
        }

        await _repository.InsertAsync(model);
        // 删除缓存
        await _cache.DelAsync(CacheConst.TenantInfo);
    }

    /// <summary>
    /// 初始化租户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task InitTenantInfo(InitTenantInfoInput input)
    {
        // 判断是否存在租户
        var newTenantInfo = await _repository.FirstOrDefaultAsync(f => f.Id == input.Id);
        if (newTenantInfo.IsEmpty())
            throw Oops.Bah("租户信息不存在！");

        // 查询是否存在数据库信息
        if (!await _repository.Context.Queryable<SysTenantDataBaseModel>().AnyAsync(wh =>
                wh.TenantId == input.Id && wh.SugarSysDbType == SugarDbTypeEnum.Tenant.GetHashCode()))
            throw Oops.Bah("租户数据库信息不存在！");

        // 获取所有数据库Model
        var entityTypeList = EntityHelper.ReflexGetAllTEntityList();

        // 初始化数据
        await InitNewTenant(newTenantInfo,
            entityTypeList.Where(wh => wh.DbType == SugarDbTypeEnum.Tenant.GetHashCode()).ToList());

        // 删除缓存
        await _cache.DelAsync(CacheConst.TenantInfo);
    }

    /// <summary>
    /// 初始化新租户数据
    /// </summary>
    /// <param name="newTenant"></param>
    /// <param name="entityTypeList">初始化Entity类型</param>
    /// <param name="isInit">是否为初始化</param>
    /// <returns></returns>
    [NonAction]
    public async Task InitNewTenant(SysTenantModel newTenant, List<SugarEntityTypeInfo> entityTypeList, bool isInit = false)
    {
        var (_db,_)  = _tenant.LoadSqlSugar<TenUserModel>(_cache, newTenant.Id);

        // 初始化数据库
        _db.DbMaintenance.CreateDatabase();

        // 判断是否存在用户表
        if (await _db.Ado.GetIntAsync(
                $"SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_NAME = '{typeof(TenUserModel).GetSugarTableName()}'") >
            0)
            throw Oops.Bah("租户数据库已存在！");

        _db.CodeFirst.InitTables(entityTypeList!.Where(wh => wh.IsSplitTable == false).Select(sl => sl.Type).ToArray());
        _db.CodeFirst.SplitTables().InitTables(entityTypeList.Where(wh => wh.IsSplitTable).Select(sl => sl.Type).ToArray());

        // 初始化公司（组织架构）
        var newAdminOrg = new TenOrgModel
        {
            ParentId = 0,
            ParentIds = new List<long> {0},
            OrgName = newTenant.ChName,
            OrgCode = "org_hq",
            Contacts = newTenant.AdminName,
            Tel = newTenant.Phone
        };
        newAdminOrg = await _db.Insertable(newAdminOrg).ExecuteReturnEntityAsync();

        var newAdminRole = new TenRoleModel
        {
            RoleName = RoleTypeEnum.AdminRole.GetDescription(),
            RoleCode = "manager_role",
            Sort = 1,
            DataScopeType = DataScopeTypeEnum.All,
            RoleType = RoleTypeEnum.AdminRole
        };
        // 初始化租户管理员角色
        newAdminRole = await _db.Insertable(newAdminRole).ExecuteReturnEntityAsync();

        // 判断如果是初始化
        if (isInit)
        {
            // 初始化超级管理员
            await _db.Insertable(new TenUserModel
            {
                Id = CommonConst.DefaultSuperAdminId,
                Account = "SuperAdmin",
                Password = MD5Encryption.Encrypt(CommonConst.DefaultAdminPassword),
                UserName = "超级管理员",
                NickName = "超级管理员",
                Avatar = CommonConst.Default_Avatar_Url,
                Sex = GenderEnum.Unknown,
                Email = "superAdmin@18kboy.icu",
                Phone = "18888888888",
                AdminType = AdminTypeEnum.SuperAdmin
            }).ExecuteCommandAsync();
        }

        // 初始化系统默认账号，账号以邮箱号码为准
        await _db.Insertable(new TenUserModel
        {
            Account = StringUtil.GetShortGuid(),
            Password = MD5Encryption.Encrypt(CommonConst.DefaultAdminPassword),
            UserName = newTenant.AdminName,
            NickName = newTenant.AdminName,
            Avatar = CommonConst.Default_Avatar_Url,
            Sex = GenderEnum.Unknown,
            Email = newTenant.Email,
            Phone = newTenant.Phone,
            AdminType = AdminTypeEnum.Default
        }).ExecuteReturnEntityAsync();

        // 初始化租户系统管理员
        await _db.Insertable(new TenUserModel
        {
            Id = CommonConst.DefaultSystemAdminId,
            Account = "SystemAdmin",
            Password = MD5Encryption.Encrypt(CommonConst.DefaultAdminPassword),
            UserName = "系统管理员",
            NickName = "系统管理员",
            Avatar = CommonConst.Default_Avatar_Url,
            Sex = GenderEnum.Unknown,
            Email = "systemAdmin@18kboy.icu",
            Phone = "15188888888",
            AdminType = AdminTypeEnum.SystemAdmin
        }).ExecuteCommandAsync();

        // 初始化租户管理员，账号以邮箱号码为准
        var newAdminUser = new TenUserModel
        {
            Account = newTenant.Email,
            Password = MD5Encryption.Encrypt(CommonConst.DefaultAdminPassword),
            UserName = newTenant.AdminName,
            NickName = newTenant.AdminName,
            Avatar = CommonConst.Default_Avatar_Url,
            Sex = GenderEnum.Unknown,
            Email = newTenant.Email,
            Phone = newTenant.Phone,
            AdminType = AdminTypeEnum.TenantAdmin
        };
        newAdminUser = await _db.Insertable(newAdminUser).ExecuteReturnEntityAsync();

        // 初始化职工信息
        // TODO:这里的工号要自动生成
        await _db.Insertable(new TenEmpModel {SysUserId = newAdminUser.Id, JobNum = "10001", OrgId = newAdminOrg.Id})
            .ExecuteCommandAsync();

        // 初始化用户角色
        await _db.Insertable(new TenUserRoleModel {SysUserId = newAdminUser.Id, SysRoleId = newAdminRole.Id})
            .ExecuteCommandAsync();

        // 初始化用户数据范围
        await _db.Insertable(new TenUserDataScopeModel {SysUserId = newAdminUser.Id, SysOrgId = newAdminOrg.Id,})
            .ExecuteCommandAsync();

        // 初始化角色数据范围
        await _db.Insertable(new TenRoleDataScopeModel {SysRoleId = newAdminRole.Id, SysOrgId = newAdminOrg.Id,})
            .ExecuteCommandAsync();

        // 初始化业务库种子数据
        var seedDataTypes = SeedDataProgram.GetSeedDataType(typeof(ITenantSeedData));

        // 开启事务
        await _db.Ado.BeginTranAsync();
        try
        {
            SeedDataProgram.ExecSeedData(_db.Ado.Context, seedDataTypes);

            // 提交事务
            await _db.Ado.CommitTranAsync();
        }
        catch (Exception)
        {
            // 回滚事务
            await _db.Ado.RollbackTranAsync();
            throw;
        }
    }

    #endregion
}