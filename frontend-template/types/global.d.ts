/**
 * 拓展 Window 对象
 * @interface Window
 */
interface Window {
    /**
     * 是否存在 Loading
     * @type {boolean}
     * @memberof Window
     */
    existLoading: boolean;
    /**
     * 加载语言包句柄
     * @type {Record<string, any>}
     * @memberof Window
     */
    loadLangHandle: Record<string, any>;
    /**
     * 懒加载时间
     * @type {number}
     * @memberof Window
     */
    lazy: number;
}

/**
 * 匿名对象
 * @interface anyObj
 */
interface anyObj {
    [key: string]: any;
}

/**
 * Api 响应对象
 * @interface ApiResponse
 */
interface ApiResponse<T = any> {
    /**
     * 执行成功
     * @type {boolean}
     * @memberof ApiResponse
     */
    success: boolean;
    /**
     * 状态码
     * @type {number}
     * @memberof ApiResponse
     */
    code: number;
    /**
     * 错误信息
     * @type {string}
     * @memberof ApiResponse
     */
    message: string;
    /**
     * 数据
     * @type {T}
     * @memberof ApiResponse
     */
    data: T;
    /**
     * 时间戳
     * @type {number}
     * @memberof ApiResponse
     */
    timestamp: number;
}

/**
 * Api Promise
 */
type ApiPromise<T = any> = Promise<ApiResponse<T>>;

/**
 * 分页返回结果
 * @interface PageResult
 */
interface PageResult<T = any> {
    /**
     * 当前页
     * @type {number}
     * @memberof PageResult
     */
    pageIndex: number;
    /**
     * 当前页码
     * @type {number}
     * @memberof PageResult
     */
    pageSize: number;
    /**
     * 总页数
     * @type {number}
     * @memberof PageResult
     */
    totalPage?: number;
    /**
     * 总条数
     * @type {number}
     * @memberof PageResult
     */
    totalRows: number;
    /**
     * Data
     * @type {Array<T>}
     * @memberof PageResult
     */
    rows?: Array<T> | null;
    /**
     * 是否有上一页
     * @type {boolean}
     * @memberof PageResult
     */
    hasPrevPages?: boolean;
    /**
     * 是否有下一页
     * @type {boolean}
     * @memberof PageResult
     */
    hasNextPages?: boolean;
}

/**
 * 通用搜索输入
 * @interface PageInput
 */
interface PageInput {
    /**
     * 当前页面索引值，默认为1
     * @type {number}
     * @memberof PageInput
     */
    pageIndex?: number;
    /**
     * 页码容量
     * @type {number}
     * @memberof PageInput
     */
    pageSize?: number;
    /**
     * 搜索值
     * @type {string}
     * @memberof PageInput
     */
    searchValue?: string;
    /**
     * 搜索时间
     * @type {Array<Date>}
     * @memberof PageInput
     */
    searchTimeList?: Array<Date>;
}
