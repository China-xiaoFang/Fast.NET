using Fast.Core.AdminFactory.EnumFactory;
using Fast.Core.Const;
using Fast.Core.Operation.Config;
using Fast.Core.Operation.Config.Dto;
using Fast.Core.Operation.Dict;
using Fast.Core.Operation.Dict.Dto;
using Furion.DynamicApiController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Admin.Application;

/// <summary>
/// 测试接口
/// </summary>
[ApiDescriptionSettings(ApiGroupConst.Default, Name = "Test", Order = 100), AllowAnonymous]
public class TestApplication : IDynamicApiController
{
    /// <summary>
    /// 测试接口
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("/test/test", "测试接口")]
    public string Test(string name = "接口默认")
    {
        return name;
    }

    /// <summary>
    /// 测试静态字典获取
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("/test/testDict", "测试静态字典获取")]
    public async Task<SysDictTypeInfo> TestDict(string name = "AdminType")
    {
        var result = await DictOperation.GetDictionaryAsync(name);
        return result;
    }

    /// <summary>
    /// 测试静态配置获取
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("/test/testConfig", "测试静态配置获取")]
    public async Task<ConfigInfo> TestConfig(string name = ConfigConst.Copyright.ICPCode)
    {
        var result = await ConfigOperation.GetConfigAsync(name);
        return result;
    }

    /// <summary>
    /// 测试静态配置获取
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpGet("/test/testTenConfig", "测试静态配置获取")]
    public async Task<ConfigInfo> TestTenConfig(string name = ConfigConst.Tenant.WebName)
    {
        var result = await ConfigOperation.GetConfigAsync(name, SysConfigTypeEnum.Tenant);
        return result;
    }
}