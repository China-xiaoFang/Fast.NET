<template>
    <div class="nav-menus" :class="configStore.layout.layoutMode">
        <router-link class="h100" target="_blank" :title="t('layouts.components.NavMenus.首页')" to="/">
            <div class="nav-menu-item">
                <FIcon :color="configStore.getColorVal('headerBarTabColor')" class="nav-menu-icon" name="el-icon-Monitor" size="18" />
            </div>
        </router-link>
        <el-dropdown
            @visible-change="onChangeNavMenuHandle($event, 'lang')"
            class="h100"
            size="large"
            :hide-timeout="50"
            placement="bottom"
            trigger="click"
            :hide-on-click="true"
        >
            <div class="nav-menu-item pt2" :class="state.currentNavMenu == 'lang' ? 'hover' : ''">
                <Icon :color="configStore.getColorVal('headerBarTabColor')" class="nav-menu-icon" name="local-lang" size="18" />
            </div>
            <template #dropdown>
                <el-dropdown-menu class="dropdown-menu-box">
                    <el-dropdown-item v-for="item in configStore.lang.langArray" :key="item.name" @click="editDefaultLang(item.name)">
                        {{ item.value }}
                    </el-dropdown-item>
                </el-dropdown-menu>
            </template>
        </el-dropdown>
        <div @click="onClickFullScreenHandle" class="nav-menu-item" :class="state.isFullScreen ? 'hover' : ''">
            <Icon
                :color="configStore.getColorVal('headerBarTabColor')"
                class="nav-menu-icon"
                v-if="state.isFullScreen"
                name="local-full-screen-cancel"
                size="18"
            />
            <Icon :color="configStore.getColorVal('headerBarTabColor')" class="nav-menu-icon" v-else name="el-icon-FullScreen" size="18" />
        </div>
        <el-dropdown
            @visible-change="onChangeNavMenuHandle($event, 'clear')"
            class="h100"
            size="large"
            :hide-timeout="50"
            placement="bottom"
            trigger="click"
            :hide-on-click="true"
        >
            <div class="nav-menu-item" :class="state.currentNavMenu == 'clear' ? 'hover' : ''">
                <Icon :color="configStore.getColorVal('headerBarTabColor')" class="nav-menu-icon" name="el-icon-Delete" size="18" />
            </div>
            <template #dropdown>
                <el-dropdown-menu class="dropdown-menu-box">
                    <el-dropdown-item v-if="userInfo.supperAdmin || userInfo.admin" @click="onClickClearCacheHandle('tp')">
                        {{ t("layouts.components.NavMenus.清理系统缓存") }}
                    </el-dropdown-item>
                    <el-dropdown-item @click="onClickClearCacheHandle('storage')">{{
                        t("layouts.components.NavMenus.清理浏览器缓存")
                    }}</el-dropdown-item>
                    <el-dropdown-item v-if="userInfo.supperAdmin" @click="onClickClearCacheHandle('all')" divided>
                        {{ t("layouts.components.NavMenus.一键清理所有") }}
                    </el-dropdown-item>
                </el-dropdown-menu>
            </template>
        </el-dropdown>
        <el-popover
            @show="onChangeNavMenuHandle(true, 'userInfo')"
            @hide="onChangeNavMenuHandle(false, 'userInfo')"
            placement="bottom-end"
            :hide-after="0"
            :width="260"
            trigger="click"
            popper-class="user-info-box"
            v-model:visible="state.showUserInfoPopover"
        >
            <template #reference>
                <div class="user-info" :class="state.currentNavMenu == 'userInfo' ? 'hover' : ''">
                    <el-avatar :size="25" fit="fill">
                        <img :src="fullUrl(userInfo.avatar)" alt="" />
                    </el-avatar>
                    <div class="user-name">{{ userInfo.nickName ?? userInfo.userName }}</div>
                </div>
            </template>
            <div>
                <div class="user-info-base">
                    <el-avatar :size="70" fit="fill">
                        <img :src="fullUrl(userInfo.avatar)" alt="" />
                    </el-avatar>
                    <div class="user-info-other">
                        <div class="user-info-name">{{ userInfo.nickName ?? userInfo.userName }}</div>
                        <div class="user-info-last-time">{{ userInfo.lastLoginTime }}</div>
                    </div>
                </div>
                <div class="user-info-footer">
                    <el-button @click="onClickUserInfoHandle" type="primary" plain>{{ t("layouts.components.NavMenus.个人资料") }}</el-button>
                    <el-button @click="userInfo.logout" type="danger" plain>{{ t("layouts.components.NavMenus.注销") }}</el-button>
                </div>
            </div>
        </el-popover>
        <div @click="configStore.setLayout('showSettingDrawer', true)" class="nav-menu-item">
            <Icon :color="configStore.getColorVal('headerBarTabColor')" class="nav-menu-icon" name="fa fa-cogs" size="18" />
        </div>
    </div>
</template>

<script lang="ts" setup name="LayoutNavMenus">
import { reactive } from "vue";
import { editDefaultLang } from "@/lang";
import screenfull from "screenfull";
import { ElMessage } from "element-plus";
import { useI18n } from "vue-i18n";
import { useConfig } from "@/stores/config";
import { useUserInfo } from "@/stores/userInfo";
import { fullUrl } from "@/utils";
import { CACHE_USER_INFO } from "@/stores/constant";
import { Local, Session } from "@/utils/storage";

const { t } = useI18n();

const configStore = useConfig();
const userInfo = useUserInfo();

const state = reactive({
    /**
     * 全屏显示
     */
    isFullScreen: false,
    /**
     * 当前选中的导航菜单
     */
    currentNavMenu: "",
    /**
     * 显示用户信息弹出层
     */
    showUserInfoPopover: false,
});

/**
 * 导航菜单改变
 * @param status
 * @param name
 */
const onChangeNavMenuHandle = (status: boolean, name: string) => {
    state.currentNavMenu = status ? name : "";
};

/**
 * 全屏
 */
const onClickFullScreenHandle = () => {
    if (!screenfull.isEnabled) {
        ElMessage.warning(t("layouts.components.NavMenus.您的浏览器不支持全屏，请更换浏览器再试~"));
        return false;
    }
    screenfull.toggle();
    screenfull.onchange(() => {
        state.isFullScreen = screenfull.isFullscreen;
    });
};

/**
 * 用户信息点击
 */
const onClickUserInfoHandle = () => {
    state.showUserInfoPopover = false;
};

/**
 * 清理缓存点击
 * @param type
 */
const onClickClearCacheHandle = (type: string) => {
    if (type == "storage" || type == "all") {
        const userInfo = Local.get(CACHE_USER_INFO);
        Session.clear();
        Local.clear();
        Local.set(CACHE_USER_INFO, userInfo);
        if (type == "storage") return;
    }
};
</script>

<style scoped lang="scss">
.nav-menus.Default {
    border-radius: var(--el-border-radius-base);
    box-shadow: var(--el-box-shadow-light);
}
.nav-menus {
    display: flex;
    align-items: center;
    height: 100%;
    margin-left: auto;
    background-color: v-bind('configStore.getColorVal("headerBarBackground")');
    .nav-menu-item {
        height: 100%;
        width: 40px;
        display: flex;
        align-items: center;
        justify-content: center;
        cursor: pointer;
        .nav-menu-icon {
            box-sizing: content-box;
            color: v-bind('configStore.getColorVal("headerBarTabColor")');
        }
        &:hover {
            .icon {
                animation: twinkle 0.3s ease-in-out;
            }
        }
    }
    .user-info {
        display: flex;
        height: 100%;
        padding: 0 10px;
        align-items: center;
        cursor: pointer;
        user-select: none;
        color: v-bind('configStore.getColorVal("headerBarTabColor")');
    }
    .user-name {
        padding-left: 6px;
        white-space: nowrap;
    }
    .nav-menu-item:hover,
    .user-info:hover,
    .nav-menu-item.hover,
    .user-info.hover {
        background: v-bind('configStore.getColorVal("headerBarHoverBackground")');
    }
}
.dropdown-menu-box :deep(.el-dropdown-menu__item) {
    justify-content: center;
}
.user-info-base {
    display: flex;
    justify-content: center;
    flex-wrap: wrap;
    padding-top: 10px;
    .user-info-other {
        display: block;
        width: 100%;
        text-align: center;
        padding: 10px 0;
        .user-info-name {
            font-size: var(--el-font-size-large);
        }
    }
}
.user-info-footer {
    padding: 10px 0;
    margin: 0 -12px -12px -12px;
    display: flex;
    justify-content: space-around;
}
.pt2 {
    padding-top: 2px;
}

@keyframes twinkle {
    0% {
        transform: scale(0);
    }
    80% {
        transform: scale(1.2);
    }
    100% {
        transform: scale(1);
    }
}
</style>
