import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";
import { notification } from "ant-design-vue";
import NProgress from "nprogress";
import "nprogress/nprogress.css";
import systemRouter from "./systemRouter";
import whiteListRouters from "./whiteList";
import { afterEach, beforeEach } from "./scrollBehavior";
import sysConfig from "@/config/index";
import cacheKey from "@/config/cacheKey";
import tool from "@/utils/tool";
import store from "@/store";

const modules = import.meta.glob("/src/views/**/**.vue");

// 进度条配置
NProgress.configure({ showSpinner: false, speed: 500 });

// 站点信息
const webSiteInfo = tool.cache.get(cacheKey.WEB_SITE_INFO);

// 系统路由
const routes = [...systemRouter, ...whiteListRouters];

// 系统特殊路由
const routes_404 = {
	path: "/:pathMatch(.*)*",
	hidden: true,
	component: () => import("@/layout/other/404.vue"),
};
let routes_404_r = () => {};

const router = createRouter({
	// 此方式不带 # 号 // createWebHashHistory()带#号
	history: createWebHistory(),
	routes,
});

// App语言
const appLang = tool.cache.get(cacheKey.APP_LANG)
	? tool.cache.get(cacheKey.APP_LANG)
	: sysConfig.LANG;

// 站点标题
const siteTile =
	appLang === "zh-CN" ? webSiteInfo?.chName : webSiteInfo?.enName;

// 设置标题
document.title = siteTile;

// 判断是否已加载过动态/静态路由
let isGetRouter = false;

// 白名单校验
const exportWhiteListFromRouter = (router) => {
	const res = [];
	for (const item of router) res.push(item.path);
	return res;
};
const whiteList = exportWhiteListFromRouter(whiteListRouters);

// 加载动态/静态路由
const handleGetRouter = (to) => {
	console.log("动态路由");
	if (!isGetRouter) {
		console.log("构建动态路由");
		store.dispatch("getLoginUser");
		store.dispatch("getLoginMenu").then((res) => {
			let menuRouter = filterAsyncRouter(res);
			menuRouter.forEach((item) => {
				router.addRoute("layout", item);
			});
			store.commit("search/init", menuRouter);
			routes_404_r = router.addRoute(routes_404);
			if (to && to.matched.length === 0) {
				router.push(to.fullPath);
			}
			isGetRouter = true;
		});
	}
};

router.beforeEach(async (to, from, next) => {
	NProgress.start();
	// 动态标题
	document.title = to.meta.title
		? `${to.meta.title} - ${siteTile}`
		: `${siteTile}`;

	// 过滤白名单
	if (whiteList.includes(to.path)) {
		next();
		return false;
	}

	const token = tool.cache.get(cacheKey.TOKEN);
	if (to.path === "/login") {
		// 当用户输入了login路由，将其跳转首页即可
		if (token) {
			next({
				path: "/",
			});
			return false;
		}
		// 删除路由(替换当前layout路由)
		router.addRoute(routes[0]);
		// 删除路由(404)
		routes_404_r();
		isGetRouter = false;
		next();
		return false;
	}

	if (!token) {
		next({
			path: "/login",
		});
		return false;
	}
	// 整页路由处理
	if (to.meta.fullPage) {
		to.matched = [to.matched[to.matched.length - 1]];
	}
	// 加载动态/静态路由
	handleGetRouter(to);
	beforeEach(to, from);
	next();
});

router.afterEach((to, from) => {
	afterEach(to);
	NProgress.done();
});

router.onError((error) => {
	NProgress.done();
	notification.error({
		message: "路由错误",
		description: error.message,
	});
});

// 转换
const filterAsyncRouter = (routerMap) => {
	const accessedRouters = [];
	routerMap.forEach((item) => {
		item.meta = item.meta ? item.meta : {};
		// 处理外部链接特殊路由
		if (item.meta.type === "iframe") {
			item.meta.url = item.path;
			item.path = `/i/${item.name}`;
		}
		// MAP转路由对象
		const route = {
			path: item.path,
			name: item.name,
			meta: item.meta,
			redirect: item.redirect,
			children: item.children ? filterAsyncRouter(item.children) : null,
			component: loadComponent(item.component),
		};
		accessedRouters.push(route);
	});
	return accessedRouters;
};

const loadComponent = (component) => {
	if (component) {
		if (component.includes("/")) {
			return modules[`/src/views/${component}.vue`];
		}
		return modules[`/src/views/${component}/index.vue`];
	} else {
		return () => import(/* @vite-ignore */ `/src/layout/other/empty.vue`);
	}
};

// 路由扁平化
const flatAsyncRoutes = (routes, breadcrumb = []) => {
	const res = [];
	routes.forEach((route) => {
		const tmp = { ...route };
		if (tmp.children) {
			const childrenBreadcrumb = [...breadcrumb];
			childrenBreadcrumb.push(route);
			const tmpRoute = { ...route };
			tmpRoute.meta.breadcrumb = childrenBreadcrumb;
			delete tmpRoute.children;
			res.push(tmpRoute);
			const childrenRoutes = flatAsyncRoutes(
				tmp.children,
				childrenBreadcrumb
			);
			childrenRoutes.map((item) => {
				res.push(item);
			});
		} else {
			const tmpBreadcrumb = [...breadcrumb];
			tmpBreadcrumb.push(tmp);
			tmp.meta.breadcrumb = tmpBreadcrumb;
			res.push(tmp);
		}
	});
	return res;
};

export default router;
