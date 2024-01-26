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
export type FTableBreakPoint = "xs" | "sm" | "md" | "lg" | "xl";

/**
 * @type FTableResponsive
 */
export type FTableResponsive = {
    span?: number;
    offset?: number;
};

/**
 * FTable 合计方法 Props
 */
export interface SummaryMethodProps<T = any> {
    columns: FTableColumn<T>[];
    data: T[];
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
export interface FTableGridProps {
    cols?: number | Record<FTableBreakPoint, number>;
    collapsed?: boolean;
    collapsedRows?: number;
    gap?: [number, number] | number;
}

/**
 * FTable SearchFormItem Props 属性
 * @interface FTableSearchFormItemProps
 */
export interface FTableSearchFormItemProps {
    /**
     * 类配置
     */
    column: FTableColumn;
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
    columns?: FTableColumn[];
    /**
     * 搜索参数
     */
    searchParam: anyObj;
    /**
     * 搜索列配置
     */
    searchFormColumns: number | Record<FTableBreakPoint, number>;
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
 * 枚举Props
 * @interface FTableEnumColumn
 */
export interface FTableEnumColumn {
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
    children?: FTableEnumColumn[];
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
 * @interface FTableSearchColumn
 */
type FTableSearchColumn = {
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
 * @interface FTableColumn
 */
export interface FTableColumn<T = any> extends Partial<Omit<TableColumnCtx<T>, "children" | "renderHeader" | "renderCell" | "sortable">> {
    /**
     * 对应列的类型
     * index：显示该行的索引（从 1 开始计算）
     * expand：可展开的按钮
     * image：图片
     * date：日期
     * operation：操作列
     */
    type?: "index" | "expand" | "image" | "date" | "operation";
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
    search?: FTableSearchColumn;
    /**
     * 枚举类型（渲染值的字典）
     */
    enum?: FTableEnumColumn[] | ((params?: any) => Promise<any>);
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
    headerRender?: (row: FTableColumn, index?: number) => VNodeChild;
    /**
     * 自定义单元格内容渲染（tsx语法）
     * @param scope
     * @returns
     */
    render?: (scope: { row: T; index?: number }) => any;
    /**
     * 多级表头
     */
    _children?: FTableColumn<T>[];
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
 * FTable Column Props 属性
 */
export interface FTableColumnProps {
    /**
     * 列配置
     */
    column: FTableColumn;
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
    columns?: FTableColumn[];
    /**
     * 自动请求
     */
    requestAuto?: boolean;
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
     * 搜索列配置
     */
    searchFormColumns: number | Record<FTableBreakPoint, number>;
    /**
     * 是否带有纵向边框 ==> 非必传（默认为true）
     */
    border?: boolean;
    /**
     * 行数据的 Key，用来优化 Table 的渲染，当表格数据多选时，所指定的 id ==> 非必传（默认为 id）
     */
    rowKey?: string;
    /**
     * 显示搜索表单
     */
    showSearchForm?: boolean;
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
     * 是否显示表格功能按钮 ==> 非必传（默认为true）
     */
    toolButton?: boolean;
    /**
     * 是否为树形数据
     */
    treeData?: boolean;
    /**
     * 树形节点名称，默认 children
     */
    treeChildrenName?: string;
    /**
     * 单选
     */
    singleChoice?: boolean;
}

/**
 * FTable state 属性
 */
export interface FTableState<TInput = any, TOutput = any> {
    /**
     * 表格加载
     */
    loading: boolean;
    /**
     * 源列数据
     */
    orgColumns: FTableColumn[];
    /**
     * 搜素列
     */
    searchColumns: FTableColumn[];
    /**
     * 正常显示的列
     */
    tableColumns: FTableColumn[];
    /**
     * 表格数据
     */
    tableData: TOutput[];
    /**
     * 分页数据
     */
    tablePagination: PageResult<TOutput>;
    /**
     * 搜索参数
     */
    searchParam: PageInput | TInput;
    /**
     * 显示搜索
     */
    showSearch: boolean;
    /**
     * 是否选中数据
     */
    selected: boolean;
    /**
     * 选中数据列表
     */
    selectedList: TOutput[];
    /**
     * 当前选中数据的ids
     */
    selectedListIds: string[];
}

/**
 * FTable Emits 属性
 */
export interface FTableEmits {
    /**
     * 表单重置
     * @returns
     */
    tableReset: () => void;
    /**
     * 选中改变
     * @param selectedList
     * @returns
     */
    selectionChange: (selectedList: any[]) => void;
    /**
     * 排序更改
     * @param column
     * @param prop
     * @param order
     * @returns
     */
    sortChange: ({ column, prop, order }: { column: FTableColumn; prop: string; order: string }) => void;
    /**
     * 页面大小改变
     * @param size
     * @returns
     */
    sizeChange: (size: number) => void;
    /**
     * 分页改变
     * @param index
     * @param size
     * @returns
     */
    paginationChange: (index, size) => void;
}
