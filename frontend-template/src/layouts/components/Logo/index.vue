<template>
    <div class="layout-logo">
        <img v-if="!configStore.layout.menuCollapse" class="logo-img" src="~assets/logo.png" alt="logo" />
        <div v-if="!configStore.layout.menuCollapse" :style="{ color: configStore.getColorVal('menuActiveColor') }" class="website-name">
            {{ siteConfigStore.state.siteName }}
        </div>
    </div>
</template>

<script setup lang="ts" name="LayoutLogo">
import { useConfig } from "@/stores/config";
import { useSiteConfig } from "@/stores/siteConfig";

const configStore = useConfig();
const siteConfigStore = useSiteConfig();
</script>

<style scoped lang="scss">
.layout-logo {
    width: 100%;
    height: 50px;
    display: flex;
    align-items: center;
    justify-content: center;
    box-sizing: border-box;
    padding: 10px;
    background: v-bind('configStore.layout.layoutMode != "Streamline" ?  configStore.getColorVal("menuTopBarBackground"):"transparent"');
}
.logo-img {
    width: 28px;
}
.website-name {
    display: block;
    width: 180px;
    padding-left: 4px;
    font-size: var(--el-font-size-extra-large);
    font-weight: 600;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
}
</style>
