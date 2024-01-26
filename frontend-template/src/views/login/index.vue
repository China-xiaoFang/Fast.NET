<template>
    <div>
        <div class="switch-language">
            <el-dropdown size="large" :hide-timeout="50" placement="bottom-end" :hide-on-click="true">
                <span>
                    <FIcon name="fa fa-globe" color="var(--el-text-color-secondary)" size="28" />
                </span>
                <template #dropdown>
                    <el-dropdown-menu class="chang-lang">
                        <el-dropdown-item v-for="item in configStore.lang.langArray" :key="item.name" @click="editDefaultLang(item.name)">
                            {{ item.value }}
                        </el-dropdown-item>
                    </el-dropdown-menu>
                </template>
            </el-dropdown>
        </div>
        <el-card class="login-warp">
            <el-row class="login">
                <el-col :span="12"> </el-col>
                <el-col :span="12">
                    <el-tabs v-model="state.loginMethod" stretch class="from">
                        <el-tab-pane :label="t('views.login.账号')" :name="LoginMethodEnum.Account">
                            <AccountForm
                                :rememberPassword="state.rememberPassword"
                                :account="state.account"
                                :password="state.password"
                                @login="loginHandle"
                            />
                        </el-tab-pane>
                        <el-tab-pane :label="t('views.login.手机号')" :name="LoginMethodEnum.Mobile">
                            <MobileForm
                                :rememberPassword="state.rememberPassword"
                                :account="state.account"
                                :password="state.password"
                                @login="loginHandle"
                            />
                        </el-tab-pane>
                        <el-tab-pane :label="t('views.login.邮箱')" :name="LoginMethodEnum.Email">
                            <EmailForm
                                :rememberPassword="state.rememberPassword"
                                :account="state.account"
                                :password="state.password"
                                @login="loginHandle"
                            />
                        </el-tab-pane>
                    </el-tabs>
                </el-col>
            </el-row>
        </el-card>
        <FDialog
            ref="fDialogRef"
            :title="t('views.login.选择租户登录')"
            :disabledConfirmButton="!fTableRef?.selected"
            @confirmClick="handleSelectTenantConfirm"
        >
            <FTable ref="fTableRef" singleChoice :requestAuto="false" :data="state.tenantList" :columns="tableColumns"> </FTable>
        </FDialog>
    </div>
</template>

<script setup lang="ts" name="Login">
import { onMounted, reactive, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useConfig } from "@/stores/config";
import { useUserInfo } from "@/stores/userInfo";
import { editDefaultLang } from "@/lang";
import { Md5 } from "ts-md5";
import { Local } from "@/utils/storage";
import * as loginApi from "@/api/login";
import { LoginMethodEnum } from "@/api/modules/login-method-enum";
import AccountForm from "./modules/account.vue";
import MobileForm from "./modules/mobile.vue";
import EmailForm from "./modules/email.vue";
import { FTableColumn } from "@/components/FTable/interface";

const { t } = useI18n();

const configStore = useConfig();
const useUserInfoStore = useUserInfo();

const fDialogRef = ref();
const fTableRef = ref();

const state = reactive({
    loading: false,
    rememberPassword: false,
    account: "",
    password: "",
    loginMethod: LoginMethodEnum.Account,
    tenantList: [],
});

const tableColumns: FTableColumn[] = reactive([
    {
        prop: "chName",
        label: "租户名称",
    },
    {
        prop: "adminType",
        label: "类型",
    },
    {
        prop: "nickName",
        label: "昵称",
    },
    {
        prop: "jobNumber",
        label: "工号",
    },
    {
        prop: "departmentName",
        label: "部门名称",
    },
    {
        prop: "lastLoginTime",
        label: "最后登录时间",
    },
    {
        prop: "lastLoginIp",
        label: "最后登录Ip",
    },
]);

onMounted(() => {
    // 判断是否记住密码，且存在缓存密码
    const localLoginForm = Local.get<any>("Login-Form", true);
    if (localLoginForm) {
        state.rememberPassword = localLoginForm.rememberPassword;
        state.account = localLoginForm.account;
        state.password = localLoginForm.password;
        state.loginMethod = localLoginForm.loginMethod;
    }
});

/**
 * 登录
 * @param formData
 */
const loginHandle = (formData: any) => {
    state.loading = true;
    const localAccount = formData.account || formData.mobile || formData.email;
    let localPassword = "";
    let isMd5 = true;
    // 判断原本是否就记住密码
    if (state.rememberPassword) {
        if (formData.password != state.password) {
            isMd5 = true;
        } else {
            isMd5 = false;
        }
    } else {
        isMd5 = true;
    }
    if (isMd5) {
        const md5: any = new Md5();
        md5.appendAsciiStr(formData.password);
        localPassword = md5.end();
    } else {
        localPassword = state.password;
    }
    loginApi
        .login({
            account: localAccount,
            password: localPassword,
            loginMethod: state.loginMethod,
        })
        .then((res) => {
            if (res.success) {
                if (res.data.isAutoLogin) {
                    loginSuccess(formData, {
                        rememberPassword: true,
                        account: localAccount,
                        password: localPassword,
                        loginMethod: state.loginMethod,
                    });
                } else {
                    state.tenantList = res.data.tenantList;
                    fDialogRef.value.open();
                }
            } else {
                console.log(res);
            }
        });
};

/**
 * 选择租户确认
 */
const handleSelectTenantConfirm = () => {};

/**
 * 登录成功
 * @param formData
 * @param data
 */
const loginSuccess = (formData: any, data: any) => {
    // 判断是否记住密码
    if (formData.rememberPassword) {
        Local.set("Login-Form", data, null, true);
    }
    useUserInfoStore.login();
};
</script>

<style scoped lang="scss">
.switch-language {
    position: fixed;
    top: 20px;
    right: 20px;
    z-index: 1;
}
.login-warp {
    background-color: black;
    position: absolute;
    top: 0;
    display: flex;
    width: 100vw;
    height: 100vh;
    align-items: center;
    justify-content: center;
    .login {
        overflow: hidden;
        width: 760px;
        padding: 0;
        background: var(--fast-bg-color-overlay);
        margin-bottom: 80px;
    }
    .from {
        padding: 40px;
        :deep(.submit-button) {
            width: 100%;
            letter-spacing: 2px;
            font-weight: 300;
            margin-top: 15px;
            --el-button-bg-color: var(--el-color-primary);
        }
    }
}
</style>
