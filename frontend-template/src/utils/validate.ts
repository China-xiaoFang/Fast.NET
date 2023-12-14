/**
 * 验证方法
 */

/**
 * 验证是否为外部链接
 * @description 判断是否为外部链接，支持http、https、mailto和tel协议
 * @param arg 参数，表示待验证的链接
 * @returns 返回一个布尔值，指定参数是否为外部链接
 */
export const isExternal = (arg: string): boolean => {
    // 使用正则表达式匹配http、https、mailto、tel和ftp开头的链接
    const reg = /^(https?:|mailto:|tel:|ftp:)/;
    return reg.test(arg);
};

/**
 * 验证是否为有效的URL
 * @description 判断是否为有效的URL，支持http和https协议
 * @param arg 参数，表示待验证的链接
 * @returns 返回一个布尔值，指定参数是否为有效的URL
 */
export const isValidURL = (arg: string): boolean => {
    // 使用正则表达式验证是否为有效的URL
    const reg =
        /^(https?|ftp):\/\/([a-zA-Z0-9.-]+(:[a-zA-Z0-9.&%$-]+)*@)*((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9][0-9]?)(\.(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])){3}|([a-zA-Z0-9-]+\.)*[a-zA-Z0-9-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{2}))(:[0-9]+)*(\/($|[a-zA-Z0-9.,?'\\+&%$#=~_-]+))*$/;
    return reg.test(arg);
};

/**
 * 是否为手机设备
 */
export const isMobile = () => {
    return !!navigator.userAgent.match(
        /android|webos|ip(hone|ad|od)|opera (mini|mobi|tablet)|iemobile|windows.+(phone|touch)|mobile|fennec|kindle (Fire)|Silk|maemo|blackberry|playbook|bb10\; (touch|kbd)|Symbian(OS)|Ubuntu Touch/i
    );
};
