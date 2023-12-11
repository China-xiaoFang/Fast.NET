/**
 * Stores SiteConfig 变量定义
 */

/**
 * 用户信息
 * @interface SiteConfig
 */
export interface SiteConfig {
    /**
     * 标题
     * @type {string}
     * @memberof SiteConfig
     */
    title: string;
    /**
     * 版权有效开始年份
     * @type {number}
     * @memberof SiteConfig
     */
    copyrightValidStartYear: number;
    /**
     * 版权有效结束年份
     * @type {number}
     * @memberof SiteConfig
     */
    copyrightValidEndYear: number;
    /**
     * 版权所有
     * @type {string}
     * @memberof SiteConfig
     */
    copyrighted: string;
    /**
     * 版权所有相关链接
     * @type {string}
     * @memberof SiteConfig
     */
    copyrightedUrl: string;
    /**
     * ICP备案信息
     * @type {string}
     * @memberof SiteConfig
     */
    icpInfo: string;
    /**
     * 公安备案信息
     * @type {string}
     * @memberof SiteConfig
     */
    publicInfo: string;
    /**
     * 版本号
     * @type {string}
     * @memberof SiteConfig
     */
    version: string;
}
