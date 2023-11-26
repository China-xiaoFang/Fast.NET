<template>
    <el-config-provider :locale="lang">
        <router-view></router-view>
    </el-config-provider>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";
import { useConfig } from "@/stores/config";

const config = useConfig();

// 初始化 element 的语言包
const { getLocaleMessage } = useI18n();
const lang = getLocaleMessage(config.lang.defaultLang) as any;
</script>
