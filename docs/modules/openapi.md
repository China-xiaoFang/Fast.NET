# Fast.OpenApi

OpenAPI document generation and TypeScript code generation support.

## Installation

```bash
dotnet add package Fast.OpenApi
```

## Configuration

```json
{
  "OpenApiSettings": {
    "FolderGroup": true,
    "IgnoreSchemas": [],
    "PagedSchemaProperties": ["pageIndex", "pageSize", "searchValue"],
    "ImportSchemaMappings": [],
    "ImportTypeMappings": [],
    "BaseTypeMappings": {
      "Int64": "number",
      "String": "string",
      "date-time": "Date"
    }
  }
}
```

## Features

### API Document Generation

Generates structured OpenAPI documents with:
- Path definitions
- Component schemas
- Property definitions
- Type mappings

### TypeScript Code Generation

Maps .NET types to TypeScript types for frontend code generation:

| .NET Type | TypeScript Type |
| --- | --- |
| `Int32`, `Int64`, `Decimal` | `number` |
| `String` | `string` |
| `Boolean` | `boolean` |
| `DateTime` | `Date` |
| `Guid` | `string` |
| `List<T>` | `T[]` |

### Custom Schema Mappings

Configure custom import paths for generated code:

```json
{
  "ImportSchemaMappings": [
    {
      "Name": "ElSelectorOutput",
      "WebImportPath": "fast-element-plus",
      "MobileImportPath": ""
    }
  ]
}
```

## Difference from Fast.Swagger

| Aspect | Fast.OpenApi | Fast.Swagger |
| --- | --- | --- |
| Purpose | TypeScript codegen, structured API docs | Swashbuckle-based interactive documentation |
| Output | JSON document + generated code | Interactive HTML UI |
| Type Mapping | Custom TypeScript type mappings | JSON Schema types |
| Audience | Frontend developers | API consumers |
