import { defineStore } from "pinia";
import { reactive } from "vue";
import { STORE_CONFIG } from "@/stores/constant";
import type { Lang, Axios } from "@/stores/interface";
import { i18n } from "@/lang";

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
            /**
             * 默认加载选项
             */
            loading: {
                fullscreen: true,
                lock: true,
                text: i18n.global.t("utils.axios.有效期应为一个有效数值"),
                background: "rgba(0, 0, 0, 0.7)",
            },
            /**
             * 错误白名单路径
             */
            errorWhiteUrls: ["logout"],
            /**
             * 重新登录Code
             */
            reloadLoginCodes: [401],
            /**
             * 重新登录消息
             */
            reloadLoginMessage: i18n.global.t("utils.axios.登录已失效，请重新登录！"),
            /**
             * 重新登录弹窗按钮文本
             */
            reloadLoginButtonText: i18n.global.t("utils.axios.重新登录"),
            /**
             * 错误Code消息
             */
            errorCodeMessages: {
                cancelDuplicate: i18n.global.t("utils.axios.重复请求，自动取消！"),
                offLine: i18n.global.t("utils.axios.您断网了！"),
                fileDownloadError: i18n.global.t("utils.axios.文件下载失败或此文件不存在！"),
                302: i18n.global.t("utils.axios.接口重定向了！"),
                400: i18n.global.t("utils.axios.参数不正确！"),
                401: i18n.global.t("utils.axios.您没有权限操作（令牌、用户名、密码错误）！"),
                403: i18n.global.t("utils.axios.您的访问是被禁止的！"),
                404: i18n.global.t("utils.axios.请求的资源不存在！"),
                405: i18n.global.t("utils.axios.请求的格式不正确！"),
                408: i18n.global.t("utils.axios.请求超时！"),
                409: i18n.global.t("utils.axios.系统已存在相同数据！"),
                410: i18n.global.t("utils.axios.请求的资源被永久删除，且不会再得到的！"),
                422: i18n.global.t("utils.axios.当创建一个对象时，发生一个验证错误！"),
                429: i18n.global.t("utils.axios.请求过于频繁，请稍后再试！"),
                500: i18n.global.t("utils.axios.服务器内部错误！"),
                501: i18n.global.t("utils.axios.服务未实现！"),
                502: i18n.global.t("utils.axios.网关错误！"),
                503: i18n.global.t("utils.axios.服务不可用，服务器暂时过载或维护！"),
                504: i18n.global.t("utils.axios.服务暂时无法访问，请稍后再试！"),
                505: i18n.global.t("utils.axios.HTTP版本不受支持！"),
                default: i18n.global.t("utils.axios.请求错误！"),
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
