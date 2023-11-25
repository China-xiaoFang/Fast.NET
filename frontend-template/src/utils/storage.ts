/**
 * 本地缓存
 */

import { CACHE_PREFIX, CACHE_EXPIRE_PREFIX } from "@/stores/constant/index"

export const Local = {
    set(key: string, val: any, expire: number | null = null): void {
        // 判断是否存在缓存过期时间
    }
}
