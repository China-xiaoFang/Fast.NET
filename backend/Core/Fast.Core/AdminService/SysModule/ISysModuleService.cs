using Fast.Core.AdminService.SysModule.Dto;
using Fast.Core.Restful.Internal;

namespace Fast.Core.AdminService.SysModule;

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
    /// 查询系统模块选择器
    /// </summary>
    /// <returns></returns>
    Task<List<SysModuleOutput>> QuerySysModuleSelector();

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
}