<template>
    <div class="nav-bar"></div>
</template>

<script setup lang="ts" name="layoutDefaultNavBar">
import { useConfig } from "@/stores/config";

const configStore = useConfig();
</script>

<style lang="scss" scoped>
.nav-bar {
    display: flex;
    height: 50px;
    margin: 20px var(--fast-main-space) 0 var(--fast-main-space);
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
            user-select: none;
            opacity: 0.7;
            color: v-bind('configStore.getColorVal("headerBarTabColor")');
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
                opacity: 1;
            }
        }
        .nav-tabs-active-box {
            position: absolute;
            height: 40px;
            border-radius: var(--el-border-radius-base);
            background-color: v-bind('configStore.getColorVal("headerBarTabActiveBackground")');
            box-shadow: var(--el-box-shadow-light);
            transition: all 0.2s;
            -webkit-transition: all 0.2s;
        }
    }
}
</style>
