import { RouteRecordRaw } from "vue-router";

const Layout = () => import("@/layouts/index.vue");

/** 白名单路由，免登录 */
export const whiteRoutes: RouteRecordRaw[] = [
    {
        path: "/",
        name: "layout",
        component: Layout,
        redirect: "/",
        children: [
            // {
            //     path: "/login",
            //     component: () => import("@/views/login/index.vue"),
            //     name: "Login",
            //     meta: {
            //         title: "登录",
            //     },
            // },
        ],
    },
    {
        path: "/login",
        component: () => import("@/views/login/index.vue"),
        name: "Login",
        meta: {
            title: "登录",
        },
    },
    {
        path: "/:path(.*)*",
        redirect: "/404",
    },
];
