using System.Linq.Expressions;
using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.Core.AdminFactory.ServiceFactory.Tenant.Dto;
using Fast.Core.Util.Restful.Internal;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Core.AdminFactory.ServiceFactory.Tenant;

/// <summary>
/// 租户服务接口
/// </summary>
public interface ISysTenantService
{
    /// <summary>
    /// 获取所有租户信息
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    Task<List<SysTenantModel>> GetAllTenantInfo(Expression<Func<SysTenantModel, bool>> predicate = null);

    /// <summary>
    /// 分页查询租户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task<PageResult<TenantOutput>> QueryTenantPageList([FromQuery] QueryTenantInput input);

    /// <summary>
    /// Web站点初始化
    /// </summary>
    /// <returns></returns>
    Task<WebSiteInitOutput> WebSiteInit();

    /// <summary>
    /// 添加租户
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task AddTenant(AddTenantInput input);

    /// <summary>
    /// 初始化租户信息
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    Task InitTenantInfo(InitTenantInfoInput input);

    /// <summary>
    /// 初始化新租户数据
    /// </summary>
    /// <param name="newTenant"></param>
    /// <param name="entityTypeList">初始化Entity类型</param>
    /// <param name="isInit">是否为初始化</param>
    /// <returns></returns>
    Task InitNewTenant(SysTenantModel newTenant, IEnumerable<Type> entityTypeList, bool isInit = false);
}