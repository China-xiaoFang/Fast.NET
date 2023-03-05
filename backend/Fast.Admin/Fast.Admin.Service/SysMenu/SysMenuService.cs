using Fast.Admin.Service.SysMenu.Dto;
using Fast.Core.AdminFactory.ModelFactory.Sys;

namespace Fast.Admin.Service.SysMenu;

/// <summary>
/// 系统菜单服务
/// </summary>
public class SysMenuService : ISysMenuService, ITransient
{
    private readonly ISqlSugarRepository<SysMenuModel> _repository;

    public SysMenuService(ISqlSugarRepository<SysMenuModel> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 查询系统菜单树形
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<List<SysMenuTreeOutput>> QuerySysMenuTree(QuerySysMenuTreeInput input)
    {
        var list = await _repository.AsQueryable().WhereIF(!input.ModuleId.IsNullOrZero(), wh => wh.ModuleId == input.ModuleId)
            .WhereIF(!input.MenuName.IsEmpty(), wh => wh.MenuName.Contains(input.MenuName))
            .WhereIF(!input.IsSystem.IsNullOrZero(), wh => wh.IsSystem == input.IsSystem)
            .WhereIF(!input.Status.IsNullOrZero(), wh => wh.Status == input.Status).Select<SysMenuTreeOutput>().ToListAsync();

        var result = new TreeBuildUtil<SysMenuTreeOutput>().Build(list);

        return result;
    }

    /// <summary>
    /// 添加系统菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task AddSysMenu(AddSysMenuInput input)
    {
        // 菜单Code不能重复
        if (await _repository.IsExistsAsync(wh => wh.MenuCode == input.MenuCode))
        {
            throw Oops.Bah("菜单编码不能重复！");
        }

        // 转换Model
        var model = input.Adapt<SysMenuModel>();

        // 默认为启用的
        model.Status = CommonStatusEnum.Enable;

        // 添加数据库
        await _repository.InsertAsync(model);
    }

    /// <summary>
    /// 更新系统菜单
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task UpdateSysMenu(UpdateSysMenuInput input)
    {
        // 查询源数据
        var model = await _repository.FirstOrDefaultAsync(f => f.Id == input.Id);

        if (model == null)
        {
            throw Oops.Bah("数据不存在！");
        }

        // 保留编码
        var menuCode = model.MenuCode;

        // 系统菜单不能修改类型，路由地址，组件地址，内链，外链地址
        if (model.IsSystem == YesOrNotEnum.Y)
        {
            if (model.MenuType != input.MenuType)
            {
                throw Oops.Bah("系统级别数据不允许修改类型");
            }

            if (model.Router != input.Router)
            {
                throw Oops.Bah("系统级别数据不允许修改路由地址");
            }

            if (model.Component != input.Component)
            {
                throw Oops.Bah("系统级别数据不允许修改组件地址");
            }

            if (model.Link != input.Link)
            {
                throw Oops.Bah("系统级别数据不允许修改内链/外链地址");
            }

            if (model.IsSystem != input.IsSystem)
            {
                throw Oops.Bah("系统级别数据不允许修改系统级别！");
            }
        }

        // 覆盖源数据
        model = input.Adapt(model);
        model.MenuCode = menuCode;

        // 更新数据
        await _repository.Context.Updateable(model).ExecuteCommandWithOptLockAsync(true);
    }
}