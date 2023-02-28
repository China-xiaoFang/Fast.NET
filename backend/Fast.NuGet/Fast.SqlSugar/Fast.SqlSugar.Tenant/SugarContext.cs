using Fast.SqlSugar.Tenant.Internal.Options;
using Fast.SqlSugar.Tenant.SugarEntity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using SqlSugar;

namespace Fast.SqlSugar.Tenant;

/// <summary>
/// SqlSugar通用上下文
/// </summary>
public class SugarContext
{
    #region Func委托

    /// <summary>
    /// 信息日志
    /// </summary>
    public static Action<string> ActionLogInformation { get; set; }

    /// <summary>
    /// 设置委托信息日志
    /// </summary>
    /// <param name="actionLogInformation"></param>
    public static void SetActionLogInformation(Action<string> actionLogInformation)
    {
        ActionLogInformation = actionLogInformation;
    }

    /// <summary>
    /// 警告日志
    /// </summary>
    public static Action<string> ActionLogWarning { get; set; }

    /// <summary>
    /// 设置委托警告日志
    /// </summary>
    /// <param name="actionLogWarning"></param>
    public static void SetActionLogWarning(Action<string> actionLogWarning)
    {
        ActionLogWarning = actionLogWarning;
    }

    /// <summary>
    /// 错误日志
    /// </summary>
    public static Action<string> ActionLogError { get; set; }

    /// <summary>
    /// 设置委托错误日志
    /// </summary>
    /// <param name="actionLogError"></param>
    public static void SetActionLogError(Action<string> actionLogError)
    {
        ActionLogError = actionLogError;
    }

    /// <summary>
    /// 设置Sugar日志委托
    /// </summary>
    /// <param name="actionLogInformation"></param>
    /// <param name="actionLogWarning"></param>
    /// <param name="actionLogError"></param>
    public static void SetSugarLogFunc(Action<string> actionLogInformation, Action<string> actionLogWarning,
        Action<string> actionLogError)
    {
        ActionLogInformation = actionLogInformation;
        ActionLogWarning = actionLogWarning;
        ActionLogError = actionLogError;
    }

    /// <summary>
    /// 日志
    /// </summary>
    internal static class Log
    {
        public static void Information(string message)
        {
            if (ActionLogInformation == null)
            {
                Console.WriteLine($"Fast.SqlSugar.Information：{message}");
            }
            else
            {
                ActionLogInformation?.Invoke(message);
            }
        }

        public static void Warning(string message)
        {
            if (ActionLogWarning == null)
            {
                Console.WriteLine($"Fast.SqlSugar.Warning：{message}");
            }
            else
            {
                ActionLogWarning?.Invoke(message);
            }
        }

        public static void Error(string message)
        {
            if (ActionLogError == null)
            {
                Console.WriteLine($"Fast.SqlSugar.Error：{message}");
            }
            else
            {
                ActionLogError?.Invoke(message);
            }
        }
    }

    /// <summary>
    /// 委托获取租户Id
    /// </summary>
    public static Func<long> FuncGetTenantId { get; set; }

    /// <summary>
    /// 设置委托获取租户Id
    /// </summary>
    /// <param name="funcGetTenantId"></param>
    public static void SetFuncGetTenantId(Func<long> funcGetTenantId)
    {
        FuncGetTenantId = funcGetTenantId;
    }

    /// <summary>
    /// 委托获取用户Id
    /// </summary>
    public static Func<long> FuncGetUserId { get; set; }

    /// <summary>
    /// 设置委托获取用户Id
    /// </summary>
    /// <param name="funcGetUserId"></param>
    public static void SetFuncGetUserId(Func<long> funcGetUserId)
    {
        FuncGetUserId = funcGetUserId;
    }

    /// <summary>
    /// 委托获取用户名称
    /// </summary>
    public static Func<string> FuncGetUserName { get; set; }

    /// <summary>
    /// 设置委托获取用户名称
    /// </summary>
    /// <param name="funcGetUserName"></param>
    public static void SetFuncGetUserName(Func<string> funcGetUserName)
    {
        FuncGetUserName = funcGetUserName;
    }

    /// <summary>
    /// 委托获取是否超级管理员
    /// </summary>
    public static Func<bool> FuncGetIsSuperAdmin { get; set; }

    /// <summary>
    /// 设置委托获取是否超级管理员
    /// </summary>
    /// <param name="funcGetIsSuperAdmin"></param>
    public static void SetFuncGetIsSuperAdmin(Func<bool> funcGetIsSuperAdmin)
    {
        FuncGetIsSuperAdmin = funcGetIsSuperAdmin;
    }

    /// <summary>
    /// 委托获取是否系统管理员
    /// </summary>
    public static Func<bool> FuncGetIsSystemAdmin { get; set; }

    /// <summary>
    /// 设置委托获取是否系统管理员
    /// </summary>
    /// <param name="funcGetIsSystemAdmin"></param>
    public static void SetFuncGetIsSystemAdmin(Func<bool> funcGetIsSystemAdmin)
    {
        FuncGetIsSystemAdmin = funcGetIsSystemAdmin;
    }

    /// <summary>
    /// 委托获取是否租户管理员
    /// </summary>
    public static Func<bool> FuncGetIsTenantAdmin { get; set; }

    /// <summary>
    /// 设置委托获取是否租户管理员
    /// </summary>
    /// <param name="funcGetIsTenantAdmin"></param>
    public static void SetFuncGetIsTenantAdmin(Func<bool> funcGetIsTenantAdmin)
    {
        FuncGetIsTenantAdmin = funcGetIsTenantAdmin;
    }

    /// <summary>
    /// 设置Sugar委托
    /// </summary>
    /// <param name="funcGetTenantId"></param>
    /// <param name="funcGetUserId"></param>
    /// <param name="funcGetUserName"></param>
    /// <param name="funcGetIsSuperAdmin"></param>
    /// <param name="funcGetIsSystemAdmin"></param>
    /// <param name="funcGetIsTenantAdmin"></param>
    public static void SetSugarFunc(Func<long> funcGetTenantId, Func<long> funcGetUserId, Func<string> funcGetUserName,
        Func<bool> funcGetIsSuperAdmin, Func<bool> funcGetIsSystemAdmin, Func<bool> funcGetIsTenantAdmin)
    {
        FuncGetTenantId = funcGetTenantId;
        FuncGetUserId = funcGetUserId;
        FuncGetUserName = funcGetUserName;
        FuncGetIsSuperAdmin = funcGetIsSuperAdmin;
        FuncGetIsSystemAdmin = funcGetIsSystemAdmin;
        FuncGetIsTenantAdmin = funcGetIsTenantAdmin;
    }

    #endregion

    /// <summary>
    /// 分布式内存缓存
    /// </summary>
    internal static IMemoryCache _memoryCache { get; set; }

    /// <summary>
    /// Web托管信息
    /// </summary>
    internal static IHostEnvironment HostEnvironment { get; set; }

    /// <summary>
    /// 租户Id
    /// </summary>
    public static long TenantId
    {
        get
        {
            try
            {
                return FuncGetTenantId();
            }
            catch (Exception)
            {
                return 0L;
            }
        }
    }

    /// <summary>
    /// 自定义租户Id
    /// 根据业务自定义设置的租户Id
    /// 请谨慎使用
    /// </summary>
    public static long CustomTenantId { get; set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    public static long UserId
    {
        get
        {
            try
            {
                return FuncGetUserId();
            }
            catch (Exception)
            {
                return 0L;
            }
        }
    }

    /// <summary>
    /// 用户名称
    /// </summary>
    public static string UserName
    {
        get
        {
            try
            {
                return FuncGetUserName();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    public static bool IsSuperAdmin
    {
        get
        {
            try
            {
                return FuncGetIsSuperAdmin();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 是否系统管理员
    /// </summary>
    public static bool IsSystemAdmin
    {
        get
        {
            try
            {
                return FuncGetIsSystemAdmin();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 是否租户管理员
    /// </summary>
    public static bool IsTenantAdmin
    {
        get
        {
            try
            {
                return FuncGetIsTenantAdmin();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    /// <summary>
    /// 获取租户Id，
    /// 复杂业务请用此方法
    /// </summary>
    /// <param name="isThrow">是否抛出错误</param>
    /// <returns></returns>
    public static long GetTenantId(bool isThrow = true)
    {
        if (TenantId.ToString().Trim() != "0")
        {
            return TenantId;
        }

        if (CustomTenantId.ToString().Trim() != "0")
        {
            return CustomTenantId;
        }

        if (isThrow)
            // 租户系统异常！
            throw new SqlSugarException("租户系统异常！");

        return 0;
    }

    /// <summary>
    /// 数据库配置
    /// </summary>
    public static ConnectionStringsOptions ConnectionStringsOptions { get; set; }

    /// <summary>
    /// Sugar数据库信息
    /// </summary>
    public static List<SysTenantDataBaseModel> SugarDbInfoList { get; set; } = new();

    /// <summary>
    /// 获取Sugar数据库信息
    /// </summary>
    /// <param name="id">标识Id</param>
    public static SysTenantDataBaseModel GetSugarDbInfo(long id)
    {
        return SugarDbInfoList.FirstOrDefault(f => f.Id == id);
    }

    /// <summary>
    /// 获取Sugar数据库信息
    /// </summary>
    /// <param name="connectionId">SqlSugarClient 连接Id</param>
    public static SysTenantDataBaseModel GetSugarDbInfo(string connectionId)
    {
        return SugarDbInfoList.FirstOrDefault(f => f.ConnectionId == connectionId);
    }

    /// <summary>
    /// 添加Sugar数据库信息
    /// </summary>
    /// <param name="model"></param>
    public static void AddSugarDbInfo(SysTenantDataBaseModel model)
    {
        if (model == null)
            return;
        if (SugarDbInfoList.All(wh => wh.Id != model.Id))
        {
            SugarDbInfoList.Add(model);
        }
    }

    /// <summary>
    /// 删除Sugar数据库信息
    /// </summary>
    /// <param name="id">标识Id</param>
    public static void DeleteSugarDbInfo(long id)
    {
        // 查找下表
        var index = SugarDbInfoList.FindIndex(f => f.Id == id);
        if (index >= 0)
        {
            SugarDbInfoList.RemoveAt(index);
        }
    }

    /// <summary>
    /// 删除Sugar数据库信息
    /// </summary>
    /// <param name="connectionId">SqlSugarClient 连接Id</param>
    public static void DeleteSugarDbInfo(string connectionId)
    {
        // 查找下表
        var index = SugarDbInfoList.FindIndex(f => f.ConnectionId == connectionId);
        if (index >= 0)
        {
            SugarDbInfoList.RemoveAt(index);
        }
    }

    /// <summary>
    /// 清空Sugar数据库信息
    /// </summary>
    public static void ClearSugarDbInfo()
    {
        SugarDbInfoList = new List<SysTenantDataBaseModel>();
    }
}