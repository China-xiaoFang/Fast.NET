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

/**
 * 检测字符串是否为正确的Url地址
 * @param url
 * @returns
 */
export function checkUrl(url: string): boolean {
	//url= 协议://(ftp的登录信息)[IP|域名](:端口号)(/或?请求参数)
	var strRegex =
		"^((https|http|ftp)://)?" + //(https或http或ftp):// 可有可无
		"(([\\w_!~*'()\\.&=+$%-]+: )?[\\w_!~*'()\\.&=+$%-]+@)?" + //ftp的user@  可有可无
		"(([0-9]{1,3}\\.){3}[0-9]{1,3}" + // IP形式的URL- 3位数字.3位数字.3位数字.3位数字
		"|" + // 允许IP和DOMAIN（域名）
		"(localhost)|" + //匹配localhost
		"([\\w_!~*'()-]+\\.)*" + // 域名- 至少一个[英文或数字_!~*\'()-]加上.
		"\\w+\\." + // 一级域名 -英文或数字  加上.
		"[a-zA-Z]{1,6})" + // 顶级域名- 1-6位英文
		"(:[0-9]{1,5})?" + // 端口- :80 ,1-5位数字
		"((/?)|" + // url无参数结尾 - 斜杆或这没有
		"(/[\\w_!~*'()\\.;?:@&=+$,%#-]+)+/?)$"; //请求参数结尾- 英文或数字和[]内的各种字符

	var regex = new RegExp(strRegex, "i"); //i不区分大小写
	//将url做uri转码后再匹配，解除请求参数中的中文和空字符影响
	if (regex.test(encodeURI(url))) {
		return true;
	} else {
		return false;
	}
}

/**
 * 跳转Html页面
 * @param {string} url
 */
export function toHtmlPage(url: string) {
	// 判断是否为正确的Url地址
	if (checkUrl(url)) {
		window.location.replace(url);
	} else {
		window.location.replace(`${window.location.origin}${url}`);
	}
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
	checkUrl: checkUrl,
	toHtmlPage: toHtmlPage,
};

export default tool;
