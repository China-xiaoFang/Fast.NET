<template>
    <component :is="configStore.layout.layoutMode"></component>
</template>

<script setup lang="ts" name="layout">
import { reactive } from "vue";
import { useConfig } from "@/stores/config";
// import { useNavTabs } from "@/stores/navTabs";
// import { useSiteConfig } from "@/stores/siteConfig";
import { useUserInfo } from "@/stores/userInfo";
// import { useRoute } from "vue-router";
import Default from "@/layouts/container/Default/index.vue";
import Classic from "@/layouts/container/Classic/index.vue";
import { onMounted, onBeforeMount } from "vue";
import { Session } from "@/utils/storage";
// import { handleAdminRoute, getFirstRoute, routePush } from "@/router/utils";
import router from "@/router";
import { useEventListener } from "@vueuse/core";
import { CACHE_BEFORE_RESIZE_LAYOUT } from "@/stores/constant";
// import { isEmpty } from "lodash-es";
import { setNavTabsWidth } from "@/layouts/utils";

defineOptions({
    components: { Default, Classic },
});

// const navTabsStore = useNavTabs();
const configStore = useConfig();
// const route = useRoute();
// const siteConfigStore = useSiteConfig();
const userInfoStore = useUserInfo();

const state = reactive({
    autoMenuCollapseLock: false,
});

onMounted(() => {
    // if (!userInfoStore.token) return router.push({ name: "adminLogin" });

    // 初始化，这里可以加载动态路由，和用户信息
    // init();
    setNavTabsWidth();
    useEventListener(window, "resize", setNavTabsWidth);
});
onBeforeMount(() => {
    onAdaptiveLayout();
    useEventListener(window, "resize", onAdaptiveLayout);
});

const onAdaptiveLayout = () => {
    let defaultBeforeResizeLayout = {
        layoutMode: configStore.layout.layoutMode,
        menuCollapse: configStore.layout.menuCollapse,
    };
    let beforeResizeLayout = Session.get<any>(CACHE_BEFORE_RESIZE_LAYOUT);
    if (!beforeResizeLayout) Session.set(CACHE_BEFORE_RESIZE_LAYOUT, defaultBeforeResizeLayout);

    const clientWidth = document.body.clientWidth;
    if (clientWidth < 1024) {
        /**
         * 锁定窗口改变自动调整 menuCollapse
         * 避免已是小窗且打开了菜单栏时，意外的自动关闭菜单栏
         */
        if (!state.autoMenuCollapseLock) {
            state.autoMenuCollapseLock = true;
            configStore.setLayout("menuCollapse", true);
        }
        configStore.setLayout("shrink", true);
        configStore.setLayoutMode("Classic");
    } else {
        state.autoMenuCollapseLock = false;
        let beforeResizeLayoutTemp = beforeResizeLayout || defaultBeforeResizeLayout;

        configStore.setLayout("menuCollapse", beforeResizeLayoutTemp.menuCollapse);
        configStore.setLayout("shrink", false);
        configStore.setLayoutMode(beforeResizeLayoutTemp.layoutMode);
    }
};
</script>
