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
}