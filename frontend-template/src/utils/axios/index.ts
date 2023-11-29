import type { AxiosRequestConfig, AxiosResponse, InternalAxiosRequestConfig } from "axios";
import axios from "axios";
import type { Options, LoadingInstance } from "@/utils/axios/interface";
import { ElLoading, ElMessage, ElMessageBox, type LoadingOptions } from "element-plus";
import { useConfig } from "@/stores/config";
import { useUserInfo } from "@/stores/userInfo";
import { i18n } from "@/lang";
import { downloadFile } from "@/utils/utils";

const pendingMap = new Map();

/**
 * 登录弹出层的变量
 */
let loginCallBack = false;

/**
 * 加载实例
 */
const loadingInstance: LoadingInstance = {
    target: null,
    count: 0,
};

/**
 * Axios 默认配置
 */
const axiosDefaultConfig = {
    /**
     * 默认加载选项
     */
    loading: {
        fullscreen: true,
        lock: true,
        text: i18n.global.t("utils.axios.有效期应为一个有效数值"),
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
    reloadLoginMessage: i18n.global.t("utils.axios.登录已失效，请重新登录！"),
    /**
     * 重新登录弹窗按钮文本
     */
    reloadLoginButtonText: i18n.global.t("utils.axios.重新登录"),
    /**
     * 错误Code消息
     */
    errorCodeMessages: {
        cancelDuplicate: i18n.global.t("utils.axios.重复请求，自动取消！"),
        offLine: i18n.global.t("utils.axios.您断网了！"),
        fileDownloadError: i18n.global.t("utils.axios.文件下载失败或此文件不存在！"),
        302: i18n.global.t("utils.axios.接口重定向了！"),
        400: i18n.global.t("utils.axios.参数不正确！"),
        401: i18n.global.t("utils.axios.您没有权限操作（令牌、用户名、密码错误）！"),
        403: i18n.global.t("utils.axios.您的访问是被禁止的！"),
        404: i18n.global.t("utils.axios.请求的资源不存在！"),
        405: i18n.global.t("utils.axios.请求的格式不正确！"),
        408: i18n.global.t("utils.axios.请求超时！"),
        409: i18n.global.t("utils.axios.系统已存在相同数据！"),
        410: i18n.global.t("utils.axios.请求的资源被永久删除，且不会再得到的！"),
        422: i18n.global.t("utils.axios.当创建一个对象时，发生一个验证错误！"),
        429: i18n.global.t("utils.axios.请求过于频繁，请稍后再试！"),
        500: i18n.global.t("utils.axios.服务器内部错误！"),
        501: i18n.global.t("utils.axios.服务未实现！"),
        502: i18n.global.t("utils.axios.网关错误！"),
        503: i18n.global.t("utils.axios.服务不可用，服务器暂时过载或维护！"),
        504: i18n.global.t("utils.axios.服务暂时无法访问，请稍后再试！"),
        505: i18n.global.t("utils.axios.HTTP版本不受支持！"),
        default: i18n.global.t("utils.axios.请求错误！"),
    },
};

/**
 * 获取请求 Url 地址
 * @returns
 */
const getUrl = (): string => {
    // 获取请求路径
    const baseUrl: string = import.meta.env.VITE_AXIOS_BASE_URL;
    // 获取代理地址
    const url: string = import.meta.env.VITE_AXIOS_PROXY_URL;

    if (url === "getCurrentDomain") {
        return `${window.location.protocol}//${window.location.host}${baseUrl}`;
    } else {
        return `${url}${baseUrl}`;
    }
};

function createAxios<Data = any, T = ApiPromise<Data>>(axiosConfig: AxiosRequestConfig, options: Options = {}, loading: LoadingOptions = {}): T {
    const timestamp = new Date().getTime();
    const localConfig = useConfig();
    const localUserInfo = useUserInfo();

    // 创建 Axios 请求
    const Axios = axios.create({
        baseURL: getUrl(),
        timeout: import.meta.env.VITE_AXIOS_API_TIMEOUT,
        headers: {
            // 携带浏览器语言环境表示
            "fast-lang": localConfig.lang.defaultLang,
        },
        responseType: "json",
    });

    // 合并选项
    options = Object.assign(localConfig.axios.options, options);

    /**
     * 请求拦截
     */
    Axios.interceptors.request.use(
        (config: InternalAxiosRequestConfig<any>) => {
            // 删除重复请求
            removePending(config);

            // 判断是否开启取消重复请求
            options.cancelDuplicateRequest && addPending(config);

            // 判断是否显示loading层
            if (options.loading) {
                loadingInstance.count++;
                if (loadingInstance.count === 1) {
                    // 合并 Loading 配置
                    loading = Object.assign(axiosDefaultConfig.loading, loading);

                    loadingInstance.target = ElLoading.service(loading);
                }
            }

            if (config.headers) {
                // Token 处理
                const { token, refreshToken } = localUserInfo.getToken();
                token && (config.headers["Authorization"] = token);
                // 刷新 Token
                refreshToken && (config.headers["X-Authorization"] = refreshToken);

                // TODO：这里待增加设备ID
            }

            // TODO:请求参数加密

            // GET 请求缓存处理
            if (config.method.toUpperCase() === "GET") {
                config.params = config.params || {};
                config.params._ = timestamp;
            }

            return config;
        },
        (error) => {
            return Promise.reject(error);
        }
    );

    /**
     * 响应拦截
     */
    Axios.interceptors.response.use(
        (response: AxiosResponse) => {
            // 删除重复请求标识
            removePending(response.config);
            // 关闭loading层
            options.loading && closeLoading(options);
            // 判断是否包含错误白名单路径
            if (axiosDefaultConfig.errorWhiteUrls.includes(response.config.url)) {
                return Promise.resolve(true);
            }
            // 设置 Token
            localUserInfo.setToken(response);

            // 重新登录处理
            if (
                reloadLoginHandle(
                    response,
                    axiosDefaultConfig.reloadLoginCodes,
                    axiosDefaultConfig.reloadLoginMessage,
                    axiosDefaultConfig.reloadLoginButtonText,
                    () => {
                        localUserInfo.logout();
                    }
                )
            ) {
                return Promise.reject(false);
            }

            switch (response.config.responseType) {
                // 配置了blob，不处理直接返回文件流
                case "blob":
                    if (response.status === 200) {
                        // 判断是否自动下载
                        if (localConfig.axios.options.autoDownloadFile === true) {
                            // 下载文件
                            downloadFile(response);
                        }
                        return Promise.resolve(response);
                    } else {
                        ElMessage.error(axiosDefaultConfig.errorCodeMessages["fileDownloadError"]);
                        return Promise.reject(false);
                    }
                // 正常 JSON 格式响应处理
                case "json":
                    // 获取 Restful 风格返回数据
                    const data = response.data;
                    const code = data.code;
                    if (code < 200 || code > 299) {
                        // 判断是否显示错误消息
                        if (options.showCodeMessage) {
                            // 判断返回的 message 是否为对象类型
                            if (typeof data.message === "object" && data.message) {
                                ElMessage.error(JSON.stringify(data.message));
                            } else {
                                ElMessage.error(data.message);
                            }
                        }
                        return Promise.resolve(data);
                    }

                    // TODO:请求响应解密

                    // 简洁响应
                    return options.simpleDataFormat ? Promise.resolve(data) : Promise.resolve(response);
                default:
                    // 简洁响应
                    return options.simpleDataFormat ? Promise.resolve(response.data) : Promise.resolve(response);
            }
        },
        (error) => {
            // 删除重复请求标识
            error?.config && removePending(error?.config);
            // 关闭loading层
            options.loading && closeLoading(options);
            // 判断是否包含错误白名单路径
            if (axiosDefaultConfig.errorWhiteUrls.includes(error?.config?.url)) {
                return Promise.resolve(true);
            }
            // 重新登录处理
            if (
                reloadLoginHandle(
                    error?.response,
                    axiosDefaultConfig.reloadLoginCodes,
                    axiosDefaultConfig.reloadLoginMessage,
                    axiosDefaultConfig.reloadLoginButtonText,
                    () => {
                        localUserInfo.logout();
                    }
                )
            ) {
                return Promise.reject(false);
            }
            // 处理错误状态码
            options.showErrorMessage && httpErrorStatusHandle(error, axiosDefaultConfig.errorCodeMessages);
            // 错误继续返回给到具体页面
            return Promise.reject(error);
        }
    );

    return Axios(axiosConfig) as T;
}

/**
 * 默认导出
 */
export default createAxios;

/**
 * Http 错误状态码处理
 * @param error
 * @param errorCodeMessages
 */
function httpErrorStatusHandle(error: any, errorCodeMessages: anyObj) {
    // 判断请求是否被取消
    if (axios.isCancel(error)) {
        console.error(errorCodeMessages["cancelDuplicate"]);
        return;
    }
    let message = "";
    // 判断是否断网
    if (!window.navigator.onLine) {
        message = errorCodeMessages["offLine"];
    } else {
        // 其他错误码处理
        // 尝试获取 Restful 风格返回Code，或者获取响应状态码
        const code = error?.response?.data?.code || error?.response?.status || "default";
        message = errorCodeMessages[code];
    }
    ElMessage.error(message);
}

/**
 * 重新登录处理
 * @param response
 * @param reloadLoginCodes
 * @param reloadLoginMessage
 * @param reloadLoginButtonText
 * @param loginCallBackFunc
 */
function reloadLoginHandle(
    response: anyObj,
    reloadLoginCodes: Array<number>,
    reloadLoginMessage: string,
    reloadLoginButtonText: string,
    loginCallBackFunc: Function
): boolean {
    // 尝试获取 Restful 风格返回Code，或者获取响应状态码
    const code = response?.data?.code || response?.status;
    if (code && reloadLoginCodes.includes(code)) {
        if (!loginCallBack) {
            loginCallBack = true;
            ElMessageBox.alert(reloadLoginMessage, {
                type: "warning",
                showClose: false,
                confirmButtonText: reloadLoginButtonText,
                callback: () => {
                    loginCallBack = false;
                    loginCallBackFunc();
                },
            });
        }
        return true;
    }
    return false;
}

/**
 * 关闭Loading层实例
 */
function closeLoading(options: Options) {
    if (options.loading && loadingInstance.count > 0) loadingInstance.count--;
    if (loadingInstance.count === 0) {
        loadingInstance.target.close();
        loadingInstance.target = null;
    }
}

/**
 * 储存每个请求的唯一cancel回调, 以此为标识
 */
function addPending(axiosConfig: AxiosRequestConfig) {
    const pendingKey = getPendingKey(axiosConfig);
    axiosConfig.cancelToken =
        axiosConfig.cancelToken ||
        new axios.CancelToken((cancel) => {
            if (!pendingMap.has(pendingKey)) {
                pendingMap.set(pendingKey, cancel);
            }
        });
}

/**
 * 删除重复的请求
 */
function removePending(axiosConfig: AxiosRequestConfig) {
    const pendingKey = getPendingKey(axiosConfig);
    if (pendingMap.has(pendingKey)) {
        const cancelToken = pendingMap.get(pendingKey);
        cancelToken(pendingKey);
        pendingMap.delete(pendingKey);
    }
}

/**
 * 生成每个请求的唯一key
 */
function getPendingKey(axiosConfig: AxiosRequestConfig) {
    let { data } = axiosConfig;
    const { url, method, params, headers } = axiosConfig;
    // response里面返回的config.data是个字符串对象
    if (typeof data === "string") data = JSON.parse(data);
    return [
        url,
        method,
        headers && (headers as anyObj)["Authorization"] ? (headers as anyObj)["Authorization"] : "",
        headers && (headers as anyObj)["X-Authorization"] ? (headers as anyObj)["X-Authorization"] : "",
        JSON.stringify(params),
        JSON.stringify(data),
    ].join("&");
}
