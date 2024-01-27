import { AdminTypeEnum } from "./admin-type-enum";

/**
 *
 *
 * @export
 * @interface LoginTenantDto
 */
export interface LoginTenantDto {
    /**
     * Id
     *
     * @type {number}
     * @memberof LoginTenantDto
     */
    id?: number;

    /**
     * 工号
     *
     * @type {string}
     * @memberof LoginTenantDto
     */
    jobNumber?: string | null;

    /**
     * 昵称
     *
     * @type {string}
     * @memberof LoginTenantDto
     */
    nickName?: string | null;

    /**
     * 头像
     *
     * @type {string}
     * @memberof LoginTenantDto
     */
    avatar?: string | null;

    /**
     * 部门名称
     *
     * @type {string}
     * @memberof LoginTenantDto
     */
    departmentName?: string | null;

    /**
     * @type {AdminTypeEnum}
     * @memberof LoginTenantDto
     */
    adminType?: AdminTypeEnum;

    /**
     * 最后登录设备
     *
     * @type {string}
     * @memberof LoginTenantDto
     */
    lastLoginDevice?: string | null;

    /**
     * 最后登录操作系统（版本）
     *
     * @type {string}
     * @memberof LoginTenantDto
     */
    lastLoginOS?: string | null;

    /**
     * 最后登录浏览器（版本）
     *
     * @type {string}
     * @memberof LoginTenantDto
     */
    lastLoginBrowser?: string | null;

    /**
     * 最后登录省份
     *
     * @type {string}
     * @memberof LoginTenantDto
     */
    lastLoginProvince?: string | null;

    /**
     * 最后登录城市
     *
     * @type {string}
     * @memberof LoginTenantDto
     */
    lastLoginCity?: string | null;

    /**
     * 最后登录Ip
     *
     * @type {string}
     * @memberof LoginTenantDto
     */
    lastLoginIp?: string | null;

    /**
     * 最后登录时间
     *
     * @type {Date}
     * @memberof LoginTenantDto
     */
    lastLoginTime?: Date | null;

    /**
     * 租户公司中文简称
     *
     * @type {string}
     * @memberof LoginTenantDto
     */
    chShortName?: string | null;
}
