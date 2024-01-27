import { LoginMethodEnum } from "./login-method-enum";

/**
 * Fast.Admin.Service.Authentication.Login.Dto.LoginInput 登录输入
 *
 * @export
 * @interface LoginInput
 */
export interface LoginInput {
    /**
     * 账号/邮箱/手机号码
     *
     * @type {string}
     * @memberof LoginInput
     * @example superAdmin
     */
    account?: string | null;

    /**
     * 密码
     *
     * @type {string}
     * @memberof LoginInput
     */
    password?: string | null;

    /**
     * @type {LoginMethodEnum}
     * @memberof LoginInput
     */
    loginMethod?: LoginMethodEnum;
}
