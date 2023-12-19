import { type App } from "vue";
import * as commonComponents from "@/components";

export function loadCommonComponent(app: App) {
    /**
     * 全局注册公共组件
     */
    for (const [key, component] of Object.entries(commonComponents)) {
        app.component(key, component);
    }
}
