[简体中文](COMMENTING_GUIDE.zh.md) | [**English**](COMMENTING_GUIDE.md) · [Back to README](../README.md)

# Comments and public API documentation

Comments explain caller constraints, design rationale, and non-obvious behavior. They should not translate self-explanatory code line by line.

## Public APIs

Every `public` or `protected` API and every interface member must use C# XML documentation:

- `<summary>` explains purpose instead of repeating the member name.
- Every parameter and type parameter has its own `<param>` or `<typeparam>`, including every overload.
- Every non-`void` method has `<returns>` describing the value and important null or status semantics.
- Use `<exception>` for exceptions callers are expected to handle and `<remarks>` for cost, concurrency, security, or platform constraints.
- Use `<inheritdoc />` when an implementation already has a complete interface or base-class contract.
- Preserve identifier casing and suffixes when comments refer to code names, for example `UserId`, `用户Id`, and `租户Id`, rather than `UserID` or `用户 ID`.
- Do not end XML documentation tags (including `<returns>`), type comments, or ordinary `//` comments with `。！？；：，、…,.!?;:`. Preserve parentheses, brackets, separators, and single-line license headers.

Bad:

```csharp
/// <summary>Sets cache.</summary>
bool Set(string key, object value);
```

Preferred:

```csharp
/// <summary>
/// Stores a value under the specified cache key
/// </summary>
/// <param name="key">The cache key</param>
/// <param name="value">The value to store</param>
/// <returns><see langword="true"/> when the value is stored successfully; otherwise <see langword="false"/></returns>
bool Set(string key, object value);
```

## Implementation comments

Comments are valuable for:

- Concurrency order, double checks, lock scope, and cache-stampede protection.
- Security boundaries such as token signature validation, trusted proxies, and output escaping responsibilities.
- Cross-platform, encoding, reflection, runtime-loading, and third-party constraints.
- Operations with significant cost or external blocking behavior.
- Code that looks simplifiable but is constrained by a protocol or framework contract.

Do not retain commented-out code, ownerless `TODO` items, or conversational placeholders. Version control already preserves historical implementations.
