using Fast.Core;
using Fast.DynamicApplication;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Test.Api.Application;

/// <summary>
/// 授权接口
/// </summary>
[ApiDescriptionSettings(Name = "auth", Order = 1)]
public class AuthApplication : IDynamicApplication
{
    /// <summary>
    /// 获取登录用户信息
    /// </summary>
    /// <returns></returns>
    [HttpGet("/getLoginUser")]
    public string Get()
    {
        return "我是Get请求";
    }

    /// <summary>
    /// 退出登录
    /// </summary>
    /// <returns></returns>
    [HttpPost("/logout")]
    public string Post()
    {
        return "我是Post请求";
    }

    /// <summary>
    /// 测试
    /// </summary>
    /// <returns></returns>
    [HttpGet("/test")]
    public async Task<string> Test()
    {
        Console.WriteLine("我执行了-----------");
        return "1";
    }
}