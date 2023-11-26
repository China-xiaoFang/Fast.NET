/**
 * Stores 变量定义
 */

/**
 * 多语言
 * @interface Lang
 */
export interface Lang {
    /**
     * 默认语言
     * @type {string}
     * @memberof Lang
     */
    defaultLang: string;
    /**
     * 当在默认语言包找不到翻译时，继续在 fallbackLang 语言包内查找翻译
     * @type {string}
     * @memberof Lang
     */
    fallbackLang: string;
    /**
     * 支持的语言列表
     * @type {Array}
     * @memberof Lang
     */
    langArray: Array<{ name: string; value: string }>;
}

import type { Options } from "@/utils/axios/interface";
import type { LoadingOptions } from "element-plus";

/**
 * Axios 配置
 * @interface Axios
 */
export interface Axios {
    /**
     * 默认选项
     * @type {Options}
     * @memberof Axios
     */
    options: Options;
    /**
     * 默认加载选项
     * @type {LoadingOptions}
     * @memberof Axios
     */
    loading: LoadingOptions;
    /**
     * 错误白名单路径
     * @type {Array<string>}
     * @memberof Axios
     */
    errorWhiteUrls: Array<string>;
    /**
     * 重新登录Code
     * @type {Array<number>}
     * @memberof Axios
     */
    reloadLoginCodes: Array<number>;
    /**
     * 重新登录Code
     * @type {string}
     * @memberof Axios
     */
    reloadLoginMessage: string;
    /**
     * 重新登录弹窗按钮文本
     * @type {string}
     * @memberof Axios
     */
    reloadLoginButtonText: string;
    /**
     * 错误Code消息
     * @type {Array<number>}
     * @memberof Axios
     */
    errorCodeMessages: anyObj;
}

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
}
