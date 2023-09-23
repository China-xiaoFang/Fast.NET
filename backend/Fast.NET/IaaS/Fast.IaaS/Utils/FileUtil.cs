using System.Security.Cryptography;

namespace Fast.IaaS.Utils;

/// <summary>
/// 文件工具类
/// </summary>
public static class FileUtil
{
    /// <summary>
    /// 获取文件的 SHA1 哈希值。
    /// </summary>
    /// <param name="filePath">文件的完整路径。</param>
    /// <returns>由小写字母组成的 SHA1 哈希值字符串。</returns>
    public static string GetFileSHA1(string filePath)
    {
        var strResult = "";
        var strHashData = "";
        FileStream oFileStream = null;

        // 创建 SHA1 实例
        var osha1 = SHA1.Create();

        // 打开文件流，读取文件内容
        oFileStream = new FileStream(filePath.Replace("\"", ""), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        // 计算文件的 SHA1 哈希值
        var arrBytHashValue = osha1.ComputeHash(oFileStream);

        // 关闭文件流
        oFileStream.Close();

        // 将哈希值转换为十六进制字符串，并去掉连字符（“-”）
        strHashData = BitConverter.ToString(arrBytHashValue);
        strHashData = strHashData.Replace("-", "");

        // 转换为小写字母形式，作为最终的哈希值结果
        strResult = strHashData.ToLower();
        return strResult;
    }
}