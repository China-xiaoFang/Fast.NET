using Fast.Admin.Model.Enum;
using Fast.Admin.Model.Model.Sys.Menu;
using Fast.Admin.Model.Model.Tenant.Auth;
using Fast.Admin.Model.Model.Tenant.Organization.User;
using Fast.Admin.Service.SysModule.Dto;
using Fast.Core.Restful.Extension;
using Fast.Core.Restful.Internal;
using Fast.SDK.Common.Cache;

namespace Fast.Admin.Service.SysModule;

/// <summary>
/// 系统模块服务
/// </summary>
public class SysModuleService : ISysModuleService, ITransient
{
    private readonly ISqlSugarRepository<SysModuleModel> _repository;
    private readonly ICache _cache;

    public SysModuleService(ISqlSugarRepository<SysModuleModel> repository, ICache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    /// <summary>
    /// 分页查询系统模块信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PageResult<SysModuleOutput>> QuerySysModulePageList(QuerySysModuleInput input)
    {
        return await _repository.AsQueryable()
            .WhereIF(!input.ModuleName.IsEmpty(), wh => wh.ModuleName.Contains(input.ModuleName))
            .WhereIF(!input.ViewType.IsNullOrZero(), wh => wh.ViewType == input.ViewType)
            .WhereIF(!input.IsSystem.IsNullOrZero(), wh => wh.IsSystem == input.IsSystem)
            .WhereIF(!input.Status.IsNullOrZero(), wh => wh.Status == input.Status).OrderBy(ob => ob.Sort)
            .OrderByIF(input.IsOrderBy, input.OrderByStr).Select<SysModuleOutput>()
            .ToXnPagedListAsync(input.PageNo, input.PageSize);
    }

    /// <summary>
    /// 查询系统模块选择器
    /// </summary>
    /// <returns></returns>
    public async Task<List<SysModuleOutput>> QuerySysModuleSelector()
    {
        // 从缓存中读取
        var result = await _cache.GetAsync<List<SysModuleModel>>($"{CacheConst.AuthModule}{GlobalContext.UserId}");

        // 判断缓存中是否存在
        if (result != null && result.Any())
            return result.Adapt<List<SysModuleOutput>>();

        // 判断是否为超级管理员
        if (GlobalContext.IsSuperAdmin)
        {
            // 查询所有的模块
            result = await _repository.AsQueryable(wh => wh.Status == CommonStatusEnum.Enable).OrderBy(ob => ob.Sort)
                .ToListAsync();
        }
        // 判断是否为系统管理员
        else if (GlobalContext.IsSystemAdmin)
        {
            // 查询所有的非超级管理员查看的模块
            result = await _repository
                .AsQueryable(wh => wh.Status == CommonStatusEnum.Enable && wh.ViewType != ModuleViewTypeEnum.SuperAdmin)
                .OrderBy(ob => ob.Sort).ToListAsync();
        }
        else
        {
            // 查询角色授权菜单
            var roleMenuIdList = await _repository.Context.Queryable<TenUserRoleModel>()
                .LeftJoin<TenRoleAuthMenuModel>((t1, t2) => t1.SysRoleId == t2.SysRoleId)
                .Where(t1 => t1.SysUserId == GlobalContext.UserId).Select((t1, t2) => t2.SysMenuId).ToListAsync();
            // 查询用户授权菜单
            var userMenuIdList = await _repository.Context.Queryable<TenUserAuthMenuModel>()
                .Where(wh => wh.SysUserId == GlobalContext.UserId).Select(sl => sl.SysMenuId).ToListAsync();
            var menuIdList = new List<long>();
            menuIdList.AddRange(roleMenuIdList);
            menuIdList.AddRange(userMenuIdList);
            // 判断是否为租户管理员
            if (GlobalContext.IsTenantAdmin)
            {
                // 查询授权菜单的模块信息
                result = await _repository.Context.Queryable<SysModuleModel>()
                    .LeftJoin<SysMenuModel>((t1, t2) => t1.Id == t2.ModuleId).Where((t1, t2) =>
                        t1.Status == CommonStatusEnum.Enable && t1.ViewType != ModuleViewTypeEnum.SuperAdmin &&
                        t1.ViewType != ModuleViewTypeEnum.SystemAdmin && !SqlFunc.IsNullOrEmpty(t2.Id) &&
                        menuIdList.Contains(t2.Id)).OrderBy(t1 => t1.Sort).Select<SysModuleModel>("t1.*").Distinct()
                    .ToListAsync();
            }
            else
            {

                // 查询授权菜单的模块信息，非管理员查看的
                result = await _repository.Context.Queryable<SysModuleModel>()
                    .LeftJoin<SysMenuModel>((t1, t2) => t1.Id == t2.ModuleId).Where((t1, t2) =>
                        t1.Status == CommonStatusEnum.Enable && t1.ViewType == ModuleViewTypeEnum.All &&
                        !SqlFunc.IsNullOrEmpty(t2.Id) && menuIdList.Contains(t2.Id)).OrderBy(t1 => t1.Sort)
                    .Select<SysModuleModel>("t1.*").Distinct().ToListAsync();
            }
        }

        // 放入缓存
        await _cache.SetAsync($"{CacheConst.AuthModule}{GlobalContext.UserId}", result);

        return result.Adapt<List<SysModuleOutput>>();
    }

    /// <summary>
    /// 添加系统模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task AddModule(AddModuleInput input)
    {
        // 模块名称不能重复
        if (await _repository.IsExistsAsync(wh => wh.ModuleName == input.ModuleName))
        {
            throw Oops.Bah("模块名称不能重复！");
        }

        // 转换Model
        var model = input.Adapt<SysModuleModel>();

        // 默认为启用的
        model.Status = CommonStatusEnum.Enable;

        // 添加数据库
        await _repository.InsertAsync(model);
    }

    /// <summary>
    /// 更新系统模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task UpdateModule(UpdateModuleInput input)
    {
        // 模块名称不能重复
        if (await _repository.IsExistsAsync(wh => wh.Id != input.Id && wh.ModuleName == input.ModuleName))
        {
            throw Oops.Bah("模块名称不能重复！");
        }

        // 查询源数据
        var model = await _repository.FirstOrDefaultAsync(f => f.Id == input.Id);

        if (model == null)
        {
            throw Oops.Bah("数据不存在！");
        }

        // 系统模块不能修改查看类型和名称
        if (model.IsSystem == YesOrNotEnum.Y)
        {
            if (model.ViewType != input.ViewType)
            {
                throw Oops.Bah("系统级别数据不允许修改查看类型！");
            }

            if (model.IsSystem != input.IsSystem)
            {
                throw Oops.Bah("系统级别数据不允许修改系统级别！");
            }
        }

        // 覆盖源数据
        model = input.Adapt(model);

        // 更新数据
        await _repository.Context.Updateable(model).ExecuteCommandWithOptLockAsync(true);
    }
}