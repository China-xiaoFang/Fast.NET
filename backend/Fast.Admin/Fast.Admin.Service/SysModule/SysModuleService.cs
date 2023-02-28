using Fast.Admin.Service.SysModule.Dto;
using Fast.Core.AdminFactory.ModelFactory.Sys;

namespace Fast.Admin.Service.SysModule;

public class SysModuleService
{
    private readonly ISqlSugarRepository<SysModuleModel> _repository;

    public SysModuleService(ISqlSugarRepository<SysModuleModel> repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 分页查询模块信息
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

    public async Task AddModule(AddModuleInput input)
    {
    }
}