import loginApi from "@/api/auth/loginApi";
import { Modal, message } from "ant-design-vue";
import store from "@/store";
import { translate as $t } from "@/locales";

export default {
	init(app) {
		const initLoading = message.loading($t("message.systemInit"), 0);

		// 根据Host判断是否为真实的租户
		loginApi
			.webSiteInit()
			.then((res) => {
				if (res.success) {
					// 缓存请求到的数据
					app.config.globalProperties.$tool.cache.set(
						app.config.globalProperties.$cacheKey.WEB_SITE_INFO,
						res.data
					);
					// 设置全局存储
					store.commit("setWebSiteInfo", res.data);
					// 设置站点图标和站点名称
					let faviconScript = document.createElement("link");
					faviconScript.rel = "icon";
					faviconScript.href = res.data.logoUrl;
					document.head.appendChild(faviconScript);
					document.title = res.data.chName;
					// 挂载app
					app.mount("#app");
					initLoading();
					message.success($t("message.authSuccess"));
				} else {
					initLoading();
					Modal.error({
						title: $t("message.hostError"),
						content: $t("message.webSiteError"),
					});
				}
			})
			.catch((err) => {
				// 删除缓存
				app.config.globalProperties.$tool.cache.remove(
					app.config.globalProperties.$cacheKey.WEB_SITE_INFO
				);
				initLoading();
				Modal.error({
					title: $t("message.hostError"),
					content: err.message,
					onOk() {
						app.config.globalProperties.$tool.toHtmlPage(
							app.config.globalProperties.$sysConfig
								.HTML_PAGE[404]
						);
					},
				});
			});
	},
};
