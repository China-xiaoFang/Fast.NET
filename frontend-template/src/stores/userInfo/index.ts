import { defineStore } from "pinia";
import { STORE_USER_INFO } from "@/stores/constant";
import { ElMessage } from "element-plus";
import type { UserInfo } from "./interface";
import { type AxiosResponse } from "axios";
import * as loginApi from "@/api/login";
import * as authApi from "@/api/auth";
import { Local } from "@/utils/storage";
import router from "@/router";

export const useUserInfo = defineStore("userInfo", {
    state: (): UserInfo => {
        return {
            // Token
            token: "",
            // Refresh Token
            refreshToken: "",
            // 是否为超级管理员
            supperAdmin: false,
            // 是否为管理员
            admin: false,
            // 用户名称
            userName: "",
            // 用户昵称
            nickName: "",
            // 头像
            avatar: "",
            // 最后登录时间
            lastLoginTime: null,
            // 动态生成路由
            asyncRouterGen: false,
        };
    },
    actions: {
        /**
         * 设置用户信息
         * @param userInfo
         */
        setUserInfo(userInfo: UserInfo): void {
            this.$state = { ...this.$state, ...userInfo };
        },
        /**
         * 删除 Token
         */
        removeToken(): void {
            this.token = "";
            this.refreshToken = "";
        },
        /**
         * 设置 Token
         * @param axiosResponse
         */
        setToken(axiosResponse: AxiosResponse): void {
            // 从请求头部中获取 Token
            const token = axiosResponse.headers["access-token"];
            // 从请求头部中获取 Refresh Token
            const refreshToken = axiosResponse.headers["x-access-token"];
            // 判断是否为无效 Token
            if (token === "invalid_token") {
                // 删除 Token
                this.removeToken();
            } else if (token && refreshToken && refreshToken !== "invalid_token") {
                // 设置 Token
                this.token = token;
                this.refreshToken = refreshToken;
            }
        },
        /**
         * 获取 Token
         * @description 从缓存中获取
         * @returns
         */
        getToken(): { token: string | null; refreshToken: string | null } {
            return { token: this.token, refreshToken: this.refreshToken };
        },
        /**
         * 解析Token
         * @description 如果Token过期，会解析不出来
         * @param token 可以传入，也可以直接获取 pinia 中的
         * @param refreshToken 可以传入，也可以直接获取 pinia 中的
         * @returns
         */
        resolveToken(token: string | null = null, refreshToken: string | null = null): { token: string | null; refreshToken: string | null } {
            token ??= this.token;
            refreshToken ??= this.refreshToken;
            if (token) {
                // 解析 JwtToken
                const jwtToken = JSON.parse(
                    decodeURIComponent(encodeURIComponent(window.atob(token.replace(/_/g, "/").replace(/-/g, "+").split(".")[1])))
                ) as anyObj;
                // 获取 Token 的过期时间
                var exp = new Date(jwtToken.exp * 1000);
                if (new Date() >= exp) {
                    return { token: `Bearer ${token}`, refreshToken: `Bearer ${refreshToken}` };
                }
                return { token: `Bearer ${token}`, refreshToken: null };
            }
            return { token: null, refreshToken: null };
        },
        /**
         * 刷新用户信息
         */
        async refreshUserInfo() {
            const userInfo = await authApi.getLoginUserInfo();
            if (userInfo.success) {
                this.userName = userInfo.data.userName;
                this.nickName = userInfo.data.nickName;
                this.avatar = userInfo.data.avatar;
                this.lastLoginTime = userInfo.data.lastLoginTime;
            } else {
                throw new Error(userInfo.message);
            }
        },
        /**
         * 登录
         */
        login(): void {
            ElMessage.success("登录成功");
            // 确保 getLoginUser 获取用户信息
            this.asyncRouterGen = false;
            // 进入系统
            router.push({ path: "/" });
        },
        /**
         * 退出登录
         */
        logout(): void {
            // 调用退出登录的接口
            loginApi.logout().finally(() => {
                Local.remove(STORE_USER_INFO);
                router.go(0);
            });
        },
    },
    persist: {
        key: STORE_USER_INFO,
    },
});
