using Fast.Core.Operation.Dict;
using Fast.Core.Operation.Dict.Dto;
using Furion.DependencyInjection;
using Furion.DynamicApiController;

namespace Fast.Core.AdminFactory.ServiceFactory;

public class TestService : IDynamicApiController, ITransient
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
}