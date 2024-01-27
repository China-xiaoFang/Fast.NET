using Fast.Admin.Core.Constants;
using Fast.Admin.Core.Entity.System.Account;
using Fast.Admin.Core.Entity.System.Database;
using Fast.Admin.Core.Entity.System.Tenant;
using Fast.Admin.Core.Enum.Common;
using Fast.Admin.Core.Enum.Db;
using Fast.Admin.Core.Enum.System;
using Fast.Admin.Entity.Tenant.Organization;
using Fast.DependencyInjection;
using Fast.IaaS;
using Fast.SqlSugar;
using Fast.SqlSugar.Extensions;
using Fast.SqlSugar.Options;
using Mapster;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.Admin.Service.Tenant.CodeFirst;

/// <summary>
/// <see cref="TenantCodeFirstService"/> 租户库CodeFirst
/// </summary>
public class TenantCodeFirstService : ITenantCodeFirstService, ITransientDependency
{
    /// <summary>
    /// 初始化新租户信息
    /// </summary>
    /// <param name="newTenantModel"><see cref="SysTenantModel"/> 新租户信息</param>
    /// <param name="sysAdminCoreDatabaseModel"><see cref="SysTenantMainDatabaseModel"/> 租户库数据库配置</param>
    /// <param name="isInit"><see cref="bool"/> 是否为初始化</param>
    /// <returns></returns>
    public async Task InitNewTenant(SysTenantModel newTenantModel, SysTenantMainDatabaseModel sysAdminCoreDatabaseModel,
        bool isInit = false)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"初始化租户数据库中......");

        // 获取默认库
        var db = new SqlSugarClient(SqlSugarContext.DefaultConnectionConfigNoAop);

        sysAdminCoreDatabaseModel = await db.Insertable(sysAdminCoreDatabaseModel).ExecuteReturnEntityAsync();

        // 获取系统Admin核心库连接配置
        var adminCodeDb = new SqlSugarClient(
            SqlSugarContext.GetConnectionConfig(sysAdminCoreDatabaseModel.Adapt<ConnectionSettingsOptions>()));

        // 创建系统Admin核心库
        adminCodeDb.DbMaintenance.CreateDatabase();

        // 判断是否存在用户表
        if (await adminCodeDb.Ado.GetIntAsync(
                $"SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_NAME = '{typeof(TenUserModel).GetSugarTableName()}'") >
            0)
        {
            return;
        }

        // 初始化系统Admin核心库的表
        adminCodeDb.CodeFirst.InitTables(SqlSugarContext.SqlSugarEntityList
            .Where(wh => !wh.IsSplitTable && wh.SugarDbType != null &&
                         (FastDbTypeEnum) wh.SugarDbType == FastDbTypeEnum.SysAdminCore).Select(sl => sl.EntityType).ToArray());
        adminCodeDb.CodeFirst.SplitTables().InitTables(SqlSugarContext.SqlSugarEntityList
            .Where(wh => wh.IsSplitTable && wh.SugarDbType != null &&
                         (FastDbTypeEnum) wh.SugarDbType == FastDbTypeEnum.SysAdminCore).Select(sl => sl.EntityType).ToArray());

        // 初始化公司（组织架构）
        await adminCodeDb.Insertable(new TenOrgModel
        {
            Id = YitIdHelper.NextId(),
            ParentId = 0,
            ParentIds = new List<long> {0},
            OrgName = newTenantModel.ChName,
            OrgCode = "org_hq",
            Contacts = newTenantModel.AdminName,
        }).ExecuteReturnEntityAsync();

        var newAdminRole = new TenRoleModel
        {
            RoleName = RoleTypeEnum.AdminRole.GetDescription(),
            RoleCode = "manager_role",
            Sort = 1,
            DataScopeType = DataScopeTypeEnum.All,
            RoleType = RoleTypeEnum.AdminRole
        };
        // 初始化租户管理员角色
        newAdminRole = await adminCodeDb.Insertable(newAdminRole).ExecuteReturnEntityAsync();

        // 判断是否为初始化
        if (isInit)
        {
            // 初始化超级管理员
            await adminCodeDb.Insertable(new TenUserModel
            {
                Id = SystemConst.DefaultSuperAdminId,
                AccountId = SystemConst.DefaultSuperAdminId,
                JobNumber = "2024010101",
                NickName = "超级管理员",
                AdminType = AdminTypeEnum.SuperAdmin,
                Status = CommonStatusEnum.Enable,
            }).ExecuteCommandAsync();
            // 添加超级管理员关联
            await db.Insertable(new SysTenantAccountModel
            {
                Id = YitIdHelper.NextId(),
                AccountId = SystemConst.DefaultSuperAdminId,
                UserId = SystemConst.DefaultSuperAdminId,
                JobNumber = "2001010101",
                NickName = "超级管理员",
                AdminType = AdminTypeEnum.SuperAdmin,
                Status = CommonStatusEnum.Enable,
                TenantId = newTenantModel.Id,
            }).ExecuteCommandAsync();
        }

        var adminSysAccountModel = await db.Queryable<SysAccountModel>()
            .Where(wh => wh.Account == newTenantModel.Mobile).FirstAsync();

        // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
        if (adminSysAccountModel == null)
        {
            // 不存在，新增一个
            adminSysAccountModel = await db.Insertable(new SysAccountModel
            {
                Id = YitIdHelper.NextId(),
                Account = newTenantModel.Mobile,
                Password = CryptoUtil.MD5Encrypt(SystemConst.DefaultPassword),
                UserName = newTenantModel.AdminName,
                Sex = GenderEnum.Unknown,
                Email = newTenantModel.Email,
                Mobile = newTenantModel.Mobile,
                Status = CommonStatusEnum.Enable
            }).ExecuteReturnEntityAsync();
        }

        var adminTenUserModel = await adminCodeDb.Insertable(new TenUserModel
        {
            Id = YitIdHelper.NextId(),
            AccountId = adminSysAccountModel.Id,
            JobNumber = $"{DateTime.Now:yyyyMMdd}01",
            NickName = "系统管理员",
            AdminType = AdminTypeEnum.SystemAdmin,
            Status = CommonStatusEnum.Enable,
        }).ExecuteReturnEntityAsync();

        await db.Insertable(new SysTenantAccountModel
        {
            Id = YitIdHelper.NextId(),
            AccountId = adminSysAccountModel.Id,
            UserId = adminTenUserModel.Id,
            JobNumber = adminTenUserModel.JobNumber,
            NickName = "系统管理员",
            AdminType = AdminTypeEnum.SystemAdmin,
            Status = CommonStatusEnum.Enable,
            TenantId = SystemConst.DefaultSystemTenantId,
        }).ExecuteCommandAsync();

        // 初始化职工信息
        await adminCodeDb.Insertable(new TenEmpModel {UserId = adminTenUserModel.Id}).ExecuteCommandAsync();

        // 初始化用户角色
        await adminCodeDb.Insertable(new TenUserRoleModel {UserId = adminTenUserModel.Id, RoleId = newAdminRole.Id})
            .ExecuteCommandAsync();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@"初始化租户数据库完成！");
    }
}