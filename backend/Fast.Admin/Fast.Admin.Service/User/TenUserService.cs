using Fast.Core.AdminFactory.ModelFactory.Tenant;
using Fast.Core.ServiceCollection.Cache;
using Fast.SqlSugar.Tenant.Repository;

namespace Fast.Admin.Service.User;

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