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

// 处理  类型“AxiosResponse<any, any>”上不存在属性“errorinfo”。ts(2339) 脑壳疼！关键一步。
declare module "axios" {
	interface AxiosResponse<T = any> {
		/**
		 * Code 状态码
		 */
		code: number;
		/**
		 * 是否成功
		 */
		success: boolean;
		/**
		 * 数据
		 */
		data: T;
		/**
		 * 消息
		 */
		message: string;
		/**
		 * 附加数据
		 */
		extras: any;
		/**
		 * 时间戳
		 */
		timestamp: number;
	}
	export function create(config?: AxiosRequestConfig): AxiosInstance;
}

// 以下这些code需要重新登录
const reloadCodes = [401];
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
// 定义一个重新登录弹出窗的变量
const loginBack = ref(false);
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
		}
		if (!sysConfig.REQUEST_CACHE && config.method === "get") {
			config.params = config.params || {};
			config.params._ = timestamp;
		}
		// Request Body，Data AES加密
		if (config.data) {
			console.debug(`HTTP Request Param("${config.url}")`, config.data);
			let dataStr = JSON.stringify(config.data);
			let decryptData = AESEncrypt(
				dataStr,
				`Fast.NET.XnRestful.${timestamp}`,
				`FIV${timestamp}`
			);
			// 组装请求格式
			config.data = {
				data: decryptData,
				timestamp: timestamp,
			};
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
			const responseUrl = response.config.url;
			const apiNameArray = [
				"add",
				"edit",
				"delete",
				"update",
				"grant",
				"reset",
				"start",
				"stop",
				"pass",
				"disable",
				"enable",
				"revoke",
				"suspend",
				"active",
				"turn",
				"adjust",
				"reject",
			];
			apiNameArray.forEach((apiName) => {
				if (responseUrl.includes(apiName)) {
					message.success(data.msg);
				}
			});
		}
		// 处理AES加密
		data.data = AESDecrypt(
			data.data,
			`Fast.NET.XnRestful.${data.timestamp}`,
			`FIV${data.timestamp}`
		);
		console.debug(`HTTP Result Data("${response.config.url}")`, data);
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
 * Get 请求
 * @param url
 * @param value
 * @param options
 * @returns
 */
export const get = (url: string, value: any = {}, options: any = {}) =>
	baseRequest(url, value, "get", options);

/**
 * Post 请求
 * @param url
 * @param value
 * @param options
 * @returns
 */
export const post = (url: string, value: any = {}, options: any = {}) =>
	baseRequest(url, value, "post", options);

/**
 * 基础请求
 * @param url
 * @param value
 * @param method get post put delete
 * @param options
 * @returns
 */
export const baseRequest = (
	url: string,
	value: any = {},
	method: string,
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

export default service;
