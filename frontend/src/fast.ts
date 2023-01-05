import VueStorage from "vue-ls";
import tool from "./utils/tool";
import errorHandler from "./utils/errorHandler";

export default {
	install(app) {
		// 挂载全局对象
		app.config.globalProperties.$TOOL = tool;

		// 全局代码错误捕捉
		app.config.errorHandler = errorHandler;
	},
};
