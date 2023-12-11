import { type App } from "vue";
import { commonComponents } from "@/components";

export function loadCommonComponent(app: App) {
    /**
     * 全局注册公共组件
     */
    /** 注册所有 Element Plus Icon */
    for (const [key, component] of Object.entries(commonComponents)) {
        app.component(key, component);
    }
}
