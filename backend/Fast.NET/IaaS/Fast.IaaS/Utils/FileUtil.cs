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