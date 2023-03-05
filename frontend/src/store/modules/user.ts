import loginApi from "@/api/auth/loginApi";
import cacheKey from "@/config/cacheKey";
import tool from "@/utils/tool";

export default {
	state: {
		// 登录用户信息
		loginUser: {},
		// 菜单信息
		menuList: [],
		// 按钮编码
		buttonCodeList: [],
	},
	mutations: {
		setLoginUser(state, info) {
			state.loginUser = info;
			state.menuList = info.menuList;
			state.buttonCodeList = info.buttonCodeList;
		},
	},
	actions: {
		getLoginUser({ commit }) {
			return new Promise((resolve, reject) => {
				loginApi
					.getLoginUser()
					.then((res) => {
						if (res.success) {
							tool.cache.set(cacheKey.USER_INFO, res.data);
							tool.cache.set(cacheKey.MENU, res.data.menuList);
							tool.cache.set(
								cacheKey.PERMISSIONS,
								res.data.buttonCodeList
							);
							commit("setLoginUser", res.data);
							resolve(res.data);
						} else {
							reject(new Error(res.message));
						}
					})
					.catch((err) => {
						reject(err);
					});
			});
		},
	},
};
