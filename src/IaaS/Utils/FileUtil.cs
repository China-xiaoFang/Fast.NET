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

// ReSharper disable once CheckNamespace
namespace Fast.IaaS;

/// <summary>
/// <see cref="FileUtil"/> 文件工具类
/// </summary>
public static class FileUtil
{
    /// <summary>
    /// 获取文件的 SHA1 哈希值。
    /// </summary>
    /// <param name="filePath"><see cref="string"/> 文件的完整路径。</param>
    /// <returns><see cref="string"/> 由小写字母组成的 SHA1 哈希值字符串。</returns>
    public static string GetFileSHA1(string filePath)
    {
        // 创建 SHA1 实例
        var osha1 = SHA1.Create();

        // 打开文件流，读取文件内容
        var oFileStream = new FileStream(filePath.Replace("\"", ""), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        // 计算文件的 SHA1 哈希值
        var arrBytHashValue = osha1.ComputeHash(oFileStream);

        // 关闭文件流
        oFileStream.Close();

        // 将哈希值转换为十六进制字符串，并去掉连字符（“-”）
        var strHashData = BitConverter.ToString(arrBytHashValue);
        strHashData = strHashData.Replace("-", "");

        // 转换为小写字母形式，作为最终的哈希值结果
        var strResult = strHashData.ToLower();
        return strResult;
    }

    /// <summary>
    /// 复制文件
    /// </summary>
    /// <param name="fromPath"><see cref="string"/>来源文件路径</param>
    /// <param name="toPath"><see cref="string"/>复制的文件路径</param>
    public static void CopyFile(string fromPath, string toPath)
    {
        if (!File.Exists(fromPath))
        {
            throw new FileNotFoundException("源文件不存在！");
        }

        // 创建目标文件夹（如果不存在）
        var destinationDirectory = Path.GetDirectoryName(toPath);
        Directory.CreateDirectory(destinationDirectory);

        // 复制文件
        File.Copy(fromPath, toPath, true);
    }

    /// <summary>
    /// 尝试创建文件夹
    /// </summary>
    /// <param name="path"><see cref="string"/>路径</param>
    public static void TryCreateDirectory(string path)
    {
        // 创建目标文件夹（如果不存在）
        var destinationDirectory = Path.GetDirectoryName(path);
        Directory.CreateDirectory(destinationDirectory);
    }
}