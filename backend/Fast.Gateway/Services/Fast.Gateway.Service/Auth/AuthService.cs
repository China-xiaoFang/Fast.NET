// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using Fast.Gateway.Entities.Entities.User;
using Fast.Gateway.Service.Auth.Dto;
using Fast.JwtBearer.Services;
using Microsoft.AspNetCore.Authentication;

namespace Fast.Gateway.Service.Auth;

/// <summary>
/// <see cref="AuthService"/> 授权服务
/// </summary>
public class AuthService : IAuthService, ITransientDependency
{
    private readonly ISqlSugarRepository<UserAccount> _repository;
    private readonly IJwtBearerCryptoService _jwtBearerCryptoService;

    public AuthService(ISqlSugarRepository<UserAccount> repository, IJwtBearerCryptoService jwtBearerCryptoService)
    {
        _repository = repository;
        _jwtBearerCryptoService = jwtBearerCryptoService;
    }

    /// <summary>
    /// <see cref="Login"/> 登录
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public async Task<LoginOutput> Login(LoginInput input)
    {
        // 根据账号判断是否存在
        var model = await _repository.Where(wh => wh.Account == input.Account).FirstAsync();

        if (model == null)
        {
            throw new UserFriendlyException("账号不存在！");
        }

        if (model.Enabled == false)
        {
            throw new UserFriendlyException("账号已被禁用！");
        }

        // 判断密码是否相同
        if (input.Password != model.Password)
        {
            throw new UserFriendlyException("密码错误！");
        }

        // 生成Token令牌，30分钟
        var accessToken = _jwtBearerCryptoService.GenerateToken(new Dictionary<string, object>
        {
            {"Id", model.Id}, {"Account", model.Account}, {"Name", model.Name}
        });

        // 获取刷新Token，30天
        var refreshToken = _jwtBearerCryptoService.GenerateRefreshToken(accessToken);

        // 设置Token令牌
        FastContext.HttpContext.Response.Headers["access-token"] = accessToken;

        // 设置刷新Token令牌
        FastContext.HttpContext.Response.Headers["x-access-token"] = refreshToken;

        return new LoginOutput {Account = model.Account, Name = model.Name};
    }
}