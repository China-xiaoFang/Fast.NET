<template>
    <el-config-provider :locale="lang">
        <h1>Hello World</h1>
        <router-view></router-view>
    </el-config-provider>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

// 初始化 element 的语言包
const { getLocaleMessage } = useI18n();
const lang = getLocaleMessage("zh-cn") as any;
</script>
