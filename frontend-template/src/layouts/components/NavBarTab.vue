<template>
    <div class="nav-tabs" ref="tabScrollbarRef">
        <div
            v-for="(item, idx) in navTabsStore.state.tabsView"
            @click="onClickTabHandle(item)"
            @contextmenu.prevent="onContextMenuHandle(item, $event)"
            class="fast-nav-tab"
            :class="navTabsStore.state.activeIndex == idx ? 'active' : ''"
            :ref="tabsRefs.set"
            :key="idx"
        >
            {{ item.meta.title }}
            <transition @after-leave="selectNavTabHandle(tabsRefs[navTabsStore.state.activeIndex])" name="el-fade-in">
                <FIcon
                    v-show="navTabsStore.state.tabsView.length > 1"
                    class="close-icon"
                    @click.stop="onClickCloseTabHandle(item)"
                    size="15"
                    name="el-icon-Close"
                />
            </transition>
        </div>
        <div :style="activeBoxStyle" class="nav-tabs-active-box"></div>
    </div>
    <FContextMenu ref="fContextMenuRef" :items="state.contextMenuItems" @onClick="onClickContextMenuItemHandle" />
</template>

<script setup lang="ts">
import { nextTick, onMounted, reactive, ref } from "vue";
import { useRoute, useRouter, onBeforeRouteUpdate, type RouteLocationNormalized } from "vue-router";
import { useConfig } from "@/stores/config";
import { useNavTabs } from "@/stores/navTabs";
import { useTemplateRefsList } from "@vueuse/core";
import type { ContextMenuItem, ContextMenuItemClickEmitArg } from "@/components/FContextMenu/interface";
import useCurrentInstance from "@/hooks/useCurrentInstance";

const route = useRoute();
const router = useRouter();
const configStore = useConfig();
const navTabsStore = useNavTabs();

const { proxy } = useCurrentInstance();
const tabScrollbarRef = ref();
const tabsRefs = useTemplateRefsList<HTMLDivElement>();

const fContextMenuRef = ref();

const state: {
    contextMenuItems: ContextMenuItem[];
} = reactive({
    contextMenuItems: [
        { name: "refresh", label: "重新加载", icon: "fa fa-refresh" },
        { name: "close", label: "关闭标签", icon: "fa fa-times" },
        { name: "fullScreen", label: "当前标签全屏", icon: "el-icon-FullScreen" },
        { name: "closeOther", label: "关闭其他标签", icon: "fa fa-minus" },
        { name: "closeAll", label: "关闭全部标签", icon: "fa fa-stop" },
    ],
});

const activeBoxStyle = reactive({
    width: "0",
    transform: "translateX(0px)",
});

/**
 * Tab 点击
 * @param menu
 */
const onClickTabHandle = (menu: RouteLocationNormalized) => {
    router.push(menu);
};

/**
 * Tab 右键点击
 * @param menu
 * @param el
 */
const onContextMenuHandle = (menu: RouteLocationNormalized, el: MouseEvent) => {
    // 禁用刷新
    state.contextMenuItems[0].disabled = route.path !== menu.path;
    // 禁用关闭其他和关闭全部
    state.contextMenuItems[4].disabled = state.contextMenuItems[3].disabled = navTabsStore.state.tabsView.length == 1 ? true : false;

    const { clientX, clientY } = el;
    fContextMenuRef.value.onShowContextmenu(menu, {
        x: clientX,
        y: clientY,
    });
};

/**
 * Tab 激活状态切换
 * @param dom
 */
const selectNavTabHandle = (dom: HTMLDivElement) => {
    if (!dom) {
        return false;
    }
    activeBoxStyle.width = dom.clientWidth + "px";
    activeBoxStyle.transform = `translateX(${dom.offsetLeft}px)`;

    let scrollLeft = dom.offsetLeft + dom.clientWidth - tabScrollbarRef.value.clientWidth;
    if (dom.offsetLeft < tabScrollbarRef.value.scrollLeft) {
        tabScrollbarRef.value.scrollTo(dom.offsetLeft, 0);
    } else if (scrollLeft > tabScrollbarRef.value.scrollLeft) {
        tabScrollbarRef.value.scrollTo(scrollLeft, 0);
    }
};

/**
 * 跳转到最后一个 Tab 页
 */
const toLastTab = () => {
    const lastTab = navTabsStore.state.tabsView.slice(-1)[0];
    if (lastTab) {
        router.push(lastTab);
    } else {
        router.push("/");
    }
};

/**
 * 关闭 Tab
 * @param route
 */
const onClickCloseTabHandle = (route: RouteLocationNormalized) => {
    navTabsStore.closeTab(route);
    proxy.eventBus.emit("onTabViewClose", route);
    if (navTabsStore.state.activeRoute?.path === route.path) {
        toLastTab();
    } else {
        navTabsStore.setActiveRoute(navTabsStore.state.activeRoute!);
        nextTick(() => {
            selectNavTabHandle(tabsRefs.value[navTabsStore.state.activeIndex]);
        });
    }

    fContextMenuRef.value.onHideContextmenu();
};

/**
 * 关闭其他 Tab
 * @param route
 */
const closeOtherTab = (route: RouteLocationNormalized) => {
    navTabsStore.closeTabs(route);
    navTabsStore.setActiveRoute(route);
    if (navTabsStore.state.activeRoute?.path !== route.path) {
        router.push(route!.path);
    }
};

/**
 * 关闭全部 Tab
 * @param route
 */
const closeAllTab = (route: RouteLocationNormalized) => {
    let firstRoute = getFirstRoute(navTabsStore.state.tabsViewRoutes);
    if (firstRoute && firstRoute.path == route.path) {
        return closeOtherTab(route);
    }
    if (firstRoute && firstRoute.path == navTabsStore.state.activeRoute?.path) {
        return closeOtherTab(navTabsStore.state.activeRoute);
    }
    navTabsStore.closeTabs(false);
    if (firstRoute) routePush(firstRoute.path);
};

const onContextmenuItem = async (item: ContextMenuItemClickEmitArg) => {
    const { name, menu } = item;
    if (!menu) return;
    switch (name) {
        case "refresh":
            proxy.eventBus.emit("onTabViewRefresh", menu);
            break;
        case "close":
            onClickCloseTabHandle(menu);
            break;
        case "closeOther":
            closeOtherTab(menu);
            break;
        case "closeAll":
            closeAllTab(menu);
            break;
        case "fullScreen":
            if (route.path !== menu?.path) {
                router.push(menu?.path as string);
            }
            navTabsStore.setFullScreen(true);
            break;
    }
};

const updateTab = function (newRoute: RouteLocationNormalized) {
    // 添加tab
    navTabsStore.addTab(newRoute);
    // 激活当前tab
    navTabsStore.setActiveRoute(newRoute);

    nextTick(() => {
        selectNavTabHandle(tabsRefs.value[navTabsStore.state.activeIndex]);
    });
};

onBeforeRouteUpdate(async (to) => {
    updateTab(to);
});

onMounted(() => {
    updateTab(router.currentRoute.value);
    new horizontalScroll(tabScrollbarRef.value);
});
</script>

<style scoped lang="scss">
.dark {
    .close-icon {
        color: v-bind('configStore.getColorVal("headerBarTabColor")') !important;
    }
    .fast-nav-tab.active {
        .close-icon {
            color: v-bind('configStore.getColorVal("headerBarTabActiveColor")') !important;
        }
    }
}
.nav-tabs {
    overflow-x: auto;
    overflow-y: hidden;
    margin-right: var(--ba-main-space);
    scrollbar-width: none;

    &::-webkit-scrollbar {
        height: 5px;
    }
    &::-webkit-scrollbar-thumb {
        background: #eaeaea;
        border-radius: var(--el-border-radius-base);
        box-shadow: none;
        -webkit-box-shadow: none;
    }
    &::-webkit-scrollbar-track {
        background: v-bind('configStore.layout.layoutMode == "Default" ? "none":configStore.getColorVal("headerBarBackground")');
    }
    &:hover {
        &::-webkit-scrollbar-thumb:hover {
            background: #c8c9cc;
        }
    }
}
.fast-nav-tab {
    white-space: nowrap;
    height: 40px;
}
</style>
