# Fast.IaaS

Infrastructure utilities library — a comprehensive collection of extension methods and utility classes for common operations.

> **Note:** This module targets `netstandard2.1` and can be used in any .NET application, not just ASP.NET Core.

## Installation

```bash
dotnet add package Fast.IaaS
```

## Utility Classes

### CryptoUtil — Cryptography

```csharp
// Hashing
string md5 = CryptoUtil.Md5("text");
string sha1 = CryptoUtil.Sha1("text");
string sha256 = CryptoUtil.Sha256("text");

// AES encryption
string encrypted = CryptoUtil.AesEncrypt("plaintext", "key");
string decrypted = CryptoUtil.AesDecrypt(encrypted, "key");
```

### MaskingUtil — Data Masking

```csharp
string phone = MaskingUtil.MaskPhone("13812345678");     // "138****5678"
string email = MaskingUtil.MaskEmail("user@example.com"); // "u***@example.com"
string idCard = MaskingUtil.MaskIdCard("110101199001011234"); // "110***********1234"
```

### VerificationUtil — Validation

```csharp
bool isEmail = VerificationUtil.IsEmail("user@example.com");
bool isPhone = VerificationUtil.IsPhone("13812345678");
bool isIdCard = VerificationUtil.IsIdCard("110101199001011234");
bool isUrl = VerificationUtil.IsUrl("https://example.com");
```

### TreeBuildUtil — Tree Building

```csharp
var tree = TreeBuildUtil.BuildTree(flatList);
```

### DateTimeUtil — Date Operations

```csharp
var firstDay = DateTimeUtil.GetCurMonthFirstDay();
var lastDay = DateTimeUtil.GetCurMonthLastDay();
var parsed = DateTimeUtil.ParseToDateTime("2024-01-15");
```

### CoordinateUtil — Geographic Calculations

```csharp
double distance = CoordinateUtil.CalculateDistance(lat1, lon1, lat2, lon2);
```

### RetryUtil — Retry Mechanism

```csharp
await RetryUtil.InvokeAsync(async () =>
{
    await SomeUnreliableOperation();
}, retryCount: 3, retryInterval: 1000);
```

### NumberUtil — Number Operations

```csharp
bool isInt = NumberUtil.IsInteger("123");
bool isDec = NumberUtil.IsDecimal("123.45");
int random = NumberUtil.GenerateRandomNumber(1000, 9999);
```

### GuidUtil — GUID Generation

```csharp
Guid sequential = GuidUtil.CreateSequentialGuid();
string newId = GuidUtil.NewId();
```

## Extension Methods

### String Extensions

```csharp
"helloWorld".SplitCamelCase();     // "hello World"
"hello_world".ToCamelCase();       // "helloWorld"
"hello".FirstCharToUpper();        // "Hello"
"Hello".FirstCharToLower();        // "hello"
"Hello World".GetSubStringWithEllipsis(5); // "Hello..."
```

### Type Parsing Extensions

```csharp
"123".ParseToInt();                // 123
"123".ParseToLong();               // 123L
"123.45".ParseToDecimal();         // 123.45m
"2024-01-15".ParseToDateTime();    // DateTime
"true".ParseToBool();              // true
"guid-string".ParseToGuid();       // Guid
```

### Collection Extensions

```csharp
list.CastSuper<BaseType>();        // Cast with inheritance
dictionary.GetOrDefault("key");    // Safe get
```

### DateTime Extensions

```csharp
dateTime.ToUnixTimestamp();
dateTime.StartOfDay();
dateTime.EndOfDay();
```

### Base64 Extensions

```csharp
"text".ToBase64();                 // Encode
"dGV4dA==".FromBase64();           // Decode
```
