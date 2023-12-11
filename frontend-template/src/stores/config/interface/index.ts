/**
 * Stores Config 变量定义
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
}
