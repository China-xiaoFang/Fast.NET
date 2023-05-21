using Fast.ServiceCollection.Extension;

namespace Fast.ServiceCollection.Util;

/// <summary>
/// AES加密解密工具类
/// </summary>
public static class AESUtil
{
    /// <summary>
    /// AES加密
    /// </summary>
    /// <param name="dataStr"></param>
    /// <param name="key"></param>
    /// <param name="vector"></param>
    /// <param name="cipherMode"></param>
    /// <param name="paddingMode"></param>
    /// <returns></returns>
    public static string AESEncrypt(string dataStr, string key, string vector, CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        if (dataStr.IsEmpty())
        {
            return null;
        }

        if (key.IsEmpty())
        {
            return null;
        }

        if (vector.IsEmpty())
        {
            return null;
        }

        var dataBytes = Encoding.UTF8.GetBytes(dataStr);
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var vectorBytes = Encoding.UTF8.GetBytes(vector);

        using var aesAlg = Aes.Create();
        aesAlg.Mode = cipherMode;
        aesAlg.Padding = paddingMode;
        using var encryptor = aesAlg.CreateEncryptor(keyBytes, vectorBytes);
        using var msEncrypt = new MemoryStream();
        using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write, true);
        csEncrypt.Write(dataBytes, 0, dataBytes.Length);
        csEncrypt.FlushFinalBlock();
        var array = msEncrypt.ToArray();
        return Convert.ToBase64String(array);
    }

    /// <summary>
    /// AES解密
    /// </summary>
    /// <param name="dataStr"></param>
    /// <param name="key"></param>
    /// <param name="vector"></param>
    /// <param name="cipherMode"></param>
    /// <param name="paddingMode"></param>
    /// <returns></returns>
    public static string AESDecrypt(string dataStr, string key, string vector, CipherMode cipherMode = CipherMode.CBC,
        PaddingMode paddingMode = PaddingMode.PKCS7)
    {
        if (dataStr.IsEmpty())
        {
            return null;
        }

        if (key.IsEmpty())
        {
            return null;
        }

        if (vector.IsEmpty())
        {
            return null;
        }

        var dataBytes = Convert.FromBase64String(dataStr);
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var vectorBytes = Encoding.UTF8.GetBytes(vector);

        using var aesAlg = Aes.Create();
        aesAlg.Mode = cipherMode;
        aesAlg.Padding = paddingMode;

        using var decryptor = aesAlg.CreateDecryptor(keyBytes, vectorBytes);
        using var msDecryptor = new MemoryStream(dataBytes);
        using var csDecryptor = new CryptoStream(msDecryptor, decryptor, CryptoStreamMode.Read);
        using var srDecryptor = new StreamReader(csDecryptor);
        return srDecryptor.ReadToEnd();
    }
}