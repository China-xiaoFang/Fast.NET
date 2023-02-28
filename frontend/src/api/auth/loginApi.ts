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
	 * 测试Get请求参数加密
	 * @param value
	 * @returns
	 */
	testGet1(value) {
		return getRequest("/testGet1", value);
	},

	/**
	 * 测试Get请求参数加密
	 * @param value
	 * @returns
	 */
	testGet2(value) {
		return getRequest("/testGet2", value);
	},
};
