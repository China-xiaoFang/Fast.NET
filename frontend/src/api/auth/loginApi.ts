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

	/**
	 * 测试Get请求参数加密
	 * @param value
	 * @returns
	 */
	testGet1(value) {
		return get("/testGet1", value);
	},

	/**
	 * 测试Get请求参数加密
	 * @param value
	 * @returns
	 */
	testGet2(value) {
		return get("/testGet2", value);
	},
};
