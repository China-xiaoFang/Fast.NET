using System.Diagnostics;
using Fast.Core.AdminFactory.ServiceFactory.Tenant;

namespace Fast.Core.AdminFactory.ServiceFactory.InitDataBase;

/// <summary>
/// 数据化数据库服务
/// </summary>
public class InitDataBaseService : IInitDataBaseService, ITransient
{
    private readonly ISqlSugarClient _db;
    private readonly ConnectionStringsOptions _connectionStringsOptions;

    public InitDataBaseService(ISqlSugarClient db, IOptions<ConnectionStringsOptions> connectionStringsOptions)
    {
        _db = ((SqlSugarClient) db).GetConnection(connectionStringsOptions.Value.DefaultConnectionId);
        _connectionStringsOptions = connectionStringsOptions.Value;
    }

    /// <summary>
    /// 初始化数据库
    /// </summary>
    /// <returns></returns>
    public async Task InitDataBase()
    {
        // 创建核心业务库
        if (_db.DbMaintenance.CreateDatabase())
        {
            var sw = new Stopwatch();
            sw.Start();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"
            
             初始化数据库中......
              
            ");
            // 获取所有数据库Model
            var entityTypeList = SqlSugarConfig.ReflexGetAllTEntityList();

            // 创建核心业务库的所有表
            foreach (var (_, _, type) in entityTypeList.Where(wh => wh.dbType == SysDataBaseTypeEnum.Admin))
            {
                _db.CodeFirst.InitTables(type);
            }

            // 初始化租户信息
            var superAdminTenantInfo = new SysTenantModel
            {
                Id = ClaimConst.DEFAULT_SUPERADMIN_TENANT_ID,
                Name = "Fast.NET",
                ShortName = "Fast",
                Secret = StringUtil.GetGuid(),
                AdminName = "租户管理员",
                Email = "email@gmail.com",
                Phone = "15288888888",
                TenantType = TenantTypeEnum.System,
                WebUrl = new List<string> {"http:fast.18kboy.icu", "http:127.0.0.1:8080"},
                LogoUrl = "https://gitee.com/Net-18K/Fast.NET/raw/master/frontend/public/logn.png"
            };
            superAdminTenantInfo = await _db.Insertable(superAdminTenantInfo).ExecuteReturnEntityAsync();

            // TODO：这里存在线程或者事务锁表问题，待解决

            // 初始化租户业务库信息
            await _db.Insertable(new SysTenantDataBaseModel
            {
                ServiceIp = _connectionStringsOptions.DefaultServiceIp,
                Port = _connectionStringsOptions.DefaultPort,
                DbName = $"Fast.Main_{superAdminTenantInfo.Id}",
                DbUser = _connectionStringsOptions.DefaultDbUser,
                DbPwd = _connectionStringsOptions.DefaultDbPwd,
                SysDbType = SysDataBaseTypeEnum.Tenant,
                DbType = _connectionStringsOptions.DefaultDbType,
                TenantId = superAdminTenantInfo.Id
            }).ExecuteCommandAsync();

            // 初始化新租户数据
            await GetService<ISysTenantService>().InitNewTenant(superAdminTenantInfo,
                entityTypeList.Where(wh => wh.dbType == SysDataBaseTypeEnum.Tenant).Select(sl => sl.type), true);

            sw.Stop();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@$"
            
             数据库初始化完成！
             用时（毫秒）：{sw.ElapsedMilliseconds}
              
            ");
        }
    }
}