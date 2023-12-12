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
     * 侧边菜单背景色
     * @type {Array<string>}
     * @memberof Layout
     */
    menuBackground: string[];
    /**
     * 侧边菜单文字颜色
     * @type {Array<string>}
     * @memberof Layout
     */
    menuColor: string[];
    /**
     * 侧边菜单激活项背景色
     * @type {Array<string>}
     * @memberof Layout
     */
    menuActiveBackground: string[];
    /**
     * 侧边菜单激活项文字色
     * @type {Array<string>}
     * @memberof Layout
     */
    menuActiveColor: string[];
    /**
     * 侧边菜单顶栏背景色
     * @type {Array<string>}
     * @memberof Layout
     */
    menuTopBarBackground: string[];
    /**
     * 侧边菜单宽度(展开时)，单位px
     * @type {number}
     * @memberof Layout
     */
    menuWidth: number;
    /**
     * 侧边菜单项默认图标
     * @type {string}
     * @memberof Layout
     */
    menuDefaultIcon: string;
    /**
     * 是否水平折叠收起菜单
     * @type {boolean}
     * @memberof Layout
     */
    menuCollapse: boolean;
    /**
     * 是否只保持一个子菜单的展开(手风琴)
     * @type {boolean}
     * @memberof Layout
     */
    menuUniqueOpened: boolean;
    /**
     * 显示菜单栏顶栏(LOGO)
     * @type {boolean}
     * @memberof Layout
     */
    menuShowTopBar: boolean;

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
