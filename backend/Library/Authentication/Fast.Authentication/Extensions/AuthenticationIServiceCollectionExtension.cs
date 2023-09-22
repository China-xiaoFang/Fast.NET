using Fast.Authentication.Realize;
using Microsoft.Extensions.DependencyInjection;

namespace Fast.Authentication.Extensions;

/// <summary>
/// <see cref="IServiceCollection"/> 拓展类
/// </summary>
public static class AuthenticationIServiceCollectionExtension
{
    /// <summary>
    /// 添加鉴权用户
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddAuthentication(this IServiceCollection services)
    {
        // 单例注入
        services.AddSingleton<IUser, User>();

        return services;
    }
}