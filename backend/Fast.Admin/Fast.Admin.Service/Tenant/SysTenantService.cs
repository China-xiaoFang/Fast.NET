using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Fast.Admin.Service.Tenant.Dto;
using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.Core.AdminFactory.ModelFactory.Tenant;
using Fast.Core.CodeFirst;
using Fast.Core.CodeFirst.Internal;
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
        tenantList = await _tenant.Queryable<SysTenantModel>().Includes(app => app.AppList).Includes(db => db.DataBaseList)
            .ToListAsync();
        // 获取租户两个管理员信息
        // 注：这里如果租户过多的话可能存在卡顿
        foreach (var tenant in tenantList)
        {
            // 加载租户数据库Db
            var _db = _tenant.LoadSqlSugar<TenUserModel>(tenant.Id);
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
        var tenantList = await GetAllTenantInfo(wh => wh.WebUrl.Contains(webUrl));

        if (tenantList is not {Count: > 0})
            throw Oops.Bah("租户信息不存在！");

        // 获取第一个
        var tenantInfo = tenantList[0];

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
        return await _repository.Where(wh => wh.TenantType != TenantTypeEnum.System)
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
            if (input.WebUrl.Any(url => !Regex.IsMatch(url, CommonConst.RegexStr.HttpsUrl)))
            {
                throw Oops.Bah("租户WebUrl必须是Https协议！");
            }
        }

        // 判断租户信息是否存在
        if (await _repository.AnyAsync(wh =>
                wh.ChName == input.Name || wh.ChShortName == input.ShortName || wh.Email == input.Email))
            throw Oops.Bah("已存在同名租户信息！");

        // 判断租户的WebUrl是否存在
        if (await _repository.AnyAsync(wh => SqlFunc.ContainsArray(wh.WebUrl, input.WebUrl)))
            throw Oops.Bah("已存在同主机租户信息！");

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
            entityTypeList.Where(wh => wh.DbType == SugarDbTypeEnum.Tenant.GetHashCode()).Select(sl => sl.Type));

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
    public async Task InitNewTenant(SysTenantModel newTenant, IEnumerable<Type> entityTypeList, bool isInit = false)
    {
        var _db = _tenant.LoadSqlSugar<TenUserModel>(newTenant.Id);

        // 初始化数据库
        _db.DbMaintenance.CreateDatabase();

        // 判断是否存在用户表
        if (await _db.Ado.GetIntAsync(
                $"SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_NAME = '{typeof(TenUserModel).GetSugarTableName()}'") >
            0)
            throw Oops.Bah("租户数据库已存在！");

        _db.CodeFirst.InitTables(entityTypeList.ToArray());

        // 初始化公司（组织架构）
        var newAdminOrg = new TenOrgModel
        {
            ParentId = 0,
            ParentIds = new List<long> {0},
            Name = newTenant.ChName,
            Code = "org_hq",
            Contacts = newTenant.AdminName,
            Tel = newTenant.Phone
        };
        newAdminOrg = await _db.Insertable(newAdminOrg).ExecuteReturnEntityAsync();

        var newAdminRole = new TenRoleModel
        {
            Name = RoleTypeEnum.AdminRole.GetDescription(),
            Code = "manager_role",
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
                Id = ClaimConst.DefaultSuperAdminId,
                Account = "SuperAdmin",
                Password = MD5Encryption.Encrypt(CommonConst.DefaultAdminPassword),
                Name = "超级管理员",
                NickName = "超级管理员",
                Avatar = CommonConst.Default_Avatar_Url,
                Sex = GenderEnum.Unknown,
                Email = "superAdmin@18kboy.icu",
                Phone = "18888888888",
                AdminType = AdminTypeEnum.SuperAdmin
            }).ExecuteCommandAsync();
        }

        // 初始化租户系统管理员
        await _db.Insertable(new TenUserModel
        {
            Id = ClaimConst.DefaultSystemAdminId,
            Account = "SystemAdmin",
            Password = MD5Encryption.Encrypt(CommonConst.DefaultAdminPassword),
            Name = "系统管理员",
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
            Name = newTenant.AdminName,
            NickName = newTenant.AdminName,
            Avatar = CommonConst.Default_Avatar_Url,
            Sex = GenderEnum.Unknown,
            Email = newTenant.Email,
            Phone = newTenant.Phone,
            AdminType = AdminTypeEnum.TenantAdmin
        };
        newAdminUser = await _db.Insertable(newAdminUser).ExecuteReturnEntityAsync();

        // 初始化职工
        await _db.Insertable(new TenEmpModel {Id = newAdminUser.Id, JobNum = "10001", OrgId = newAdminOrg.Id})
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

        await _cache.DelAsync(CacheConst.UserDataScope);
        await _cache.DelAsync(CacheConst.DataScope);

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