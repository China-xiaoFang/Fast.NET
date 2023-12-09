import { createRouter, createWebHistory } from "vue-router";
import NProgress from "nprogress";
import "nprogress/nprogress.css";
import { whiteRoutes } from "./modules/whiteRoute";
import { asyncRoutes } from "./modules/asyncRoute";
// import { loading } from "@/plugins/loading";
// import { useConfig } from "@/stores/config";
// import { useUserInfo } from "@/stores/userInfo";

const constantRoutes = [...whiteRoutes, ...asyncRoutes];

const router = createRouter({
    history: createWebHistory(import.meta.env.VITE_PUBLIC_PATH),
    routes: constantRoutes,
});

/**
 * 配置 NProgress
 */
NProgress.configure({
    // 动画方式
    easing: "ease",
    // 递增进度条的速度
    speed: 500,
    // 是否显示加载ico
    showSpinner: false,
    // 自动递增间隔
    trickleSpeed: 200,
    // 初始化时的最小百分比
    minimum: 0.3,
});

// router.beforeEach((to, from, next) => {
//     // 开启进度条
//     NProgress.start();
//     // to.matched 是一个包含当前路由和所有嵌套路径片段的数组
//     const matchedRoutes = to.matched.map((route) => route.path);

//     debugger;

//     // 判断是否已经存在加载动画
//     if (!window.existLoading) {
//         // 显示加载动画
//         loading.show();
//         window.existLoading = true;
//     }

//     // 按需动态加载页面的语言包
//     let loadPath: string[] = [];
//     const config = useConfig();

//     if (to.path) {
//     }
// });

export default router;
