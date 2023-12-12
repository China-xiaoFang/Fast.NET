import { defineStore } from "pinia";
import { reactive } from "vue";
import { STORE_CONFIG } from "@/stores/constant";
import type { Lang, Axios, Layout } from "./interface";

export const useConfig = defineStore(
    "config",
    () => {
        // 多语言
        const lang: Lang = reactive({
            // 默认语言
            defaultLang: "zh-cn",
            // 当在默认语言包找不到翻译时，继续在 fallbackLang 语言包内查找翻译
            fallbackLang: "zh-cn",
            // 支持的语言列表
            langArray: [
                { name: "zh-cn", value: "中文简体" },
                { name: "zh-tw", value: "中文繁体" },
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
                loading: false,
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
         * 设置布局方式
         * @param data
         */
        function setLayoutMode(data: "Default" | "Classic") {
            layout.layoutMode = data;
            // onSetLayoutColor(data);
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

        return { lang, axios, layout, setLayout, setLayoutMode, getColorVal };
    },
    {
        persist: {
            key: STORE_CONFIG,
        },
    }
);
