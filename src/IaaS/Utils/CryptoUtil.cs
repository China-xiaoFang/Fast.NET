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
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Fast.IaaS;

/// <summary>
/// <see cref="CryptoUtil"/> 加密解密工具类
/// </summary>
public static class CryptoUtil
{
    #region AES

    /// <summary>
    /// 使用AES算法对给定字符串进行加密。
    /// </summary>
    /// <param name="dataStr">要加密的字符串。</param>
    /// <param name="key">用于加密的密钥。必须32位</param>
    /// <param name="vector">用于加密的向量（IV）。必须16位</param>
    /// <param name="cipherMode">加密模式，默认为CBC模式。</param>
    /// <param name="paddingMode">填充模式，默认为PKCS7。</param>
    /// <returns>加密后的Base64编码字符串。</returns>
    /// <remarks>同一密钥重复使用固定 IV 会泄露明文模式；调用方应为不同数据提供不可预测且不重复的 IV。</remarks>
    public static string AESEncrypt(string dataStr, string key, string vector, CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        if (string.IsNullOrWhiteSpace(dataStr))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(vector))
        {
            return null;
        }

        // 处理Key不足32位的问题
        if (key.Length < 32)
        {
            // 不足
            key = key.PadRight(32, 'f');
        }

        // 处理Key超过32位的问题
        if (key.Length > 32)
        {
            // 超过
            key = key[..32];
        }

        // 处理IV不足32位的问题
        if (vector.Length < 16)
        {
            // 不足
            vector = vector.PadRight(16, 'f');
        }

        // 处理IV超过32位的问题
        if (vector.Length > 16)
        {
            // 超过
            vector = vector[..16];
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
    /// <param name="key">用于解密的密钥。必须32位</param>
    /// <param name="vector">用于解密的向量（IV）。必须16位</param>
    /// <param name="cipherMode">解密模式，默认为CBC模式。</param>
    /// <param name="paddingMode">填充模式，默认为PKCS7。</param>
    /// <returns>解密后的原始字符串。</returns>
    public static string AESDecrypt(string dataStr, string key, string vector, CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        if (string.IsNullOrWhiteSpace(dataStr))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(vector))
        {
            return null;
        }

        // 处理Key不足32位的问题
        if (key.Length < 32)
        {
            // 不足
            key = key.PadRight(32, 'f');
        }

        // 处理Key超过32位的问题
        if (key.Length > 32)
        {
            // 超过
            key = key[..32];
        }

        // 处理IV不足32位的问题
        if (vector.Length < 16)
        {
            // 不足
            vector = vector.PadRight(16, 'f');
        }

        // 处理IV超过32位的问题
        if (vector.Length > 16)
        {
            // 超过
            vector = vector[..16];
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

    /// <summary>
    /// 使用 AES-GCM 加密并认证字符串。
    /// </summary>
    /// <param name="dataStr">待加密字符串。</param>
    /// <param name="key">密钥材料；内部使用 SHA-256 归一化为 256 位密钥。</param>
    /// <returns>包含格式版本、随机 nonce、认证标签和密文的 Base64 字符串。</returns>
    /// <remarks>新数据应优先使用此方法；旧的 CBC 接口仅用于兼容已有密文格式。</remarks>
    public static string AESEncryptAuthenticated(string dataStr, string key)
    {
        if (dataStr == null)
            throw new ArgumentNullException(nameof(dataStr));
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("密钥不能为空。", nameof(key));

        const byte formatVersion = 1;
        const int nonceLength = 12;
        const int tagLength = 16;

        var plaintext = Encoding.UTF8.GetBytes(dataStr);
        var keyBytes = CryptographyCompat.ComputeSHA256(Encoding.UTF8.GetBytes(key));
        var nonce = CryptographyCompat.GetRandomBytes(nonceLength);
        var tag = new byte[tagLength];
        var ciphertext = new byte[plaintext.Length];

        try
        {
#if NET8_0_OR_GREATER
            using var aesGcm = new AesGcm(keyBytes, tagLength);
#else
            using var aesGcm = new AesGcm(keyBytes);
#endif
            aesGcm.Encrypt(nonce, plaintext, ciphertext, tag);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(keyBytes);
            CryptographicOperations.ZeroMemory(plaintext);
        }

        // 格式：[1 字节版本][12 字节随机 nonce][16 字节认证标签][密文]。
        var payload = new byte[1 + nonceLength + tagLength + ciphertext.Length];
        payload[0] = formatVersion;
        Buffer.BlockCopy(nonce, 0, payload, 1, nonceLength);
        Buffer.BlockCopy(tag, 0, payload, 1 + nonceLength, tagLength);
        Buffer.BlockCopy(ciphertext, 0, payload, 1 + nonceLength + tagLength, ciphertext.Length);
        return Convert.ToBase64String(payload);
    }

    /// <summary>
    /// 解密并验证 <see cref="AESEncryptAuthenticated"/> 生成的字符串。
    /// </summary>
    /// <param name="dataStr">带认证信息的 Base64 密文。</param>
    /// <param name="key">加密时使用的密钥材料。</param>
    /// <returns>解密后的原始字符串。</returns>
    /// <exception cref="CryptographicException">密钥错误、密文被篡改或格式不受支持。</exception>
    public static string AESDecryptAuthenticated(string dataStr, string key)
    {
        if (dataStr == null)
            throw new ArgumentNullException(nameof(dataStr));
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("密钥不能为空。", nameof(key));

        const byte supportedVersion = 1;
        const int nonceLength = 12;
        const int tagLength = 16;

        var payload = Convert.FromBase64String(dataStr);
        if (payload.Length < 1 + nonceLength + tagLength || payload[0] != supportedVersion)
            throw new CryptographicException("AES-GCM 密文格式无效或版本不受支持。");

        var nonce = payload.AsSpan(1, nonceLength)
            .ToArray();
        var tag = payload.AsSpan(1 + nonceLength, tagLength)
            .ToArray();
        var ciphertext = payload.AsSpan(1 + nonceLength + tagLength)
            .ToArray();
        var plaintext = new byte[ciphertext.Length];
        var keyBytes = CryptographyCompat.ComputeSHA256(Encoding.UTF8.GetBytes(key));

        try
        {
#if NET8_0_OR_GREATER
            using var aesGcm = new AesGcm(keyBytes, tagLength);
#else
            using var aesGcm = new AesGcm(keyBytes);
#endif
            // 认证失败时 AesGcm.Decrypt 会抛出 CryptographicException，不返回未经验证的明文。
            aesGcm.Decrypt(nonce, ciphertext, tag, plaintext);
            return Encoding.UTF8.GetString(plaintext);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(keyBytes);
            CryptographicOperations.ZeroMemory(plaintext);
        }
    }

    #endregion

    #region MD5

    /// <summary>
    /// 使用 MD5 算法计算字符串哈希。
    /// </summary>
    /// <param name="content">要加密的字符串。</param>
    /// <returns>哈希字符串。</returns>
    /// <remarks>仅用于兼容旧协议或校验值，不得用于密码存储、签名或安全用途；新代码请使用 <see cref="SHA256Encrypt"/>。</remarks>
    public static string MD5Encrypt(string content)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));

        // 创建 MD5 实例
        using var mi = MD5.Create();

        // 将输入的字符串转换为字节数组
        var buffer = Encoding.UTF8.GetBytes(content);

        // 对字节数组进行加密
        var newBuffer = mi.ComputeHash(buffer);

        // 创建 StringBuilder 对象用于保存加密后的字符串
        return CryptographyCompat.ToHexString(newBuffer)
            .ToLowerInvariant();
    }

    #endregion

    #region SHA1

    /// <summary>
    /// 计算 SHA-1 哈希
    /// </summary>
    /// <param name="str"><see cref="string"/></param>
    /// <returns><see cref="string"/></returns>
    /// <remarks>仅用于兼容旧协议或校验值；新代码请使用 <see cref="SHA256Encrypt"/>。</remarks>
    public static string SHA1Encrypt(string str)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str));

        using var sha1 = SHA1.Create();
        var inputStrBytes = Encoding.UTF8.GetBytes(str);
        var outputBytes = sha1.ComputeHash(inputStrBytes);
        return CryptographyCompat.ToHexString(outputBytes);
    }

    #endregion

    #region SHA256

    /// <summary>
    /// 计算字符串的 SHA-256 哈希值。
    /// </summary>
    /// <param name="content">待计算内容。</param>
    /// <returns>大写十六进制哈希值。</returns>
    public static string SHA256Encrypt(string content)
    {
        if (content == null)
            throw new ArgumentNullException(nameof(content));

        var inputBytes = Encoding.UTF8.GetBytes(content);
        return CryptographyCompat.ToHexString(CryptographyCompat.ComputeSHA256(inputBytes));
    }

    #endregion
}