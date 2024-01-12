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

using System.Reflection;
using Fast.IaaS;
using Fast.Swagger.Commons;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Fast.Swagger.Options;

/// <summary>
/// <see cref="SwaggerSettingsOptions"/> Swagger配置选项
/// </summary>
public sealed class SwaggerSettingsOptions : IPostConfigure
{
    /// <summary>
    /// 是否启用/注入规范化文档
    /// </summary>
    public bool? Enable { get; set; }

    /// <summary>
    /// 文档标题
    /// </summary>
    public string DocumentTitle { get; set; }

    /// <summary>
    /// 默认分组名
    /// </summary>
    public string DefaultGroupName { get; set; }

    /// <summary>
    /// 启用授权支持
    /// </summary>
    public bool? EnableAuthorized { get; set; }

    /// <summary>
    /// 格式化为V2版本
    /// </summary>
    public bool? FormatAsV2 { get; set; }

    /// <summary>
    /// 配置规范化文档地址
    /// </summary>
    public string RoutePrefix { get; set; }

    /// <summary>
    /// 文档展开设置
    /// </summary>
    public DocExpansion? DocExpansionState { get; set; }

    /// <summary>
    /// XML 描述文件
    /// </summary>
    public string[] XmlComments { get; set; }

    /// <summary>
    /// 分组信息
    /// </summary>
    public SwaggerOpenApiInfo[] GroupOpenApiInfos { get; set; }

    /// <summary>
    /// 安全定义
    /// </summary>
    public SwaggerOpenApiSecurityScheme[] SecurityDefinitions { get; set; }

    /// <summary>
    /// 配置 Servers
    /// </summary>
    public OpenApiServer[] Servers { get; set; }

    /// <summary>
    /// 隐藏 Servers
    /// </summary>
    public bool? HideServers { get; set; }

    /// <summary>
    /// 默认 swagger.json 路由模板
    /// </summary>
    public string RouteTemplate { get; set; }

    /// <summary>
    /// 配置安装第三方包的分组名
    /// </summary>
    public string[] PackagesGroups { get; set; }

    /// <summary>
    /// 启用枚举 Schema 筛选器
    /// </summary>
    public bool? EnableEnumSchemaFilter { get; set; }

    /// <summary>
    /// 启用标签排序筛选器
    /// </summary>
    public bool? EnableTagsOrderDocumentFilter { get; set; }

    /// <summary>
    /// 服务目录（修正 IIS 创建 Application 问题）
    /// </summary>
    public string ServerDir { get; set; }

    /// <summary>
    /// 配置规范化文档登录信息
    /// </summary>
    public SwaggerLoginInfo LoginInfo { get; set; }

    /// <summary>
    /// 启用 All Groups 功能
    /// </summary>
    public bool? EnableAllGroups { get; set; }

    /// <summary>
    /// 枚举类型生成值类型
    /// </summary>
    public bool? EnumToNumber { get; set; }

    /// <summary>
    /// 后期配置
    /// </summary>
    public void PostConfigure()
    {
        Enable ??= true;

        DocumentTitle ??= "Specification Api Document";
        DefaultGroupName ??= "Default";
        FormatAsV2 ??= false;
        DocExpansionState ??= DocExpansion.List;

        // 加载项目注册和模块化/插件注释
        var frameworkPackageName = GetType().GetTypeInfo().Assembly.GetName().Name;
        var projectXmlComments = IaaSContext.Assemblies.Where(u => u.GetName().Name != frameworkPackageName)
            .Select(t => t.GetName().Name);
        XmlComments ??= projectXmlComments.ToArray();

        GroupOpenApiInfos ??= new[] {new SwaggerOpenApiInfo {Group = DefaultGroupName}};

        EnableAuthorized ??= true;
        if (EnableAuthorized == true)
        {
            SecurityDefinitions ??= new[]
            {
                new SwaggerOpenApiSecurityScheme
                {
                    Id = "Bearer",
                    Type = SecuritySchemeType.Http,
                    Name = "Authorization",
                    Description = "JWT Authorization header using the Bearer scheme.",
                    BearerFormat = "JWT",
                    Scheme = "bearer",
                    In = ParameterLocation.Header,
                    Requirement = new SwaggerOpenApiSecurityRequirementItem
                    {
                        Scheme = new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {Id = "Bearer", Type = ReferenceType.SecurityScheme}
                        },
                        Accesses = Array.Empty<string>()
                    }
                }
            };
        }

        Servers ??= Array.Empty<OpenApiServer>();
        HideServers ??= true;
        RouteTemplate ??= "swagger/{documentName}/swagger.json";
        PackagesGroups ??= Array.Empty<string>();
        EnableEnumSchemaFilter ??= true;
        EnableTagsOrderDocumentFilter ??= true;
        EnableAllGroups ??= false;
        EnumToNumber ??= false;
    }
}