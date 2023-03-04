using Fast.Admin.Service.SysModule.Dto;
using Fast.Core.AdminFactory.ModelFactory.Sys;

namespace Fast.Admin.Service.SysModule;

/// <summary>
/// 系统模块服务
/// </summary>
public class SysModuleService : ISysModuleService
{
    private readonly ISqlSugarRepository<SysModuleModel> _repository;

    public SysModuleService(ISqlSugarRepository<SysModuleModel> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 分页查询系统模块信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<PageResult<SysModuleOutput>> QuerySysModulePageList(QuerySysModuleInput input)
    {
        return await _repository.AsQueryable().WhereIF(!input.Name.IsEmpty(), wh => wh.Name.Contains(input.Name))
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
        return await _repository.AsQueryable(wh => wh.Status == CommonStatusEnum.Enable).OrderBy(ob => ob.Sort)
            .Select<SysModuleOutput>().ToListAsync();
    }

    /// <summary>
    /// 添加系统模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task AddModule(AddModuleInput input)
    {
        // 模块名称不能重复
        if (await _repository.IsExistsAsync(wh => wh.Name == input.Name))
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
        if (await _repository.IsExistsAsync(wh => wh.Id != input.Id && wh.Name == input.Name))
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