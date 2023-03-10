using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.SDK.Common.CodeFirst.Internal;
using SqlSugar;
using Yitter.IdGenerator;

namespace Fast.CodeFirst.SeedData.Admin.Sys;

/// <summary>
/// 系统菜单种子数据
/// </summary>
public class SysMenuSeedData : ISystemSeedData
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <param name="_db"></param>
    /// <returns></returns>
    public async Task SeedData(SqlSugarProvider _db)
    {
        var sysModuleList = new List<SysModuleModel>();
        var sysMenuList = new List<SysMenuModel>();
        // Fast.NET
        var fastModuleInfo = new SysModuleModel
        {
            Id = YitIdHelper.NextId(),
            ModuleName = "Fast.NET",
            Color = "#05a045",
            Icon = "profile-outlined",
            ViewType = ModuleViewTypeEnum.All,
            IsSystem = YesOrNotEnum.Y,
            Sort = 0,
            Status = CommonStatusEnum.Enable
        };
        sysModuleList.Add(fastModuleInfo);
        // 系统首页
        var indexMenuInfo = new SysMenuModel
        {
            Id = YitIdHelper.NextId(),
            MenuCode = "index",
            MenuName = "系统首页",
            MenuTitle = "首页",
            ParentId = 0,
            ModuleId = fastModuleInfo.Id,
            MenuType = MenuTypeEnum.Menu,
            Icon = "home-outlined",
            Router = "/index",
            Component = "index/index",
            Link = null,
            IsSystem = YesOrNotEnum.Y,
            Sort = 0,
            Status = CommonStatusEnum.Enable
        };
        sysMenuList.Add(indexMenuInfo);
        // Saas功能
        var tenantModuleInfo = new SysModuleModel
        {
            Id = YitIdHelper.NextId(),
            ModuleName = "Saas功能",
            Color = "#d81b43",
            Icon = "appstore-add-outlined",
            ViewType = ModuleViewTypeEnum.SuperAdmin,
            IsSystem = YesOrNotEnum.Y,
            Sort = 999,
            Status = CommonStatusEnum.Enable
        };
        sysModuleList.Add(tenantModuleInfo);
        // Saas租户
        var saasMenuInfo = new SysMenuModel
        {
            Id = YitIdHelper.NextId(),
            MenuCode = "saasManage",
            MenuName = "Saas租户",
            MenuTitle = null,
            ParentId = 0,
            ModuleId = tenantModuleInfo.Id,
            MenuType = MenuTypeEnum.Catalog,
            Icon = "audit-outlined",
            Router = null,
            Component = null,
            Link = null,
            IsSystem = YesOrNotEnum.Y,
            Sort = 1,
            Status = CommonStatusEnum.Enable
        };
        sysMenuList.Add(saasMenuInfo);
        // 租户管理
        var tenantMenuInfo = new SysMenuModel
        {
            Id = YitIdHelper.NextId(),
            MenuCode = "tenantManage",
            MenuName = "租户管理",
            MenuTitle = "租户管理",
            ParentId = saasMenuInfo.Id,
            ModuleId = tenantModuleInfo.Id,
            MenuType = MenuTypeEnum.Menu,
            Icon = "usergroup-add-outlined",
            Router = "/index",
            Component = "index/index",
            Link = null,
            IsSystem = YesOrNotEnum.Y,
            Sort = 1,
            Status = CommonStatusEnum.Enable
        };
        sysMenuList.Add(tenantMenuInfo);

        await _db.Insertable(sysModuleList).ExecuteCommandAsync();
        await _db.Insertable(sysMenuList).ExecuteCommandAsync();
    }
}