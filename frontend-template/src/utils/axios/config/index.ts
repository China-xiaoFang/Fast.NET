import { i18n } from "@/lang";

/**
 * Axios 配置
 * @description 这里因为循环引用的问题，所以采用方法获取，在需要的时候调用方法获取
 * @returns
 */
export const getAxiosDefaultConfig = () => {
    return {
        /**
         * 默认加载选项
         */
        loading: {
            fullscreen: true,
            lock: true,
            text: i18n.global.t("utils.axios.config.加载中"),
            background: "rgba(0, 0, 0, 0.7)",
        },
        /**
         * 错误白名单路径
         */
        errorWhiteUrls: ["logout"],
        /**
         * 重新登录Code
         */
        reloadLoginCodes: [401],
        /**
         * 重新登录消息
         */
        reloadLoginMessage: i18n.global.t("utils.axios.config.登录已失效，请重新登录！"),
        /**
         * 重新登录弹窗按钮文本
         */
        reloadLoginButtonText: i18n.global.t("utils.axios.config.重新登录"),
        /**
         * 错误Code消息
         */
        errorCodeMessages: {
            cancelDuplicate: i18n.global.t("utils.axios.config.重复请求，自动取消！"),
            offLine: i18n.global.t("utils.axios.config.您断网了！"),
            fileDownloadError: i18n.global.t("utils.axios.config.文件下载失败或此文件不存在！"),
            302: i18n.global.t("utils.axios.config.接口重定向了！"),
            400: i18n.global.t("utils.axios.config.参数不正确！"),
            401: i18n.global.t("utils.axios.config.您没有权限操作（令牌、用户名、密码错误）！"),
            403: i18n.global.t("utils.axios.config.您的访问是被禁止的！"),
            404: i18n.global.t("utils.axios.config.请求的资源不存在！"),
            405: i18n.global.t("utils.axios.config.请求的格式不正确！"),
            408: i18n.global.t("utils.axios.config.请求超时！"),
            409: i18n.global.t("utils.axios.config.系统已存在相同数据！"),
            410: i18n.global.t("utils.axios.config.请求的资源被永久删除，且不会再得到的！"),
            422: i18n.global.t("utils.axios.config.当创建一个对象时，发生一个验证错误！"),
            429: i18n.global.t("utils.axios.config.请求过于频繁，请稍后再试！"),
            500: i18n.global.t("utils.axios.config.服务器内部错误！"),
            501: i18n.global.t("utils.axios.config.服务未实现！"),
            502: i18n.global.t("utils.axios.config.网关错误！"),
            503: i18n.global.t("utils.axios.config.服务不可用，服务器暂时过载或维护！"),
            504: i18n.global.t("utils.axios.config.服务暂时无法访问，请稍后再试！"),
            505: i18n.global.t("utils.axios.config.HTTP版本不受支持！"),
            default: i18n.global.t("utils.axios.config.请求错误！"),
        },
    };
};
