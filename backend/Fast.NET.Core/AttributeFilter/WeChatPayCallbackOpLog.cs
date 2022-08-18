namespace Fast.NET.Core.AttributeFilter;

/// <summary>
/// 微信支付回调操作日志
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class WeChatPayCallbackOpLog : Attribute
{
}