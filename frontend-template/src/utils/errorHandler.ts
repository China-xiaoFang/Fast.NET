/**
 * 全局代码错误捕捉
 * 比如 null.length 就会被捕捉到
 */
import { ElNotification } from "element-plus";
import { nextTick } from "vue";
import { i18n } from "@/lang";

export default (error: any) => {
    // 过滤HTTP请求错误
    if (error?.code) {
        return false;
    }
    const errorMap: any = {
        InternalError: i18n.global.t("utils.errorHandler.Javascript引擎内部错误"),
        ReferenceError: i18n.global.t("utils.errorHandler.未找到对象"),
        TypeError: i18n.global.t("utils.errorHandler.使用了错误的类型或对象"),
        RangeError: i18n.global.t("utils.errorHandler.使用内置对象时，参数超范围"),
        SyntaxError: i18n.global.t("utils.errorHandler.语法错误"),
        EvalError: i18n.global.t("utils.errorHandler.错误的使用了Eval"),
        URIError: i18n.global.t("utils.errorHandler.URI错误"),
    };
    const errorName = errorMap[error.name] || i18n.global.t("utils.errorHandler.未知错误");
    nextTick(() => {
        ElNotification({
            title: i18n.global.t("utils.errorHandler.错误"),
            message: errorName,
            duration: 3000,
            position: "top-right",
        });
        console.error(error);
    });
};
