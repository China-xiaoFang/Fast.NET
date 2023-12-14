import { type App } from "vue";
import * as ElementPlusIconsVue from "@element-plus/icons-vue";
import FIcon from "@/components/FIcon/index.vue";

export function loadIcon(app: App) {
    /**
     * 全局注册 FIcon
     * 使用方法：<FIcon name="name" size="size" color="color">
     */
    app.component("FIcon", FIcon);

    /** 注册所有 Element Plus Icon */
    for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
        // 这里是给 FIcon 使用的
        app.component(`el-icon-${key}`, component);
        app.component(key, component);
    }
}
