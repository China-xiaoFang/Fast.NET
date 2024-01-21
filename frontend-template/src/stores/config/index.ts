import { defineStore } from "pinia";
import { reactive } from "vue";
import { STORE_CONFIG } from "@/stores/constant";
import type { Lang, Axios, Layout } from "./interface";
import { getLocalLang } from "@/lang";

export const useConfig = defineStore(
    "config",
    () => {
        // 多语言
        const lang: Lang = reactive({
            // 默认语言。这里获取浏览器默认语言
            defaultLang: getLocalLang(navigator.language),
            // 当在默认语言包找不到翻译时，继续在 fallbackLang 语言包内查找翻译
            fallbackLang: getLocalLang(navigator.language),
            // 支持的语言列表
            langArray: [
                { name: "zh-CN", value: "中文简体" },
                { name: "zh-TW", value: "中文繁体" },
                { name: "en", value: "English" },
            ],
        });

        // Axios 配置
        const axios: Axios = reactive({
            /**
             * 默认选项
             */
            options: {
                // 是否开启取消重复请求, 默认为 true
                cancelDuplicateRequest: true,
                // 是否开启loading层效果, 默认为 true
                loading: true,
                // Get请求缓存问题处理，默认为 true
                getMethodCacheHandle: true,
                // 是否开启简洁的数据结构响应, 默认为 true
                simpleDataFormat: true,
                // 是否开启接口错误信息展示, 默认为 true
                showErrorMessage: true,
                // 是否开启code信息提示, 默认为 true
                showCodeMessage: true,
                // 是否开启请求成功的信息提示, 默认为 false
                showSuccessMessage: false,
                // 是否开启自动下载文件, 默认为 true
                autoDownloadFile: true,
            },
        });

        // 布局配置
        const layout: Layout = reactive({
            // 显示全局设置抽屉
            showSettingDrawer: false,
            // 是否收缩布局(小屏设备)
            shrink: false,
            // 布局方式
            layoutMode: "Default",
            // 切换动画
            mainAnimation: "slide-right",
            // 是否暗黑模式
            isDark: false,

            /* 侧边菜单 */
            // 侧边菜单背景色
            menuBackground: ["#ffffff", "#1d1e1f"],
            // 侧边菜单文字颜色
            menuColor: ["#303133", "#CFD3DC"],
            // 侧边菜单激活项背景色
            menuActiveBackground: ["#ffffff", "#1d1e1f"],
            // 侧边菜单激活项文字色
            menuActiveColor: ["#409eff", "#3375b9"],
            // 侧边菜单顶栏背景色
            menuTopBarBackground: ["#fcfcfc", "#1d1e1f"],
            // 侧边菜单宽度(展开时)，单位px
            menuWidth: 260,
            // 侧边菜单项默认图标
            menuDefaultIcon: "fa fa-circle-o",
            // 是否水平折叠收起菜单
            menuCollapse: false,
            // 是否只保持一个子菜单的展开(手风琴)
            menuUniqueOpened: false,
            // 显示菜单栏顶栏(LOGO)
            menuShowTopBar: true,

            /* 顶栏 */
            // 顶栏文字色
            headerBarTabColor: ["#000000", "#CFD3DC"],
            // 顶栏激活项背景色
            headerBarTabActiveBackground: ["#ffffff", "#1d1e1f"],
            // 顶栏激活项文字色
            headerBarTabActiveColor: ["#000000", "#409EFF"],
            // 顶栏背景色
            headerBarBackground: ["#ffffff", "#1d1e1f"],
            // 顶栏悬停时背景色
            headerBarHoverBackground: ["#f5f5f5", "#18222c"],
        });

        /**
         * 设置布局的值
         * @param name
         * @param value
         */
        const setLayout = (name: keyof Layout, value: any) => {
            layout[name] = value as never;
        };

        /**
         * 获取菜单宽度
         * @returns
         */
        function menuWidth() {
            if (layout.shrink) {
                return layout.menuCollapse ? "0px" : layout.menuWidth + "px";
            }
            // 菜单是否折叠
            return layout.menuCollapse ? "64px" : layout.menuWidth + "px";
        }

        /**
         * 设置布局颜色
         * @param data
         */
        function onSetLayoutColor(data = layout.layoutMode) {
            // 切换布局时，如果是为默认配色方案，对菜单激活背景色重新赋值
            const tempValue = layout.isDark ? { idx: 1, color: "#1d1e1f", newColor: "#141414" } : { idx: 0, color: "#ffffff", newColor: "#f5f5f5" };
            if (
                data == "Classic" &&
                layout.headerBarBackground[tempValue.idx] == tempValue.color &&
                layout.headerBarTabActiveBackground[tempValue.idx] == tempValue.color
            ) {
                layout.headerBarTabActiveBackground[tempValue.idx] = tempValue.newColor;
            } else if (
                data == "Default" &&
                layout.headerBarBackground[tempValue.idx] == tempValue.color &&
                layout.headerBarTabActiveBackground[tempValue.idx] == tempValue.newColor
            ) {
                layout.headerBarTabActiveBackground[tempValue.idx] = tempValue.color;
            }
        }

        /**
         * 设置布局方式
         * @param data
         */
        function setLayoutMode(data: string) {
            layout.layoutMode = data;
            onSetLayoutColor(data);
        }

        /**
         * 获取配置颜色
         * @param name 配置名称
         * @returns
         */
        const getColorVal = function (name: keyof Layout): string {
            const colors = layout[name] as string[];
            if (layout.isDark) {
                return colors[1];
            } else {
                return colors[0];
            }
        };

        return { lang, axios, layout, setLayout, menuWidth, onSetLayoutColor, setLayoutMode, getColorVal };
    },
    {
        persist: {
            key: STORE_CONFIG,
        },
    }
);
