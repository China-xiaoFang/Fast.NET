import { createRouter, createWebHistory, RouteRecordRaw } from "vue-router";

import systemRouter from "./systemRouter";

// 系统路由
const routes = [...systemRouter];

const router = createRouter({
	// 此方式不带 # 号 // createWebHashHistory()带#号
	history: createWebHistory(),
	routes,
});

export default router;
