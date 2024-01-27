import { AdminTypeEnum } from "./admin-type-enum";
import { GenderEnum } from "./gender-enum";

/**
 * Fast.Admin.Service.Authentication.Auth.Dto.GetLoginUserInfoOutput 获取登录用户信息
 *
 * @export
 * @interface GetLoginUserInfoOutput
 */
export interface GetLoginUserInfoOutput {
    /**
     * 账号
     *
     * @type {string}
     * @memberof GetLoginUserInfoOutput
     */
    account?: string | null;

    /**
     * 工号
     *
     * @type {string}
     * @memberof GetLoginUserInfoOutput
     */
    jobNumber?: string | null;

    /**
     * 姓名
     *
     * @type {string}
     * @memberof GetLoginUserInfoOutput
     */
    userName?: string | null;

    /**
     * 昵称
     *
     * @type {string}
     * @memberof GetLoginUserInfoOutput
     */
    nickName?: string | null;

    /**
     * 头像
     *
     * @type {string}
     * @memberof GetLoginUserInfoOutput
     */
    avatar?: string | null;

    /**
     * 生日
     *
     * @type {Date}
     * @memberof GetLoginUserInfoOutput
     */
    birthday?: Date | null;

    /**
     * @type {GenderEnum}
     * @memberof GetLoginUserInfoOutput
     */
    sex?: GenderEnum;

    /**
     * 邮箱
     *
     * @type {string}
     * @memberof GetLoginUserInfoOutput
     */
    email?: string | null;

    /**
     * 手机
     *
     * @type {string}
     * @memberof GetLoginUserInfoOutput
     */
    mobile?: string | null;

    /**
     * 电话
     *
     * @type {string}
     * @memberof GetLoginUserInfoOutput
     */
    tel?: string | null;

    /**
     * 主部门Id
     *
     * @type {number}
     * @memberof GetLoginUserInfoOutput
     */
    departmentId?: number;

    /**
     * 主部门名称
     *
     * @type {string}
     * @memberof GetLoginUserInfoOutput
     */
    departmentName?: string | null;

    /**
     * @type {AdminTypeEnum}
     * @memberof GetLoginUserInfoOutput
     */
    adminType?: AdminTypeEnum;

    /**
     * 最后登录设备
     *
     * @type {string}
     * @memberof GetLoginUserInfoOutput
     */
    lastLoginDevice?: string | null;

    /**
     * 最后登录操作系统（版本）
     *
     * @type {string}
     * @memberof GetLoginUserInfoOutput
     */
    lastLoginOS?: string | null;

    /**
     * 最后登录浏览器（版本）
     *
     * @type {string}
     * @memberof GetLoginUserInfoOutput
     */
    lastLoginBrowser?: string | null;

    /**
     * 最后登录省份
     *
     * @type {string}
     * @memberof GetLoginUserInfoOutput
     */
    lastLoginProvince?: string | null;

    /**
     * 最后登录城市
     *
     * @type {string}
     * @memberof GetLoginUserInfoOutput
     */
    lastLoginCity?: string | null;

    /**
     * 最后登录Ip
     *
     * @type {string}
     * @memberof GetLoginUserInfoOutput
     */
    lastLoginIp?: string | null;

    /**
     * 最后登录时间
     *
     * @type {Date}
     * @memberof GetLoginUserInfoOutput
     */
    lastLoginTime?: Date | null;
}
