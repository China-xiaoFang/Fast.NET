// Copyright © 2018-Now 小方
// SPDX-License-Identifier: Apache-2.0
// 
// 本文件依据 Apache License 2.0 授权，完整条款见仓库根目录 LICENSE。
// 本软件按“原样”提供；保证排除和责任限制以许可证及适用法律为准。
// 版权来源、合法使用与二次开发责任说明见仓库根目录 README.zh.md。

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fast.Serialization;

/// <summary>
/// 数据脱敏类型枚举
/// </summary>
public enum DataMaskingTypeEnum
{
    /// <summary>
    /// 姓名
    /// </summary>
    Name,

    /// <summary>
    /// 姓名（保留首尾）
    /// </summary>
    NameKeepLast,

    /// <summary>
    /// 账号
    /// </summary>
    Account,

    /// <summary>
    /// 手机号
    /// </summary>
    Mobile,

    /// <summary>
    /// 身份证
    /// </summary>
    IdCard,

    /// <summary>
    /// 邮箱
    /// </summary>
    Email,

    /// <summary>
    /// 银行卡
    /// </summary>
    BankCard,

    /// <summary>
    /// 地址
    /// </summary>
    Address,

    /// <summary>
    /// 车牌号
    /// </summary>
    CarNumber,

    /// <summary>
    /// IP 地址
    /// </summary>
    Ip
}

/// <summary>
/// JSON 返回数据脱敏处理
/// </summary>
public class DataMaskingConverter : JsonConverter<string>
{
    /// <summary>
    /// 数据脱敏类型
    /// </summary>
    public DataMaskingTypeEnum MaskingType { get; set; }

    /// <summary>
    /// JSON 返回数据脱敏处理
    /// </summary>
    /// <param name="maskingType">数据脱敏方式</param>
    public DataMaskingConverter(DataMaskingTypeEnum maskingType)
    {
        MaskingType = maskingType;
    }

    /// <inheritdoc />
    public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString() ?? string.Empty;
    }

    /// <inheritdoc />
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
