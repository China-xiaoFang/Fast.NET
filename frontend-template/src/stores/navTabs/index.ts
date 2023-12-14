import { defineStore } from "pinia";
import { reactive } from "vue";
import { STORE_NAV_TABS } from "@/stores/constant";
import type { NavTabs } from "./interface";

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
         * 设置全屏
         * @param fullScreen
         */
        const setFullScreen = (fullScreen: boolean): void => {
            state.tabFullScreen = fullScreen;
        };

        return { state, setFullScreen };
    },
    {
        persist: {
            key: STORE_NAV_TABS,
            // 这里是配置 pinia 只需要持久化 tabFullScreen即可，而不是整个 store
            paths: ["state.tabFullScreen"],
        },
    }
);
