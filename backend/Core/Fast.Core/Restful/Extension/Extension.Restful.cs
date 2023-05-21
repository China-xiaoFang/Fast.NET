using System.Linq.Expressions;
using Fast.Core.AppLocalization;
using Fast.Core.Restful.Internal;
using Fast.Iaas.Extension;
using Fast.Iaas.Util;
using Fast.SqlSugar.Extension;
using Fast.SqlSugar.Internal;
using Furion.Logging;
using Furion.UnifyResult;

namespace Fast.Core.Restful.Extension;

/// <summary>
/// Restful风格返回扩展
/// </summary>
public static class Extension
{
    /// <summary>
    /// 获取规范化RESTful风格AES加密的返回值
    /// </summary>
    /// <param name="code"></param>
    /// <param name="success"></param>
    /// <param name="data"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public static XnRestfulResult<object> GetXnRestfulResult(int code, bool success, object data, object message)
    {
        // 时间
        var time = DateTimeOffset.UtcNow;

        // 时间戳
        var timestamp = time.ToUnixTimeMilliseconds();

        var resultData = data;

        // 判断是否成功
        if (success && data != null)
        {
            if (GlobalContext.SystemSettingsOptions.RequestAESDecrypt)
            {
                var dataJsonStr = data.ToJsonString();
                resultData = AESUtil.AESEncrypt(dataJsonStr, $"Fast.NET.XnRestful.{timestamp}", $"FIV{timestamp}");

                // 写日志文件
                Log.Debug($"HTTP Request AES 加密详情：\r\n\t源数据：{dataJsonStr}\r\n\tAES加密：{resultData}");
            }
        }

        // 一般为Model验证失败返回的结果
        if (message is Dictionary<string, string[]> messageObj)
        {
            var newMessage = "";
            foreach (var dicVal in messageObj.SelectMany(dicItem => dicItem.Value))
            {
                // 判断是否开启多语言
                if (GlobalContext.SystemSettingsOptions.AppLocalization)
                {
                    newMessage += $"{FL.Text(dicVal)}\r\n";
                }
                else
                {
                    newMessage += $"{dicVal}\r\n";
                }
            }

            message = newMessage.Remove(newMessage.LastIndexOf("\r\n", StringComparison.Ordinal));
        }
        else
        {
            // 字符串多语言处理
            // 判断是否开启多语言
            if (GlobalContext.SystemSettingsOptions.AppLocalization)
            {
                message = FL.Text(message.ToString());
            }
        }

        return new XnRestfulResult<object>
        {
            Code = code,
            Success = success,
            Data = resultData,
            Message = message,
            Extras = UnifyContext.Take(),
            Time = time.LocalDateTime,
            Timestamp = timestamp
        };
    }

    /// <summary>
    /// 替换SqlSugar分页
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="page"></param>
    /// <returns></returns>
    private static PageResult<T> XnPagedResult<T>(this SqlSugarPagedList<T> page)
    {
        return new PageResult<T>
        {
            PageNo = page.PageIndex,
            PageSize = page.PageSize,
            TotalPage = page.TotalPages,
            TotalRows = page.TotalCount,
            Rows = page.Items
        };
    }

    /// <summary>
    /// 小诺SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static PageResult<TEntity> ToXnPagedList<TEntity>(this ISugarQueryable<TEntity> queryable, int pageIndex, int pageSize)
    {
        return queryable.ToPagedList(pageIndex, pageSize).XnPagedResult();
    }

    /// <summary>
    /// 小诺SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public static async Task<PageResult<TEntity>> ToXnPagedListAsync<TEntity>(this ISugarQueryable<TEntity> queryable,
        int pageIndex, int pageSize)
    {
        return (await queryable.ToPagedListAsync(pageIndex, pageSize)).XnPagedResult();
    }

    /// <summary>
    /// 小诺SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static PageResult<TResult> ToXnPagedList<TEntity, TResult>(this ISugarQueryable<TEntity> queryable, int pageIndex,
        int pageSize, Expression<Func<TEntity, TResult>> expression)
    {
        return queryable.ToPagedList(pageIndex, pageSize, expression).XnPagedResult();
    }

    /// <summary>
    /// 小诺SqlSugar分页扩展
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="queryable"></param>
    /// <param name="pageIndex"></param>
    /// <param name="pageSize"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static async Task<PageResult<TResult>> ToXnPagedListAsync<TEntity, TResult>(this ISugarQueryable<TEntity> queryable,
        int pageIndex, int pageSize, Expression<Func<TEntity, TResult>> expression)
    {
        return (await queryable.ToPagedListAsync(pageIndex, pageSize, expression)).XnPagedResult();
    }
}