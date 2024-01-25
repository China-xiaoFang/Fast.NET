import { defineComponent, ref, reactive, inject, PropType, SetupContext, provide, computed, onMounted, watch } from "vue";
import type { FTableProps, FTableState, FTableEmits, FTableColumn, FTableBreakPoint } from "./interface";
import { ElTable } from "element-plus";
import styles from "./style/index.module.scss"
import { arrayDynamicSort, getRowspanMethod } from "./utils";
import SearchForm from "./modules/SearchForm"
import TableColumn from "./modules/Column"

export default defineComponent({
    name: "FTable",
    props: {
        data: {
            type: Array as PropType<any[]>,
            default: []
        },
        columns: {
            type: Array as PropType<FTableColumn[]>,
            default: []
        },
        requestAuto: {
            type: Boolean,
            default: true
        },
        requestApi: {
            type: Function as PropType<(params: PageInput | any) => Promise<ApiPromise<PageResult<any>> | Promise<any>>>,
            require: false
        },
        dataCallback: {
            type: Function as PropType<(data: any) => void>,
            require: false
        },
        pagination: {
            type: Boolean,
            default: true,
        },
        initParam: {
            type: Object as PropType<PageInput | any>,
            default: {},
        },
        searchFormColumns: {
            type: Object as PropType<number | Record<FTableBreakPoint, number>>,
            default: () => ({ xs: 1, sm: 2, md: 2, lg: 4, xl: 5 })
        },
        rowKey: {
            type: String,
            default: "id",
        },
        showSearchForm: {
            type: Boolean,
            default: true,
        },
        showHeaderCard: {
            type: Boolean,
            default: true,
        },
        showRefreshBtn: {
            type: Boolean,
            default: true,
        },
        showSearchBtn: {
            type: Boolean,
            default: true,
        },
        showColumnBtn: {
            type: Boolean,
            default: true,
        },
        toolButton: {
            type: Boolean,
            default: true,
        },
        treeData: {
            type: Boolean,
            default: false,
        },
        treeChildrenName: {
            type: String,
            default: "children",
        },
        singleChoice: {
            type: Boolean,
            default: false,
        },
    },
    setup(props: FTableProps, { attrs, slots, emit, expose }: SetupContext<FTableEmits>) {
        const state: FTableState = reactive({
            loading: false,
            orgColumns: props.columns,
            searchColumns: [],
            tableColumns: [],
            tableData: [],
            tablePagination: {
                pageIndex: 1,
                pageSize: 20,
                totalRows: 0
            },
            searchParam: {},
            showSearch: true,
            selected: false,
            selectedList: [],
            selectedListIds: []
        })

        // el-table Dom 元素
        const tableRef = ref<InstanceType<typeof ElTable>>();

        /**
         * 处理Table数据
         * @param data 
         */
        const handleTableData = (data: any[]) => {
            if (props.treeData) {
                let result: any[] = [];
                data.forEach(row => {
                    let rowList = row[props.treeChildrenName] as unknown[];
                    if (rowList && rowList.length > 0) {
                        rowList.forEach(tRow => {
                            result.push({ ...(row as any), ...(tRow as any) });
                        });
                    } else {
                        result.push(row as any);
                    }
                });
                return result;
            } else {
                return data;
            }
        }

        // 处理表格列
        state.tableColumns = props.columns.filter(f => !f.pureSearch).sort((a, b) => {
            if (a.fixed) {
                return 1;
            } else {
                const _a = (a.order || 1000) as number;
                const _b = (b.order || 1000) as number;
                return _a - _b;
            }
        })

        // 处理表格数据
        state.tableData = handleTableData(props.data);

        /**
         * 更新查询参数
         * */
        const updatedTotalParam = () => {
            // searchParam.value = {}
            // 处理查询参数，可以给查询参数加自定义前缀操作
            const newSearchParam: { [key: string]: any } = {};
            // 防止手动清空输入框携带参数（这里可以自定义查询参数前缀）
            for (const key in state.searchParam.value) {
                // * 某些情况下参数为 false/0 也应该携带参数
                if (state.searchParam.value[key] || state.searchParam.value[key] === false || state.searchParam.value[key] === 0) {
                    newSearchParam[key] = state.searchParam.value[key];
                }
                // 处理某些情况下如果为空字符串，其实是不需要传到后端的
                else if (!state.searchParam.value[key]) {
                    delete state.searchParam.value[key];
                }
            }
            Object.assign(state.searchParam.value, newSearchParam);
        };

        /**
         * 表格数据查询
         */
        const tableSearch = () => {
            // 重置到第一页
            state.tablePagination.pageIndex = 1;
            updatedTotalParam();
            loadData();
        };

        /**
         * 表格数据重置
         */
        const tableReset = () => {
            // 重置到第一页
            state.tablePagination.pageIndex = 1;
            // 清除搜索条件
            state.searchParam.value = {};
            // 重置搜索表单的时候，如果有默认搜索参数，则重置默认的搜索参数
            Object.keys(props.initParam).forEach(key => {
                state.searchParam.value[key] = props.initParam[key];
            });
            updatedTotalParam();
            emit("tableReset");
            loadData();
        };

        /**
         * 定义 enumMap 存储 enum 值（避免异步请求无法格式化单元格内容 || 无法填充搜索下拉选择）
         */
        const enumMap = ref(new Map<string, { [key: string]: any }[]>());
        provide("enumMap", enumMap);
        const setEnumMap = async (column: FTableColumn) => {
            if (!column.enum) return;
            // 如果当前 enum 为后台数据需要请求数据，则调用该请求接口，并存储到 enumMap
            if (typeof column.enum !== "function") return enumMap.value.set(column.prop!, column.enum!);
            const { data } = await column.enum();
            enumMap.value.set(column.prop!, data);
        };

        /**
         * 扁平化 columns
         */
        const flatColumnsFunc = (columns: FTableColumn[], flatArr: FTableColumn[] = []) => {
            columns.forEach(async col => {
                if (col._children?.length) flatArr.push(...flatColumnsFunc(col._children));
                flatArr.push(col);

                // 给每一项 column 添加 show && filterEnum 默认属性
                col.show = col.show ?? true;
                col.filterEnum = col.filterEnum ?? col.tag ?? false;

                // 设置 enumMap
                setEnumMap(col);
            });
            return flatArr.filter(item => !item._children?.length);
        };

        // 表格搜素列
        state.searchColumns = flatColumnsFunc(props.columns.filter(f => f.search?.el || f.pureSearch))
            .sort((a, b) => {
                const _a = (a.search?.order || 1000) as number;
                const _b = (b.search?.order || 1000) as number;
                return _a - _b;
            });

        /**
         * 自定义索引
         * @param index
         */
        const indexMethod = (index: number) => {
            return index + (state.tablePagination.pageIndex - 1) * state.tablePagination.pageSize + 1;
        };

        /**
         * 选中改变
         * @param selection 多选的数据
         * @param row 单行数据
         */
        const handleSelectClick = (selection: any[], row: any) => {
            // 判断是否开启了单选
            if (props.singleChoice) {
                tableRef.value?.clearSelection();
                if (row && selection.length) {
                    tableRef.value?.toggleRowSelection(row, true);
                }
            }
        };

        /**
         * 多选操作
         * @param rowArr 当前选择的所有的数据
         */
        const handleSelectionChange = (rowArr: any) => {
            rowArr.length === 0 ? (state.selected = false) : (state.selected = true);
            // 判断是否为单选
            if (props.singleChoice && rowArr.length > 0) {
                // 这里获取最后一个是因为选中改变的事件会触发多次，会带入旧的数据
                state.selectedList = [rowArr[rowArr.length - 1]];
            } else {
                state.selectedList = rowArr;
            }
            emit("selectionChange", state.selectedList);
        };

        /**
         * 设置列的排序为我们自定义的排序
         * @param param0
         */
        const handleHeaderClass = ({ column }) => {
            column.order = column.multiOrder;
        };

        /**
         * 当某个表头排序改变时会触发该事件
         * @param column
         */
        const handleSortChange = ({ column }) => {
            if (!column.multiOrder) {
                column.multiOrder = "descending";
            } else if (column.multiOrder === "descending") {
                column.multiOrder = "ascending";
            } else {
                column.multiOrder = null;
            }
            // 排序集合非空判断
            state.searchParam.sortList = Object.assign(props.initParam?.sortList ?? [], state.searchParam?.sortList ?? []);
            // 去原来的列中查找表格的列数据
            let orgColumn = state.orgColumns.find(f => f.prop == column.property);
            if (orgColumn == null) {
                orgColumn = column;
            }
            const enField = orgColumn?.sortableField ?? orgColumn?.prop ?? orgColumn?.property;
            const fieldIndex = state.searchParam.sortList.findIndex((f: PageSortInput) => f.enField === enField);
            if (!column.multiOrder) {
                // 如果是空的，删除排序
                state.searchParam.sortList.splice(fieldIndex, 1);
            } else if (fieldIndex === -1) {
                state.searchParam.sortList.push({
                    enField: enField,
                    cnField: orgColumn?.label,
                    mode: column.multiOrder
                });
            } else {
                state.searchParam.sortList[fieldIndex].mode = column.multiOrder;
            }
            // 判断最后的排序集合中是否还存在数据，如果不存在，则删除排序集合
            if (state.searchParam.sortList.length === 0) {
                delete state.searchParam.sortList;
            }
            // 判断请求接口的方法是否为空
            if (props.requestApi) {
                // 刷新表格数据
                loadData();
            } else {
                state.tableData = state.tableData.sort(arrayDynamicSort(state.searchParam.sortList ?? []));
            }

            emit("sortChange", { column, prop: enField, order: column.multiOrder });
        };

        /**本地匹配搜索 */
        const filterByLocal = () => {
            const _value = handleTableData(props.data);
            state.tableData = _value.filter(f => {
                const _tagAry = [] as boolean[];
                Object.keys(state.searchParam).forEach(e => {
                    const param = state.searchParam[e]?.toLowerCase();
                    if (e == "searchValue") {
                        state.tableColumns.map(col => {
                            _tagAry.push(!param || f[col.prop]?.toLowerCase().includes(param))
                        })
                    } else {
                        _tagAry.push(!param || f[e]?.toLowerCase().includes(param))
                    }
                });
                return _tagAry.every(e => e);
            });
        };

        /**
         * 清空选中数据列表
         */
        const clearSelection = () => tableRef.value!.clearSelection();

        /**
         * 选中某一行
         * @param row
         * @param selected
         */
        const toggleRowSelection = (row: any, selected: boolean) => tableRef.value!.toggleRowSelection(row, selected);

        /**
         * 合并行列
         */
        const onSpanMethod = computed(() =>
            getRowspanMethod(state.tableData,
                (props.columns.filter(f => f.span == true) ?? []).map(col => {
                    return {
                        prop: col.prop,
                        spanProp: col.spanProp ?? col.prop
                    };
                })
            )
        );

        /**
         * 获取请求参数
         */
        const getRequestParam = () => {
            const param = { ...props.initParam, ...state.searchParam, ...(props.pagination ? state.tablePagination : {}) };
            // 删除总条数
            delete param.totalRows;
            return param;
        };

        /**
         * 加载数据
         */
        const loadData = async () => {
            // 判断是否需要自动请求
            if (!props.requestAuto) return;

            if (props.requestApi === undefined) {
                throw new Error("如果需要加载FTable数据，那么参数 requestApi 是必须的！");
            }

            state.tableData = [];

            try {
                state.loading = true;
                const param = getRequestParam();
                const apiResult = await props.requestApi(param);
                if (apiResult.success) {
                    props.dataCallback && props.dataCallback(apiResult.data);
                    let pageData: never[];
                    // 解析 API 接口返回的分页数据（如果有分页更新分页信息）
                    if (props.pagination) {
                        let pageResult = apiResult.data as PageResult<never>;
                        pageData = pageResult.rows;
                        // 更新分页信息
                        Object.assign(state.tablePagination, {
                            pageIndex: pageResult.pageIndex,
                            pageSize: pageResult.pageSize,
                            totalRows: pageResult.totalRows
                        });
                    } else {
                        pageData = apiResult.data as never[];
                    }
                    state.tableData = handleTableData(pageData);
                }
            } catch (error) {
                state.tableData = [];
            } finally {
                state.loading = false;
            }
        };

        /**
         * 每页条数改变
         * @param val 当前条数
         */
        const handleSizeChange = (val: number) => {
            state.tablePagination.pageIndex = 1;
            state.tablePagination.pageSize = val;
            loadData();
            emit("sizeChange", val);
            emit("paginationChange", 1, val);
        };

        /**
         * 当前页改变
         * @param val 当前页
         */
        const handleCurrentChange = (val: number) => {
            state.tablePagination.pageIndex = val;
            loadData();
            emit("sizeChange", val);
            emit("paginationChange", val, state.tablePagination.pageSize);
        };

        /**
         * 刷新表格
         */
        const refreshTable = () => {
            if (!props.requestAuto) {
                filterByLocal();
                // console.log(tableData);
            } else {
                loadData();
            }
        };

        /**
         * 页面渲染
         * 初始化请求
         */
        onMounted(() => {
            loadData();
        });

        /**
         * 监听事件
         * 监听非 API 请求的 Table Data
         */
        watch(
            () => props.data,
            () => {
                state.tableData = handleTableData(props.data);
            },
            // 深度监听
            { deep: true }
        );

        // 暴漏出去的方法
        expose({
            ...state,
            loadData,
            tableReset,
            clearSelection,
            toggleRowSelection,
            refreshTable,
        });

        return () => (
            <>
                {
                    props.showSearchForm ? (
                        <SearchForm
                            loading={state.loading}
                            columns={state.searchColumns}
                            searchParam={state.searchParam}
                            searchFormColumns={props.searchFormColumns}
                            search={tableSearch}
                            reset={tableReset}
                        >
                            <>
                                {
                                    Object.keys(slots).map((slot: string) => (
                                        <template key={slots} v-slots={{
                                            [slot]: (scope) => (
                                                <slot name={slots} v-bind={scope} />
                                            )
                                        }}>
                                        </template>
                                    ))
                                }
                            </>
                        </SearchForm>
                    ) : (null)
                }
                <div>
                    {slots.tableTopHeader()}
                </div>
                <div className={styles["f-table-main"]} class="el-card">
                    {
                        props.showHeaderCard ? (
                            <div className={styles["f-table-header"]}>
                                <div className={styles["f-table-header-left"]}>
                                    {slots.tableHeader(state)}
                                </div>
                                {
                                    props.toolButton ? (
                                        <div className={styles["f-table-header-right"]}>
                                            {/* <el-input
          loading={state.loading}
          prefix-icon={Search}
          placeholder="关键字搜索"
          v-model.trim="searchParam.searchValue"
          clearable
          style="margin-right: 12px; width: 235px; margin-bottom: 5px"
          @change="refreshTable(true)"
        /> */}
                                        </div>
                                    ) : (null)
                                }
                            </div>
                        ) : (null)
                    }
                </div>
            </>
        )
    },
});
