/**
 * 工具类
 */

import { nextTick } from "vue";
import { AxiosResponse } from "axios";
import router from "@/router";
import { useTitle } from "@vueuse/core";
import { useSiteConfig } from "@/stores/siteConfig";
import { getUrl } from "@/utils/axios";
import { i18n } from "@/lang";

/**
 * 下载文件
 * @param res
 */
export function downloadFile(res: AxiosResponse<any, any>) {
    var blob = new Blob([res.data], { type: "application/octet-stream;charset=UTF-8" });
    var contentDisposition = res.headers["content-disposition"];
    var pat = new RegExp("filename=([^;]+\\.[^\\.;]+);*");
    var result = pat.exec(contentDisposition);
    var filename = result[1];
    var downloadElement = document.createElement("a");
    var href = window.URL.createObjectURL(blob); // 创建下载的链接
    var reg = /^["](.*)["]$/g;
    downloadElement.style.display = "none";
    downloadElement.href = href;
    downloadElement.download = decodeURI(filename.replace(reg, "$1")); // 下载后文件名
    document.body.appendChild(downloadElement);
    downloadElement.click(); // 点击下载
    document.body.removeChild(downloadElement); // 下载完成移除元素
    window.URL.revokeObjectURL(href);
}

/**
 * 加载网络css文件
 * @param url css资源url
 */
export function loadCss(url: string): void {
    const link = document.createElement("link");
    link.rel = "stylesheet";
    link.href = url;
    link.crossOrigin = "anonymous";
    document.getElementsByTagName("head")[0].appendChild(link);
}

/**
 * 加载网络js文件
 * @param url js资源url
 */
export function loadJs(url: string): void {
    const link = document.createElement("script");
    link.src = url;
    document.body.appendChild(link);
}

/**
 * 设置浏览器标题
 * @param webTitle 新的标题
 */
export function setTitle(webTitle: string) {
    if (router.currentRoute.value) {
        router.currentRoute.value.meta.title = webTitle;
    }
    nextTick(() => {
        const title = useTitle();
        const siteConfigStore = useSiteConfig();
        title.value = `${webTitle}${siteConfigStore.state.siteName ? " - " + siteConfigStore.state.siteName : ""}`;
    });
}

/**
 * 全局防抖
 * @description 与其余防抖函数不同的是，间隔期间如果再次传递不同的函数，两个函数也只会执行一次
 * @param fn 执行函数
 * @param ms 间隔毫秒数
 * @returns
 */
export const globalDebounce = (fn: Function, ms: number = 500) => {
    return (...args: any[]) => {
        if (window.lazy) {
            clearTimeout(window.lazy);
        }
        window.lazy = window.setTimeout(() => {
            fn(...args);
        }, ms);
    };
};

/**
 * 获取资源完整地址
 * @param relativeUrl 资源相对地址
 * @param domain 指定域名
 */
export const fullUrl = (relativeUrl: string, domain = "") => {
    const siteConfigStore = useSiteConfig();
    if (!domain) {
        domain = siteConfigStore.state.cdnUrl ? siteConfigStore.state.cdnUrl : getUrl();
    }
    if (!relativeUrl) return domain;

    const regUrl = new RegExp(/^http(s)?:\/\//);
    const regexImg = new RegExp(/^((?:[a-z]+:)?\/\/|data:image\/)(.*)/i);
    if (!domain || regUrl.test(relativeUrl) || regexImg.test(relativeUrl)) {
        return relativeUrl;
    }
    return domain + relativeUrl;
};

/**
 * 获取一组资源的完整地址
 * @param relativeUrls 资源相对地址
 * @param domain 指定域名
 */
export const arrayFullUrl = (relativeUrls: string | string[], domain = "") => {
    if (typeof relativeUrls === "string") {
        relativeUrls = relativeUrls == "" ? [] : relativeUrls.split(",");
    }
    for (const key in relativeUrls) {
        relativeUrls[key] = fullUrl(relativeUrls[key], domain);
    }
    return relativeUrls;
};

/**
 * 格式化时间戳
 * @param dateTime 时间戳
 * @param fmt 格式化方式，默认：yyyy-mm-dd hh:MM:ss
 */
export const timeFormat = (dateTime: string | number | null = null, fmt = "yyyy-mm-dd hh:MM:ss") => {
    if (dateTime == "none") return i18n.global.t("utils.无");
    if (!dateTime) dateTime = Number(new Date());
    if (dateTime.toString().length === 10) {
        dateTime = +dateTime * 1000;
    }

    const date = new Date(dateTime);
    let ret;
    const opt: anyObj = {
        "y+": date.getFullYear().toString(), // 年
        "m+": (date.getMonth() + 1).toString(), // 月
        "d+": date.getDate().toString(), // 日
        "h+": date.getHours().toString(), // 时
        "M+": date.getMinutes().toString(), // 分
        "s+": date.getSeconds().toString(), // 秒
    };
    for (const k in opt) {
        ret = new RegExp("(" + k + ")").exec(fmt);
        if (ret) {
            fmt = fmt.replace(ret[1], ret[1].length == 1 ? opt[k] : padStart(opt[k], ret[1].length, "0"));
        }
    }
    return fmt;
};

/**
 * 字符串补位
 */
const padStart = (str: string, maxLength: number, fillString = " ") => {
    if (str.length >= maxLength) return str;

    const fillLength = maxLength - str.length;
    let times = Math.ceil(fillLength / fillString.length);
    while ((times >>= 1)) {
        fillString += fillString;
        if (times === 1) {
            fillString += fillString;
        }
    }
    return fillString.slice(0, fillLength) + str;
};

/**
 * 根据当前时间生成问候语
 */
export const getGreet = () => {
    const now = new Date();
    const hour = now.getHours();
    let greet = "";

    if (hour < 5) {
        greet = i18n.global.t("utils.夜深了，注意身体哦！");
    } else if (hour < 9) {
        greet = i18n.global.t("utils.早上好！") + i18n.global.t("utils.欢迎回来！");
    } else if (hour < 12) {
        greet = i18n.global.t("utils.上午好！") + i18n.global.t("utils.欢迎回来！");
    } else if (hour < 14) {
        greet = i18n.global.t("utils.中午好！") + i18n.global.t("utils.欢迎回来！");
    } else if (hour < 18) {
        greet = i18n.global.t("utils.下午好！") + i18n.global.t("utils.欢迎回来！");
    } else if (hour < 24) {
        greet = i18n.global.t("utils.晚上好！") + i18n.global.t("utils.欢迎回来！");
    } else {
        greet = i18n.global.t("utils.您好！") + i18n.global.t("utils.欢迎回来！");
    }
    return greet;
};
