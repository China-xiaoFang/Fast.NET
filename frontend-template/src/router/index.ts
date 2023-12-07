import { createRouter, createWebHistory } from "vue-router";
import { whiteRoutes } from "./modules/whiteRoute";
import { asyncRoutes } from "./modules/asyncRoute";

const constantRoutes = [...whiteRoutes, ...asyncRoutes];

const router = createRouter({
    history: createWebHistory(import.meta.env.VITE_PUBLIC_PATH),
    routes: constantRoutes,
});

export default router;
