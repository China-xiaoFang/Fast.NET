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

namespace Fast.IaaS;

/// <summary>
/// <see cref="CoordinateUtil"/> 坐标工具类
/// </summary>
/// <remarks>
/// <para>WGS-84：全球标准坐标系，常用于 GPS 定位</para>
/// <para>GCJ-02：中国国测局坐标系，又称火星坐标系，国内地图（高德、腾讯、百度等）使用</para>
/// </remarks>
public static class CoordinateUtil
{
    /// <summary>π 常量，用于角度与弧度转换</summary>
    private const double pi = 3.1415926535897932384626;

    /// <summary>长半轴，地球椭球体的长半轴（单位：米）</summary>
    private const double a = 6378245.0;

    /// <summary>偏心率平方，椭球体的形状参数</summary>
    private const double ee = 0.00669342162296594323;

    /// <summary>
    /// 判断给定经纬度是否在中国境内
    /// <para>GCJ-02 偏移只在中国境内有效，境外坐标无需转换</para>
    /// </summary>
    /// <param name="lat"><see cref="double"/> 纬度</param>
    /// <param name="lng"><see cref="double"/> 经度</param>
    /// <returns>在中国境内返回 true，否则 false</returns>
    public static bool IsInChina(double lat, double lng)
    {
        if (lng < 72.004 || lng > 137.8347)
            return false;
        if (lat < 0.8293 || lat > 55.8271)
            return false;
        return true;
    }

    /// <summary>
    /// 纬度偏移计算公式
    /// </summary>
    /// <remarks>根据国测局算法对纬度进行偏移</remarks>
    private static double TransformLat(double x, double y)
    {
        var ret = -100.0 + 2.0 * x + 3.0 * y + 0.2 * y * y + 0.1 * x * y + 0.2 * Math.Sqrt(Math.Abs(x));
        ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
        ret += (20.0 * Math.Sin(y * pi) + 40.0 * Math.Sin(y / 3.0 * pi)) * 2.0 / 3.0;
        ret += (160.0 * Math.Sin(y / 12.0 * pi) + 320 * Math.Sin(y * pi / 30.0)) * 2.0 / 3.0;
        return ret;
    }

    /// <summary>
    /// 经度偏移计算公式
    /// </summary>
    /// <remarks>根据国测局算法对经度进行偏移</remarks>
    private static double TransformLng(double x, double y)
    {
        var ret = 300.0 + x + 2.0 * y + 0.1 * x * x + 0.1 * x * y + 0.1 * Math.Sqrt(Math.Abs(x));
        ret += (20.0 * Math.Sin(6.0 * x * pi) + 20.0 * Math.Sin(2.0 * x * pi)) * 2.0 / 3.0;
        ret += (20.0 * Math.Sin(x * pi) + 40.0 * Math.Sin(x / 3.0 * pi)) * 2.0 / 3.0;
        ret += (150.0 * Math.Sin(x / 12.0 * pi) + 300.0 * Math.Sin(x / 30.0 * pi)) * 2.0 / 3.0;
        return ret;
    }

    /// <summary>
    /// WGS-84 -> GCJ-02（火星坐标）
    /// </summary>
    /// <remarks>如果坐标在中国境外，则返回原坐标</remarks>
    /// <param name="wgLat"><see cref="double"/> WGS-84 纬度</param>
    /// <param name="wgLng"><see cref="double"/> WGS-84 经度</param>
    /// <returns>GCJ-02 坐标（纬度, 经度）</returns>
    public static (double lat, double lng) WGS84ToGCJ02(double wgLat, double wgLng)
    {
        if (!IsInChina(wgLat, wgLng))
            return (wgLat, wgLng);

        // 偏移量计算
        var dLat = TransformLat(wgLng - 105.0, wgLat - 35.0);
        var dLng = TransformLng(wgLng - 105.0, wgLat - 35.0);

        // 纬度弧度化
        var radLat = wgLat / 180.0 * pi;

        // 椭球体修正系数
        var magic = Math.Sin(radLat);
        magic = 1 - ee * magic * magic;
        var sqrtMagic = Math.Sqrt(magic);

        // 调整偏移量为实际经纬度偏移
        dLat = dLat * 180.0 / (a * (1 - ee) / (magic * sqrtMagic) * pi);
        dLng = dLng * 180.0 / (a / sqrtMagic * Math.Cos(radLat) * pi);

        // 返回加上偏移后的 GCJ-02 坐标
        var mgLat = wgLat + dLat;
        var mgLng = wgLng + dLng;
        return (mgLat, mgLng);
    }

    /// <summary>
    /// GCJ-02（火星坐标）-> WGS-84
    /// </summary>
    /// <remarks>精确逆算比较复杂，这里使用迭代近似方法</remarks>
    /// <param name="mgLat"><see cref="double"/> GCJ-02 纬度</param>
    /// <param name="mgLng"><see cref="double"/> GCJ-02 经度</param>
    /// <returns>WGS-84 坐标（纬度, 经度）</returns>
    public static (double lat, double lng) GCJ02ToWGS84(double mgLat, double mgLng)
    {
        if (!IsInChina(mgLat, mgLng))
            return (mgLat, mgLng);

        // 先将 GCJ-02 坐标正向转换到 WGS-84 坐标
        var (lat1, lng1) = WGS84ToGCJ02(mgLat, mgLng);

        // 偏移量
        var dLat = lat1 - mgLat;
        var dLng = lng1 - mgLng;

        // WGS-84 近似值 = GCJ-02 坐标 - 偏移量
        return (mgLat - dLat, mgLng - dLng);
    }
}