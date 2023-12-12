/**
 * Stores NavTabs 变量定义
 */
import type { RouteRecordRaw, RouteLocationNormalized } from "vue-router";

/**
 * 用户信息
 * @interface NavTabs
 */
export interface NavTabs {
    /**
     * 激活tab的index
     * @type {number}
     * @memberof NavTabs
     */
    activeIndex: number;
    /**
     * 激活的tab
     * @type {Array<RouteLocationNormalized>}
     * @memberof NavTabs
     */
    activeRoute: RouteLocationNormalized[] | null;
    /**
     * tab列表
     * @type {Array<RouteLocationNormalized>}
     * @memberof NavTabs
     */
    tabsView: RouteLocationNormalized[];
    /**
     * 当前tab是否全屏
     * @type {boolean}
     * @memberof NavTabs
     */
    tabFullScreen: boolean;
    /**
     * 从后台加载到的菜单路由列表
     * @type {Array<RouteRecordRaw>}
     * @memberof NavTabs
     */
    tabsViewRoutes: RouteRecordRaw[];
    /**
     * 按钮权限节点
     * @type {Map<string, string[]>}
     * @memberof NavTabs
     */
    authNode: Map<string, string[]>;
}
