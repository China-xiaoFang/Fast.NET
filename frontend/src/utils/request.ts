/**
 * 客户端请求Axios处理
 * 所有的请求统一走此处
 */

import axios from "axios";
import { Modal, message, notification } from "ant-design-vue";
import sysConfig from "@/config/index";
import tool from "@/utils/tool";

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
	400: "发出的请求有错误，服务器没有进行新建或修改数据的操作。",
	401: "用户没有权限（令牌、用户名、密码错误）。",
	403: "用户得到授权，但是访问是被禁止的。",
	404: "The requested resource does not exist（请求的资源不存在）",
	406: "请求的格式不可得。",
	410: "请求的资源被永久删除，且不会再得到的。",
	422: "当创建一个对象时，发生一个验证错误。",
	429: "The request is too frequent, please try again later（请求过于频繁，请稍后再试）",
	500: "Internal Server Error（服务器内部错误）",
	502: "网关错误。",
	503: "Service unavailable, server temporarily overloaded or maintained（服务不可用，服务器暂时过载或维护）",
	504: "网关超时。",
	default: "Request timeout（请求超时）",
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
		const token = tool.cache.get("TOKEN");
		if (token) {
			config.headers[sysConfig.TOKEN_NAME] =
				sysConfig.TOKEN_PREFIX + token;
		}
		if (!sysConfig.REQUEST_CACHE && config.method === "get") {
			config.params = config.params || {};
			config.params._ = new Date().getTime();
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
		title: "提示：",
		okText: "重新登录",
		content: "登录已失效， 请重新登录",
		onOk: () => {
			loginBack.value = false;
			tool.cache.remove("TOKEN");
			tool.cache.remove("USER_INFO");
			tool.cache.remove("MENU");
			tool.cache.remove("PERMISSIONS");
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
				message.warning("文件下载失败或此文件不存在");
				return;
			}
		}
		const data = response.data;
		const code = data.code;
		if (reloadCodes.includes(code)) {
			if (!loginBack.value) {
				error();
			}
			return;
		}
		if (code !== 200) {
			message.error(data.message);
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
		return Promise.resolve(data);
	},
	(error) => {
		if (error) {
			const res = error?.response || null || null;
			const status = res?.status ?? "default";
			const description = errorCodeMap[status];
			notification.error({
				message: "请求错误",
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
	url = sysConfig.API_URL + url;
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
