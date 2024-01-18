// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using System.Diagnostics;
using Fast.Admin.Core.Constants;
using Fast.Admin.Core.Entity.System.Account;
using Fast.Admin.Core.Entity.System.App;
using Fast.Admin.Core.Entity.System.DataBase;
using Fast.Admin.Core.Entity.System.Tenant;
using Fast.Admin.Core.Enum.Common;
using Fast.Admin.Core.Enum.Db;
using Fast.Admin.Core.Enum.System;
using Fast.Admin.Entity.Tenant.Organization;
using Fast.IaaS;
using Fast.NET.Core;
using Fast.SqlSugar;
using Fast.SqlSugar.Extensions;
using Fast.SqlSugar.Options;
using Mapster;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Admin.Web.CodeFirst;

/// <summary>
/// <see cref="CodeFirstFilter"/> Code First
/// </summary>
public class CodeFirstFilter : IStartupFilter
{
    /// <summary>
    /// Extends the provided <paramref name="action" /> and returns an <see cref="T:System.Action" /> of the same type.
    /// </summary>
    /// <param name="action">The Configure method to extend.</param>
    /// <returns>A modified <see cref="T:System.Action" />.</returns>
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> action)
    {
        return app =>
        {
            // 判断是否初始化数据库
            if (FastContext.Configuration.GetSection("AppSettings:InitializeDatabase").Get<bool>())
            {
                // 获取 IHostApplicationLifetime 实例
                var hostApplicationLifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();

                // 订阅 ApplicationStarted 事件
                hostApplicationLifetime?.ApplicationStarted.Register(() =>
                {
                    // 在应用程序完全启动后执行自定义逻辑
                    // 初始化数据库

                    // 获取默认库
                    var db = new SqlSugarClient(SqlSugarContext.DefaultConnectionConfig);

                    // 创建核心业务库
                    db.DbMaintenance.CreateDatabase();

                    // 查询核心表是否存在
                    if (db.Ado.GetInt(
                            @$"SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_NAME = '{typeof(SysTenantModel).GetSugarTableName()}'") >
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

                    db.CodeFirst.InitTables(SqlSugarContext.SqlSugarEntityList
                        .Where(wh => !wh.IsSplitTable &&
                                     (wh.SugarDbType == null || (FastDbTypeEnum) wh.SugarDbType == FastDbTypeEnum.SysCore))
                        .Select(sl => sl.EntityType).ToArray());
                    db.CodeFirst.SplitTables().InitTables(SqlSugarContext.SqlSugarEntityList
                        .Where(wh => wh.IsSplitTable &&
                                     (wh.SugarDbType == null || (FastDbTypeEnum) wh.SugarDbType == FastDbTypeEnum.SysCore))
                        .Select(sl => sl.EntityType).ToArray());

                    // 初始化租户信息
                    var superAdminTenantInfo = new SysTenantModel
                    {
                        Id = SystemConst.DefaultSystemTenantId,
                        TenantNo = "Fast.Admin",
                        ChName = "Fast.Admin",
                        EnName = "Fast.Admin",
                        ChShortName = "Fast",
                        EnShortName = "Fast",
                        Secret = GuidUtil.GetGuid(),
                        PublicKey = "公钥",
                        PrivateKey = "私钥",
                        AdminName = "租户管理员",
                        Email = "email@gmail.com",
                        Mobile = "15288888888",
                        TenantType = TenantTypeEnum.System,
                        LogoUrl = "/logo.png"
                    };
                    superAdminTenantInfo = db.Insertable(superAdminTenantInfo).ExecuteReturnEntity();

                    // 初始化租户授权信息
                    var superAdminTenantAppInfoList = new List<SysTenantAppInfoModel>
                    {
                        new()
                        {
                            TenantId = SystemConst.DefaultSystemTenantId,
                            AppType = AppTypeEnum.WebAdmin,
                            AppKey = "http://127.0.0.1:2001",
                            AuthStartTime = new DateTime(2020, 01, 01),
                            AuthEndTime = new DateTime(2099, 12, 31),
                            Remark = ""
                        },
                        new()
                        {
                            TenantId = SystemConst.DefaultSystemTenantId,
                            AppType = AppTypeEnum.WebAdmin,
                            AppKey = "http://fast.18kboy.icu",
                            AuthStartTime = new DateTime(2020, 01, 01),
                            AuthEndTime = new DateTime(2099, 12, 31),
                            Remark = ""
                        }
                    };
                    db.Insertable(superAdminTenantAppInfoList).ExecuteReturnEntity();

                    // 初始化系统日志库信息
                    var sysCodeLogDataBaseModel = new SysTenantMainDataBaseModel
                    {
                        Id = YitIdHelper.NextId(),
                        TenantId = SystemConst.DefaultSystemTenantId,
                        FastDbType = FastDbTypeEnum.SysCoreLog,
                        IsSystem = YesOrNotEnum.Y,
                        ServiceIp = SqlSugarContext.ConnectionSettings.ServiceIp,
                        Port = SqlSugarContext.ConnectionSettings.Port,
                        DbName = "Fast.Log",
                        DbUser = SqlSugarContext.ConnectionSettings.DbUser,
                        DbPwd = SqlSugarContext.ConnectionSettings.DbPwd,
                        DbType = SqlSugarContext.ConnectionSettings.DbType,
                        CommandTimeOut = SqlSugarContext.ConnectionSettings.CommandTimeOut,
                        SugarSqlExecMaxSeconds = 0,
                        DiffLog = false,
                        DisableAop = true,
                    };
                    db.Insertable(sysCodeLogDataBaseModel).ExecuteCommand();

                    // 获取系统日志库连接配置
                    var logDb = new SqlSugarClient(
                        SqlSugarContext.GetConnectionConfig(sysCodeLogDataBaseModel.Adapt<ConnectionSettingsOptions>()));

                    // 创建系统日志库
                    logDb.DbMaintenance.CreateDatabase();

                    // 初始化系统日志库的表
                    logDb.CodeFirst.InitTables(SqlSugarContext.SqlSugarEntityList
                        .Where(wh => !wh.IsSplitTable && wh.SugarDbType != null &&
                                     (FastDbTypeEnum) wh.SugarDbType == FastDbTypeEnum.SysCoreLog).Select(sl => sl.EntityType)
                        .ToArray());
                    logDb.CodeFirst.SplitTables().InitTables(SqlSugarContext.SqlSugarEntityList
                        .Where(wh => wh.IsSplitTable && wh.SugarDbType != null &&
                                     (FastDbTypeEnum) wh.SugarDbType == FastDbTypeEnum.SysCoreLog).Select(sl => sl.EntityType)
                        .ToArray());

                    // 初始化Admin核心库信息
                    var sysAdminCoreDataBaseModel = new SysTenantMainDataBaseModel
                    {
                        Id = YitIdHelper.NextId(),
                        TenantId = SystemConst.DefaultSystemTenantId,
                        FastDbType = FastDbTypeEnum.SysCoreLog,
                        IsSystem = YesOrNotEnum.Y,
                        ServiceIp = SqlSugarContext.ConnectionSettings.ServiceIp,
                        Port = SqlSugarContext.ConnectionSettings.Port,
                        DbName = $"Fast.Code_{SystemConst.DefaultSystemTenantId}",
                        DbUser = SqlSugarContext.ConnectionSettings.DbUser,
                        DbPwd = SqlSugarContext.ConnectionSettings.DbPwd,
                        DbType = SqlSugarContext.ConnectionSettings.DbType,
                        CommandTimeOut = SqlSugarContext.ConnectionSettings.CommandTimeOut,
                        SugarSqlExecMaxSeconds = SqlSugarContext.ConnectionSettings.SugarSqlExecMaxSeconds,
                        DiffLog = true,
                        DisableAop = false,
                    };
                    db.Insertable(sysAdminCoreDataBaseModel).ExecuteCommand();

                    // 获取系统Admin核心库连接配置
                    var adminCodeDb = new SqlSugarClient(
                        SqlSugarContext.GetConnectionConfig(sysAdminCoreDataBaseModel.Adapt<ConnectionSettingsOptions>()));

                    // 创建系统Admin核心库
                    adminCodeDb.DbMaintenance.CreateDatabase();

                    // 初始化系统Admin核心库的表
                    adminCodeDb.CodeFirst.InitTables(SqlSugarContext.SqlSugarEntityList
                        .Where(wh => !wh.IsSplitTable && wh.SugarDbType != null &&
                                     (FastDbTypeEnum) wh.SugarDbType == FastDbTypeEnum.SysAdminCore).Select(sl => sl.EntityType)
                        .ToArray());
                    adminCodeDb.CodeFirst.SplitTables().InitTables(SqlSugarContext.SqlSugarEntityList
                        .Where(wh => wh.IsSplitTable && wh.SugarDbType != null &&
                                     (FastDbTypeEnum) wh.SugarDbType == FastDbTypeEnum.SysAdminCore).Select(sl => sl.EntityType)
                        .ToArray());

                    // 初始化超级管理员
                    db.Insertable(new SysAccountModel
                    {
                        Id = SystemConst.DefaultSuperAdminId,
                        Account = "15288888888",
                        Password = CryptoUtil.MD5Encrypt(SystemConst.DefaultAdminPassword),
                        UserName = "小方",
                        Birthday = null,
                        Sex = GenderEnum.Man,
                        Email = "superAdmin@fastdotnet.com",
                        Mobile = "15288888888",
                        Tel = null,
                        Status = CommonStatusEnum.Enable
                    }).ExecuteCommand();
                    db.Insertable(new SysTenantAccountModel
                    {
                        Id = YitIdHelper.NextId(),
                        AccountId = SystemConst.DefaultSuperAdminId,
                        UserId = SystemConst.DefaultSuperAdminId,
                        JobNumber = "2024010101",
                        NickName = "超级管理员",
                        Avatar = "",
                        DepartmentId = 0,
                        DepartmentName = "",
                        AdminType = AdminTypeEnum.SuperAdmin,
                        Status = CommonStatusEnum.Enable,
                    }).ExecuteCommand();
                    adminCodeDb.Insertable(new TenUserModel
                    {
                        Id = SystemConst.DefaultSuperAdminId,
                        AccountId = SystemConst.DefaultSuperAdminId,
                        JobNumber = "2024010101",
                        NickName = "超级管理员",
                        Avatar = "",
                        DepartmentId = 0,
                        DepartmentName = "",
                        AdminType = AdminTypeEnum.SuperAdmin,
                        Status = CommonStatusEnum.Enable,
                    }).ExecuteCommand();

                    sw.Stop();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(@$"

数据库初始化完成！
用时（毫秒）：{sw.ElapsedMilliseconds}
  
");
                });
            }

            // 调用启动层的 Startup
            action(app);
        };
    }
}