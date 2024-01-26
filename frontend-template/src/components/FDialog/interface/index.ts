/**
 * Props 属性
 * @interface Props
 */
export interface Props {
    /**
     * 宽度
     */
    width?: string;
    /**
     * 高度
     */
    height?: string;
    /**
     * 显示全屏图标
     */
    showFullscreen?: boolean;
    /**
     * 显示关闭图标
     */
    showClose?: boolean;
    /**
     * 显示关闭按钮
     */
    showCloseButton?: boolean;
    /**
     * 显示确认按钮
     */
    showConfirmButton?: boolean;
    /**
     * 禁用确认按钮
     */
    disabledConfirmButton?: boolean;
    /**
     * 关闭按钮文字，默认取消
     */
    closeButtonText?: string;
    /**
     * 确认按钮文字，默认确认
     */
    confirmButtonText?: string;
    /**
     * 显示关闭回调
     */
    showBeforeClose?: boolean;
    /**
     * 显示底部操作
     */
    showFooterOperator?: boolean;
    /**
     * 全屏显示
     */
    fullscreen?: boolean;
    /**
     * Dialog 自身是否插入至 body 元素上。 嵌套的 Dialog 必须指定该属性并赋值为 true
     */
    appendToBody?: boolean;
    /**
     * 屏幕滚动
     */
    scrollbar?: boolean;
    /**
     * 撑满高度
     */
    fillHeight?: boolean;
    /**
     * 关闭回调
     * @returns
     */
    closeCallBack?: () => boolean;
}

/**
 * Emits 属性
 * @interface Emits
 */
export interface Emits {
    /**
     * 确认按钮点击事件
     */
    onConfirmClick: (state: State) => void;
    /**
     * 打开回调
     */
    onOpen: () => void;
    /**
     * 关闭回调
     */
    onClose: () => boolean;
}

/**
 * State 属性
 * @interface State
 */
export interface State {
    /**
     * 表格加载
     */
    loading: boolean;
    /**
     * 控制显示还是隐藏
     */
    visible: boolean;
    /**
     * 全屏显示
     */
    fullscreen: boolean;
}
