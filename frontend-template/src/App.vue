<template>
    <el-config-provider :locale="lang">
        <br />
        <el-radio-group v-model="radioLang" class="ml-4" @change="changeLang">
            <el-radio v-for="item in config.lang.langArray" :label="item.name">{{ item.value }}</el-radio>
        </el-radio-group>
        <br />
        <div class="demo-time-range">
            <el-table mb-1 :data="[]" />
            <el-pagination :total="100" />
        </div>
        <br />
        <!-- <router-view></router-view> -->
        <h1>{{ $t("App.你好啊！") }}</h1>
        <loginForm />
    </el-config-provider>
</template>

<script setup lang="ts" name="App">
import { useI18n } from "vue-i18n";
import { useConfig } from "@/stores/config";
import { ref } from "vue";
import { editDefaultLang } from "@/lang";

const radioLang = ref<string>();

const changeLang = (value: string) => {
    editDefaultLang(value);
};

const config = useConfig();

// 初始化 element 的语言包
const { getLocaleMessage } = useI18n();
const lang = getLocaleMessage(config.lang.defaultLang) as any;
</script>
