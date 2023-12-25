/**
 * Stores SiteConfig 变量定义
 */

/**
 * 站点配置
 * @interface SiteConfig
 */
export interface SiteConfig {
    /**
     * 站点名称
     * @type {string}
     * @memberof SiteConfig
     */
    siteName?: string;
    /**
     * 版权有效开始年份
     * @type {number}
     * @memberof SiteConfig
     */
    copyrightValidStartYear?: number;
    /**
     * 版权有效结束年份
     * @type {number}
     * @memberof SiteConfig
     */
    copyrightValidEndYear?: number;
    /**
     * 版权所有
     * @type {string}
     * @memberof SiteConfig
     */
    copyrighted?: string;
    /**
     * 版权所有相关链接
     * @type {string}
     * @memberof SiteConfig
     */
    copyrightedUrl?: string;
    /**
     * 版本号
     * @type {string}
     * @memberof SiteConfig
     */
    version?: string;
    /**
     * ICP备案信息
     * @type {string}
     * @memberof SiteConfig
     */
    icpInfo?: string;
    /**
     * 公安备案省份
     * @type {string}
     * @memberof SiteConfig
     */
    publicProvince?: string;
    /**
     * 公安备案号
     * @type {string}
     * @memberof SiteConfig
     */
    publicCode?: string;
    /**
     * 站点资源 cdn 加速地址
     * @type {string}
     * @memberof SiteConfig
     */
    cdnUrl?: string;
}
