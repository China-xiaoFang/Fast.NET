import { VNodeChild } from "vue";
import { TableColumnCtx } from "element-plus";

/**
 * @field xs 最小
 * @field sm 小
 * @field md 中等
 * @field lg 大
 * @field xl 最大
 * @type FTableBreakPoint
 */
type FTableBreakPoint = "xs" | "sm" | "md" | "lg" | "xl";

/**
 * @type FTableResponsive
 */
type FTableResponsive = {
    span?: number;
    offset?: number;
};

/**
 * 枚举Props
 * @interface FTableEnumProps
 */
interface FTableEnumProps {
    /**
     * 选项框显示的文字
     */
    label: string;
    /**
     * 选项框值
     */
    value: string | number | boolean;
    /**
     * 是否显示
     */
    isShow?: boolean;
    /**
     * 是否禁用此选项
     */
    disabled?: boolean;
    /**
     * 当 tag 为 true 时，此选择会指定 tag 显示类型
     */
    tagTape?: string;
    /**
     * 为树形选择是，可以通过 children 属性指定子选项
     */
    children?: FTableEnumProps[];
    /**
     * 提示
     */
    tips?: string;
    /**
     * 字体颜色
     */
    textColor?: string;
    /**
     * 背景颜色
     */
    bgColor?: string;
}

/**
 * 搜索的 Props
 * @export
 * @interface FTableSearchProps
 */
type FTableSearchProps = {
    /**
     * 当前项搜索框的类型
     */
    el?:
        | ""
        | "input"
        | "input-number"
        | "select"
        | "select-v2"
        | "tree-select"
        | "cascader"
        | "date-picker"
        | "time-picker"
        | "time-select"
        | "switch"
        | "slider"
        | "slot";
    /**
     * 搜索项参数，根据 element plus 官方文档来传递，该属性所有值会透传到组件
     */
    props?: any;
    /**
     * 当搜索项 label 不为 col 属性时，可通过 key 指定
     */
    label?: string;
    /**
     * 当搜索项 key 不为 prop 属性时，可通过 key 指定
     */
    key?: string;
    /**
     * 搜索项排序（从大到小）
     */
    order?: number;
    /**
     * 搜索项所占用的列数，默认为1列
     * @type {number}
     */
    span?: number;
    /**
     * 搜索字段左侧偏移列数
     */
    offset?: number;
    /**
     * 插槽名称
     */
    slot?: string;
} & Partial<Record<FTableBreakPoint, FTableResponsive>>;

/**
 * FTable列Props
 * @interface FTableColumnProps
 */
export interface FTableColumnProps<T = any> extends Partial<Omit<TableColumnCtx<T>, "children" | "renderHeader" | "renderCell" | "sortable">> {
    /**
     * 对应列的类型
     * index：显示该行的索引（从 1 开始计算）
     * expand：可展开的按钮
     */
    type?: "index" | "expand" | "image" | "date";
    /**
     * 图片大小
     * 当 type 等于 image 的时候才会生效
     * normal：原图
     * small：小图
     * thumb：缩略图
     */
    imgSize?: "normal" | "small" | "thumb";
    /**
     * 插槽名称
     */
    slot?: string;
    /**
     * 表格头部插槽名称
     */
    headerSlot?: string;
    /**
     * 显示在页面中的日期格式
     */
    dateFormat?: "YYYY-MM-DD HH:mm:ss" | "YYYY-MM-DD HH:mm" | "YYYY-MM-DD" | "YYYY-MM" | "YYYY" | "MM" | "DD" | "MM-DD";
    /**
     * 是否是标签展示，默认 false
     */
    tag?: boolean;
    /**
     * 是否显示在表格当中，默认 true
     */
    show?: boolean;
    /**
     * 是否为纯搜索字段，默认 false
     */
    pureSearch?: boolean;
    /**
     * 显示搜索过滤
     */
    showFilter?: boolean;
    /**
     * 排序字段
     */
    sortableField?: string;
    /**
     * 显示时间格式化字符串，默认 false
     */
    dateFix?: boolean;
    /**
     * 搜索项配置
     */
    search?: FTableSearchProps;
    /**
     * 枚举类型（渲染值的字典）
     */
    enum?: FTableEnumProps[] | ((params?: any) => Promise<any>);
    /**
     * 当前单元格值是否根据 enum 格式化
     */
    filterEnum?: boolean;
    /**
     * 指定 label && value 的 key 值
     */
    fieldNames?: { label: string; value: string };
    /**
     * 自定义表头内容渲染（tsx语法）
     * @param row
     * @returns
     */
    headerRender?: (row: FTableColumnProps, index?: number) => VNodeChild;
    /**
     * 自定义单元格内容渲染（tsx语法）
     * @param scope
     * @returns
     */
    render?: (scope: { row: T; index?: number }) => any;
    /**
     * 多级表头
     */
    _children?: FTableColumnProps<T>[];
    /**
     * 是否为 Link Button
     */
    link?: boolean;
    /**
     * 点击事件
     * @param row
     * @returns
     */
    click?: (row: any) => void;
    /**
     * 合并行字段
     */
    span?: boolean;
    /**
     * 合并行用到的字段属性
     */
    spanProp?: string;
    /**
     * 对应列是否可以排序，
     * 默认为 'custom' 模式，即远程排序，
     * 需要监听 Table 的 sort-change 事件
     */
    sortable?: boolean;
}

/**
 * FTable 合计方法 Props
 */
export interface SummaryMethodProps<T = any> {
    columns: FTableColumnProps<T>[];
    data: T[];
}

/**
 * FTable SearchFormItem Props 属性
 * @interface FTableSearchFormItemProps
 */
export interface FTableSearchFormItemProps {
    /**
     * 类配置
     */
    column: FTableColumnProps;
    /**
     * 搜索参数
     */
    searchParam: anyObj;
    /**
     * 搜索方法
     */
    search: () => void;
}

/**
 * FTable SearchForm Props 属性
 * @interface FTableSearchFormProps
 */
export interface FTableSearchFormProps {
    /**
     * 加载状态
     */
    loading?: boolean;
    /**
     * 搜索配置列
     */
    columns?: FTableColumnProps[];
    /**
     * 搜索参数
     */
    searchParam: anyObj;
    /**
     * 搜索方法
     */
    search: () => void;
    /**
     * 重置方法
     */
    reset: () => void;
}

/**
 * FTable Pagination Props 属性
 * @interface FTablePaginationProps
 */
export interface FTablePaginationProps {
    handleSizeChange: (size: number) => void;
    handleCurrentChange: (currentPage: number) => void;
    pageIndex: number;
    pageSize: number;
    totalRows: number;
}

/**
 * FTable GridItem Props 属性
 * @interface FTableGridItemProps
 */
export interface FTableGridItemProps {
    offset?: number;
    span?: number;
    suffix?: boolean;
    xs?: FTableResponsive;
    sm?: FTableResponsive;
    md?: FTableResponsive;
    lg?: FTableResponsive;
    xl?: FTableResponsive;
}

/**
 * FTable Grid Props 属性
 * @interface FTableGridProps
 */
export interface FTableGridItemProps {
    cols?: number | Record<FTableBreakPoint, number>;
    collapsed?: boolean;
    collapsedRows?: number;
    gap?: [number, number] | number;
}

/**
 * FTableProps 属性
 * @interface FTableProps
 */
export interface FTableProps<TInput = any, TOutput = any> {
    /**
     * 表格数据
     */
    data?: TOutput[];
    /**
     * 列配置项
     */
    columns?: FTableColumnProps[];
    /**
     * 请求表格数据的api
     * @param params
     */
    requestApi?: (params: PageInput | TInput) => Promise<ApiPromise<PageResult<TOutput>> | Promise<any>>;
    /**
     * 返回数据的回调函数，可以对数据进行处理 ==> 非必传
     * @param data
     */
    dataCallback?: (data: TOutput) => void;
    /**
     * 是否需要分页组件 ==> 非必传（默认为true）
     */
    pagination?: boolean;
    /**
     * 初始化请求参数 ==> 非必传（默认为{}）
     */
    initParam?: PageInput | TInput;
    /**
     * 是否带有纵向边框 ==> 非必传（默认为true）
     */
    border?: boolean;
    /**
     * 行数据的 Key，用来优化 Table 的渲染，当表格数据多选时，所指定的 id ==> 非必传（默认为 id）
     */
    rowKey?: string;
    /**
     * 是否显示表格功能按钮 ==> 非必传（默认为true）
     */
    toolButton?: boolean;
    /**
     * 显示表格头部区域
     */
    showHeaderCard?: boolean;
    /**
     * 显示刷新按钮
     */
    showRefreshBtn?: boolean;
    /**
     * 显示搜索按钮
     */
    showSearchBtn?: boolean;
    /**
     * 显示列设置按钮
     */
    showColumnBtn?: boolean;
    /**
     * 显示搜索表单
     */
    showSearchForm?: boolean;
}
