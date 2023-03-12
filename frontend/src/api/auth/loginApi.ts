import { getRequest, postRequest } from "@/utils/request";

export default {
	/**
	 * Web站点初始化
	 * @returns
	 */
	webSiteInit() {
		return getRequest("/webSiteInit", null);
	},

	/**
	 * Web登录
	 * @param value
	 * @returns
	 */
	webLogin(value) {
		return postRequest("/webLogin", value);
	},

	/**
	 * 获取登录用户信息
	 * @returns
	 */
	getLoginUser() {
		return getRequest("/getLoginUser");
	},

	/**
	 * 获取登录用户菜单
	 * @returns
	 */
	getLoginMenu() {
		return getRequest("/getLoginMenu");
	},
};
