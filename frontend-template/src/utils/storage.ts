/**
 * 本地缓存
 */

import { i18n } from "@/lang";
import { CACHE_PREFIX, CACHE_EXPIRE_SUFFIX } from "@/stores/constant/index";

/**
 * window.localStorage
 */
export const Local = {
    /**
     * 设置
     * @param key 缓存的Key
     * @param val 缓存值
     * @param expire 过期时间，单位分钟
     */
    set(key: string, val: any, expire: number | null = null): void {
        // 判断是否存在缓存过期时间
        if (expire !== null) {
            if (isNaN(expire) || expire < 1) {
                throw new Error(i18n.global.t("utils.storage.有效期应为一个有效数值"));
            }
            // 设置过期时间的缓存
            const expireData = {
                time: Date.now(),
                expire: expire,
            };
            const expireJson = JSON.stringify(expireData);
            window.localStorage.setItem(`${CACHE_PREFIX}${key}${CACHE_EXPIRE_SUFFIX}`, expireJson);
        }
        // 统一转为 JSON 字符串
        const valJson = JSON.stringify(val);
        window.localStorage.setItem(`${CACHE_PREFIX}${key}`, valJson);
    },
    /**
     * 获取
     * @param key 缓存的Key
     * @returns {T} 传入的对象类型，默认为 string
     */
    get<T = "string">(key: string): T | null {
        // 获取缓存 JSON 字符串
        const valJson = window.localStorage.getItem(`${CACHE_PREFIX}${key}`);
        if (valJson) {
            try {
                // 尝试获取缓存过期时间的 JSON 字符串
                const expireJson = window.localStorage.getItem(`${CACHE_PREFIX}${key}${CACHE_EXPIRE_SUFFIX}`);
                // 判断是否存在过期时间
                if (expireJson) {
                    const expireData = JSON.parse(expireJson) as anyObj;
                    if (Date.now() > expireData.time + expireData.expire * 60 * 1000) {
                        // 过期了，删除对应的缓存数据
                        window.localStorage.removeItem(`${CACHE_PREFIX}${key}`);
                        window.localStorage.removeItem(`${CACHE_PREFIX}${key}${CACHE_EXPIRE_SUFFIX}`);
                        return null;
                    }
                }
                return JSON.parse(valJson) as T;
            } catch {
                return null;
            }
        }

        return null;
    },
    /**
     * 移除
     * @param key 缓存的Key
     */
    remove(key: string): void {
        window.localStorage.removeItem(`${CACHE_PREFIX}${key}`);
        window.localStorage.removeItem(`${CACHE_PREFIX}${key}${CACHE_EXPIRE_SUFFIX}`);
    },
    /**
     * 根据前缀移除
     * @param key 缓存的Key
     */
    removeByPrefix(key: string): void {
        for (var itemKey in window.localStorage) {
            if (itemKey.indexOf(`${CACHE_PREFIX}${key}`) !== -1) {
                window.localStorage.removeItem(itemKey);
            }
        }
    },
    /**
     * 移除全部
     */
    clear(): void {
        window.localStorage.clear();
    },
};

/**
 * window.sessionStorage
 */
export const Session = {
    /**
     * 设置会话缓存
     * @param key 缓存的Key
     * @param val 缓存值
     * @param expire 过期时间，单位分钟
     */
    set(key: string, val: any, expire: number | null = null): void {
        // 判断是否存在缓存过期时间
        if (expire !== null) {
            if (isNaN(expire) || expire < 1) {
                throw new Error(i18n.global.t("utils.storage.有效期应为一个有效数值"));
            }
            // 设置过期时间的缓存
            const expireData = {
                time: Date.now(),
                expire: expire,
            };
            const expireJson = JSON.stringify(expireData);
            window.sessionStorage.setItem(`${CACHE_PREFIX}${key}${CACHE_EXPIRE_SUFFIX}`, expireJson);
        }
        // 统一转为 JSON 字符串
        const valJson = JSON.stringify(val);
        window.sessionStorage.setItem(`${CACHE_PREFIX}${key}`, valJson);
    },
    /**
     * 获取会话缓存
     * @param key 缓存的Key
     * @returns {T} 传入的对象类型，默认为 string
     */
    get<T = "string">(key: string): T | null {
        // 获取缓存 JSON 字符串
        const valJson = window.sessionStorage.getItem(`${CACHE_PREFIX}${key}`);
        if (valJson) {
            try {
                // 尝试获取缓存过期时间的 JSON 字符串
                const expireJson = window.sessionStorage.getItem(`${CACHE_PREFIX}${key}${CACHE_EXPIRE_SUFFIX}`);
                // 判断是否存在过期时间
                if (expireJson) {
                    const expireData = JSON.parse(expireJson) as anyObj;
                    if (Date.now() > expireData.time + expireData.expire * 60 * 1000) {
                        // 过期了，删除对应的缓存数据
                        window.sessionStorage.removeItem(`${CACHE_PREFIX}${key}`);
                        window.sessionStorage.removeItem(`${CACHE_PREFIX}${key}${CACHE_EXPIRE_SUFFIX}`);
                        return null;
                    }
                }
                return JSON.parse(valJson) as T;
            } catch {
                return null;
            }
        }

        return null;
    },
    /**
     * 移除会话缓存
     * @param key 缓存的Key
     */
    remove(key: string): void {
        window.sessionStorage.removeItem(`${CACHE_PREFIX}${key}`);
        window.sessionStorage.removeItem(`${CACHE_PREFIX}${key}${CACHE_EXPIRE_SUFFIX}`);
    },
    /**
     * 根据前缀移除会话缓存
     * @param key 缓存的Key
     */
    removeByPrefix(key: string): void {
        for (var itemKey in window.sessionStorage) {
            if (itemKey.indexOf(`${CACHE_PREFIX}${key}`) !== -1) {
                window.sessionStorage.removeItem(itemKey);
            }
        }
    },
    /**
     * 移除全部会话缓存
     */
    clear(): void {
        window.sessionStorage.clear();
    },
};
