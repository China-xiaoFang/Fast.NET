import { type App } from "vue";
import copy from "./modules/copy";
import debounce from "./modules/debounce";
import draggable from "./modules/draggable";
import longpress from "./modules/longpress";
import throttle from "./modules/throttle";
import errorHandler from "@/utils/errorHandler";

/** 挂载自定义指令 */
export function loadDirectives(app: App) {
    // 全局代码错误捕捉
    app.config.errorHandler = errorHandler;

    const directivesList: any = {
        copy,
        debounce,
        draggable,
        longpress,
        throttle,
    };

    Object.keys(directivesList).forEach((key) => {
        // 注册所有自定义指令
        app.directive(key, directivesList[key]);
    });
}
