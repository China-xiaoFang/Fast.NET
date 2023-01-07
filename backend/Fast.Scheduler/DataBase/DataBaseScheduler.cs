using System.Diagnostics;
using System.Reflection;
using Fast.Core;
using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.Core.AdminFactory.ModelFactory.Tenant;
using Fast.Core.AdminFactory.ServiceFactory.Tenant;
using Fast.Core.CodeFirst;
using Fast.Core.CodeFirst.Internal;
using Fast.Core.Const;
using Fast.Core.Internal.AttributeFilter;
using Fast.Core.ServiceCollection.Cache;
using Fast.Iaas.Util;
using Fast.SqlSugar.Tenant;
using Fast.SqlSugar.Tenant.Extension;
using Fast.SqlSugar.Tenant.Helper;
using Fast.SqlSugar.Tenant.Internal.Enum;
using Fast.SqlSugar.Tenant.SugarEntity;
using Furion.DependencyInjection;
using Furion.TaskScheduler;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;

namespace Fast.Scheduler.DataBase;

/// <summary>
/// 数据库任务调度
/// </summary>
public class DataBaseJobWorker : ISpareTimeWorker
{
    /// <summary>
    /// 初始化数据库
    /// </summary>
    /// <param name="timer"></param>
    /// <param name="count"></param>
    [SpareTime(100, "初始化数据库", DoOnce = true, StartNow = true)]
    public async Task InitDataBast(SpareTimer timer, long count)
    {
        if (GlobalContext.SystemSettingsOptions?.InitDataBase != true)
        {
            // 同步枚举字典，手动执行是因为怕数据库还没有生成完毕。
            SpareTime.Start("同步枚举字典");
            return;
        }

        await Scoped.CreateAsync(async (_, scope) =>
        {
            var service = scope.ServiceProvider;

            var db = service.GetService<ISqlSugarClient>();

            // ReSharper disable once PossibleNullReferenceException
            var _db = db.AsTenant().GetConnection(SugarContext.ConnectionStringsOptions.DefaultConnectionId);
            var _sysTenantService = service.GetService<ISysTenantService>();

            // 创建核心业务库
            _db.DbMaintenance.CreateDatabase();

            // 查询核心表是否存在
            if (await _db.Ado.GetIntAsync(
                    $"SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_NAME = '{typeof(SysTenantModel).GetSugarTableName()}'") >
                0)
            {
                // 同步枚举字典，手动执行是因为怕数据库还没有生成完毕。
                SpareTime.Start("同步枚举字典");
                return;
            }

            var sw = new Stopwatch();
            sw.Start();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"
            
             初始化数据库中......
              
            ");
            // 获取所有数据库Model
            var entityTypeList = EntityHelper.ReflexGetAllTEntityList();

            // 创建核心业务库的所有表
            _db.CodeFirst.InitTables(entityTypeList.Where(wh => wh.DbType == SugarDbTypeEnum.Default.GetHashCode())
                .Select(sl => sl.Type).ToArray());

            // 初始化租户信息
            var superAdminTenantInfo = new SysTenantModel
            {
                Id = ClaimConst.DefaultSuperAdminTenantId,
                ChName = "Fast.NET",
                EnName = "Fast.Net",
                ChShortName = "Fast",
                EnShortName = "Fast",
                Secret = StringUtil.GetGuid(),
                AdminName = "租户管理员",
                Email = "email@gmail.com",
                Phone = "15288888888",
                TenantType = TenantTypeEnum.System,
                WebUrl = new List<string> {"http://fast.18kboy.icu", "http://127.0.0.1:2001"},
                LogoUrl = "https://gitee.com/Net-18K/Fast.NET/raw/master/frontend/public/logo.png"
            };
            superAdminTenantInfo = await _db.Insertable(superAdminTenantInfo).ExecuteReturnEntityAsync();

            // 初始化租户业务库信息
            await _db.Insertable(new SysTenantDataBaseModel
            {
                ServiceIp = SugarContext.ConnectionStringsOptions.DefaultServiceIp,
                Port = SugarContext.ConnectionStringsOptions.DefaultPort,
                DbName = $"Fast.Main_{superAdminTenantInfo.EnShortName}",
                DbUser = SugarContext.ConnectionStringsOptions.DefaultDbUser,
                DbPwd = SugarContext.ConnectionStringsOptions.DefaultDbPwd,
                SugarSysDbType = SugarDbTypeEnum.Tenant.GetHashCode(),
                DbType = SugarContext.ConnectionStringsOptions.DefaultDbType,
                TenantId = superAdminTenantInfo.Id
            }).ExecuteCommandAsync();

            // 初始化租户库种子数据
            var seedDataTypes = SeedDataProgram.GetSeedDataType(typeof(ISystemSeedData));

            // 开启事务
            await _db.Ado.BeginTranAsync();
            try
            {
                SeedDataProgram.ExecSeedData(_db, seedDataTypes);

                // 提交事务
                await _db.Ado.CommitTranAsync();
            }
            catch (Exception)
            {
                // 回滚事务
                await _db.Ado.RollbackTranAsync();
                throw;
            }

            // 初始化新租户数据
            // ReSharper disable once PossibleNullReferenceException
            await _sysTenantService.InitNewTenant(superAdminTenantInfo,
                entityTypeList.Where(wh => wh.DbType == SugarDbTypeEnum.Tenant.GetHashCode()).Select(sl => sl.Type), true);

            sw.Stop();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@$"
            
             数据库初始化完成！
             用时（毫秒）：{sw.ElapsedMilliseconds}
              
            ");

            // 同步枚举字典，手动执行是因为怕数据库还没有生成完毕。
            SpareTime.Start("同步枚举字典");
        });
    }

    /// <summary>
    /// 同步枚举字典
    /// </summary>
    /// <param name="timer"></param>
    /// <param name="count"></param>
    [SpareTime(100, "同步枚举字典", DoOnce = true, StartNow = false)]
    public async Task SyncEnumDict(SpareTimer timer, long count)
    {
        if (GlobalContext.SystemSettingsOptions?.SyncEnumDict != true)
            return;

        await Scoped.CreateAsync(async (_, scope) =>
        {
            var service = scope.ServiceProvider;

            var _cache = service.GetService<ICache>();

            var db = service.GetService<ISqlSugarClient>();

            // ReSharper disable once PossibleNullReferenceException
            var _db = db.AsTenant().GetConnection(SugarContext.ConnectionStringsOptions.DefaultConnectionId);

            var sw = new Stopwatch();
            sw.Start();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@"
            
             同步枚举字典中......
              
            ");

            // 获取所有的实现了枚举特性的枚举类
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(sl => sl.GetTypes().Where(wh =>
                wh.IsEnum && (wh.GetCustomAttribute<FastEnumAttribute>() != null || wh.Name == nameof(SugarDbTypeEnum))));

            var typeModels = new List<SysDictTypeModel>();
            var dataModels = new List<SysDictDataModel>();

            var typeIdIndex = 1000000001;
            var dataIdIndex = 2000000001;

            // 遍历获取到的类型集合
            foreach (var type in types)
            {
                // 获取中文名称
                var enumAtt = type.GetCustomAttribute<FastEnumAttribute>();

                // 去重枚举类尾部的Enum
                var typeName = type.Name.Remove(type.Name.LastIndexOf("Enum", StringComparison.Ordinal));

                var typeChName = enumAtt?.ChName ?? typeName;

                var typeInfo = new SysDictTypeModel
                {
                    Id = typeIdIndex,
                    Code = typeName,
                    ChName = typeChName,
                    EnName = enumAtt?.EnName ?? typeName,
                    Level = SysLevelEnum.Default,
                    Sort = -1,
                    Remark = enumAtt?.Remark ?? typeChName
                };
                typeIdIndex += 1;
                typeModels.Add(typeInfo);

                // 获取枚举详情
                var enumInfos = type.EnumToList();

                var dataSort = 1;

                foreach (var dataInfo in enumInfos)
                {
                    dataModels.Add(new SysDictDataModel
                    {
                        Id = dataIdIndex,
                        TypeId = typeInfo.Id,
                        ChValue = dataInfo.Describe ?? dataInfo.Name,
                        EnValue = dataInfo.Name,
                        Code = dataInfo.Value,
                        Sort = dataSort,
                        Remark = dataInfo.Describe ?? dataInfo.Name,
                    });
                    dataIdIndex += 1;
                    dataSort += 1;
                }
            }

            // 开启事务
            await _db.Ado.BeginTranAsync();
            try
            {
                // 查询所有的默认级别枚举字典的类型Id
                var orgTypeId = await _db.Queryable<SysDictTypeModel>().Where(wh => wh.Level == SysLevelEnum.Default)
                    .Select(sl => sl.Id).ToListAsync();

                // 删除所有的默认级别枚举字典类型
                await _db.Deleteable<SysDictTypeModel>().Where(wh => wh.Level == SysLevelEnum.Default).ExecuteCommandAsync();

                // 删除所有的默认级别枚举字典数据
                await _db.Deleteable<SysDictDataModel>().Where(wh => orgTypeId.Contains(wh.TypeId)).ExecuteCommandAsync();

                // 加入数据库
                await _db.Insertable(typeModels).ExecuteCommandAsync();
                await _db.Insertable(dataModels).ExecuteCommandAsync();

                // 删除缓存中的数据
                // ReSharper disable once PossibleNullReferenceException
                await _cache.DelAsync(CacheConst.SysDictInfo);

                // 提交事务
                await _db.Ado.CommitTranAsync();
            }
            catch (Exception)
            {
                // 回滚事务
                await _db.Ado.RollbackTranAsync();
                throw;
            }

            sw.Stop();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(@$"
            
             同步枚举字典完成！
             用时（毫秒）：{sw.ElapsedMilliseconds}
              
            ");
        });
    }
}