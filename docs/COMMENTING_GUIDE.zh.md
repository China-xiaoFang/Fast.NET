[**简体中文**](COMMENTING_GUIDE.zh.md) | [English](COMMENTING_GUIDE.md) · [返回 README](../README.zh.md)

# 注释与公共 API 文档规范

注释用于说明调用约束、设计原因和非直观行为，不用于逐行翻译代码。能通过清晰命名直接表达的逻辑不需要额外注释。

## 公共 API

所有 `public`、`protected` API 以及接口成员必须使用 C# XML 文档：

- `<summary>` 说明 API 的用途，而不是重复成员名称。
- 每个参数和泛型参数分别提供 `<param>`、`<typeparam>`，包括所有重载。
- 非 `void` 方法提供 `<returns>`，说明返回值含义以及重要的空值或状态语义。
- 调用方需要处理的明确异常使用 `<exception>`；高开销、并发、安全和平台限制使用 `<remarks>`。
- 实现成员已有完整接口或基类契约时可以使用 `<inheritdoc />`；同时核对生成 XML 与文档站是否正确解析，避免 IDE 提示完整但发布内容缺失。
- 标识符和参数引用使用 `<see cref="…"/>`、`<paramref name="…"/>`，减少重命名后的说明漂移。
- 注释提及与代码对应的标识符时，保持代码中的大小写和后缀形式，例如 `UserId`、`用户Id`、`租户Id`，不写成 `UserID`、`用户 ID`。
- 标点由内容决定：标签型短说明不使用结束标点，完整陈述、原因和限制正常断句。不要按 XML 标签或注释类型批量删除标点；历史许可证头另行维护。

错误示例：

```csharp
/// <summary>设置缓存</summary>
bool Set(string key, object value);
```

推荐写法：

```csharp
/// <summary>
/// 写入指定键的缓存值。
/// </summary>
/// <param name="key">缓存键</param>
/// <param name="value">要写入缓存的值</param>
/// <returns>缓存写入成功时返回 <see langword="true"/>；否则返回 <see langword="false"/>。</returns>
bool Set(string key, object value);
```

## 实现注释

优先注释以下内容：

- 并发顺序、二次检查、锁粒度和缓存击穿保护。
- 安全边界，例如令牌签名验证、代理信任和输出转义责任。
- 跨平台、编码、反射、运行时加载和第三方库限制。
- 性能代价明显或可能阻塞外部服务的操作。
- 看似可以简化、但受协议或框架约束不能简化的代码。

不要保留被注释掉的代码、无负责人和期限的 `TODO`，也不要使用“后续再看”“报错了不管”等无法指导维护的描述。历史实现可由版本控制查询。
