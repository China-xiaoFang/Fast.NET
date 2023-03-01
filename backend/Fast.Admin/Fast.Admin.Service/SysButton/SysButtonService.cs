using Fast.Admin.Service.SysButton.Dto;
using Fast.Core.AdminFactory.ModelFactory.Sys;

namespace Fast.Admin.Service.SysButton;

/// <summary>
/// 系统按钮服务
/// </summary>
public class SysButtonService : ISysButtonService
{
    private readonly ISqlSugarRepository<SysButtonModel> _repository;

    public SysButtonService(ISqlSugarRepository<SysButtonModel> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 根据菜单Id查询系统按钮
    /// </summary>
    /// <param name="menuId"></param>
    /// <returns></returns>
    public async Task<List<SysButtonOutput>> QuerySysButtonListById(long menuId)
    {
        return await _repository.AsQueryable().Where(wh => wh.MenuId == menuId).Select<SysButtonOutput>().ToListAsync();
    }

    /// <summary>
    /// 添加系统按钮
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task AddSysButton(AddSysButtonInput input)
    {
        // 判断菜单Id是否存在
        if (!await _repository.Context.Queryable<SysMenuModel>().AnyAsync(wh => wh.Id == input.MenuId))
        {
            throw Oops.Bah("菜单不存在！");
        }

        // 判断编码是否重复
        if (await _repository.IsExistsAsync(wh => wh.MenuId == input.MenuId && wh.Code == input.Code))
        {
            throw Oops.Bah("按钮编码重复！");
        }

        // 判断名称是否重复
        if (await _repository.IsExistsAsync(wh => wh.MenuId == input.MenuId && wh.Name == input.Name))
        {
            throw Oops.Bah("按钮名称重复！");
        }

        // 转换Model
        var model = input.Adapt<SysButtonModel>();

        await _repository.InsertAsync(model);
    }

    /// <summary>
    /// 更新系统按钮
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task UpdateSysButton(UpdateSysButtonInput input)
    {
        // 判断菜单Id是否存在
        if (!await _repository.Context.Queryable<SysMenuModel>().AnyAsync(wh => wh.Id == input.MenuId))
        {
            throw Oops.Bah("菜单不存在！");
        }

        // 判断编码是否重复
        if (await _repository.IsExistsAsync(wh => wh.Id != input.Id && wh.MenuId == input.MenuId && wh.Code == input.Code))
        {
            throw Oops.Bah("按钮编码重复！");
        }

        // 判断名称是否重复
        if (await _repository.IsExistsAsync(wh => wh.Id != input.Id && wh.MenuId == input.MenuId && wh.Name == input.Name))
        {
            throw Oops.Bah("按钮名称重复！");
        }

        // 查询源数据
        var model = await _repository.FirstOrDefaultAsync(f => f.Id == input.Id);

        if (model == null)
        {
            throw Oops.Bah("数据不存在！");
        }

        // 覆盖源数据
        model = input.Adapt(model);

        // 更新数据
        await _repository.Context.Updateable(model).ExecuteCommandWithOptLockAsync(true);
    }

    /// <summary>
    /// 删除系统按钮
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task DeleteSysButton(long id)
    {
        // 查询源数据
        var model = await _repository.FirstOrDefaultAsync(f => f.Id == id);

        if (model == null)
        {
            throw Oops.Bah("数据不存在！");
        }

        // 软删除
        model.IsDeleted = true;

        // 更新数据
        await _repository.Context.Updateable(model).ExecuteCommandWithOptLockAsync(true);
    }
}