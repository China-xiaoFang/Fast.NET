<template>
    <div>
        <Particles id="tsparticles" class="login__particles" :options="particlesOptions" />
        <div class="switch-language">
            <el-dropdown size="large" :hide-timeout="50" placement="bottom-end" :hide-on-click="true">
                <span class="el-dropdown-link">
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
    </div>
</template>

<script setup lang="ts" name="HelloWorld">
import { onMounted, reactive } from "vue";
import { useI18n } from "vue-i18n";
import { useConfig } from "@/stores/config";
import { editDefaultLang } from "@/lang";
import { Md5 } from "ts-md5";
import { Local } from "@/utils/storage";
import * as loginApi from "@/api/login";
import { LoginMethodEnum } from "@/api/modules/login-method-enum";
import AccountForm from "./modules/account.vue";
import MobileForm from "./modules/mobile.vue";
import EmailForm from "./modules/email.vue";

const { t } = useI18n();

const configStore = useConfig();

const state = reactive({
    loading: false,
    rememberPassword: false,
    account: "",
    password: "",
    loginMethod: LoginMethodEnum.Account,
});

const particlesOptions = {
    fpsLimit: 60,
    interactivity: {
        detectsOn: "canvas",
        events: {
            onClick: {
                // 开启鼠标点击的效果
                enable: true,
                mode: "push",
            },
            onHover: {
                // 开启鼠标悬浮的效果(线条跟着鼠标移动)
                enable: true,
                mode: "grab",
            },
            resize: true,
        },
        modes: {
            // 配置动画效果
            bubble: {
                distance: 400,
                duration: 2,
                opacity: 0.8,
                size: 40,
            },
            push: {
                quantity: 4,
            },
            grab: {
                distance: 200,
                duration: 0.4,
            },
            attract: {
                // 鼠标悬浮时，集中于一点，鼠标移开时释放产生涟漪效果
                distance: 200,
                duration: 0.4,
                factor: 5,
            },
        },
    },
    particles: {
        color: {
            value: "#BA55D3", // 粒子点的颜色
        },
        links: {
            color: "#FFBBFF", // 线条颜色
            distance: 150, //线条距离
            enable: true,
            opacity: 0.4, // 不透明度
            width: 1.2, // 线条宽度
        },
        collisions: {
            enable: true,
        },
        move: {
            attract: { enable: false, rotateX: 600, rotateY: 1200 },
            bounce: false,
            direction: "none",
            enable: true,
            out_mode: "out",
            random: false,
            speed: 0.5, // 移动速度
            straight: false,
        },
        number: {
            density: {
                enable: true,
                value_area: 800,
            },
            value: 80, //粒子数
        },
        opacity: {
            //粒子透明度
            value: 0.7,
        },
        shape: {
            //粒子样式
            type: "star",
        },
        size: {
            //粒子大小
            random: true,
            value: 3,
        },
    },
    detectRetina: true,
};

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
                // 判断是否记住密码
                if (formData.rememberPassword) {
                    Local.set(
                        "Login-Form",
                        {
                            rememberPassword: true,
                            account: localAccount,
                            password: localPassword,
                            loginMethod: state.loginMethod,
                        },
                        null,
                        true
                    );
                }
            } else {
                console.log(res);
            }
        });
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
