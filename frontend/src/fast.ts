import * as antDVIcons from "@ant-design/icons-vue";
import { Modal, message, notification } from "ant-design-vue";
import sysConfig from "@/config/index";
import tool from "./utils/tool";
import cacheKey from "./config/cacheKey";
import errorHandler from "./utils/errorHandler";
import customIcons from "./assets/icons/index";
import "highlight.js/styles/atom-one-dark.css";
import hljsCommon from "highlight.js/lib/common";
import hljsVuePlugin from "@highlightjs/vue-plugin";
import STable from "./components/Table/index.vue";
import Ellipsis from "./components/Ellipsis/index.vue";

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

		// 注册常用组件
		app.component("STable", STable);
		app.component("Ellipsis", Ellipsis);

		// 统一注册antDV图标
		for (const icon in antDVIcons) {
			app.component(icon, antDVIcons[icon]);
		}
		// 统一注册自定义全局图标
		app.use(customIcons);

		// 注册代码高亮组件 （博客：https://blog.csdn.net/weixin_41897680/article/details/124925222）
		// 注意：解决Vue使用highlight.js build打包发布后样式消失问题，原因是webpack在打包的时候没有把未被使用的代码打包进去，因此，在此处引用一下，看似无意义实则有用
		hljsCommon.highlightAuto(
			"<h1>Highlight.js has been registered successfully!</h1>"
		).value;
		app.use(hljsVuePlugin);

		// 全局代码错误捕捉
		app.config.errorHandler = errorHandler;
	},
};
