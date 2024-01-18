// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
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

using Fast.Admin.Core.Authentication;
using Fast.Admin.Core.Constants;
using Fast.Admin.Core.Entity.Log.Entities;
using Fast.Admin.Core.Enum.Common;
using Fast.Admin.Core.Enum.Db;
using Fast.Admin.Core.Enum.System;
using Fast.Admin.Core.EventSubscriber.Sources;
using Fast.Admin.Core.EventSubscriber.SysLogSql;
using Fast.Admin.Core.Services;
using Fast.EventBus;
using Fast.SqlSugar.Attributes;
using Fast.SqlSugar.Handlers;
using Fast.SqlSugar.Options;
using SqlSugar;

namespace Fast.Admin.Core.Handlers;

/// <summary>
/// <see cref="SqlSugarEntityHandler"/> Sugar实体处理
/// </summary>
public class SqlSugarEntityHandler : ISqlSugarEntityHandler
{
    private readonly HttpContext _httpContext;
    private readonly ISqlSugarEntityService _sqlSugarEntityService;
    private readonly IEventPublisher _eventPublisher;

    public SqlSugarEntityHandler(IHttpContextAccessor httpContextAccessor, ISqlSugarEntityService sqlSugarEntityService,
        IEventPublisher eventPublisher)
    {
        _httpContext = httpContextAccessor.HttpContext;
        _sqlSugarEntityService = sqlSugarEntityService;
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// 获取 IUser，如果未登录或者报错，则为空
    /// </summary>
    /// <returns></returns>
    private IUser GetUser()
    {
        try
        {
            return FastContext.GetService<IUser>();
        }
        catch
        {
            return default;
        }
    }

    /// <summary>根据实体类型获取连接字符串</summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="sqlSugarClient"><see cref="T:SqlSugar.ISqlSugarClient" /> 默认库SqlSugar客户端</param>
    /// <param name="sugarDbType">实体类头部的 <see cref="T:Fast.SqlSugar.Attributes.SugarDbTypeAttribute" /> 特性，如果不存在可能为空</param>
    /// <param name="entityType"><see cref="T:System.Type" /> 实体类型</param>
    /// <returns></returns>
    public async Task<ConnectionSettingsOptions> GetConnectionSettings<TEntity>(ISqlSugarClient sqlSugarClient,
        SugarDbTypeAttribute sugarDbType, Type entityType)
    {
        // 获取枚举字符串
        var fastDbTypeStr = sugarDbType.Type?.ToString();

        // 判断是否为空
        if (sugarDbType.Type == null || fastDbTypeStr.IsEmpty())
        {
            return null;
        }

        // 获取数据库类型
        var fastDbType = System.Enum.Parse<FastDbTypeEnum>(sugarDbType.Type.ToString());

        // 租户Id
        var tenantId = 0L;

        // 是否为系统库
        var isSystem = YesOrNotEnum.N;

        // 数据库类型判断
        switch (fastDbType)
        {
            // 默认系统库
            case FastDbTypeEnum.SysCore:
                return null;
            // 系统日志库
            case FastDbTypeEnum.SysCoreLog:
                tenantId = SystemConst.DefaultSystemTenantId;
                isSystem = YesOrNotEnum.Y;
                break;
            // 
            case FastDbTypeEnum.SysAdminCore:
            default:
                tenantId = GetUser().TenantId;
                isSystem = YesOrNotEnum.N;
                break;
        }

        return await _sqlSugarEntityService.GetConnectionSettings(fastDbType, tenantId, isSystem);
    }

    /// <summary>执行Sql</summary>
    /// <param name="rawSql"><see cref="T:System.String" /> 原始Sql语句</param>
    /// <param name="parameters"><see cref="T:SqlSugar.SugarParameter" /> Sql参数</param>
    /// <param name="executionTime"><see cref="T:System.TimeSpan" /> 执行时间</param>
    /// <param name="handlerSql"><see cref="T:System.String" /> 参数化处理后的Sql语句</param>
    /// <returns></returns>
    public async Task ExecuteAsync(string rawSql, SugarParameter[] parameters, TimeSpan executionTime, string handlerSql)
    {
        // 获取日志库连接配置
        var logConnectionConfig = await _sqlSugarEntityService.GetLogSqlSugarClient();

        var _user = GetUser();

        var dateTime = DateTime.Now;

        // 组装数据
        var sysLogSqlExecModel = new SysLogSqlExecModel
        {
            DepartmentId = _user?.DepartmentId,
            DepartmentName = _user?.DepartmentName,
            CreatedUserId = _user?.UserId,
            CreatedUserName = _user?.UserName,
            CreatedTime = dateTime,
            Account = _user?.Account,
            JobNumber = _user?.JobNumber,
            RawSql = rawSql,
            Parameters = parameters,
            PureSql = handlerSql,
            ExecuteTime = dateTime,
            TenantId = _user?.TenantId
        };
        sysLogSqlExecModel.RecordCreate(_httpContext);

        // 事件总线执行日志
        await _eventPublisher.PublishAsync(new SqlSugarChannelEventSource(SysLogSqlEventSubscriberEnum.AddExecuteLog,
            logConnectionConfig, sysLogSqlExecModel));
    }

    /// <summary>执行Sql超时</summary>
    /// <param name="fileName"><see cref="T:System.String" /> 文件名称</param>
    /// <param name="fileLine"><see cref="T:System.Int32" /> 文件行数</param>
    /// <param name="methodName"><see cref="T:System.String" /> 方法名称</param>
    /// <param name="rawSql"><see cref="T:System.String" /> 未处理的Sql语句</param>
    /// <param name="parameters"><see cref="T:SqlSugar.SugarParameter" /> Sql参数</param>
    /// <param name="executeTime"><see cref="T:System.TimeSpan" /> 执行时间</param>
    /// <param name="handlerSql"><see cref="T:System.String" /> 参数化处理后的Sql语句</param>
    /// <param name="message"><see cref="T:System.String" /></param>
    /// <returns></returns>
    public async Task ExecuteTimeoutAsync(string fileName, int fileLine, string methodName, string rawSql,
        SugarParameter[] parameters, TimeSpan executeTime, string handlerSql, string message)
    {
        // 获取日志库连接配置
        var logConnectionConfig = await _sqlSugarEntityService.GetLogSqlSugarClient();

        var _user = GetUser();

        var dateTime = DateTime.Now;

        // 组装数据
        var sysLogSqlTimeoutModel = new SysLogSqlTimeoutModel
        {
            DepartmentId = _user?.DepartmentId,
            DepartmentName = _user?.DepartmentName,
            CreatedUserId = _user?.UserId,
            CreatedUserName = _user?.UserName,
            CreatedTime = dateTime,
            Account = _user?.Account,
            JobNumber = _user?.JobNumber,
            FileName = fileName,
            FileLine = fileLine,
            MethodName = methodName,
            TimeoutSeconds = executeTime.TotalSeconds,
            RawSql = rawSql,
            Parameters = parameters,
            PureSql = handlerSql,
            TimeoutTime = dateTime,
            TenantId = _user?.TenantId
        };
        sysLogSqlTimeoutModel.RecordCreate(_httpContext);

        // 事件总线执行日志
        await _eventPublisher.PublishAsync(new SqlSugarChannelEventSource(SysLogSqlEventSubscriberEnum.AddTimeoutLog,
            logConnectionConfig, sysLogSqlTimeoutModel));
    }

    /// <summary>执行Sql差异</summary>
    /// <param name="diffType"><see cref="T:SqlSugar.DiffType" /> 差异类型</param>
    /// <param name="tableName"><see cref="T:System.String" /> 表名称</param>
    /// <param name="tableDescription"><see cref="T:System.String" /> 表描述</param>
    /// <param name="diffDescription"><see cref="T:System.String" /> 差异描述</param>
    /// <param name="beforeColumnList"><see cref="T:System.String" /> 执行前列信息</param>
    /// <param name="afterColumnList"><see cref="T:System.String" /> 执行后列信息</param>
    /// <param name="rawSql"><see cref="T:System.String" /> 未处理的Sql语句</param>
    /// <param name="parameters"><see cref="T:SqlSugar.SugarParameter" /> Sql参数</param>
    /// <param name="executeTime"><see cref="T:System.TimeSpan" /> 执行时间</param>
    /// <param name="handlerSql"><see cref="T:System.String" /> 参数化处理后的Sql语句</param>
    /// <returns></returns>
    public async Task ExecuteDiffLogAsync(DiffType diffType, string tableName, string tableDescription, string diffDescription,
        List<List<DiffLogColumnInfo>> beforeColumnList, List<List<DiffLogColumnInfo>> afterColumnList, string rawSql,
        SugarParameter[] parameters, TimeSpan? executeTime, string handlerSql)
    {
        // 获取日志库连接配置
        var logConnectionConfig = await _sqlSugarEntityService.GetLogSqlSugarClient();

        var _user = GetUser();

        var dateTime = DateTime.Now;

        var localDiffType = diffType switch
        {
            DiffType.insert => DiffLogTypeEnum.Insert,
            DiffType.update => DiffLogTypeEnum.Update,
            DiffType.delete => DiffLogTypeEnum.Delete,
            _ => DiffLogTypeEnum.None
        };

        // 组装数据
        var sysLogSqlDiffModel = new SysLogSqlDiffModel
        {
            DepartmentId = _user?.DepartmentId,
            DepartmentName = _user?.DepartmentName,
            CreatedUserId = _user?.UserId,
            CreatedUserName = _user?.UserName,
            CreatedTime = dateTime,
            Account = _user?.Account,
            JobNumber = _user?.JobNumber,
            TableName = tableName,
            TableDescription = tableDescription,
            DiffDescription = diffDescription,
            BeforeColumnList = beforeColumnList,
            AfterColumnList = afterColumnList,
            ExecuteSeconds = executeTime?.TotalSeconds,
            RawSql = rawSql,
            Parameters = parameters,
            PureSql = handlerSql,
            DiffType = localDiffType,
            DiffTime = dateTime,
            TenantId = _user?.TenantId
        };
        sysLogSqlDiffModel.RecordCreate(_httpContext);

        // 事件总线执行日志
        await _eventPublisher.PublishAsync(new SqlSugarChannelEventSource(SysLogSqlEventSubscriberEnum.AddDiffLog,
            logConnectionConfig, sysLogSqlDiffModel));
    }

    /// <summary>执行Sql错误</summary>
    /// <param name="fileName"><see cref="T:System.String" /> 文件名称</param>
    /// <param name="fileLine"><see cref="T:System.Int32" /> 文件行数</param>
    /// <param name="methodName"><see cref="T:System.String" /> 方法名称</param>
    /// <param name="rawSql"><see cref="T:System.String" /> 原始Sql语句</param>
    /// <param name="parameters"><see cref="T:SqlSugar.SugarParameter" /> Sql参数</param>
    /// <param name="handlerSql"><see cref="T:System.String" /> 参数化处理后的Sql语句</param>
    /// <param name="message"><see cref="T:System.String" /></param>
    /// <returns></returns>
    public async Task ExecuteErrorAsync(string fileName, int fileLine, string methodName, string rawSql,
        SugarParameter[] parameters, string handlerSql, string message)
    {
        // 获取日志库连接配置
        var logConnectionConfig = await _sqlSugarEntityService.GetLogSqlSugarClient();

        var _user = GetUser();

        var dateTime = DateTime.Now;

        // 组装数据
        var sysLogSqlExModel = new SysLogSqlExModel
        {
            DepartmentId = _user?.DepartmentId,
            DepartmentName = _user?.DepartmentName,
            CreatedUserId = _user?.UserId,
            CreatedUserName = _user?.UserName,
            CreatedTime = dateTime,
            Account = _user?.Account,
            JobNumber = _user?.JobNumber,
            FileName = fileName,
            FileLine = fileLine,
            MethodName = methodName,
            RawSql = rawSql,
            Parameters = parameters,
            PureSql = handlerSql,
            ExceptionTime = dateTime,
            TenantId = _user?.TenantId
        };
        sysLogSqlExModel.RecordCreate(_httpContext);

        // 事件总线执行日志
        await _eventPublisher.PublishAsync(new SqlSugarChannelEventSource(SysLogSqlEventSubscriberEnum.AddErrorLog,
            logConnectionConfig, sysLogSqlExModel));
    }

    /// <summary>指派租户Id</summary>
    /// <returns></returns>
    public long? AssignTenantId()
    {
        return GetUser()?.TenantId;
    }

    /// <summary>指派部门Id</summary>
    /// <returns></returns>
    public long? AssignDepartmentId()
    {
        return GetUser()?.DepartmentId;
    }

    /// <summary>指派部门名称</summary>
    /// <returns></returns>
    public string AssignDepartmentName()
    {
        return GetUser()?.DepartmentName;
    }

    /// <summary>指派用户Id</summary>
    /// <returns></returns>
    public long? AssignUserId()
    {
        return GetUser()?.UserId;
    }

    /// <summary>指派用户名称</summary>
    /// <returns></returns>
    public string AssignUserName()
    {
        return GetUser()?.UserName;
    }

    /// <summary>是否为超级管理员</summary>
    /// <returns></returns>
    public bool IsSuperAdmin()
    {
        return GetUser()?.IsSuperAdmin ?? false;
    }

    /// <summary>是否为管理员</summary>
    /// <returns></returns>
    public bool IsAdmin()
    {
        return false;
    }
}