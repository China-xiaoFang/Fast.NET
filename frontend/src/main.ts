import { createApp } from "vue";
import Antd from "ant-design-vue";
import "./style/index.less";
import fast from "./fast";
import router from "./router";
import store from "./store";
import App from "./App.vue";
import "./tailwind.css";

const app = createApp(App);
app.use(store);
app.use(router);
app.use(Antd);
app.use(fast);

import loginApi from "@/api/auth/loginApi";
import { Modal, message } from "ant-design-vue";
import tool from "@/utils/tool";

const initLoading = message.loading("系统初始化中，请耐心等待", 0);

// 根据Host判断是否为真实的租户
loginApi.webSiteInit().then((res) => {
	if (res.success) {
		// 缓存请求到的数据
		tool.cache.set("WebSiteInfo", res.data);
		// 设置站点图标和站点名称
		let faviconScript = document.createElement("link");
		faviconScript.rel = "icon";
		faviconScript.href = res.data.logoUrl;
		document.head.appendChild(faviconScript);
		document.title = res.data.chName;
		// 挂载app
		app.mount("#app");
		initLoading();
	} else {
		initLoading();
		Modal.error({
			title: "不知名的主机访问！",
			content: "未知的站点，请确认访问的站点是否正确！",
		});
	}
});
