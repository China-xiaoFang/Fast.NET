using Fast.Core.AdminFactory.ModelFactory.Basis;
using Fast.Core.CodeFirst.Internal;

namespace Fast.Core.CodeFirst.SeedData;

/// <summary>
/// 租户配置种子数据
/// </summary>
public class TenConfigSeedData : ITenantSeedData
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <param name="_db"></param>
    /// <returns></returns>
    public async Task SeedData(SqlSugarProvider _db)
    {
        var data = new List<TenConfigModel>
        {
            new()
            {
                Id = 1000000001,
                Code = ConfigConst.Tenant.WebName,
                ChName = "系统名称",
                EnName = "Web Name",
                Value = "Fast.NET",
                Remark = ""
            },
            new()
            {
                Id = 1000000002,
                Code = ConfigConst.Tenant.WebLogo,
                ChName = "系统Logo",
                EnName = "Web Logo",
                Value = "https://gitee.com/Net-18K/fast.net/raw/master/frontend/public/logo.png",
                Remark = ""
            },
            new()
            {
                Id = 1000000003,
                Code = ConfigConst.Tenant.WebDescribe,
                ChName = "系统描述",
                EnName = "Web Describe,",
                Value = "Fast.NET 后台管理系统。（持续集百家所长，完善与丰富本框架基础设施，为.NET生态增加一种选择！）",
                Remark = ""
            },
            new()
            {
                Id = 1000000004,
                Code = ConfigConst.Tenant.VerCodeSwitch,
                ChName = "验证码开关",
                EnName = "Ver Code",
                Value = "true",
                Remark = "true 开；false 关；用于登录界面验证码。"
            },
        };
        await _db.Insertable(data).ExecuteCommandAsync();
    }
}