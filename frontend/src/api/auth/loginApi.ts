import { get } from "@/utils/request";

export default {
	/**
	 * Web 站点初始化
	 * @returns
	 */
	webSiteInit() {
		return get("/webSiteInit", null);
	},
};
