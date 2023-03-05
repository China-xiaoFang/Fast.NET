// 这里的引用虽然看起来没用到，但是确实不能删除
import { AxiosRequestConfig, AxiosInstance } from "axios";

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
