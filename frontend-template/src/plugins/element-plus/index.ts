import { type App } from "vue";
import { ElDialog, ElMessageBox, ElMessageBoxOptions } from "element-plus";
import { i18n } from "@/lang";

/** Element Plus 组件全局配置 */

// ELDialog 默认拖拽
ElDialog.props.draggable.default = true;

// ElMessageBox 默认配置
const elMessageBox = (message: ElMessageBoxOptions["message"], options: ElMessageBoxOptions, type: "alert" | "confirm" | "prompt") => {
    options = options ?? {};
    if (options?.title === undefined) {
        // 默认提示
        options.title = i18n.global.t("common.温馨提示");
    }
    if (options?.draggable === undefined) {
        // 默认拖拽
        options.draggable = true;
    }
    if (options?.cancelButtonText === undefined) {
        // 默认 取消按钮的文本内容
        options.cancelButtonText = i18n.global.t("common.取消");
    }
    if (options?.confirmButtonText === undefined) {
        // 默认 确定按钮的文本内容
        options.confirmButtonText = i18n.global.t("common.确定");
    }
    if (options?.closeOnClickModal === undefined) {
        // 默认 是否可通过点击遮罩层关闭 MessageBox
        options.closeOnClickModal = false;
    }
    if (options?.closeOnPressEscape === undefined) {
        // 默认 是否可通过按下 ESC 键关闭 MessageBox
        options.closeOnPressEscape = false;
    }

    // 根据类型有一些判断
    switch (type) {
        case "alert":
            break;
        case "confirm":
            if (options?.showCancelButton == undefined) {
                options.showCancelButton = true;
            }
            break;
        case "prompt":
            if (options?.showCancelButton == undefined) {
                options.showCancelButton = true;
            }
            break;
    }

    return ElMessageBox(
        Object.assign(
            {
                message,
            },
            options,
            {
                boxType: type,
            }
        )
    );
};

ElMessageBox.alert = (message: ElMessageBoxOptions["message"], options?: ElMessageBoxOptions) => elMessageBox(message, options, "alert");

ElMessageBox.prompt = (message: ElMessageBoxOptions["message"], options?: ElMessageBoxOptions) => elMessageBox(message, options, "prompt");

ElMessageBox.confirm = (message: ElMessageBoxOptions["message"], options?: ElMessageBoxOptions) => elMessageBox(message, options, "confirm");

import ElementPlus from "element-plus";

export function loadElementPlus(app: App) {
    /** Element Plus 组件完整引入 */
    app.use(ElementPlus);
}
