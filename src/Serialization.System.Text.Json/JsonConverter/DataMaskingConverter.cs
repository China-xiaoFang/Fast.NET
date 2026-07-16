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

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fast.Serialization;

/// <summary>
/// <see cref="DataMaskingTypeEnum"/> 数据脱敏类型枚举
/// </summary>
public enum DataMaskingTypeEnum
{
    /// <summary>姓名</summary>
    Name,

    /// <summary>姓名（保留首尾）</summary>
    NameKeepLast,

    /// <summary>账号</summary>
    Account,

    /// <summary>手机号</summary>
    Mobile,

    /// <summary>身份证</summary>
    IdCard,

    /// <summary>邮箱</summary>
    Email,

    /// <summary>银行卡</summary>
    BankCard,

    /// <summary>地址</summary>
    Address,

    /// <summary>车牌号</summary>
    CarNumber,

    /// <summary>IP 地址</summary>
    Ip
}

/// <summary>
/// <see cref="DataMaskingConverter"/> Json返回数据脱敏处理
/// </summary>
public class DataMaskingConverter : JsonConverter<string>
{
    /// <summary>
    /// 数据脱敏类型
    /// </summary>
    public DataMaskingTypeEnum MaskingType { get; set; }

    /// <summary>
    /// <see cref="DataMaskingConverter"/> Json返回数据脱敏处理
    /// </summary>
    /// <param name="maskingType"><see cref="DataMaskingTypeEnum"/> 数据脱敏类型</param>
    public DataMaskingConverter(DataMaskingTypeEnum maskingType)
    {
        MaskingType = maskingType;
    }

    /// <summary>Reads and converts the JSON to type <see cref="string"/>.</summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert">The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString() ?? string.Empty;
    }

    /// <summary>Writes a specified value as JSON.</summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The value to convert to JSON.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            writer.WriteStringValue(value);
        }
        else
        {
            writer.WriteStringValue(MaskingType switch
            {
                DataMaskingTypeEnum.Name => MaskingUtil.NameMasking(value),
                DataMaskingTypeEnum.NameKeepLast => MaskingUtil.NameKeepLastMasking(value),
                DataMaskingTypeEnum.Account => MaskingUtil.AccountMasking(value),
                DataMaskingTypeEnum.Mobile => MaskingUtil.MobileMasking(value),
                DataMaskingTypeEnum.IdCard => MaskingUtil.IdCardMasking(value),
                DataMaskingTypeEnum.Email => MaskingUtil.EmailMasking(value),
                DataMaskingTypeEnum.BankCard => MaskingUtil.BankCardMasking(value),
                DataMaskingTypeEnum.Address => MaskingUtil.AddressMasking(value),
                DataMaskingTypeEnum.CarNumber => MaskingUtil.CarNumberMasking(value),
                DataMaskingTypeEnum.Ip => MaskingUtil.IpMasking(value),
                _ => throw new InvalidOperationException($"不支持的数据脱敏类型：{MaskingType}。")
            });
        }
    }
}