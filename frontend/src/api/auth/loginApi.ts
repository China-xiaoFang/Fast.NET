import { baseRequest } from "@/utils/request";

export default {
	/**
	 * Web 站点初始化
	 * @returns
	 */
	webSiteInit() {
		return baseRequest("/webSiteInit", null, "get");
	},
};
