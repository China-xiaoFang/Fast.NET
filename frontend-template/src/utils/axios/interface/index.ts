/**
 * Axios 变量定义
 */

/**
 * Axios 加载实例
 * @interface LoadingInstance
 */
export interface LoadingInstance {
    /**
     * ElLoading 的实例信息
     * @type {any}
     * @memberof LoadingInstance
     */
    target: any;
    /**
     * 总数
     * @type {number}
     * @memberof LoadingInstance
     */
    count: number;
}

/**
 * Axios 选项
 * @interface Options
 */
export interface Options {
    /**
     * 是否开启取消重复请求, 默认为 true
     * @type {boolean}
     * @memberof Options
     */
    cancelDuplicateRequest?: boolean;
    /**
     * 是否开启loading层效果, 默认为 true
     * @type {boolean}
     * @memberof Options
     */
    loading?: boolean;
    /**
     * Get请求缓存问题处理，默认为 true
     * @type {boolean}
     * @memberof Options
     */
    getMethodCacheHandle?: boolean;
    /**
     * 是否开启简洁的数据结构响应, 默认为 true
     * - 开启，返回类型为 "ApiPromise"
     * - 关闭，返回类型为 "AxiosPromise"
     * @type {boolean}
     * @memberof Options
     */
    simpleDataFormat?: boolean;
    /**
     * 是否开启接口错误信息展示, 默认为 true
     * @type {boolean}
     * @memberof Options
     */
    showErrorMessage?: boolean;
    /**
     * 是否开启code信息提示, 默认为 true
     * - code >= 200 && code <= 299 则不提示
     * @type {boolean}
     * @memberof Options
     */
    showCodeMessage?: boolean;
    /**
     * 是否开启请求成功的信息提示, 默认为 false
     * - 只有 code >= 200 && code <= 299 才提示
     * @type {boolean}
     * @memberof Options
     */
    showSuccessMessage?: boolean;
    /**
     * 是否开启自动下载文件, 默认为 true
     * - 只有 responseType 配置了 "blob" 才会自动下载
     * @type {boolean}
     * @memberof Options
     */
    autoDownloadFile?: boolean;
}
