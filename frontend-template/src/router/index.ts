import { createRouter, createWebHistory } from "vue-router";
import { asyncRoutes } from "./modules/asyncRoute";
import { whiteRoutes } from "./modules/whiteRoute";

const constantRoutes = [...asyncRoutes, ...whiteRoutes];

export default createRouter({
    history: createWebHistory(import.meta.env.VITE_PUBLIC_PATH),
    routes: constantRoutes,
});
