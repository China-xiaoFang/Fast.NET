import { createStore } from "vuex";

import webSiteInfo from "./modules/webSiteInfo";
import iframe from "./modules/iframe";
import keepAlive from "./modules/keepAlive";
import viewTags from "./modules/viewTags";
import search from "./modules/search";
import user from "./modules/user";

// 自动import导入所有 vuex 模块
export default createStore({
	state: {},
	getters: {},
	mutations: {},
	actions: {},
	modules: {
		webSiteInfo,
		iframe,
		keepAlive,
		viewTags,
		search,
		user,
	},
});
