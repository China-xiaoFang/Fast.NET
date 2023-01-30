import { createStore } from "vuex";

import webSiteInfo from "./modules/webSiteInfo";

// 自动import导入所有 vuex 模块
export default createStore({
	state: {},
	getters: {},
	mutations: {},
	actions: {},
	modules: {
		webSiteInfo,
	},
});
