import { defineStore } from "pinia";
import { reactive } from "vue";
import { STORE_SITE_CONFIG } from "@/stores/constant";
import type { SiteConfig } from "./interface";

export const useSiteConfig = defineStore(
    "siteConfig",
    () => {
        // 站点配置
        const state: SiteConfig = reactive({
            // 站点名称
            siteName: "Fast.NET",
            // 版权有效开始年份
            copyrightValidStartYear: 2018,
            // 版权有效结束年份
            copyrightValidEndYear: 2023,
            // 版权所有
            copyrighted: "Fast.NET",
            // 版权所有相关链接
            copyrightedUrl: "https://fastdotnet.com",
            // 版本号
            version: "v1.0.0",
            // ICP备案信息
            icpInfo: "xICP备xxx号",
            // 公安备案信息
            publicInfo: "x公网安备xxx号",
            // 站点资源 cdn 加速地址
            cdnUrl: "",
        });

        return { state };
    },
    {
        persist: {
            key: STORE_SITE_CONFIG,
        },
    }
);
