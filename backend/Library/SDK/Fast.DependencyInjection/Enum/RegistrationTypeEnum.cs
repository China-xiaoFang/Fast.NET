
namespace Furion.DependencyInjection;

/// <summary>
/// 服务注册方式
/// </summary>
public enum RegistrationTypeEnum
{
    /// <summary>
    /// 添加服务
    /// </summary>
    Add = 0,

    /// <summary>
    /// 尝试添加服务
    /// </summary>
    TryAdd,

    /// <summary>
    /// 尝试添加服务集合
    /// </summary>
    TryAddEnumerable,

    /// <summary>
    /// 替换服务
    /// </summary>
    Replace
}