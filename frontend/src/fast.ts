import { Modal, message, notification } from "ant-design-vue";
import sysConfig from "@/config/index";
import tool from "./utils/tool";
import cacheKey from "./config/cacheKey";
import errorHandler from "./utils/errorHandler";

export default {
	install(app) {
		// 挂载全局对象

		// 消息提示
		// app.config.globalProperties.$message = message;

		// 弹窗提示
		// app.config.globalProperties.$modal = Modal;

		// 通知
		// app.config.globalProperties.$notification = notification;

		// 系统配置
		app.config.globalProperties.$sysConfig = sysConfig;

		// 工具类
		app.config.globalProperties.$tool = tool;

		// 缓存Key
		app.config.globalProperties.$cacheKey = cacheKey;

		// 全局代码错误捕捉
		app.config.errorHandler = errorHandler;
	},
};
