/**
 * Web站点信息
 */
interface WebSiteInfo {
	chName: string;
	chShortName: string;
	enName: string;
	enShortName: string;
	logoUrl: string;
	tenantId: Number;
}

import { base64ToStr } from "@/utils/base64";

export default {
	state: {
		chName: "",
		chShortName: "",
		enName: "",
		enShortName: "",
		logoUrl: "",
		base64TenantId: "",
		tenantId: null,
	},
	mutations: {
		/**
		 * 设置Web站点信息
		 * @param state
		 * @param info
		 */
		setWebSiteInfo(state, info: WebSiteInfo) {
			state.chName = info.chName;
			state.chShortName = info.chShortName;
			state.enName = info.enName;
			state.enShortName = info.enShortName;
			state.logoUrl = info.logoUrl;
			state.base64TenantId = info.tenantId;
			// 转换租户Id
			state.tenantId = Number(base64ToStr(info.tenantId));
		},
		removeWebSiteInfo(state) {
			state.chName = "";
			state.chShortName = "";
			state.enName = "";
			state.enShortName = "";
			state.logoUrl = "";
			state.base64TenantId = "";
			state.tenantId = null;
		},
	},
};
