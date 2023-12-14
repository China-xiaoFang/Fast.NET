import type { App } from "vue";
import { createI18n } from "vue-i18n";
import type { I18n, Composer } from "vue-i18n";
import { useConfig } from "@/stores/config";
import { isEmpty } from "lodash-es";

/**
 * 默认值只引入 element-plus 的简体中文，繁体中文，英文语言包
 * 其他语言请自行在此 import，并添加到 defaultLocale 内
 * 动态 import 只支持相对路径，所以无法按需 import element-plus 的语言包
 * 但 i18n 的 messages 内是按需载入的
 */

import elementZhCnLocale from "element-plus/dist/locale/zh-cn.mjs";
import elementZhTwLocale from "element-plus/dist/locale/zh-tw.mjs";
import elementEnLocale from "element-plus/dist/locale/en.mjs";

/**
 * i18n
 */
export let i18n: {
    global: Composer;
};

/**
 * 准备要合并的语言包
 */
const assignLocale: anyObj = {
    "zh-cn": [elementZhCnLocale],
    "zh-tw": [elementZhTwLocale],
    en: [elementEnLocale],
};

/**
 * 加载多语言
 * @param app Vue 应用实例
 * @returns 返回一个 Promise，表示多语言加载的异步操作
 */
export const loadLang = async (app: App) => {
    // 获取配置信息
    const configStore = useConfig();
    // 获取默认语言
    const locale = configStore.lang.defaultLang;

    /**
     * 因 import.meta.glob 不支持变量，所以这里只能手动写死
     * 加载公用的语言包，除去 views 中的全部加载在这里
     * 按需加载语言包文件的句柄
     */
    switch (locale) {
        case "zh-cn":
            window.loadLangHandle = {
                ...import.meta.glob("./zh-cn/views/**/*.ts"),
            };
            assignLocale[locale].push(getLangFileMessage(import.meta.glob("./zh-cn/components/**/*.ts", { eager: true }), locale));
            assignLocale[locale].push(getLangFileMessage(import.meta.glob("./zh-cn/layouts/**/*.ts", { eager: true }), locale));
            assignLocale[locale].push(getLangFileMessage(import.meta.glob("./zh-cn/utils/**/*.ts", { eager: true }), locale));
            break;
        case "zh-tw":
            window.loadLangHandle = {
                ...import.meta.glob("./zh-tw/views/**/*.ts"),
            };
            assignLocale[locale].push(getLangFileMessage(import.meta.glob("./zh-tw/components/**/*.ts", { eager: true }), locale));
            assignLocale[locale].push(getLangFileMessage(import.meta.glob("./zh-tw/layouts/**/*.ts", { eager: true }), locale));
            assignLocale[locale].push(getLangFileMessage(import.meta.glob("./zh-tw/utils/**/*.ts", { eager: true }), locale));
            break;
        case "en":
            window.loadLangHandle = {
                ...import.meta.glob("./en/views/**/*.ts"),
            };
            assignLocale[locale].push(getLangFileMessage(import.meta.glob("./en/components/**/*.ts", { eager: true }), locale));
            assignLocale[locale].push(getLangFileMessage(import.meta.glob("./en/layouts/**/*.ts", { eager: true }), locale));
            assignLocale[locale].push(getLangFileMessage(import.meta.glob("./en/utils/**/*.ts", { eager: true }), locale));
            break;
    }

    const messages = { [locale]: {} };

    // 合并语言包(含element-puls、页面语言包)
    Object.assign(messages[locale], ...assignLocale[locale]);

    // 创建 i18n 实例
    i18n = createI18n({
        locale: locale,
        legacy: false, // 使用组合式 API
        globalInjection: true, // 挂载 $t, $d 等到全局
        fallbackLocale: configStore.lang.fallbackLang,
        messages: messages,
    });

    // 在应用中使用 i18n 插件
    app.use(i18n as I18n);
    return i18n;
};

/**
 * 根据语言文件信息和当前 locale 获取合并后的消息对象
 * @param mList 语言文件信息
 * @param locale 当前 locale
 * @returns 合并后的消息对象
 */
function getLangFileMessage(mList: any, locale: string): anyObj {
    let msg: anyObj = {};
    locale = "/" + locale;
    for (const path in mList) {
        if (mList[path].default) {
            // 获取文件名
            const pathName = path.slice(path.lastIndexOf(locale) + (locale.length + 1), path.lastIndexOf("."));
            /**
             * 这里处理 index.ts 文件后缀的问题
             * 原：utils.axios.index.
             * 处理后：utils.axios.
             */
            const prefixName = pathName.endsWith("index") ? pathName.substring(0, pathName.lastIndexOf("/")) : pathName;
            if (pathName.indexOf("/") > 0) {
                msg = handleMsgList(msg, mList[path].default, prefixName);
            } else {
                msg[prefixName] = mList[path].default;
            }
        }
    }
    return msg;
}

/**
 * 合并消息对象到全局 i18n 实例
 * @param message 消息对象
 * @param pathName 指定路径名
 */
export function mergeMessage(message: anyObj, pathName = ""): void {
    if (isEmpty(message)) return;
    if (!pathName) {
        return i18n.global.mergeLocaleMessage(i18n.global.locale.value, message);
    }
    let msg: anyObj = {};
    if (pathName.indexOf("/") > 0) {
        msg = handleMsgList(msg, message, pathName);
    } else {
        msg[pathName] = message;
    }
    i18n.global.mergeLocaleMessage(i18n.global.locale.value, msg);
}

/**
 * 处理消息列表，将消息对象按照指定路径名进行合并
 * @param msg 目标消息对象
 * @param mList 源消息对象
 * @param pathName 指定路径名
 * @returns 合并后的消息对象
 */
export function handleMsgList(msg: anyObj, mList: anyObj, pathName: string): anyObj {
    const pathNameTmp = pathName.split("/");
    let obj: anyObj = {};
    for (let i = pathNameTmp.length - 1; i >= 0; i--) {
        if (i === pathNameTmp.length - 1) {
            obj = {
                [pathNameTmp[i]]: mList,
            };
        } else {
            obj = {
                [pathNameTmp[i]]: obj,
            };
        }
    }
    return mergeMsg(msg, obj);
}

/**
 * 合并两个消息对象
 * @param msg 目标消息对象
 * @param obj 源消息对象
 * @returns 合并后的消息对象
 */
export function mergeMsg(msg: anyObj, obj: anyObj): anyObj {
    for (const key in obj) {
        if (typeof msg[key] === "undefined") {
            msg[key] = obj[key];
        } else if (typeof msg[key] === "object") {
            msg[key] = mergeMsg(msg[key], obj[key]);
        }
    }
    return msg;
}

/**
 * 修改默认语言设置并重新加载应用
 * @param lang 新的默认语言
 */
export function editDefaultLang(lang: string): void {
    // 获取配置信息
    const config = useConfig();
    // 设置新的默认语言
    config.lang.defaultLang = lang;
    // 语言包是按需加载的，比如默认语言为中文，则只在 app 实例内加载了中文语言包，所以切换语言需要进行 reload
    location.reload();
}
