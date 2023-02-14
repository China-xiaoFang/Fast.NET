import cryptoJS from "crypto-js";

/**
 * AES解密
 * @param dataStr
 * @param key
 * @returns
 */
export const AESDecrypt = (
	dataStr: string,
	key: string,
	vector: string
): any => {
	if (dataStr == null || dataStr == "") {
		return dataStr;
	}
	let resAESData = cryptoJS.AES.decrypt(
		dataStr,
		cryptoJS.enc.Utf8.parse(key),
		{ iv: cryptoJS.enc.Utf8.parse(vector), mode: cryptoJS.mode.CBC }
	);
	try {
		let result = resAESData.toString(cryptoJS.enc.Utf8);

		var obj = JSON.parse(result);
		if (typeof obj == "object" && obj) {
			return obj;
		} else {
			return result;
		}
	} catch (e) {
		return null;
	}
};

/**
 * AES加密
 * @param dataStr
 * @param key
 * @returns
 */
export const AESEncrypt = (
	dataStr: string,
	key: string,
	vector: string
): string => {
	if (dataStr == null || dataStr == "") {
		return dataStr;
	}
	return cryptoJS.AES.encrypt(dataStr, cryptoJS.enc.Utf8.parse(key), {
		iv: cryptoJS.enc.Utf8.parse(vector),
		mode: cryptoJS.mode.CBC,
	}).toString();
};
