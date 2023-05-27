using Fast.Core.AdminModel.Tenant.Organization.User;
using Fast.Core.Cache;
using Fast.Core.SqlSugar.Repository;

namespace Fast.Core.AdminService.User;

/// <summary>
/// 租户用户服务
/// </summary>
public class TenUserService
{
    private readonly ISqlSugarRepository<TenUserModel> _repository;
    public readonly ICache _cache;

    public TenUserService(ISqlSugarRepository<TenUserModel> repository, ICache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    //public 
}