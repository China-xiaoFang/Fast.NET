import { get, post } from "@/utils/request";

export default {
	/**
	 * Web站点初始化
	 * @returns
	 */
	webSiteInit() {
		return get("/webSiteInit", null);
	},

	/**
	 * Web登录
	 * @param value
	 * @returns
	 */
	webLogin(value) {
		return post("/webLogin", value);
	},
};
