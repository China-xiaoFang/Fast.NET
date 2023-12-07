import { defineStore } from "pinia";
import { reactive } from "vue";
import { STORE_CONFIG } from "@/stores/constant";
import type { Lang, Axios } from "@/stores/interface";

export const useConfig = defineStore(
    "config",
    () => {
        /**
         * 多语言
         */
        const lang: Lang = reactive({
            /**
             * 默认语言
             */
            defaultLang: "zh-cn",
            /**
             * 当在默认语言包找不到翻译时，继续在 fallbackLang 语言包内查找翻译
             */
            fallbackLang: "zh-cn",
            /**
             * 支持的语言列表
             */
            langArray: [
                { name: "zh-cn", value: "中文简体" },
                { name: "zh-tw", value: "中文繁体" },
                { name: "en", value: "English" },
            ],
        });

        /**
         * Axios 配置
         */
        const axios: Axios = reactive({
            /**
             * 默认选项
             */
            options: {
                cancelDuplicateRequest: true,
                loading: false,
                getMethodCacheHandle: true,
                simpleDataFormat: true,
                showErrorMessage: true,
                showCodeMessage: true,
                showSuccessMessage: false,
                autoDownloadFile: true,
            },
        });

        return { lang, axios };
    },
    {
        persist: {
            key: STORE_CONFIG,
        },
    }
);
