// Copyright © 2018-Present 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供，相关免责声明及责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.md。

using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fast.Swagger;

/// <summary>
/// 修正 规范化文档 object schema，统一显示为 any
/// </summary>
/// <remarks>相关 issue：https://github.com/swagger-api/swagger-codegen-generators/issues/692</remarks>
internal sealed class AnySchemaFilter : ISchemaFilter
{
    /// <inheritdoc />
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        Type type = context.Type;

        if (type == typeof(object) && schema is OpenApiSchema openApiSchema)
        {
            openApiSchema.AdditionalPropertiesAllowed = false;
        }
    }
}
