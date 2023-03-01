using Fast.Admin.Service.SysModule.Dto;

namespace Fast.Admin.Service.SysModule;

/// <summary>
/// 系统模块服务接口
/// </summary>
public interface ISysModuleService
{
    /// <summary>
    /// 分页查询系统模块信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<PageResult<SysModuleOutput>> QuerySysModulePageList(QuerySysModuleInput input);

    /// <summary>
    /// 添加系统模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task AddModule(AddModuleInput input);

    /// <summary>
    /// 更新系统模块
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task UpdateModule(UpdateModuleInput input);

    /// <summary>
    /// 更新系统模块状态
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task UpdateModuleStatus(UpdateModuleStatusInput input);
}