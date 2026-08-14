// ------------------------------------------------------------------------
// Apache开源许可证
// 
// 版权所有 © 2018-Now 小方
// 
// 许可授权：
// 本协议授予任何获得本软件及其相关文档（以下简称“软件”）副本的个人或组织。
// 在遵守本协议条款的前提下，享有使用、复制、修改、合并、发布、分发、再许可、销售软件副本的权利：
// 1.所有软件副本或主要部分必须保留本版权声明及本许可协议。
// 2.软件的使用、复制、修改或分发不得违反适用法律或侵犯他人合法权益。
// 3.修改或衍生作品须明确标注原作者及原软件出处。
// 
// 特别声明：
// - 本软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// - 在任何情况下，作者或版权持有人均不对因使用或无法使用本软件导致的任何直接或间接损失的责任。
// - 包括但不限于数据丢失、业务中断等情况。
// 
// 免责条款：
// 禁止利用本软件从事危害国家安全、扰乱社会秩序或侵犯他人合法权益等违法活动。
// 对于基于本软件二次开发所引发的任何法律纠纷及责任，作者不承担任何责任。
// ------------------------------------------------------------------------

namespace Fast.Serialization;

/// <summary>
/// 数据脱敏工具类。
/// </summary>
internal static class MaskingUtil
{
    /// <summary>
    /// 姓名脱敏（只保留首字）。
    /// </summary>
    /// <param name="name">名称。</param>
    /// <returns>姓名脱敏（只保留首字）。</returns>
    public static string NameMasking(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length == 1)
            return name;
        if (name.Length == 2)
            return $"{name[0]}*";

        return name[0] + new string('*', name.Length - 1);
    }

    /// <summary>
    /// 姓名脱敏（保留首尾）。
    /// </summary>
    /// <param name="name">名称。</param>
    /// <returns>姓名脱敏（保留首尾）。</returns>
    public static string NameKeepLastMasking(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length == 1)
            return name;
        if (name.Length == 2)
            return $"{name[0]}*";

        return name[0] + new string('*', name.Length - 2) + name[^1];
    }

    /// <summary>
    /// 账号脱敏（adm**123）。
    /// </summary>
    /// <param name="account">要脱敏的账户标识。</param>
    /// <returns>账号脱敏（adm**123）。</returns>
    public static string AccountMasking(string account)
    {
        if (string.IsNullOrWhiteSpace(account) || account.Length < 6)
            return account;

        var maskLength = account.Length - 6;
        return account[..3] + new string('*', maskLength) + account[^3..];
    }

    /// <summary>
    /// 手机号脱敏（152****5552）。
    /// </summary>
    /// <param name="mobile">要脱敏的手机号码。</param>
    /// <returns>手机号脱敏（152****5552）。</returns>
    public static string MobileMasking(string mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile) || mobile.Length < 7)
            return mobile;

        // 长号码保留前 3 位和第 8 至 11 位，其余字符脱敏。
        var tailStart = 7;
        var tailLength = Math.Min(4, mobile.Length - tailStart);
        var tail = mobile.Substring(tailStart, tailLength);

        // 从第 12 个字符起全部脱敏。
        var remaining = mobile.Length > 11 ? new string('*', mobile.Length - 11) : string.Empty;

        return mobile[..3] + "****" + tail + remaining;
    }

    /// <summary>
    /// 身份证脱敏处理（前 4 后 4）。
    /// </summary>
    /// <param name="idCard">要脱敏的身份证号码。</param>
    /// <returns>身份证脱敏处理（前 4 后 4）。</returns>
    public static string IdCardMasking(string idCard)
    {
        if (string.IsNullOrWhiteSpace(idCard) || idCard.Length < 8)
            return idCard;

        return idCard[..4] + new string('*', idCard.Length - 8) + idCard[^4..];
    }

    /// <summary>
    /// 邮箱脱敏（最多保留 3 位字符 + 域名）。
    /// </summary>
    /// <param name="email">要脱敏的电子邮箱地址。</param>
    /// <returns>邮箱脱敏（最多保留 3 位字符 + 域名）。</returns>
    public static string EmailMasking(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return email;

        var index = email.IndexOf('@');
        if (index <= 0)
            return email;

        var user = email[..index];
        var domain = email[index..];

        return user.Length switch
        {
            1 => email,
            2 => user[0] + "*" + domain,
            3 => user[..2] + "*" + domain,
            _ => user[..3] + new string('*', user.Length - 3) + domain
        };
    }

    /// <summary>
    /// 银行卡脱敏（前 6 后 4）。
    /// </summary>
    /// <param name="cardNo">要脱敏的银行卡号。</param>
    /// <returns>银行卡脱敏（前 6 后 4）。</returns>
    public static string BankCardMasking(string cardNo)
    {
        if (string.IsNullOrWhiteSpace(cardNo) || cardNo.Length < 10)
            return cardNo;

        return cardNo[..6] + new string('*', cardNo.Length - 10) + cardNo[^4..];
    }

    /// <summary>
    /// 地址脱敏（优先识别省/市/区/街道等行政区划，保留上级区域）。
    /// </summary>
    /// <param name="address">目标服务地址。</param>
    /// <returns>地址脱敏（优先识别省/市/区/街道等行政区划，保留上级区域）。</returns>
    public static string AddressMasking(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return address;

        string[] keys = ["省", "市", "区", "县", "乡", "镇", "街道", "社区"];

        foreach (var key in keys)
        {
            var index = address.IndexOf(key, StringComparison.Ordinal);
            if (index > 0 && index + 1 < address.Length)
                return address[..(index + 1)] + "****";
        }

        // 无法识别地址格式时保留前 6 个字符。
        if (address.Length <= 6)
            return address;

        return address[..6] + "****";
    }

    /// <summary>
    /// 车牌号脱敏（保留前两位，如有分隔符则保留“省份+地区字母+分隔符”）。
    /// </summary>
    /// <param name="carNumber">要验证的车牌号码。</param>
    /// <returns>车牌号脱敏（保留前两位，如有分隔符则保留“省份+地区字母+分隔符”）。</returns>
    public static string CarNumberMasking(string carNumber)
    {
        if (string.IsNullOrWhiteSpace(carNumber) || carNumber.Length <= 2)
            return carNumber;

        // 支持车牌文本中常见的地区分隔符。
        char[] separators = ['·', '•', '.', '-', ' '];

        // 第三个字符是分隔符时保留前两位及分隔符，其余字符全部脱敏。
        if (carNumber.Length >= 3 && separators.Contains(carNumber[2]))
        {
            var head = carNumber[..3];
            var tailLen = carNumber.Length - 3;
            return head + new string('*', tailLen);
        }

        // 普通格式仅保留前两位，其余字符按原长度脱敏。
        return carNumber[..2] + new string('*', carNumber.Length - 2);
    }

    /// <summary>
    /// IP 地址脱敏（保留前两段）。
    /// </summary>
    /// <param name="ip">要验证的 IP 地址。</param>
    /// <returns>IP 地址脱敏（保留前两段）。</returns>
    public static string IpMasking(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
            return ip;

        var parts = ip.Split('.');
        if (parts.Length != 4)
            return ip;

        return $"{parts[0]}.{parts[1]}.*.*";
    }
}
