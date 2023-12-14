<template>
    <el-config-provider :locale="lang">
        <router-view></router-view>
    </el-config-provider>
</template>

<script setup lang="ts" name="App">
import { useI18n } from "vue-i18n";
import { useConfig } from "@/stores/config";

const configStore = useConfig();

// 初始化 element 的语言包
const { getLocaleMessage } = useI18n();
const lang = getLocaleMessage(configStore.lang.defaultLang) as any;
</script>
