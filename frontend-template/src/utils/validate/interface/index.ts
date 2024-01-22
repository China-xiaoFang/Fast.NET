/**
 * validate 变量定义
 */

/**
 * 构建表单验证实例
 * @interface buildFormValidatorInstance
 */
export interface buildFormValidatorInstance {
    /**
     * 规则名称
     * required 必填
     * account 账号
     * password 密码
     * mobile 手机号
     * idNumber 身份证
     * editorRequired 富文本必填
     * number、integer、float、date、url、email
     * @memberof buildFormValidatorInstance
     */
    name: "required" | "account" | "password" | "mobile" | "idNumber" | "editorRequired" | "number" | "integer" | "float" | "date" | "url" | "email";
    /**
     * 自定义验证消息
     * @type {string}
     * @memberof buildFormValidatorInstance
     */
    message?: string;
    /**
     * 触发验证的方式
     * @memberof buildFormValidatorInstance
     */
    trigger?: "change" | "blur";
}
