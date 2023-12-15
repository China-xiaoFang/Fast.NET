<template>
    <el-header v-if="!navTabsStore.state.tabFullScreen" class="layout-header">
        <component :is="configStore.layout.layoutMode + 'NavBar'"></component>
    </el-header>
</template>
<script setup lang="ts" name="layoutHeader">
import { useConfig } from "@/stores/config";
import { useNavTabs } from "@/stores/navTabs";
import DefaultNavBar from "@/layouts/components/Default/NavBar.vue";
import ClassicNavBar from "@/layouts/components/Classic/NavBar.vue";

defineOptions({
    components: { DefaultNavBar, ClassicNavBar },
});

const configStore = useConfig();
const navTabsStore = useNavTabs();
</script>

<style scoped lang="scss">
.layout-header {
    height: auto;
    padding: 0;
}
</style>
