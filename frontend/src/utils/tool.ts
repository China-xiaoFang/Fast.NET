/**
 * 工具集
 */

import { v4 as uuidV4 } from "uuid";

/**
 * 缓存前缀
 */
const cachePrefix: string = "fast.net_";

/**
 * 设置缓存
 * @param key 缓存Key
 * @param data Data
 */
function cacheSet(key: string, data: any): void {
	const _set = JSON.stringify(data);
	localStorage.setItem(`${cachePrefix}${key}`, _set);
}

/**
 * 获取缓存中的数据
 * @param key 缓存Key
 */
function cacheGet(key: string): any {
	let data = localStorage.getItem(`${cachePrefix}${key}`);
	try {
		data = JSON.parse(data);
	} catch (err) {
		return null;
	}
	return data;
}

/**
 * 删除缓存中的Key
 * @param key 缓存Key
 */
function cacheRemove(key: string): void {
	return localStorage.removeItem(`${cachePrefix}${key}`);
}

/**
 * 清除缓存
 * @param key 缓存Key
 */
function cacheClear(): void {
	return localStorage.clear();
}

/**
 * 设置session
 * @param key Key
 * @param data Data
 */
function sessionSet(key: string, data: any): void {
	const _set = JSON.stringify(data);
	sessionStorage.setItem(key, _set);
}

/**
 * 获取session中的数据
 * @param key Key
 */
function sessionGet(key: string): any {
	let data = sessionStorage.getItem(key);
	try {
		data = JSON.parse(data);
	} catch (err) {
		return null;
	}
	return data;
}

/**
 * 删除session中的Key
 * @param key Key
 */
function sessionRemove(key: string): void {
	sessionStorage.removeItem(key);
}

/**
 * 清楚session
 * @param key Key
 */
function sessionClear(): void {
	sessionStorage.clear();
}

/**
 * 生成UUID
 * @returns
 */
function fastUuid(): string {
	let uuid = "xxxxxxxx-xxxx-6xxx-fxxx-xxxxxxxxxxxx".replace(/[xy]/g, (c) => {
		let r = (Math.random() * 16) | 0,
			v = c === "x" ? r : (r & 0x3) | 0x8;
		return v.toString(16);
	});
	// 首字符转换成字母
	return "xn" + uuid.slice(2);
}

/**
 * 千分符
 * @param num
 * @returns
 */
function groupSeparator(num: any): any {
	num = `${num}`;
	if (!num.includes(".")) num += ".";

	return num
		.replace(/(\d)(?=(\d{3})+\.)/g, ($0, $1) => {
			return `${$1},`;
		})
		.replace(/\.$/, "");
}

let _debounceTimeout = null,
	_throttleRunning = false;

/**
 * 防抖
 * @param {Function} 执行函数
 * @param {Number} delay 延时ms
 */
export const debounce = (fn, delay = 500) => {
	clearTimeout(_debounceTimeout);
	_debounceTimeout = setTimeout(() => {
		fn();
	}, delay);
};

/**
 * 节流
 * @param {Function} 执行函数
 * @param {Number} delay 延时ms
 */
export const throttle = (fn, delay = 500) => {
	if (_throttleRunning) {
		return;
	}
	_throttleRunning = true;
	fn();
	setTimeout(() => {
		_throttleRunning = false;
	}, delay);
};

/**
 * 生成指定长度的字符串
 *
 * @param {Number} len 字符串长度
 * @param {Number} hex 代表进制，取值范围[2 - 36]，最大不能超过 36, 数字越大字母占比越高，小于11为全数字
 */
export function generateStr(len = 18, hex = 36) {
	if (hex < 2 || hex > 36) {
		throw new RangeError("hex argument must be between 2 and 36");
	}

	let res = Math.random().toString(hex).slice(2);
	let resLen = res.length;

	while (resLen < len) {
		res += Math.random().toString(hex).slice(2);
		resLen = res.length;
	}
	return res.substr(0, len);
}

/**
 * 生成UUID
 */
export function generateUUID(): string {
	return uuidV4();
}

/**
 * 获取UUID
 */
export function getUUID(): string {
	// 判断是否存在uuid
	let uuid = cacheGet("UUID");
	if (!uuid) {
		// 生成uuid
		uuid = generateUUID();
		// 放入缓存
		cacheSet("UUID", uuid);
	}
	return uuid;
}

const tool = {
	cache: {
		set: cacheSet,
		get: cacheGet,
		remove: cacheRemove,
		clear: cacheClear,
	},
	session: {
		set: sessionSet,
		get: sessionGet,
		remove: sessionRemove,
		clear: sessionClear,
	},
	groupSeparator: groupSeparator,
	fastUuid: fastUuid,
	debounce: debounce,
	throttle: throttle,
	generateStr: generateStr,
	generateUUID: generateUUID,
	getUUID: getUUID,
};

export default tool;
