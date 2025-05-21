// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Fast.JwtBearer;

/// <summary>
/// 授权策略提供器
/// </summary>
internal sealed class AppAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    /// <summary>
    /// 默认回退策略
    /// </summary>
    public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="options"></param>
    public AppAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    /// <summary>
    /// 获取默认策略
    /// </summary>
    /// <returns></returns>
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return FallbackPolicyProvider.GetDefaultPolicyAsync();
    }

    /// <summary>
    /// 获取回退策略
    /// </summary>
    /// <returns></returns>
    public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
    {
        return FallbackPolicyProvider.GetFallbackPolicyAsync();
    }

    /// <summary>
    /// 获取策略
    /// </summary>
    /// <param name="policyName"></param>
    /// <returns></returns>
    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        // 判断是否是包含授权策略前缀
        if (policyName.StartsWith("<Fast.JwtBearer.AppAuthorizeRequirement>"))
        {
            // 解析策略名并获取策略参数
            var policies = policyName["<Fast.JwtBearer.AppAuthorizeRequirement>".Length..]
                .Split(',', StringSplitOptions.RemoveEmptyEntries);

            // 添加策略需求
            var policy = new AuthorizationPolicyBuilder();
            policy.AddRequirements(new AppAuthorizeRequirement(policies));

            return Task.FromResult(policy.Build());
        }

        // 如果策略不匹配，则返回回退策略
        return FallbackPolicyProvider.GetPolicyAsync(policyName);
    }
}