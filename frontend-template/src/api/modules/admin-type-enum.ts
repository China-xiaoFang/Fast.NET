/**
 * Fast.Admin.Core.Enum.System.AdminTypeEnum 账号类型枚举
 * @export
 * @enum {string}
 */
export enum AdminTypeEnum {
    /**
     * 系统默认账号
     * @memberof AdminTypeEnum
     */
    Default = 0,
    /**
     * 超级管理员
     * @memberof AdminTypeEnum
     */
    SuperAdmin = 1,
    /**
     * 系统管理员
     * @memberof AdminTypeEnum
     */
    SystemAdmin = 2,
    /**
     * 租户管理员
     * @memberof AdminTypeEnum
     */
    TenantAdmin = 3,
    /**
     * 普通账号
     * @memberof AdminTypeEnum
     */
    None = 4,
}
