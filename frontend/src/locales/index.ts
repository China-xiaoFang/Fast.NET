import { createI18n } from "vue-i18n";
import antDZhCN from "ant-design-vue/es/locale/zh_CN";
import antDEnUS from "ant-design-vue/es/locale/en_US";
import zh_CN from "./lang/zh-cn.js";
import en_US from "./lang/en-us.js";
import tool from "@/utils/tool";
import sysConfig from "@/config/index";
import cacheKey from "@/config/cacheKey";

export const messages = {
	"zh-CN": {
		lang: antDZhCN,
		...zh_CN,
	},
	"en-US": {
		lang: antDEnUS,
		...en_US,
	},
};

// 支持的语言
const supportedCultures = ["zh-CN", "en-US"];

/**
 * 得到默认使用的语言
 * @returns
 */
const getDefaultLang = () => {
	// 用户指定了默认语言时，使用用户指定的
	const userLang = tool.cache.get(cacheKey.APP_LANG);
	if (userLang) {
		return userLang;
	} else {
		// 用户未指定时，根据游览器选择:
		if (supportedCultures.includes(navigator.language)) {
			return navigator.language;
		} else {
			return sysConfig.LANG;
		}
	}
};

const i18n = createI18n({
	locale: getDefaultLang(),
	fallbackLocale: supportedCultures[0],
	globalInjection: true,
	messages,
});

/**
 * 非 vue 文件中使用这个方法
 * @param localeKey
 * @returns
 */
export const translate = (localeKey: string) => {
	const locale = i18n.global.locale;
	// 使用i18n的 te 方法来检查是否能够匹配到对应键值
	const hasKey = i18n.global.te(localeKey, locale);
	const translatedStr = i18n.global.t(localeKey);
	if (hasKey) {
		return translatedStr;
	}
	return localeKey;
};

export default i18n;
