import { RouteRecordRaw } from "vue-router";

/** 白名单路由，免登录 */
export const whiteRoutes: RouteRecordRaw[] = [
    {
        path: "/403",
        component: () => import("@/views/common/403/index.vue"),
    },
    {
        path: "/404",
        component: () => import("@/views/common/404/index.vue"),
    },
    {
        path: "/empty",
        component: () => import("@/views/common/empty/index.vue"),
    },
    {
        path: "/login",
        component: () => import("@/views/login/index.vue"),
        name: "Login",
        meta: {
            title: "登录",
            authForbidView: true,
        },
    },
    {
        path: "/:path(.*)*",
        redirect: "/404",
    },
];
