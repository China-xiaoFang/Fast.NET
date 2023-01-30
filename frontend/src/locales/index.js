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

const i18n = createI18n({
	locale: tool.cache.get(cacheKey.APP_LANG) || sysConfig.LANG,
	fallbackLocale: "zh-cn",
	globalInjection: true,
	messages,
});

export default i18n;
