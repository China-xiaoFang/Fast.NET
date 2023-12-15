import { defineStore } from "pinia";
import { reactive } from "vue";
import { i18n } from "@/lang";
import { STORE_NAV_TABS } from "@/stores/constant";
import type { NavTabs } from "./interface";
import type { RouteLocationNormalized, RouteRecordRaw } from "vue-router";
import { encodeRoutesURI } from "@/router/utils";
import { isEmpty } from "lodash-es";

export const useNavTabs = defineStore(
    "navTabs",
    () => {
        const state: NavTabs = reactive({
            // 激活tab的index
            activeIndex: 0,
            // 激活的tab
            activeRoute: null,
            // tab列表
            tabsView: [],
            // 当前tab是否全屏
            tabFullScreen: false,
            // 从后台加载到的菜单路由列表
            tabsViewRoutes: [],
            // 按钮权限节点
            authNode: new Map(),
        });

        /**
         * 添加 Tab
         * @param route 路由
         * @returns
         */
        const addTab = (route: RouteLocationNormalized): void => {
            if (!route.meta.addTab) return;
            for (const key in state.tabsView) {
                if (state.tabsView[key].path === route.path) {
                    state.tabsView[key].params = !isEmpty(route.params) ? route.params : state.tabsView[key].params;
                    state.tabsView[key].query = !isEmpty(route.query) ? route.query : state.tabsView[key].query;
                }
            }

            if (typeof route.meta.title == "string") {
                // 这里使用别名，避免被国际化工具匹配到
                const { t: translateTitle } = i18n.global;
                // 这里默认直接使用 pagesTitle. 中的翻译内容
                route.meta.title = translateTitle(route.meta.title);
            }

            state.tabsView.push(route);
        };

        /**
         * 关闭 Tab
         * @param route
         */
        const closeTab = (route: RouteLocationNormalized): void => {
            const index = state.tabsView.findIndex((f) => f.path === route.path);
            if (index != 1) {
                state.tabsView.splice(index, 1);
            }
        };

        /**
         * 关闭多个 Tab
         * @param retainRoute 保留的路由，否则关闭全部标签
         */
        const closeTabs = (retainRoute: RouteLocationNormalized | false = false): void => {
            if (retainRoute) {
                state.tabsView = [retainRoute];
            } else {
                state.tabsView = [];
            }
        };

        /**
         * 设置活动路由
         * @param route
         * @returns
         */
        const setActiveRoute = (route: RouteLocationNormalized): void => {
            const currentRouteIndex = state.tabsView.findIndex((f) => f.path === route.path);
            if (currentRouteIndex === -1) return;
            state.activeRoute = route;
            state.activeIndex = currentRouteIndex;
        };

        /**
         * 设置路由列表
         * @param data
         */
        const setTabsViewRoutes = (data: RouteRecordRaw[]): void => {
            state.tabsViewRoutes = encodeRoutesURI(data);
        };

        /**
         * 设置全屏
         * @param fullScreen
         */
        const setFullScreen = (fullScreen: boolean): void => {
            state.tabFullScreen = fullScreen;
        };

        return { state, addTab, closeTab, closeTabs, setActiveRoute, setTabsViewRoutes, setFullScreen };
    },
    {
        persist: {
            key: STORE_NAV_TABS,
            // 这里是配置 pinia 只需要持久化 tabFullScreen即可，而不是整个 store
            paths: ["state.tabFullScreen"],
        },
    }
);
