import { createI18n } from "vue-i18n";
import zhCN from "ant-design-vue/es/locale/zh_CN";
import enGB from "ant-design-vue/es/locale/en_GB";
import zh_cn from "./lang/zh-cn.js";
import en from "./lang/en.js";
import tool from "@/utils/tool";
import sysConfig from "@/config/index";
import cacheKey from "@/config/cacheKey";

export const messages = {
	"zh-cn": {
		lang: zhCN,
		...zh_cn,
	},
	en: {
		lang: enGB,
		...en,
	},
};

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
		if (navigator.language === "zh-CN") {
			return "zh-cn";
		} else if (navigator.language === "en") {
			return "en";
		} else {
			return sysConfig.LANG;
		}
	}
};

const i18n = createI18n({
	locale: getDefaultLang(),
	fallbackLocale: "zh-cn",
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
