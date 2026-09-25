// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using System.Reflection;
using Fast.DynamicApplication;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fast.Swagger;

/// <summary>
/// 规范化文档自定义更多功能
/// </summary>
internal sealed class ApiActionFilter : IOperationFilter
{
    /// <inheritdoc />
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // 获取方法
        MethodInfo method = context.MethodInfo;

        // 处理更多描述
        if (method.IsDefined(typeof(ApiDescriptionSettingsAttribute), true))
        {
            ApiDescriptionSettingsAttribute apiDescriptionSettings =
                method.GetCustomAttribute<ApiDescriptionSettingsAttribute>(true);

            // 添加单一接口描述
            if (!string.IsNullOrWhiteSpace(apiDescriptionSettings?.Description))
            {
                operation.Description += apiDescriptionSettings.Description;
            }
        }

        // 处理过时
        if (method.IsDefined(typeof(ObsoleteAttribute), true))
        {
            ObsoleteAttribute deprecated = method.GetCustomAttribute<ObsoleteAttribute>(true);
            if (!string.IsNullOrWhiteSpace(deprecated?.Message))
            {
                operation.Description = $"<div>{deprecated.Message}</div>" + operation.Description;
            }
        }
    }
}
