import { RouteRecordRaw } from "vue-router";

/**
 * 动态路由
 * 用来放置有权限的路由，但是在后端不进行返回的页面
 * 必须带有 Name 属性
 */
export const asyncRoutes: RouteRecordRaw[] = [
    {
        path: "/",
        name: "layout",
        component: () => import("@/layouts/index.vue"),
        redirect: "/dashboard",
        children: [
            {
                path: "/dashboard",
                component: () => import("@/views/dashboard/index.vue"),
                name: "Dashboard",
                meta: {
                    title: "首页",
                    affix: true,
                    keepAlive: true,
                },
            },
        ],
    },
];
