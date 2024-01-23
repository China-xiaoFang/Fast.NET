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

using Fast.Admin.Core.Constants;
using Fast.Admin.Core.Entity.System.Account;
using Fast.Admin.Core.Entity.System.App;
using Fast.Admin.Core.Entity.System.Database;
using Fast.Admin.Core.Entity.System.Tenant;
using Fast.Admin.Core.Enum.Common;
using Fast.Admin.Core.Enum.Db;
using Fast.Admin.Core.Enum.System;
using Fast.DependencyInjection;
using Fast.IaaS;
using Fast.SqlSugar;
using Fast.SqlSugar.Extensions;
using Fast.SqlSugar.Options;
using Mapster;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Admin.Service.System.CodeFirst;

/// <summary>
/// <see cref="SystemCodeFirstService"/> 系统库CodeFirst
/// </summary>
public class SystemCodeFirstService : ISystemCodeFirstService, ITransientDependency
{
    /// <summary>
    /// 初始化系统核心库
    /// </summary>
    /// <returns></returns>
    public async Task<(SysTenantModel, SysTenantMainDatabaseModel)> InitDatabase()
    {
        // 获取默认库
        var db = new SqlSugarClient(SqlSugarContext.DefaultConnectionConfigNoAop);

        // 创建核心业务库
        db.DbMaintenance.CreateDatabase();

        // 查询核心表是否存在
        if (db.Ado.GetInt(
                @$"SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_NAME = '{typeof(SysTenantModel).GetSugarTableName()}'") >
            0)
        {
            return (null, null);
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"初始化系统数据库中......");

        db.CodeFirst.InitTables(SqlSugarContext.SqlSugarEntityList
            .Where(wh => !wh.IsSplitTable &&
                         (wh.SugarDbType == null || (FastDbTypeEnum) wh.SugarDbType == FastDbTypeEnum.SysCore))
            .Select(sl => sl.EntityType).ToArray());
        db.CodeFirst.SplitTables().InitTables(SqlSugarContext.SqlSugarEntityList
            .Where(wh => wh.IsSplitTable && (wh.SugarDbType == null || (FastDbTypeEnum) wh.SugarDbType == FastDbTypeEnum.SysCore))
            .Select(sl => sl.EntityType).ToArray());

        // 初始化租户信息
        var superAdminTenantInfo = new SysTenantModel
        {
            Id = SystemConst.DefaultSystemTenantId,
            TenantNo = "Fast.Admin",
            ChName = "Fast.Admin",
            EnName = "Fast.Admin",
            ChShortName = "Admin",
            EnShortName = "Admin",
            Secret = GuidUtil.GetGuid(),
            PublicKey = "公钥",
            PrivateKey = "私钥",
            AdminName = "小方",
            Email = "xiaofang@fastdotnet.com",
            Mobile = "15288888888",
            TenantType = TenantTypeEnum.System,
            LogoUrl = "/logo.png"
        };
        superAdminTenantInfo = await db.Insertable(superAdminTenantInfo).ExecuteReturnEntityAsync();

        // 初始化租户授权信息
        var superAdminTenantAppInfoList = new List<SysTenantAppInfoModel>
        {
            new()
            {
                Id = YitIdHelper.NextId(),
                TenantId = SystemConst.DefaultSystemTenantId,
                AppType = AppTypeEnum.WebAdmin,
                AppKey = "http://127.0.0.1:2001",
                AuthStartTime = new DateTime(2020, 01, 01),
                AuthEndTime = new DateTime(2099, 12, 31),
                Remark = ""
            },
            new()
            {
                Id = YitIdHelper.NextId(),
                TenantId = SystemConst.DefaultSystemTenantId,
                AppType = AppTypeEnum.WebAdmin,
                AppKey = "http://admin.fastdotnet.com",
                AuthStartTime = new DateTime(2020, 01, 01),
                AuthEndTime = new DateTime(2099, 12, 31),
                Remark = ""
            },
            new()
            {
                Id = YitIdHelper.NextId(),
                TenantId = SystemConst.DefaultSystemTenantId,
                AppType = AppTypeEnum.WebAdmin,
                AppKey = "https://admin.fastdotnet.com",
                AuthStartTime = new DateTime(2020, 01, 01),
                AuthEndTime = new DateTime(2099, 12, 31),
                Remark = ""
            }
        };
        await db.Insertable(superAdminTenantAppInfoList).ExecuteReturnEntityAsync();
        // 初始化超级管理员
        await db.Insertable(new SysAccountModel
        {
            Id = SystemConst.DefaultSuperAdminId,
            Account = "superAdmin",
            Password = CryptoUtil.MD5Encrypt(SystemConst.DefaultAdminPassword),
            UserName = "超级管理员",
            Sex = GenderEnum.Man,
            Email = "superAdmin@fastdotnet.com",
            Mobile = "15188888888",
            Status = CommonStatusEnum.Enable
        }).ExecuteCommandAsync();

        // 初始化系统日志库信息
        var sysCodeLogDatabaseModel = new SysTenantMainDatabaseModel
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
        await db.Insertable(sysCodeLogDatabaseModel).ExecuteCommandAsync();

        // 获取系统日志库连接配置
        var logDb = new SqlSugarClient(
            SqlSugarContext.GetConnectionConfig(sysCodeLogDatabaseModel.Adapt<ConnectionSettingsOptions>()));

        // 创建系统日志库
        logDb.DbMaintenance.CreateDatabase();

        // 初始化系统日志库的表
        logDb.CodeFirst.InitTables(SqlSugarContext.SqlSugarEntityList
            .Where(wh => !wh.IsSplitTable && wh.SugarDbType != null &&
                         (FastDbTypeEnum) wh.SugarDbType == FastDbTypeEnum.SysCoreLog).Select(sl => sl.EntityType).ToArray());
        logDb.CodeFirst.SplitTables().InitTables(SqlSugarContext.SqlSugarEntityList
            .Where(wh => wh.IsSplitTable && wh.SugarDbType != null &&
                         (FastDbTypeEnum) wh.SugarDbType == FastDbTypeEnum.SysCoreLog).Select(sl => sl.EntityType).ToArray());

        // 初始化Admin核心库信息
        var sysAdminCoreDatabaseModel = new SysTenantMainDatabaseModel
        {
            Id = YitIdHelper.NextId(),
            TenantId = SystemConst.DefaultSystemTenantId,
            FastDbType = FastDbTypeEnum.SysAdminCore,
            IsSystem = YesOrNotEnum.N,
            ServiceIp = SqlSugarContext.ConnectionSettings.ServiceIp,
            Port = SqlSugarContext.ConnectionSettings.Port,
            DbName = $"Fast.Code_{superAdminTenantInfo.EnShortName}",
            DbUser = SqlSugarContext.ConnectionSettings.DbUser,
            DbPwd = SqlSugarContext.ConnectionSettings.DbPwd,
            DbType = SqlSugarContext.ConnectionSettings.DbType,
            CommandTimeOut = SqlSugarContext.ConnectionSettings.CommandTimeOut,
            SugarSqlExecMaxSeconds = SqlSugarContext.ConnectionSettings.SugarSqlExecMaxSeconds,
            DiffLog = true,
            DisableAop = false,
        };

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"初始化系统数据库完成！");

        return (superAdminTenantInfo, sysAdminCoreDatabaseModel);
    }
}