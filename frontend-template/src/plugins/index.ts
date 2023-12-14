import { type App } from "vue";
import { loadElementPlus } from "./element-plus";
import { loadIcon } from "./icon";
import { loadCommonComponent } from "./component";

export function loadPlugins(app: App) {
    loadElementPlus(app);
    loadIcon(app);
    loadCommonComponent(app);
}
