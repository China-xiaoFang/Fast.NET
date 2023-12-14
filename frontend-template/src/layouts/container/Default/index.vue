<template>
    <el-container class="layout-container">
        <Aside />
        <el-container class="content-wrapper">
            <Header />
            <Main />
        </el-container>
    </el-container>
    <CloseFullScreen v-if="navTabsStore.state.tabFullScreen" />
</template>

<script setup lang="ts">
import Aside from "@/layouts/components/Aside/index.vue";
import Header from "@/layouts/components/Header/index.vue";
import Main from "@/layouts/components/Main/index.vue";
import CloseFullScreen from "@/layouts/components/CloseFullScreen/index.vue";
import { useNavTabs } from "@/stores/navTabs";

const navTabsStore = useNavTabs();
</script>

<style scoped>
.layout-container {
    height: 100%;
    width: 100%;
}
.content-wrapper {
    flex-direction: column;
    width: 100%;
    height: 100%;
}
</style>
