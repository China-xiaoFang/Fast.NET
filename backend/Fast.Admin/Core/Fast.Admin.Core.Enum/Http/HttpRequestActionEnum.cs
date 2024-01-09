// Apache开源许可证
//
// 版权所有 © 2018-2024 1.8K仔
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

namespace Fast.Admin.Core.Enum.Http;

/// <summary>
/// <see cref="HttpRequestActionEnum"/> Http请求行为枚举
/// </summary>
[FastEnum("Http请求行为枚举")]
public enum HttpRequestActionEnum
{
    /// <summary>
    /// 登录
    /// </summary>
    [Description("登录")]
    Auth = 1,

    /// <summary>
    /// 分页查询
    /// </summary>
    [Description("分页查询")]
    Page = 11,

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

    /// <summary>
    /// 更新
    /// </summary>
    [Description("更新")]
    Update = 31,

    /// <summary>
    /// 删除
    /// </summary>
    [Description("删除")]
    Delete = 41,

    /// <summary>
    /// 批量删除
    /// </summary>
    [Description("批量删除")]
    BatchDelete = 42,

    /// <summary>
    /// 下载
    /// </summary>
    [Description("下载")]
    Download = 51,

    /// <summary>
    /// 上传
    /// </summary>
    [Description("上传")]
    Upload = 61,

    /// <summary>
    /// 导出
    /// </summary>
    [Description("导出")]
    Export = 71,

    /// <summary>
    /// 导入
    /// </summary>
    [Description("导入")]
    Import = 81,
}