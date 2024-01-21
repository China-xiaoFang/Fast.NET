/**
 * 声明 vite 环境变量的类型（如果未声明则默认是 any）
 * @interface ImportMetaEnv
 */
declare interface ImportMetaEnv {
    /**
     * 运行 npm run dev 时绑定的端口号
     * @type {number}
     * @memberof ImportMetaEnv
     */
    readonly VITE_PORT: number;
    /**
     * 运行 npm run dev 时自动打开浏览器
     * 这里使用 boolean 会直接当作true处理，尽管是 false
     * @type {boolean}
     * @memberof ImportMetaEnv
     */
    readonly VITE_OPEN: string;
    /**
     * 打包路径
     * @type {string}
     * @memberof ImportMetaEnv
     */
    readonly VITE_BASE_PATH: string;
    /**
     * 打包输出路径
     * @type {string}
     * @memberof ImportMetaEnv
     */
    readonly VITE_OUT_DIR: string;
    /**
     * App 标题
     * @type {string}
     * @memberof ImportMetaEnv
     */
    readonly VITE_APP_TITLE: string;
    /**
     * 接口请求路径
     * @type {string}
     * @memberof ImportMetaEnv
     */
    readonly VITE_AXIOS_BASE_URL: string;
    /**
     * 请求超时时间
     * @type {number}
     * @memberof ImportMetaEnv
     */
    readonly VITE_AXIOS_API_TIMEOUT: number;
    /**
     * 接口请求代理地址
     * - 尾部无需带 ’/‘
     * @type {string}
     * @memberof ImportMetaEnv
     */
    readonly VITE_AXIOS_PROXY_URL: string;
}
