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

using Fast.Admin.Core.Enum.System;

namespace Fast.Admin.Service.Authentication.Login.Dto;

/// <summary>
/// <see cref="LoginOutput"/> 登录输出
/// </summary>
public class LoginOutput
{
    /// <summary>
    /// 是否自动登录
    /// </summary>
    public bool IsAutoLogin { get; set; }

    /// <summary>
    /// 账号ID
    /// </summary>
    public long AccountId { get; set; }

    /// <summary>
    /// 租户集合
    /// </summary>
    public List<LoginTenantDto> TenantList { get; set; }

    public class LoginTenantDto
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 工号
        /// </summary>
        public string JobNumber { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 管理员类型
        /// </summary>
        public AdminTypeEnum AdminType { get; set; }

        /// <summary>
        /// 最后登录设备
        /// </summary>
        public string LastLoginDevice { get; set; }

        /// <summary>
        /// 最后登录操作系统（版本）
        /// </summary>
        public string LastLoginOS { get; set; }

        /// <summary>
        /// 最后登录浏览器（版本）
        /// </summary>
        public string LastLoginBrowser { get; set; }

        /// <summary>
        /// 最后登录省份
        /// </summary>
        public string LastLoginProvince { get; set; }

        /// <summary>
        /// 最后登录城市
        /// </summary>
        public string LastLoginCity { get; set; }

        /// <summary>
        /// 最后登录Ip
        /// </summary>
        public string LastLoginIp { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 租户公司中文简称
        /// </summary>
        public string ChShortName { get; set; }
    }
}