# Fast.Serialization

JSON serialization modules with custom converters. Choose between `System.Text.Json` and `Newtonsoft.Json`.

## Installation

Choose one:

```bash
# System.Text.Json (recommended for new projects)
dotnet add package Fast.Serialization.System.Text.Json

# Newtonsoft.Json (for compatibility)
dotnet add package Fast.Serialization.Newtonsoft.Json
```

## Registration

```csharp
// System.Text.Json
builder.Services.AddSerialization();

// With custom options
builder.Services.AddSerialization(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
```

## Custom Converters

Both modules include 11 custom converters:

| Converter | Description |
| --- | --- |
| `DateJsonConverter` | Custom date formatting |
| `DateTimeJsonConverter` | DateTime formatting |
| `DateTimeOffsetJsonConverter` | DateTimeOffset handling |
| `DecimalJsonConverter` | Decimal precision handling |
| `DoubleJsonConverter` | Double number handling |
| `EnumJsonConverter` | Enum serialization |
| `ExceptionJsonConverter` | Exception serialization |
| `IntJsonConverter` | Integer handling |
| `LongJsonConverter` | Long integer handling (string for JS) |
| `TimeJsonConverter` | TimeSpan formatting |
| `DataMaskingConverter` | Sensitive data masking |

## Data Masking

Mark sensitive properties for automatic masking in JSON output:

```csharp
public class UserDto
{
    public string Name { get; set; }

    [DataMasking]
    public string Phone { get; set; }    // Output: "138****8888"

    [DataMasking]
    public string IdCard { get; set; }   // Output: "110***********1234"
}
```

## Long Integer Safety

JavaScript cannot handle 64-bit integers accurately. The `LongJsonConverter` automatically serializes `long` values as strings to prevent precision loss in browser clients.
