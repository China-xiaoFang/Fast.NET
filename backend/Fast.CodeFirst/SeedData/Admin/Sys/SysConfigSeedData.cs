using Fast.Core;
using Fast.Core.AdminFactory.ModelFactory.Sys;
using Fast.Core.CodeFirst.Internal;
using Fast.Core.Const;
using SqlSugar;

namespace Fast.CodeFirst.SeedData.Admin.Sys;

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
                ChName = "备案编号",
                EnName = "ICP Core",
                Value = GlobalContext.CopyrightInfoOptions.ICPCode,
            },
            new()
            {
                Id = 1000000002,
                Code = ConfigConst.Copyright.ICPUrl,
                ChName = "备案Url",
                EnName = "ICP Url",
                Value = GlobalContext.CopyrightInfoOptions.ICPUrl,
            },
            new()
            {
                Id = 1000000003,
                Code = ConfigConst.Copyright.PublicCode,
                ChName = "公安备案编号",
                EnName = "Public Core",
                Value = GlobalContext.CopyrightInfoOptions.PublicCode,
            },
            new()
            {
                Id = 1000000004,
                Code = ConfigConst.Copyright.PublicUrl,
                ChName = "公安备案Url",
                EnName = "Public Url",
                Value = GlobalContext.CopyrightInfoOptions.PublicUrl,
            },
            new()
            {
                Id = 1000000005,
                Code = ConfigConst.Copyright.CRCode,
                ChName = "版权信息",
                EnName = "Copyright Code",
                Value = GlobalContext.CopyrightInfoOptions.CRCode,
            },
            new()
            {
                Id = 1000000006,
                Code = ConfigConst.Copyright.CRUrl,
                ChName = "版权Url",
                EnName = "Copyright Url",
                Value = GlobalContext.CopyrightInfoOptions.CRUrl,
            },
            new()
            {
                Id = 1000000007,
                Code = ConfigConst.BaiduTranslator.AppId,
                ChName = "百度翻译开发者AppId",
                EnName = "Baidu Translation Developer AppId",
                Value = "",
            },
            new()
            {
                Id = 1000000008,
                Code = ConfigConst.BaiduTranslator.SecretKey,
                ChName = "百度翻译开发者密钥",
                EnName = "Baidu Translation Developer SecretKey",
                Value = "",
            },
            new()
            {
                Id = 1000000009,
                Code = ConfigConst.BaiduTranslator.Url,
                ChName = "百度翻译请求Url",
                EnName = "Baidu Translation Request Url",
                Value = "http://api.fanyi.baidu.com/api/trans/vip/translate",
            }
        };

        await _db.Insertable(data).ExecuteCommandAsync();
    }
}