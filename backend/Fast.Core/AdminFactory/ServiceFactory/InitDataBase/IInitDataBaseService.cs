namespace Fast.Core.AdminFactory.ServiceFactory.InitDataBase;

/// <summary>
/// 数据化数据库服务接口
/// </summary>
public interface IInitDataBaseService
{
    /// <summary>
    /// 初始化数据库
    /// </summary>
    /// <returns></returns>
    Task InitDataBase();
}