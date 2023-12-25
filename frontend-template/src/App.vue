<template>
    <el-config-provider :locale="lang">
        <router-view></router-view>
    </el-config-provider>
</template>

<script setup lang="ts" name="App">
import { computed, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useConfig } from "@/stores/config";
import { editDefaultLang } from "@/lang";

const configStore = useConfig();

// 初始化 element 的语言包
const { getLocaleMessage } = useI18n();
const lang = getLocaleMessage(configStore.lang.defaultLang) as any;

// 浏览器语言
const browserLang = computed(() => {
    return navigator.language;
});

/**
 * 监听浏览器语言的变化，更改显示语言
 */
watch(browserLang, (newValue, oldValue) => {
    console.log(`检测到浏览器语言变更：${oldValue} => ${newValue}`);
    // 变更语言
    editDefaultLang(newValue);
});
</script>
