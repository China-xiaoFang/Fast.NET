using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fast.Core.AdminFactory.ModelFactory.Basis;

namespace Fast.Core.AdminFactory.ServiceFactory.User
{
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
}
