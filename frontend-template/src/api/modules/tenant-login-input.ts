/**
 * Fast.Admin.Service.Authentication.Login.Dto.TenantLoginInput 租户登录输入
 *
 * @export
 * @interface TenantLoginInput
 */
export interface TenantLoginInput {
    /**
     * 账号Id
     *
     * @type {number}
     * @memberof TenantLoginInput
     */
    accountId?: number;

    /**
     * 租户账号Id
     *
     * @type {number}
     * @memberof TenantLoginInput
     */
    tenantAccountId?: number;

    /**
     * 密码
     *
     * @type {string}
     * @memberof TenantLoginInput
     */
    password?: string | null;
}
