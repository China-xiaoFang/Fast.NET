/**
 * 客户端请求Axios处理
 * 所有的请求统一走此处
 */

import axios from "axios";
import { Modal, message, notification } from "ant-design-vue";
import sysConfig from "@/config/index";
import tool from "@/utils/tool";
import cacheKey from "@/config/cacheKey";
import store from "@/store";
import { translate as $t } from "@/locales";
import { AESDecrypt, AESEncrypt } from "./AES";

// 定义一个重新登录弹出窗的变量
const loginBack = ref(false);

// 以下这些code需要重新登录
const reloadCodes = [401];

// 错误码处理集合
const errorCodeMap = {
	400: $t("requestError.400"),
	401: $t("requestError.401"),
	403: $t("requestError.403"),
	404: $t("requestError.404"),
	406: $t("requestError.406"),
	410: $t("requestError.410"),
	422: $t("requestError.422"),
	429: $t("requestError.429"),
	500: $t("requestError.500"),
	502: $t("requestError.502"),
	503: $t("requestError.503"),
	504: $t("requestError.504"),
	default: $t("requestError.default"),
};

// 统一成功提示
const hintActionNameMap = [
	"Add",
	"Update",
	"Delete",
	"Download",
	"Upload",
	"Export",
	"Import",
];

// 创建 axios 实例
const service = axios.create({
	baseURL: "/api", // api base_url
	timeout: sysConfig.TIMEOUT, // 请求超时时间
});

// HTTP request 拦截器
service.interceptors.request.use(
	(config) => {
		const token = tool.cache.get(cacheKey.TOKEN);
		const timestamp = new Date().getTime();
		if (token) {
			config.headers[sysConfig.TOKEN_NAME] =
				sysConfig.TOKEN_PREFIX + token;
			const tokenJson = decodeURIComponent(
				escape(
					window.atob(
						token
							.replace(/_/g, "/")
							.replace(/-/g, "+")
							.split(".")[1]
					)
				)
			);
			const jwtToken = JSON.parse(tokenJson);
			const exp = new Date(jwtToken.exp * 1000);
			if (new Date() >= exp) {
				const refreshToken = tool.cache.get(cacheKey.REFRESH_TOKEN);
				if (refreshToken) {
					config.headers[sysConfig.REFRESH_TOKEN_NAME] =
						sysConfig.TOKEN_PREFIX + refreshToken;
				}
			}
		}
		// Request Data AES加密
		if (process.env.REQUEST_ENCRYPT == "true") {
			let requestData = config.params || config.data;
			let dataStr = JSON.stringify(requestData);
			if (dataStr != null && dataStr != "" && dataStr != "{}") {
				console.debug(
					`HTTP Request Param("${config.url}")`,
					requestData
				);
				let decryptData = AESEncrypt(
					dataStr,
					`Fast.NET.XnRestful.${timestamp}`,
					`FIV${timestamp}`
				);
				// 组装请求格式
				requestData = {
					data: decryptData,
					timestamp: timestamp,
				};
				if (config.method === "get" || config.method === "delete") {
					config.params = requestData;
				} else {
					config.data = requestData;
				}
			}
		}
		// Get 请求缓存
		if (!sysConfig.REQUEST_CACHE && config.method === "get") {
			config.params = config.params || {};
			config.params._ = timestamp;
		}
		// 带上租户Id
		const tenantId = store.state["webSiteInfo"]?.base64TenantId;
		if (tenantId) {
			config.headers[sysConfig.TENANT_ID_NAME] = tenantId;
		}
		// 带上来源
		config.headers[sysConfig.ORIGIN_NAME] = window.location.origin;
		// 带上UUID
		config.headers[sysConfig.UUID] = tool.getUUID();
		Object.assign(config.headers, sysConfig.HEADERS);
		return config;
	},
	(error) => {
		return Promise.reject(error);
	}
);

// 保持重新登录Modal的唯一性
const error = () => {
	loginBack.value = true;
	Modal.error({
		title: $t("message.title"),
		okText: $t("login.reLogin"),
		content: $t("login.loginInvalidation"),
		onOk: () => {
			loginBack.value = false;
			tool.cache.remove(cacheKey.TOKEN);
			tool.cache.remove(cacheKey.USER_INFO);
			tool.cache.remove(cacheKey.MENU);
			tool.cache.remove(cacheKey.PERMISSIONS);
			window.location.reload();
		},
	});
};

// HTTP response 拦截器
service.interceptors.response.use(
	(response) => {
		// 获取Token
		let token = response.headers["fast-net-access-token"];
		let refreshToken = response.headers["fast-net-x-access-token"];
		// 判断是否为无效Token
		if (token === "invalid_token") {
			// 删除Token
			tool.cache.remove(cacheKey.TOKEN);
			tool.cache.remove(cacheKey.REFRESH_TOKEN);
		} else if (token && refreshToken && refreshToken !== "invalid_token") {
			// 缓存Token
			tool.cache.set(cacheKey.TOKEN, token);
			tool.cache.set(cacheKey.REFRESH_TOKEN, refreshToken);
		}
		// 配置了blob，不处理直接返回文件流
		if (response.config.responseType === "blob") {
			if (response.status === 200) {
				return response;
			} else {
				message.warning($t("requestError.fileError"));
				return;
			}
		}
		let data = response.data;
		const code = data.code;
		if (reloadCodes.includes(code)) {
			if (!loginBack.value) {
				error();
			}
			return;
		}
		// 200 为有返回值，204 为无返回值
		if (code !== 200 && code !== 204) {
			if (typeof data.message == "object" && data.message) {
				message.error(JSON.stringify(data.message));
			} else {
				message.error(data.message);
			}
			return Promise.reject(data);
		} else {
			// 统一成功提示
			const actionName = response.headers["api-action-name"];
			if (hintActionNameMap.includes(actionName)) {
				message.success($t(`requestSuccess.${actionName}`));
			}
		}
		// 处理AES加密
		if (process.env.REQUEST_ENCRYPT == "true") {
			data.data = AESDecrypt(
				data.data,
				`Fast.NET.XnRestful.${data.timestamp}`,
				`FIV${data.timestamp}`
			);
			console.debug(`HTTP Result Data("${response.config.url}")`, data);
		}
		return Promise.resolve(data);
	},
	(error) => {
		if (error) {
			const res = error?.response || null || null;
			const status = res?.status ?? "default";
			const description = errorCodeMap[status];
			notification.error({
				message: $t("requestError.requestError"),
				description,
			});
			return Promise.reject(res?.data);
		}
	}
);

/**
 * 基础请求
 * @param url
 * @param value
 * @param method get post put delete
 * @param options
 * @returns
 */
const baseRequest = (
	method: string,
	url: string,
	value: any = {},
	options: any = {}
) => {
	if (method === "post") {
		return service.post(url, value, options);
	} else if (method === "get") {
		return service.get(url, {
			params: value,
			...options,
		});
	} else if (method === "put") {
		return service.put(url, value, options);
	} else if (method === "delete") {
		return service.delete(url, {
			params: value,
			...options,
		});
	} else if (method === "formdata") {
		return service({
			method: "post",
			url,
			data: value,
			// 转换数据的方法
			transformRequest: [
				function (data) {
					let ret = "";
					for (const it in data) {
						ret += `${encodeURIComponent(it)}=${encodeURIComponent(
							data[it]
						)}&`;
					}
					ret = ret.substring(0, ret.length - 1);
					return ret;
				},
			],
			// 设置请求头
			headers: {
				"Content-Type": "multipart/form-data",
			},
		});
	}
};

/**
 * Get 请求
 * @param url
 * @param value
 * @param options
 * @returns
 */
export const getRequest = (url: string, value: any = {}, options: any = {}) =>
	baseRequest("get", url, value, options);

/**
 * Post 请求
 * @param url
 * @param value
 * @param options
 * @returns
 */
export const postRequest = (url: string, value: any = {}, options: any = {}) =>
	baseRequest("post", url, value, options);

/**
 * Put 请求
 * @param url
 * @param value
 * @param options
 * @returns
 */
export const putRequest = (url: string, value: any = {}, options: any = {}) =>
	baseRequest("put", url, value, options);

/**
 * Delete 请求
 * @param url
 * @param value
 * @param options
 * @returns
 */
export const deleteRequest = (
	url: string,
	value: any = {},
	options: any = {}
) => baseRequest("delete", url, value, options);

export default service;
