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

/**
 * 布局配置
 * @interface Layout
 */
export interface Layout {
    /**
     * 显示全局设置抽屉
     * @type {boolean}
     * @memberof Layout
     */
    showSettingDrawer: boolean;
    /**
     * 是否收缩布局(小屏设备)
     * @type {boolean}
     * @memberof Layout
     */
    shrink: boolean;
    /**
     * 布局方式
     * @description 可选值 <"Default" | "Classic">
     * @type {string}
     * @memberof Layout
     */
    layoutMode: string;
    /**
     * 切换动画
     * @description 可选值 <"slide-right" | "slide-left" | "el-fade-in-linear" | "el-fade-in" | "el-zoom-in-center" | "el-zoom-in-top" | "el-zoom-in-bottom">
     * @type {string}
     * @memberof Layout
     */
    mainAnimation: string;
    /**
     * 是否暗黑模式
     * @type {boolean}
     * @memberof Layout
     */
    isDark: boolean;

    /**
     * 顶栏文字色
     * @type {Array<string>}
     * @memberof Layout
     */
    headerBarTabColor: string[];
    /**
     * 顶栏激活项背景色
     * @type {Array<string>}
     * @memberof Layout
     */
    headerBarTabActiveBackground: string[];
    /**
     * 顶栏激活项文字色
     * @type {Array<string>}
     * @memberof Layout
     */
    headerBarTabActiveColor: string[];
    /**
     * 顶栏背景色
     * @type {Array<string>}
     * @memberof Layout
     */
    headerBarBackground: string[];
    /**
     * 顶栏悬停时背景色
     * @type {Array<string>}
     * @memberof Layout
     */
    headerBarHoverBackground: string[];
}
