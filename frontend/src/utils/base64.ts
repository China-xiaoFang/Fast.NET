import { Base64 } from "js-base64";

/**
 * Base64图片转文件
 * @param {*} url Base64字符串
 * @param {*} fileName 文件名称
 * @returns 返回File
 */
export const base64ImgToFile = (url, fileName) => {
	const arr = url.split(",");
	const mime = arr[0].match(/:(.*?);/)[1];
	const bstr = atob(arr[1]);
	let n = bstr.length;
	const u8arr = new Uint8Array(n);
	while (n--) {
		u8arr[n] = bstr.charCodeAt(n);
	}
	return new window.File([u8arr], fileName, {
		type: mime,
	});
};

//#region Base64 加密解密

/**加密字典 */
const base64PwdDic = [
	{ index: 950, randomIndex: 188 },
	{ index: 900, randomIndex: 201 },
	{ index: 800, randomIndex: 225 },
	{ index: 700, randomIndex: 255 },
	{ index: 600, randomIndex: 268 },
	{ index: 500, randomIndex: 277 },
	{ index: 400, randomIndex: 288 },
	{ index: 330, randomIndex: 327 },
	{ index: 300, randomIndex: 180 },
	{ index: 200, randomIndex: 178 },
	{ index: 100, randomIndex: 124 },
	// 100 以内字典
	{ index: 98, randomIndex: 95 },
	{ index: 92, randomIndex: 90 },
	{ index: 91, randomIndex: 87 },
	{ index: 88, randomIndex: 84 },
	{ index: 82, randomIndex: 79 },
	{ index: 78, randomIndex: 71 },
	{ index: 72, randomIndex: 69 },
	{ index: 68, randomIndex: 66 },
	{ index: 59, randomIndex: 55 },
	{ index: 48, randomIndex: 43 },
	{ index: 42, randomIndex: 37 },
	{ index: 36, randomIndex: 30 },
	{ index: 33, randomIndex: 27 },
	{ index: 24, randomIndex: 20 },
	{ index: 23, randomIndex: 18 },
	{ index: 21, randomIndex: 16 },
	{ index: 17, randomIndex: 14 },
	{ index: 13, randomIndex: 9 },
	{ index: 7, randomIndex: 4 },
	{ index: 5, randomIndex: 3 },
	{ index: 2, randomIndex: 1 },
];

/**随机字符串长度 */
const randomPrefixStrLength = 6;

/**随机字符串 */
const randomStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

/**
 * 在Base64字符串中添加加密字典
 */
function insertRandomStrToBase64Str(base64Str) {
	var strResult = base64Str;
	var items = base64PwdDic.sort(function (a, b) {
		return b.index - a.index;
	});
	items.forEach((item) => {
		if (item.index < base64Str.length) {
			var randomChar = base64Str[item.randomIndex];
			strResult =
				strResult.slice(0, item.index) +
				randomChar +
				strResult.slice(item.index);
		}
	});
	return strResult;
}

/**
 * 删除Base64字符串中的加密字典
 */
function removeBase64StrRandomStr(base64Str) {
	var items = base64PwdDic.sort(function (a, b) {
		return a.index - b.index;
	});
	var strResult = base64Str;
	items.forEach((item) => {
		if (item.index < base64Str.length) {
			strResult =
				strResult.slice(0, item.index) +
				strResult.slice(item.index + 1);
		}
	});
	return strResult;
}

/**
 * 得到随机字符串
 */
function getRandomStr(
	str = randomStr,
	prefixStrLength = randomPrefixStrLength
) {
	var result = "";
	for (let i = 0; i < prefixStrLength; i++) {
		var randomInt = Math.ceil(Math.random() * (str.length - 1));
		var randomChar = str[randomInt];
		result += randomChar;
	}
	return result;
}

/**
 * 字符串ToBase64
 */
export const toBase64 = (str, prefixStrLength = randomPrefixStrLength) => {
	if (str.length === 0) {
		return "";
	}
	var randomPrefixStr = getRandomStr();
	// var decode = decodeURI(str)
	var base64 = Base64.btoa(str);
	if (prefixStrLength !== 0) {
		base64 = insertRandomStrToBase64Str(base64);
	}
	return randomPrefixStr + base64;
};

/**
 * Base64转字符串
 */
export const base64ToStr = (str, prefixStrLength = randomPrefixStrLength) => {
	var result = str;
	if (str.length === 0) {
		return "";
	}
	var input = str.slice(prefixStrLength);
	if (prefixStrLength !== 0) {
		input = removeBase64StrRandomStr(input);
	}
	// var decode = polyfill.atob(input)
	// result = decodeURI(decode)
	result = Base64.atob(input);
	return result;
};

/**
 * 得到Base64根据File
 */
export const getBase64ByFile = (file) => {
	return new Promise((resolve, reject) => {
		const reader = new FileReader();
		reader.readAsDataURL(file);
		reader.onload = () => resolve(reader.result);
		reader.onerror = (error) => reject(error);
	});
};
