/**加密字典 */
const base64PwdDic = [
    { index: 977, randomIndex: 188 },
    { index: 926, randomIndex: 201 },
    { index: 851, randomIndex: 225 },
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
const insertRandomStrToBase64Str = (base64Str: string) => {
    var strResult = base64Str;
    var items = base64PwdDic.sort(function (a, b) {
        return b.index - a.index;
    });
    items.forEach((item) => {
        if (item.index < base64Str.length) {
            var randomChar = base64Str[item.randomIndex];
            strResult = strResult.slice(0, item.index) + randomChar + strResult.slice(item.index);
        }
    });
    return strResult;
};

/**
 * 删除Base64字符串中的加密字典
 */
const removeBase64StrRandomStr = (base64Str: string) => {
    var items = base64PwdDic.sort(function (a, b) {
        return a.index - b.index;
    });
    var strResult = base64Str;
    items.forEach((item) => {
        if (item.index < base64Str.length) {
            strResult = strResult.slice(0, item.index) + strResult.slice(item.index + 1);
        }
    });
    return strResult;
};

/**
 * 得到随机字符串
 */
const getRandomStr = (str = randomStr, prefixStrLength = randomPrefixStrLength) => {
    var result = "";
    for (let i = 0; i < prefixStrLength; i++) {
        var randomInt = Math.ceil(Math.random() * (str.length - 1));
        var randomChar = str[randomInt];
        result += randomChar;
    }
    return result;
};

export default {
    /**
     * 字符串ToBase64
     */
    toBase64(str: string, prefixStrLength = randomPrefixStrLength): string {
        if (str.length === 0) {
            return "";
        }
        var randomPrefixStr = getRandomStr();
        var base64 = btoa(str);
        if (prefixStrLength !== 0) {
            base64 = insertRandomStrToBase64Str(base64);
        }
        return randomPrefixStr + base64;
    },

    /**
     * Base64转字符串
     */
    base64ToStr(str: string, prefixStrLength = randomPrefixStrLength): string {
        var result = str;
        if (str.length === 0) {
            return "";
        }
        var input = str.slice(prefixStrLength);
        if (prefixStrLength !== 0) {
            input = removeBase64StrRandomStr(input);
        }
        result = atob(input);
        return result;
    },
};
