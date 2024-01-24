/**
 * Props 属性
 * @interface Props
 */
export interface Props {
    /**
     * 名称
     * @type {string}
     * @description el-icon- 使用 El-icon 的图标；local- 使用本地 svg 图标
     * @requires 必填
     * @interface Props
     */
    name?: string;
    /**
     * 大小
     * @type {string}
     * @default "18px"
     * @description 需要携带 px 单位
     * @interface Props
     */
    size?: string;
    /**
     * 颜色
     * @type {string}
     * @default "#000000"
     * @interface Props
     */
    color?: string;
}
