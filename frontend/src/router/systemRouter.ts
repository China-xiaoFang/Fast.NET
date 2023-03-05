import sysConfig from "@/config/index";
import cacheKey from "@/config/cacheKey";
import tool from "@/utils/tool";

/**
 * 系统路由
 */

const menuList = tool.cache.get(cacheKey.USER_INFO)?.menuList;

const routes = [
	{
		name: "layout",
		path: "/",
		component: () => import("@/layout/index.vue"),
		redirect: menuList
			? menuList[0].children[0].path
			: sysConfig.DASHBOARD_URL,
		children: [],
	},
	{
		path: "/login",
		component: () => import("@/views/auth/login/index.vue"),
		meta: {
			title: "登录",
		},
	},
];

export default routes;
