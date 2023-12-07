import { RouteRecordRaw } from "vue-router";

/**
 * 动态路由
 * 用来放置有权限的路由，但是在后端不进行返回的页面
 * 必须带有 Name 属性
 */
export const asyncRoutes: RouteRecordRaw[] = [];
