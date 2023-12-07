// Apache开源许可证
//
// 版权所有 © 2018-2023 1.8K仔
//
// 特此免费授予获得本软件及其相关文档文件（以下简称“软件”）副本的任何人以处理本软件的权利，
// 包括但不限于使用、复制、修改、合并、发布、分发、再许可、销售软件的副本，
// 以及允许拥有软件副本的个人进行上述行为，但须遵守以下条件：
//
// 在所有副本或重要部分的软件中必须包括上述版权声明和本许可声明。
//
// 软件按“原样”提供，不提供任何形式的明示或暗示的保证，包括但不限于对适销性、适用性和非侵权的保证。
// 在任何情况下，作者或版权持有人均不对任何索赔、损害或其他责任负责，
// 无论是因合同、侵权或其他方式引起的，与软件或其使用或其他交易有关。

using System.Security.Cryptography;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="CryptoUtil"/> 加密解密工具类
/// </summary>
[SuppressSniffer]
public static class CryptoUtil
{
    #region AES

    /// <summary>
    /// 使用AES算法对给定字符串进行加密。
    /// </summary>
    /// <param name="dataStr">要加密的字符串。</param>
    /// <param name="key">用于加密的密钥。</param>
    /// <param name="vector">用于加密的向量（IV）。</param>
    /// <param name="cipherMode">加密模式，默认为CBC模式。</param>
    /// <param name="paddingMode">填充模式，默认为PKCS7。</param>
    /// <returns>加密后的Base64编码字符串。</returns>
    public static string AESEncrypt(string dataStr, string key, string vector, CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        if (string.IsNullOrEmpty(dataStr))
        {
            return null;
        }

        if (string.IsNullOrEmpty(key))
        {
            return null;
        }

        if (string.IsNullOrEmpty(vector))
        {
            return null;
        }

        // 将输入的字符串、密钥和向量转换为字节数组
        var dataBytes = Encoding.UTF8.GetBytes(dataStr);
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var vectorBytes = Encoding.UTF8.GetBytes(vector);

        // 创建AES实例并设置加密模式和填充模式
        using var aesAlg = Aes.Create();
        aesAlg.Mode = cipherMode;
        aesAlg.Padding = paddingMode;

        // 创建加密器对象，并使用密钥和向量初始化
        using var encryption = aesAlg.CreateEncryptor(keyBytes, vectorBytes);

        // 创建内存流和加密流，将加密数据写入加密流
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryption, CryptoStreamMode.Write, true);
        csEncrypt.Write(dataBytes, 0, dataBytes.Length);
        csEncrypt.FlushFinalBlock();

        // 获取加密后的字节数组并转换为Base64编码字符串
        var array = msEncrypt.ToArray();
        return Convert.ToBase64String(array);
    }

    /// <summary>
    /// 使用AES算法对给定的Base64编码字符串进行解密。
    /// </summary>
    /// <param name="dataStr">要解密的Base64编码字符串。</param>
    /// <param name="key">用于解密的密钥。</param>
    /// <param name="vector">用于解密的向量（IV）。</param>
    /// <param name="cipherMode">解密模式，默认为CBC模式。</param>
    /// <param name="paddingMode">填充模式，默认为PKCS7。</param>
    /// <returns>解密后的原始字符串。</returns>
    public static string AESDecrypt(string dataStr, string key, string vector, CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        if (string.IsNullOrEmpty(dataStr))
        {
            return null;
        }

        if (string.IsNullOrEmpty(key))
        {
            return null;
        }

        if (string.IsNullOrEmpty(vector))
        {
            return null;
        }

        // 将输入的Base64字符串、密钥和向量转换为字节数组
        var dataBytes = Convert.FromBase64String(dataStr);
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var vectorBytes = Encoding.UTF8.GetBytes(vector);

        // 创建AES实例并设置解密模式和填充模式
        using var aesAlg = Aes.Create();
        aesAlg.Mode = cipherMode;
        aesAlg.Padding = paddingMode;

        // 创建解密器对象，并使用密钥和向量初始化
        using var decryption = aesAlg.CreateDecryptor(keyBytes, vectorBytes);

        // 创建内存流和解密流，将解密数据写入解密流
        using var msDecryption = new MemoryStream(dataBytes);
        using var csDecryption = new CryptoStream(msDecryption, decryption, CryptoStreamMode.Read);
        using var srDecryption = new StreamReader(csDecryption);
        return srDecryption.ReadToEnd();
    }

    #endregion

    #region MD5

    /// <summary>
    /// 使用 MD5 算法对给定的字符串进行加密。
    /// </summary>
    /// <param name="content">要加密的字符串。</param>
    /// <returns>加密后的字符串。</returns>
    public static string MD5Encrypt(string content)
    {
        // 创建 MD5 实例
        using var mi = MD5.Create();

        // 将输入的字符串转换为字节数组
        var buffer = Encoding.Default.GetBytes(content);

        // 对字节数组进行加密
        var newBuffer = mi.ComputeHash(buffer);

        // 创建 StringBuilder 对象用于保存加密后的字符串
        var sb = new StringBuilder();
        foreach (var by in newBuffer)
        {
            // 将每个字节转换为 16 进制，并添加到 StringBuilder 中
            sb.Append(by.ToString("x2"));
        }

        // 返回加密后的字符串
        return sb.ToString();
    }

    #endregion

    #region SHA1

    /// <summary>
    /// SHA1加密
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <returns><see cref="string"/></returns>
    public static string SHAEncrypt(string str)
    {
        var sha1 = SHA1.Create();
        var inputStrBytes = Encoding.UTF8.GetBytes(str);
        var outputBytes = sha1.ComputeHash(inputStrBytes);
        sha1.Clear();
        return BitConverter.ToString(outputBytes).Replace("-", "");
    }

    #endregion
}