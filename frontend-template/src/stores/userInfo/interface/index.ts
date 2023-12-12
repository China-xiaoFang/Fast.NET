/**
 * Stores UserInfo 变量定义
 */

/**
 * 用户信息
 * @interface UserInfo
 */
export interface UserInfo {
    /**
     * Token
     * @type {string}
     * @memberof UserInfo
     */
    token: string;
    /**
     * Refresh Token
     * @type {string}
     * @memberof UserInfo
     */
    refreshToken: string;
    /**
     * 是否为超级管理员
     * @type {boolean}
     * @memberof UserInfo
     */
    supperAdmin: boolean;
    /**
     * 是否为管理员
     * @type {boolean}
     * @memberof UserInfo
     */
    admin: boolean;
    /**
     * 用户名称
     * @type {string}
     * @memberof UserInfo
     */
    userName: string;
    /**
     * 用户昵称
     * @type {string}
     * @memberof UserInfo
     */
    nickName: string;
    /**
     * 头像
     * @type {string}
     * @memberof UserInfo
     */
    avatar: string;
    /**
     * 最后登录时间
     * @type {string}
     * @memberof UserInfo
     */
    lastLoginTime: string;
}
