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

using System;
using System.Linq;

namespace Fast.IaaS;

/// <summary>
/// <see cref="MaskingUtil"/> 数据脱敏工具类
/// </summary>
public static class MaskingUtil
{
    /// <summary>
    /// 姓名脱敏（只保留首字）
    /// </summary>
    public static string NameMasking(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length == 1)
            return name;
        if (name.Length == 2)
            return $"{name[0]}*";

        return name[0] + new string('*', name.Length - 1);
    }

    /// <summary>
    /// 姓名脱敏（保留首尾）
    /// </summary>
    public static string NameKeepLastMasking(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length == 1)
            return name;
        if (name.Length == 2)
            return $"{name[0]}*";

        return name[0] + new string('*', name.Length - 2) + name[^1];
    }

    /// <summary>
    /// 手机号脱敏（152****5552）
    /// </summary>
    public static string MobileMasking(string mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile) || mobile.Length < 7)
            return mobile;

        return mobile.Substring(0, 3) + "****" + mobile[^4..];
    }

    /// <summary>
    /// 身份证脱敏处理（前4后4）
    /// </summary>
    public static string IdCardMasking(string idCard)
    {
        if (string.IsNullOrWhiteSpace(idCard) || idCard.Length < 8)
            return idCard;

        return idCard[..4] + new string('*', idCard.Length - 8) + idCard[^4..];
    }

    /// <summary>
    /// 邮箱脱敏（最多保留3位字符 + 域名）
    /// </summary>
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
    /// 银行卡脱敏（前6后4）
    /// </summary>
    public static string BankCardMasking(string cardNo)
    {
        if (string.IsNullOrWhiteSpace(cardNo) || cardNo.Length < 10)
            return cardNo;

        return cardNo[..6] + new string('*', cardNo.Length - 10) + cardNo[^4..];
    }

    /// <summary>
    /// 地址脱敏（优先识别省/市/区/街道等行政区划，保留上级区域）
    /// </summary>
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

        // fallback：无法识别，则保留前 6
        if (address.Length <= 6)
            return address;

        return address[..6] + "****";
    }

    /// <summary>
    /// 车牌号脱敏（保留前两位，如有分隔符则保留“省份+地区字母+分隔符”）
    /// </summary>
    public static string CarNumberMasking(string carNumber)
    {
        if (string.IsNullOrWhiteSpace(carNumber) || carNumber.Length <= 2)
            return carNumber;

        // 常见分隔符集合）
        char[] separators = ['·', '•', '.', '-', ' '];

        // 若第三位是分隔符，保留前两位 + 分隔符，再脱敏后面所有字符
        if (carNumber.Length >= 3 && separators.Contains(carNumber[2]))
        {
            var head = carNumber[..3];
            var tailLen = carNumber.Length - 3;
            return head + new string('*', tailLen);
        }

        // 否则保留前两位，其余全部脱敏（保持原格式长度）
        return carNumber[..2] + new string('*', carNumber.Length - 2);
    }

    /// <summary>
    /// IP 地址脱敏（保留前两段）
    /// </summary>
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