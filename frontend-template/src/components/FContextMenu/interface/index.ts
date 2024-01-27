import type { RouteLocationNormalized } from "vue-router";

/**
 * 轴位置
 * @interface Axis
 */
export interface Axis {
    /**
     * X 轴位置
     * @type {number}
     * @requires 必填
     * @interface Axis
     */
    x: number;
    /**
     * Y 轴位置
     * @type {number}
     * @requires 必填
     * @interface Axis
     */
    y: number;
}

/**
 * 上下文菜单项
 * @interface ContextMenuItem
 */
export interface ContextMenuItem {
    /**
     * 名称
     * @type {string}
     * @requires 必填
     * @interface ContextMenuItem
     */
    name: string;
    /**
     * 标签
     * @type {string}
     * @requires 必填
     * @interface ContextMenuItem
     */
    label: string;
    /**
     * 图标
     * @type {string}
     * @interface ContextMenuItem
     */
    icon?: string;
    /**
     * 是否禁用
     * @type {boolean}
     * @interface ContextMenuItem
     */
    disabled?: boolean;
}

/**
 * 上下文菜单项点击 Emit参数
 * @interface ContextMenuItemClickEmitArg
 */
export interface ContextMenuItemClickEmitArg extends ContextMenuItem {
    /**
     * 菜单/路由信息
     * @type {RouteLocationNormalized}
     * @interface ContextMenuItemClickEmitArg
     */
    menu?: RouteLocationNormalized;
}

/**
 * Props 属性
 * @interface Props
 */
export interface Props {
    /**
     * 宽度
     * @type {number}
     * @default 默认 150
     * @interface Props
     */
    width?: number;
    /**
     * 菜单项
     * @type {ContextMenuItem[]}
     * @requires 必填
     * @interface Props
     */
    items: ContextMenuItem[];
}

/**
 * Emits 属性
 * @interface Emits
 */
export interface Emits {
    /**
     * 上下文菜单点击
     * @param item 点击项
     * @interface Emits
     */
    onClick: (item: ContextMenuItemClickEmitArg) => null;
}
