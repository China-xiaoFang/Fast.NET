/**
 * Props 属性
 * @interface Props
 */
export interface Props {
    /**
     * 图片源地址，同原生属性一致
     */
    src?: string;
    /**
     * 懒加载
     */
    lazy?: boolean;
    /**
     * 原图
     */
    original?: boolean;
    /**
     * 标准
     */
    normal?: boolean;
    /**
     * 小图
     */
    small?: boolean;
    /**
     * 缩略图
     */
    thumb?: boolean;
    /**
     * 是否可以预览图片
     */
    preview?: boolean;
}
