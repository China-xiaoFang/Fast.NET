using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.Core.CodeFirst.Internal;
using Fast.Core.Const;
using SqlSugar;

namespace Fast.Core.CodeFirst.SeedData;

/// <summary>
/// 系统配置种子数据
/// </summary>
public class SysConfigSeedData : ISystemSeedData
{
    /// <summary>
    /// 种子数据
    /// </summary>
    /// <returns></returns>
    public async Task SeedData(SqlSugarProvider _db)
    {
        var data = new List<SysConfigModel>
        {
            new()
            {
                Id = 1000000001,
                Code = ConfigConst.Copyright.ICPCode,
                ChName = "ICP",
                EnName = "ICP",
                Value = GlobalContext.CopyrightInfoOptions.ICPCode,
                ConfigType = SysConfigTypeEnum.System
            },
            new()
            {
                Id = 1000000002,
                Code = ConfigConst.Copyright.ICPUrl,
                ChName = "ICP",
                EnName = "ICP",
                Value = GlobalContext.CopyrightInfoOptions.ICPUrl,
                ConfigType = SysConfigTypeEnum.System
            },
            new()
            {
                Id = 1000000003,
                Code = ConfigConst.Copyright.PublicCode,
                ChName = "Public",
                EnName = "Public",
                Value = GlobalContext.CopyrightInfoOptions.PublicCode,
                ConfigType = SysConfigTypeEnum.System
            },
            new()
            {
                Id = 1000000004,
                Code = ConfigConst.Copyright.PublicUrl,
                ChName = "Public",
                EnName = "Public",
                Value = GlobalContext.CopyrightInfoOptions.PublicUrl,
                ConfigType = SysConfigTypeEnum.System
            },
            new()
            {
                Id = 1000000005,
                Code = ConfigConst.Copyright.CRCode,
                ChName = "Copyright",
                EnName = "Copyright",
                Value = GlobalContext.CopyrightInfoOptions.CRCode,
                ConfigType = SysConfigTypeEnum.System
            },
            new()
            {
                Id = 1000000006,
                Code = ConfigConst.Copyright.CRUrl,
                ChName = "Copyright",
                EnName = "Copyright",
                Value = GlobalContext.CopyrightInfoOptions.CRUrl,
                ConfigType = SysConfigTypeEnum.System
            }
        };

        await _db.Insertable(data).ExecuteCommandAsync();
    }
}