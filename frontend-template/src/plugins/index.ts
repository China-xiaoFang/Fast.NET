import { type App } from "vue";
import { loadElementPlus } from "./element-plus";
import { loadIcon } from "./icon";

export function loadPlugins(app: App) {
    loadElementPlus(app);
    loadIcon(app);
}
