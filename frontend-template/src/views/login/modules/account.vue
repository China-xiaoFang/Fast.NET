<template>
    <el-form ref="formRef" :rules="rules" :model="loginForm" @keyup.enter="loginHandle()">
        <el-form-item prop="account">
            <el-input type="text" clearable v-model="loginForm.account" :placeholder="t('views.login.modules.account.请输入账号')">
                <template #prefix>
                    <FIcon name="fa fa-user" class="form-item-icon" size="16" color="var(--el-input-icon-color)" />
                </template>
            </el-input>
        </el-form-item>
        <el-form-item prop="password">
            <el-input type="password" show-password v-model="loginForm.password" :placeholder="t('views.login.modules.account.请输入密码')">
                <template #prefix>
                    <FIcon name="fa fa-unlock-alt" class="form-item-icon" size="16" color="var(--el-input-icon-color)" />
                </template>
            </el-input>
        </el-form-item>
        <el-checkbox v-model="loginForm.rememberPassword" :label="t('views.login.modules.account.记住密码')" size="default"></el-checkbox>
        <el-form-item>
            <el-button :loading="state.loading" class="submit-button" round type="primary" @click="loginHandle()">
                {{ t("views.login.modules.account.账号登录") }}
            </el-button>
        </el-form-item>
    </el-form>
</template>

<script setup lang="ts" name="LoginAccount">
import { reactive, ref, watch } from "vue";
import type { FormInstance, FormRules } from "element-plus";
import { useI18n } from "vue-i18n";
import { validateScrollToField, buildFormValidator } from "@/utils/validate";

const { t } = useI18n();

interface LoginAccountProps {
    /**
     * 是否记住密码
     */
    rememberPassword?: boolean;
    /**
     * 缓存的账号
     */
    account?: string;
    /**
     * 缓存的密码
     */
    password?: string;
}

const props = withDefaults(defineProps<LoginAccountProps>(), {
    rememberPassword: false,
    account: "",
    password: "",
});

const emit = defineEmits(["login"]);

const state = reactive({
    loading: false,
    isCachePassword: false,
});
const loginForm = reactive({
    account: "",
    password: "",
    rememberPassword: false,
});

watch(
    () => props.rememberPassword,
    () => {
        loginForm.account = props.account;
        loginForm.password = props.password;
        loginForm.rememberPassword = props.rememberPassword;
    }
);

// 表单验证规则
const rules: FormRules = reactive({
    account: [
        buildFormValidator({ name: "required", message: t("views.login.modules.account.请输入账号") }),
        buildFormValidator({ name: "account" }),
    ],
    password: [
        buildFormValidator({ name: "required", message: t("views.login.modules.account.请输入密码") }),
        buildFormValidator({ name: "password" }),
    ],
});

const formRef = ref<FormInstance>();

const loginHandle = async () => {
    await validateScrollToField(formRef.value, () => {
        emit("login", loginForm);
    });
};
</script>
