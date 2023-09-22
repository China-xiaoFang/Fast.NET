using Fast.Core.DynamicApiController.Attributes;
using Fast.Core.DynamicApiController.Dependencies;
using Microsoft.AspNetCore.Mvc;

namespace Fast.Test.Api.Application;

/// <summary>
/// 授权接口
/// </summary>
[ApiDescriptionSettings(Name = "auth", Order = 1)]
public class AuthApplication : IDynamicApiController
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
}