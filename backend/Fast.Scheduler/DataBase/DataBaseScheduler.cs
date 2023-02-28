using System.Diagnostics;
using System.Reflection;
using System.Text;
using Fast.Admin.Service.Tenant;
using Fast.Core;
using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.Core.Cache;
using Fast.Core.CodeFirst;
using Fast.Core.CodeFirst.Internal;
using Fast.Core.Const;
using Fast.Core.Internal.AttributeFilter;
using Fast.Iaas.Util;
using Fast.SqlSugar.Tenant;
using Fast.SqlSugar.Tenant.Extension;
using Fast.SqlSugar.Tenant.Helper;
using Fast.SqlSugar.Tenant.Internal.Enum;
using Fast.SqlSugar.Tenant.SugarEntity;
using Furion.Schedule;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace Fast.Scheduler.DataBase;

/// <summary>
/// 数据库任务调度
/// </summary>
[JobDetail("初始化数据库")]
[Secondly(TriggerId = "初始化数据库", Description = "程序启动时，初始化数据库，同步枚举字典", MaxNumberOfRuns = 1, StartNow = true, RunOnStart = true,
    ResetOnlyOnce = true)]
public class DataBaseJobWorker : IJob
{
    private readonly IServiceProvider _serviceProvider;

    public DataBaseJobWorker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>具体处理逻辑</summary>
    /// <param name="context">作业执行前上下文</param>
    /// <param name="stoppingToken">取消任务 Token</param>
    /// <returns><see cref="T:System.Threading.Tasks.Task" /></returns>
    public async Task ExecuteAsync(JobExecutingContext context, CancellationToken stoppingToken)
    {
        // 创建一个作用域
        using var serviceScope = _serviceProvider.CreateScope();

        // 获取服务
        var db = serviceScope.ServiceProvider.GetService<ISqlSugarClient>();
        // ReSharper disable once PossibleNullReferenceException
        var _db = db.AsTenant().GetConnection(SugarContext.ConnectionStringsOptions.DefaultConnectionId);
        var _cache = serviceScope.ServiceProvider.GetService<ICache>();
        var _sysTenantService = serviceScope.ServiceProvider.GetService<ISysTenantService>();

        // 初始化数据库
        await InitDataBast(_db, _sysTenantService);

        // 同步枚举字典
        await SyncEnumDict(_db, _cache);

        // 同步应用本地化配置
        await SyncAppLocalization(_db, _cache);
    }

    /// <summary>
    /// 初始化数据库
    /// </summary>
    /// <param name="_db"></param>
    /// <param name="_sysTenantService"></param>
    /// <returns></returns>
    public async Task InitDataBast(SqlSugarProvider _db, ISysTenantService _sysTenantService)
    {
        if (GlobalContext.SystemSettingsOptions?.InitDataBase != true)
        {
            return;
        }

        // 创建核心业务库
        _db.DbMaintenance.CreateDatabase();

        // 查询核心表是否存在
        if (await _db.Ado.GetIntAsync(
                $"SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_NAME = '{typeof(SysTenantModel).GetSugarTableName()}'") >
            0)
        {
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
            WebUrl = new List<string> {"http://fast.18kboy.icu", "http://127.0.0.1:2001", "http://localhost:2001"},
            //LogoUrl = "https://gitee.com/Net-18K/Fast.NET/raw/master/frontend/public/logo.png"
            LogoUrl = "/logo.png"
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
    }

    /// <summary>
    /// 同步枚举字典
    /// </summary>
    /// <param name="_db"></param>
    /// <param name="_cache"></param>
    /// <returns></returns>
    public async Task SyncEnumDict(SqlSugarProvider _db, ICache _cache)
    {
        if (GlobalContext.SystemSettingsOptions?.SyncEnumDict != true)
            return;

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
    }

    /// <summary>
    /// 同步应用本地化配置
    /// </summary>
    /// <param name="_db"></param>
    /// <param name="_cache"></param>
    /// <returns></returns>
    public async Task SyncAppLocalization(SqlSugarProvider _db, ICache _cache)
    {
        if (GlobalContext.SystemSettingsOptions?.SyncAppLocalization != true)
            return;

        var sw = new Stopwatch();
        sw.Start();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"
            
             同步应用本地化配置中......
              
            ");

        // 获取程序运行路径
        var systemPath = AppDomain.CurrentDomain.BaseDirectory;

        // 英文翻译文件路径
        const string englishTranslationFilePath = "Resources\\Lang.en-US.json";

        // 判断是否存在文件
        if (!File.Exists($"{systemPath}{englishTranslationFilePath}"))
        {
            throw new Exception($"不存在{englishTranslationFilePath}文件信息！");
        }

        // 读取JSON文件信息
        var bytes = await File.ReadAllBytesAsync($"{systemPath}{englishTranslationFilePath}");
        var jsonStr = Encoding.UTF8.GetString(bytes);

        // 查询所有已有的配置
        var orgSysAppLocalizationModels = await _db.Queryable<SysAppLocalizationModel>().ToListAsync();

        var addSysAppLocalizationModels = new List<SysAppLocalizationModel>();
        var updSysAppLocalizationModels = new List<SysAppLocalizationModel>();

        using (var fileInfo = File.OpenText($"{systemPath}{englishTranslationFilePath}"))
        {
            await using (var reader = new JsonTextReader(fileInfo))
            {
                var jObject = await JToken.ReadFromAsync(reader);
                foreach (var jToken in jObject)
                {
                    var info = jToken.First();

                    var key = "";
                    // 判断是否以['开头和']结尾
                    if (info.Path.StartsWith("['") && info.Path.EndsWith("']"))
                    {
                        key = info.Path.Substring(2, info.Path.Length - 4);
                    }
                    else
                    {
                        key = info.Path;
                    }

                    var value = jToken.First().ToString();

                    // 判断是否已经在数据库中存在
                    var orgInfo = orgSysAppLocalizationModels.FirstOrDefault(wh => wh.Chinese == key);
                    if (orgInfo != null)
                    {
                        // 存在则修改
                        orgInfo.English = value;
                        updSysAppLocalizationModels.Add(orgInfo);
                    }
                    else
                    {
                        // 不存在添加
                        addSysAppLocalizationModels.Add(new SysAppLocalizationModel
                        {
                            Chinese = key,
                            English = value,
                            TranslationSource = TranslationSourceEnum.Custom,
                            IsSystem = YesOrNotEnum.Y
                        });
                    }
                }
            }
        }

        // 开启事务
        await _db.Ado.BeginTranAsync();
        try
        {
            // 修改
            await _db.Updateable(updSysAppLocalizationModels).ExecuteCommandAsync();
            // 添加
            await _db.Insertable(addSysAppLocalizationModels).ExecuteCommandAsync();

            // 删除缓存中的数据
            await _cache.DelAsync(CacheConst.SysAppLocalization);

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
            
             同步应用本地化配置完成！
             用时（毫秒）：{sw.ElapsedMilliseconds}
              
            ");
    }
}