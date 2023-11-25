/**
 * 验证方法
 */

/**
 * 类型验证
 * @description 判断是否为数组
 * @param arg 参数
 * @returns 返回一个布尔值，指定参数是否为数组
 */
export const isArray = (arg: any): boolean => {
    // 判断环境是否支持 Array.isArray 方法
    if (typeof Array.isArray === "undefined") {
        // 使用对象原型的 toString 方法进行类型检查
        return Object.prototype.toString.call(arg) === "[object Array]";
    }

    // 直接使用 Array.isArray 方法进行类型检查
    return Array.isArray(arg);
}
