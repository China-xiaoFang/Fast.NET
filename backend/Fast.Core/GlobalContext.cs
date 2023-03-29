using Fast.Admin.Model.Enum;
using Fast.Admin.Model.Model.Sys;
using Fast.Core.Const;
using Fast.Core.Options;
using Furion.FriendlyException;

namespace Fast.Core;

/// <summary>
/// 系统通用上下文
/// </summary>
public class GlobalContext
{
    /// <summary>
    /// 当前租户Id
    /// </summary>
    public static long TenantId
    {
        get
        {
            if (App.User is {Identity.IsAuthenticated: true})
            {
                // 获取Token中的
                return (App.User?.FindFirst(ClaimConst.TenantId)?.Value).ParseToLong();
            }

            if (App.HttpContext != null)
            {
                // 获取请求头中Base64加密的租户Id
                var headersTenantId = App.HttpContext.Request.Headers[ClaimConst.TenantId].ParseToString().Base64ToString()
                    .ParseToLong();
                if (!headersTenantId.IsNullOrZero())
                {
                    return headersTenantId;
                }

                //// 获取请求头中的站点Url
                //var headersWebUrl = OriginUrl;
                //if (!headersWebUrl.IsEmpty())
                //{
                //    var tenantInfo = App.GetService<ISysTenantService>().GetAllTenantInfo(wh => wh.WebUrl.Contains(headersWebUrl))
                //        .Result;
                //    if (tenantInfo is {Count: > 0})
                //    {
                //        return tenantInfo[0].Id;
                //    }
                //}
            }

            return 0L;
        }
    }

    /// <summary>
    /// 其他租户Id
    /// 根据业务自定义设置的租户Id
    /// 请谨慎使用
    /// </summary>
    public static long CustomTenantId { get; set; }

    /// <summary>
    /// 当前用户Id
    /// </summary>
    public static long UserId => (App.User?.FindFirst(ClaimConst.UserId)?.Value).ParseToLong();

    /// <summary>
    /// 当前用户账号
    /// </summary>
    public static string UserAccount => (App.User?.FindFirst(ClaimConst.Account)?.Value).ParseToString();

    /// <summary>
    /// 当前用户工号
    /// </summary>
    public static string UserJobNum => (App.User?.FindFirst(ClaimConst.JobNum)?.Value).ParseToString();

    /// <summary>
    /// 当前用户名称
    /// </summary>
    public static string UserName => (App.User?.FindFirst(ClaimConst.UserName)?.Value).ParseToString();

    /// <summary>
    /// 请求来源Url
    /// </summary>
    public static string OriginUrl => App.HttpContext.Request.Headers[ClaimConst.Origin].ParseToString();

    /// <summary>
    /// 请求客户端UUID，唯一标识，不安全
    /// </summary>
    public static string UUID => App.HttpContext.Request.Headers[ClaimConst.UUID].ParseToString();

    /// <summary>
    /// 是否超级管理员
    /// </summary>
    public static bool IsSuperAdmin
    {
        get
        {
            if (App.User == null || App.User.Identity?.IsAuthenticated == false)
                return false;
            return App.User.FindFirst(ClaimConst.AdminType)?.Value == AdminTypeEnum.SuperAdmin.GetHashCode().ParseToString();
        }
    }

    /// <summary>
    /// 是否系统管理员
    /// </summary>
    public static bool IsSystemAdmin
    {
        get
        {
            if (App.User == null || App.User.Identity?.IsAuthenticated == false)
                return false;
            return App.User.FindFirst(ClaimConst.AdminType)?.Value == AdminTypeEnum.SystemAdmin.GetHashCode().ParseToString();
        }
    }

    /// <summary>
    /// 是否租户管理员
    /// </summary>
    public static bool IsTenantAdmin
    {
        get
        {
            if (App.User == null || App.User.Identity?.IsAuthenticated == false)
                return false;
            return App.User.FindFirst(ClaimConst.AdminType)?.Value == AdminTypeEnum.TenantAdmin.GetHashCode().ParseToString();
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
            throw Oops.Oh("租户系统异常！");

        return 0;
    }

    /// <summary>
    /// 系统配置
    /// </summary>
    public static SystemSettingsOptions SystemSettingsOptions { get; set; }

    /// <summary>
    /// 缓存配置
    /// </summary>
    public static CacheOptions CacheOptions { get; set; }

    /// <summary>
    /// 数据库配置
    /// </summary>
    public static ConnectionStringsOptions ConnectionStringsOptions { get; set; }

    /// <summary>
    /// 版权信息
    /// </summary>
    public static CopyrightInfoOptions CopyrightInfoOptions { get; set; }

    /// <summary>
    /// 上传文件配置
    /// </summary>
    public static UploadFileOptions UploadFileOptions { get; set; }

    /// <summary>
    /// 服务配置集合
    /// </summary>
    public static ServiceCollectionOptions ServiceCollectionOptions { get; set; }

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