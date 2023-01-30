/**
 * 系统路由
 */

const routes = [
	{
		path: "/index",
		component: () => import("@/views/index/index.vue"),
		meta: {
			title: "首页",
		},
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
