import { defineStore } from "pinia";
import { reactive } from "vue";
import { STORE_SITE_CONFIG } from "@/stores/constant";
import type { SiteConfig } from "./interface";

export const useConfig = defineStore(
    "siteConfig",
    () => {
        /**
         * 站点配置
         */
        const site: SiteConfig = reactive({
            title: "Fast.NET",
            copyrightValidStartYear: 2018,
            copyrightValidEndYear: 2023,
            copyrighted: "Fast.NET",
            copyrightedUrl: "https://fastdotnet.com",
            icpInfo: "xICP备xxx号",
            publicInfo: "x公网安备xxx号",
            version: "v1.0.0",
        });

        return { site };
    },
    {
        persist: {
            key: STORE_SITE_CONFIG,
        },
    }
);
