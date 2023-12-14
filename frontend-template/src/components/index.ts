import FIcon from "@/components/FIcon/index.vue";

/**
 * 全局注册的公共组件
 */
export const commonComponents = {};

/**
 * 局部注册的公共组件
 */
export const localComponents = {};

// 合并导出
export default {
    // FIcon已经全局注册了，所以这里默认导出即可
    FIcon,
    ...commonComponents,
    ...localComponents,
};
