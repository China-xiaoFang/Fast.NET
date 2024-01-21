<template>
    <div>
        <div class="switch-language">
            <el-dropdown size="large" :hide-timeout="50" placement="bottom-end" :hide-on-click="true">
                <Icon name="fa fa-globe" color="var(--el-text-color-secondary)" size="28" />
                <template #dropdown>
                    <el-dropdown-menu class="chang-lang">
                        <el-dropdown-item v-for="item in configStore.lang.langArray" :key="item.name" @click="editDefaultLang(item.name)">
                            {{ item.value }}
                        </el-dropdown-item>
                    </el-dropdown-menu>
                </template>
            </el-dropdown>
        </div>
        <el-form ref="formRef" :rules="rules" size="large" :model="loginForm" @keyup.enter="loginHandle()">
            <el-form-item label="account">
                <el-input ref="accountRef" type="text" clearable v-model="loginForm.account" :placeholder="t('views.login.请输入账号')">
                    <template #prefix>
                        <FIcon name="fa fa-user" class="form-item-icon" size="16" color="var(--el-input-icon-color)" />
                    </template>
                </el-input>
            </el-form-item>
            <el-form-item label="password">
                <el-input ref="passwordRef" type="password" show-password v-model="loginForm.password" :placeholder="t('views.login.请输入密码')">
                    <template #prefix>
                        <FIcon name="fa fa-unlock-alt" class="form-item-icon" size="16" color="var(--el-input-icon-color)" />
                    </template>
                </el-input>
            </el-form-item>
            <el-checkbox v-model="loginForm.rememberPassword" :label="t('views.login.记住密码')" size="default"></el-checkbox>
            <el-form-item label="password">
                <el-button :loading="state.loading" class="submit-button" round type="primary" size="large" @click="loginHandle()">
                    {{ t("views.login.登录") }}
                </el-button>
            </el-form-item>
        </el-form>
        <h1>{{ $t("views.login.你好啊！") }}</h1>
        <indexForm />
        <testForm />
        <el-button type="primary" @click="login">尝试登录</el-button>
    </div>
</template>

<script setup lang="ts" name="HelloWorld">
import { onMounted, onBeforeUnmount, reactive, ref, nextTick } from "vue";
import type { FormInstance, InputInstance } from "element-plus";
import { useI18n } from "vue-i18n";
import { useConfig } from "@/stores/config";
import { editDefaultLang } from "@/lang";
import * as loginApi from "@/api/login";
import { LoginMethodEnum } from "@/api/modules/login-method-enum";

const { t } = useI18n();

const configStore = useConfig();

// 表单验证规则
const rules = reactive({
    username: [buildValidatorData({ name: "required", message: t("login.Please enter an account") }), buildValidatorData({ name: "account" })],
    password: [buildValidatorData({ name: "required", message: t("login.Please input a password") }), buildValidatorData({ name: "password" })],
});

const formRef = ref<FormInstance>();
const accountRef = ref<InputInstance>();
const passwordRef = ref<InputInstance>();
const state = reactive({
    loading: false,
});

const login = () => {
    loginApi
        .login({
            account: "15288888888",
            password: "Fast888888",
            loginMethod: LoginMethodEnum.Account,
        })
        .then((res) => {
            if (res.success) {
                debugger;
            } else {
                console.log(res);
            }
        });
};
</script>
