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

using System.ComponentModel;

// ReSharper disable once CheckNamespace
namespace System;

/// <summary>
/// <see cref="HttpRequestActionEnum"/> Http请求行为枚举
/// </summary>
[FastEnum("Http请求行为枚举")]
public enum HttpRequestActionEnum
{
    /// <summary>
    /// 未知
    /// </summary>
    [Description("未知")]
    None = 0,

    /// <summary>
    /// 鉴权
    /// </summary>
    [Description("鉴权")]
    Auth = 1,

    /// <summary>
    /// 分页
    /// </summary>
    [Description("分页")]
    Paged = 11,

    /// <summary>
    /// 查询
    /// </summary>
    [Description("查询")]
    Query = 12,

    /// <summary>
    /// 添加
    /// </summary>
    [Description("添加")]
    Add = 21,

    ///// <summary>
    ///// 批量添加
    ///// </summary>
    //[Description("批量添加")]
    //BatchAdd = 22,

    /// <summary>
    /// 编辑
    /// </summary>
    [Description("编辑")]
    Edit = 31,

    ///// <summary>
    ///// 批量编辑
    ///// </summary>
    //[Description("批量编辑")]
    //BatchEdit = 32,

    /// <summary>
    /// 删除
    /// </summary>
    [Description("删除")]
    Delete = 41,

    ///// <summary>
    ///// 批量删除
    ///// </summary>
    //[Description("批量删除")]
    //BatchDelete = 42,

    /// <summary>
    /// 提交
    /// </summary>
    [Description("提交")]
    Submit = 51,

    /// <summary>
    /// 上传
    /// </summary>
    [Description("上传")]
    Upload = 61,

    /// <summary>
    /// 下载
    /// </summary>
    [Description("下载")]
    Download = 62,

    /// <summary>
    /// 导入
    /// </summary>
    [Description("导入")]
    Import = 71,

    /// <summary>
    /// 导出
    /// </summary>
    [Description("导出")]
    Export = 72,

    /// <summary>
    /// 通知
    /// </summary>
    [Description("通知")]
    Notify = 253,

    /// <summary>
    /// 回调
    /// </summary>
    [Description("回调")]
    Callback = 254,

    /// <summary>
    /// 其他
    /// </summary>
    [Description("其他")]
    Other = 255
}