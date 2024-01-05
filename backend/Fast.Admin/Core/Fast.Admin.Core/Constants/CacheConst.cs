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

namespace Fast.Admin.Core.Constants;

/// <summary>
/// <see cref="CacheConst"/> 缓存常量
/// </summary>
public class CacheConst
{
    /// <summary>
    /// 获取缓存Key
    /// </summary>
    /// <param name="cacheKey"><see cref="string"/> 缓存Key</param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string GetCacheKey(string cacheKey, params object[] args)
    {
        return string.Format(cacheKey, args);
    }

    /// <summary>
    /// 租户数据库信息
    /// <remarks>{0}，租户编号 {1}，数据库名类型</remarks>
    /// </summary>
    public const string TenantDataBaseInfo = "{0}:DataBase:{1}";

    /// <summary>
    /// 授权用户信息
    /// <remarks>{0}，租户编号</remarks>
    /// </summary>
    public const string AuthUserInfo = "{0}:Auth:";

    /// <summary>
    /// 过期 Token 缓存
    /// </summary>
    public const string ExpiredToken = "{0}:Expired:Token:";

    /// <summary>
    /// 过期 RefreshToken 缓存
    /// </summary>
    public const string ExpiredRefreshToken = "{0}:Expired:RefreshToken:";
}