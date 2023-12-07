import { defineStore } from "pinia";
import { STORE_USER_INFO } from "@/stores/constant";
import type { UserInfo } from "@/stores/interface";
import { type AxiosResponse } from "axios";

export const useUserInfo = defineStore("userInfo", {
    state: (): UserInfo => {
        return {
            /**
             * Token
             */
            token: "",
            /**
             * Refresh Token
             */
            refreshToken: "",
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
         * @returns
         */
        getToken(): { token: string | null; refreshToken: string | null } {
            if (this.token) {
                // 解析 JwtToken
                const jwtToken = JSON.parse(
                    decodeURIComponent(encodeURIComponent(window.atob(this.token.replace(/_/g, "/").replace(/-/g, "+").split(".")[1])))
                ) as anyObj;
                // 获取 Token 的过期时间
                var exp = new Date(jwtToken.exp * 1000);
                if (new Date() >= exp) {
                    return { token: `Bearer ${this.token}`, refreshToken: `Bearer ${this.refreshToken}` };
                }
                return { token: `Bearer ${this.token}`, refreshToken: null };
            }
            return { token: null, refreshToken: null };
        },
        /**
         * 退出登录
         */
        logout(): void {},
    },
    persist: {
        key: STORE_USER_INFO,
    },
});
