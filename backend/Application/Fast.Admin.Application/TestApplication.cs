using Fast.Core.AdminModel.Sys.Config;
using Fast.Core.Operation.Config;
using Fast.Core.Operation.Config.Dto;
using Fast.Core.Operation.Dict;
using Fast.Core.Operation.Dict.Dto;
using Fast.Core.SqlSugar.Repository;
using Fast.Iaas.Const;
using Furion.DynamicApiController;
using Furion.FriendlyException;
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
    public async Task<DictTypeInfo> TestDict(string name = "AdminType")
    {
        var result = await DictOperation.System.GetDictionaryAsync(name);
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
        var result = await ConfigOperation.System.GetConfigAsync(name);
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
        var result = await ConfigOperation.Tenant.GetConfigAsync(name);
        return result;
    }

    /// <summary>
    /// 测试多语言百度翻译
    /// </summary>
    /// <exception cref="Exception"></exception>
    [HttpGet("/test/testBaiduTranslator", "测试多语言百度翻译")]
    public void TestBaiduTranslator()
    {
        throw Oops.Bah("中文提示：百度翻译存在异常，1234567，test apple name string int long hello changsha");
    }

    /// <summary>
    /// 测试更新提交覆盖报错功能
    /// </summary>
    /// <returns></returns>
    [HttpGet("/test/testUpdateVersion", "测试更新提交覆盖报错功能")]
    public async Task TestUpdateVersion()
    {
        var _repository = App.GetService<ISqlSugarRepository<SysConfigModel>>();

        var model = await _repository.FirstOrDefaultAsync(f => f.IsDeleted == false);
        var model2 = await _repository.FirstOrDefaultAsync(f => f.IsDeleted == false);

        await _repository.Context.Updateable(model).EnableDiffLogEvent("测试差异日志").ExecuteCommandWithOptLockAsync(true);
        await _repository.Context.Updateable(model2).EnableDiffLogEvent("测试差异日志").ExecuteCommandWithOptLockAsync();
        await _repository.Context.Deleteable<SysConfigModel>().Where(wh => wh.IsDeleted == false).EnableDiffLogEvent("测试差异日志")
            .ExecuteCommandAsync();
        await _repository.Context.Updateable(model2).Where(wh => wh.IsDeleted == true).EnableDiffLogEvent("测试差异日志")
            .ExecuteCommandWithOptLockAsync();
    }
}