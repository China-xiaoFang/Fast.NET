<template>
    <div class="nav-bar">
        <div v-if="configStore.layout.shrink && configStore.layout.menuCollapse" class="unfold">
            <FIcon @click="onMenuCollapse" name="fa fa-indent" :color="configStore.getColorVal('menuActiveColor')" size="18" />
        </div>
    </div>
</template>

<script setup lang="ts" name="layoutClassicNavBar">
import { useConfig } from "@/stores/config";
import { showShade } from "@/hooks/pageShade";

const configStore = useConfig();

const onMenuCollapse = () => {
    showShade("ba-aside-menu-shade", () => {
        configStore.setLayout("menuCollapse", true);
    });
    configStore.setLayout("menuCollapse", false);
};
</script>

<style lang="scss" scoped>
.nav-bar {
    display: flex;
    height: 50px;
    width: 100%;
    background-color: v-bind('configStore.getColorVal("headerBarBackground")');
    :deep(.nav-tabs) {
        display: flex;
        height: 100%;
        position: relative;
        .ba-nav-tab {
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 0 20px;
            cursor: pointer;
            z-index: 1;
            height: 100%;
            user-select: none;
            color: v-bind('configStore.getColorVal("headerBarTabColor")');
            transition: all 0.2s;
            -webkit-transition: all 0.2s;
            .close-icon {
                padding: 2px;
                margin: 2px 0 0 4px;
            }
            .close-icon:hover {
                background: var(--fast-color-primary-light);
                color: var(--el-border-color) !important;
                border-radius: 50%;
            }
            &.active {
                color: v-bind('configStore.getColorVal("headerBarTabActiveColor")');
            }
            &:hover {
                background-color: v-bind('configStore.getColorVal("headerBarHoverBackground")');
            }
        }
        .nav-tabs-active-box {
            position: absolute;
            height: 50px;
            background-color: v-bind('configStore.getColorVal("headerBarTabActiveBackground")');
            transition: all 0.2s;
            -webkit-transition: all 0.2s;
        }
    }
}
.unfold {
    align-self: center;
    padding-left: var(--fast-main-space);
}
</style>
