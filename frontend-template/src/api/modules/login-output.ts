import { LoginTenantDto } from "./login-tenant-dto";

/**
 * Fast.Admin.Service.Authentication.Login.Dto.LoginOutput 登录输出
 *
 * @export
 * @interface LoginOutput
 */
export interface LoginOutput {
    /**
     * 是否自动登录
     *
     * @type {boolean}
     * @memberof LoginOutput
     */
    isAutoLogin?: boolean;

    /**
     * 账号ID
     *
     * @type {number}
     * @memberof LoginOutput
     */
    accountId?: number;

    /**
     * 租户集合
     *
     * @type {Array<LoginTenantDto>}
     * @memberof LoginOutput
     */
    tenantList?: Array<LoginTenantDto> | null;
}
