using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using Fast.ServiceCollection.Extension;
using Furion.RemoteRequest.Extensions;
using Encoder = System.Drawing.Imaging.Encoder;

namespace Fast.ServiceCollection.Util;

/// <summary>
/// 图片工具类
/// </summary>
[SupportedOSPlatform("windows")]
public static class ImageUtil
{
    /// <summary>
    /// 获取外网Url图片的二进制数组
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public static async Task<byte[]> GetBytesFromUrl(this string url)
    {
        // 得到图片流
        var (stream, _) = await url.GetAsStreamAsync();
        using var binaryReader = new BinaryReader(stream);
        // 读取二进制文件流
        var resultBytes = binaryReader.ReadBytes(stream.Length.ParseToInt());
        binaryReader.Close();

        return resultBytes;
    }

    /// <summary>
    /// 保存二进制流到文件
    /// </summary>
    /// <param name="content"></param>
    /// <param name="fileName"></param>
    public static async Task WriteBytesToFile(byte[] content, string fileName)
    {
        // 检测文件夹是否存在
        if (!Directory.Exists(fileName[..fileName.LastIndexOf("/", StringComparison.Ordinal)]))
        {
            Directory.CreateDirectory(fileName[..fileName.LastIndexOf("/", StringComparison.Ordinal)]);
        }

        // 检测文件是否存在
        if (!File.Exists(fileName))
        {
            //var stream = File.Create(fileName);
            //stream.Close();
            //await stream.DisposeAsync();
            await File.WriteAllBytesAsync(fileName, content);
        }
        //var fileStream = new FileStream(fileName, FileMode.Create);
        //var binaryWriter = new BinaryWriter(fileStream);
        //try
        //{
        //    binaryWriter.Write(content);
        //}
        //finally
        //{
        //    fileStream.Close();
        //    binaryWriter.Close();
        //}
    }

    /// <summary>
    /// 保存外网Url到文件
    /// </summary>
    /// <param name="url">外网图片Url</param>
    /// <param name="fileName">要保存的文件名称</param>
    /// <returns></returns>
    public static async Task SaveFromUrlToFile(this string url, string fileName)
    {
        var bytes = await url.GetBytesFromUrl();
        await WriteBytesToFile(bytes, fileName);
    }

    /// <summary>
    /// 删除文件夹里面所有的文件
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static void DeleteFile(this string fileName)
    {
        foreach (var entry in Directory.GetFileSystemEntries(fileName))
        {
            if (!File.Exists(entry))
                continue;
            var name = Path.GetFileNameWithoutExtension(entry);
            File.Delete(entry);
        }
    }

    /// <summary>
    /// 无损压缩图片
    /// </summary>
    /// <param name="sourceFile">原图片地址</param>
    /// <param name="compressFile">压缩后保存图片地址</param>
    /// <param name="compressHeight">压缩后宽度</param>
    /// <param name="compressWidth">压缩后高度</param>
    /// <param name="flag">压缩质量（数字越小压缩率越高）</param>
    /// <param name="size">压缩后图片的最大大小</param>
    /// <param name="isFirst">是否为第一次调用</param>
    /// <returns></returns>
    public static bool CompressImage(string sourceFile, string compressFile, int compressHeight = 750, int compressWidth = 1334,
        int flag = 90, int size = 110, bool isFirst = true)
    {
        // 如果是第一次调用，原始图像的大小小于要压缩的大小，则直接复制文件，并且返回True
        var firstFileInfo = new FileInfo(sourceFile);
        if (isFirst && firstFileInfo.Length < size * 1024)
        {
            firstFileInfo.CopyTo(compressFile);
            return true;
        }

        var imageSource = Image.FromFile(sourceFile);
        var imageFormat = imageSource.RawFormat;
        var sourceWidth = 0;
        var sourceHeight = 0;

        // 按比例缩放
        var temSize = new Size(imageSource.Width, imageSource.Height);

        // 处理大小，如果是0，原大小
        if (compressHeight.IsNullOrZero())
        {
            compressHeight = temSize.Height;
        }

        if (compressWidth.IsNullOrZero())
        {
            compressWidth = temSize.Width;
        }

        if (temSize.Width > compressHeight || temSize.Width > compressWidth)
        {
            if ((temSize.Width * compressHeight) > (temSize.Width * compressWidth))
            {
                sourceWidth = compressWidth;
                sourceHeight = (compressWidth * temSize.Height) / temSize.Width;
            }
            else
            {
                sourceWidth = compressHeight;
                sourceHeight = (temSize.Height * compressHeight) / temSize.Height;
            }
        }
        else
        {
            sourceWidth = temSize.Width;
            sourceHeight = temSize.Height;
        }


        var bitmap = new Bitmap(compressWidth, compressHeight);

        var graphics = Graphics.FromImage(bitmap);
        graphics.Clear(Color.Transparent);
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.DrawImage(imageSource,
            new Rectangle((compressWidth - sourceWidth) / 2, (compressHeight - sourceHeight) / 2, sourceWidth, sourceHeight), 0,
            0, imageSource.Width, imageSource.Height, GraphicsUnit.Pixel);
        graphics.Dispose();

        // 设置压缩质量
        var encoderParameters = new EncoderParameters {Param = {[0] = new EncoderParameter(Encoder.Quality, new long[] {flag})}};

        try
        {
            // 找到系统中可用的图片编码器信息
            var imageCodecInfos = ImageCodecInfo.GetImageEncoders();
            var jpegIciInfo = imageCodecInfos.FirstOrDefault(t => t.FormatDescription.Equals("JPEG"));

            // 如果编码器存在的，可以压缩
            if (jpegIciInfo != null)
            {
                bitmap.Save(compressFile, jpegIciInfo, encoderParameters);
                var fileInfo = new FileInfo(compressFile);
                if (fileInfo.Length <= 1024 * size)
                    return true;
                flag -= 10;
                CompressImage(sourceFile, compressFile, compressHeight, compressWidth, flag, size, false);
            }
            else
            {
                bitmap.Save(compressFile, imageFormat);
            }

            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            imageSource.Dispose();
            bitmap.Dispose();
        }
    }

    /// <summary>
    /// 适配缩量尺寸
    /// </summary>
    /// <param name="spcWidth"></param>
    /// <param name="spcHeight"></param>
    /// <param name="orgWidth"></param>
    /// <param name="orgHeight"></param>
    /// <returns></returns>
    public static Size AdjustSize(int spcWidth, int spcHeight, int orgWidth, int orgHeight)
    {
        var size = new Size();
        // 原始宽高在指定宽高范围内，不作任何处理 
        if (orgWidth <= spcWidth && orgHeight <= spcHeight)
        {
            size.Width = orgWidth;
            size.Height = orgHeight;
        }
        else
        {
            // 取得比例系数 
            var w = orgWidth / (float) spcWidth;
            var h = orgHeight / (float) spcHeight;
            // 宽度比大于高度比 
            if (w > h)
            {
                size.Width = spcWidth;
                size.Height = (int) (w >= 1 ? Math.Round(orgHeight / w) : Math.Round(orgHeight * w));
            }
            // 宽度比小于高度比 
            else if (w < h)
            {
                size.Height = spcHeight;
                size.Width = (int) (h >= 1 ? Math.Round(orgWidth / h) : Math.Round(orgWidth * h));
            }
            // 宽度比等于高度比 
            else
            {
                size.Width = spcWidth;
                size.Height = spcHeight;
            }
        }

        return size;
    }

    /// <summary>
    /// 获取缩略图
    /// </summary>
    /// <param name="image"></param>
    /// <param name="spcWidth"></param>
    /// <param name="spcHeight"></param>
    /// <param name="adjust"></param>
    /// <returns></returns>
    public static Image GetThumbnailImage(this Image image, int spcWidth, int spcHeight, bool adjust = true)
    {
        if (spcWidth == 0)
            spcWidth = image.Width;
        if (spcHeight == 0)
            spcHeight = image.Height;
        if (adjust)
        {
            var size = AdjustSize(spcWidth, spcHeight, image.Width, image.Height);
            //return image.GetThumbnailImage(size.Width, size.Height, () => false, IntPtr.Zero);
            return GetReducedImage(size.Width, size.Height, image);
        }

        //return image.GetThumbnailImage(spcWidth, spcHeight, () => false, IntPtr.Zero);
        return GetReducedImage(spcWidth, spcHeight, image);
    }

    /// <summary>
    /// 从base64获取图片
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Image FromBase64(string value)
    {
        var buffer = Convert.FromBase64String(value);
        using var ms = new MemoryStream(buffer);
        using var image = Image.FromStream(ms);
        return new Bitmap(image);
    }

    /// <summary>
    /// 按照指定的高和宽生成相应的规格的图片，采用此方法生成的缩略图片不会失真
    /// </summary>
    /// <param name="width">指定宽度</param>
    /// <param name="height">指定高度</param>
    /// <param name="imageFrom">原图片</param>
    /// <returns>返回新生成的图</returns>
    public static Image GetReducedImage(int width, int height, Image imageFrom)
    {
        // 源图宽度及高度 
        var imageFromWidth = imageFrom.Width;
        var imageFromHeight = imageFrom.Height;

        // 生成的缩略图在上述"画布"上的位置
        var X = 0;
        var Y = 0;

        // 创建画布
        var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        bmp.SetResolution(imageFrom.HorizontalResolution, imageFrom.VerticalResolution);
        using var g = Graphics.FromImage(bmp);
        // 用白色清空 
        g.Clear(Color.Transparent);

        // 指定高质量的双三次插值法。执行预筛选以确保高质量的收缩。此模式可产生质量最高的转换图像。 
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.CompositingQuality = CompositingQuality.HighQuality;
        // 指定高质量、低速度呈现。 
        g.SmoothingMode = SmoothingMode.HighQuality;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;

        // 在指定位置并且按指定大小绘制指定的 Image 的指定部分。 
        g.DrawImage(imageFrom, new Rectangle(X, Y, width, height), new Rectangle(0, 0, imageFromWidth, imageFromHeight),
            GraphicsUnit.Pixel);

        //将图片以指定的格式保存到到指定的位置
        return bmp;
    }
}