import { defineComponent, computed, ref, reactive, PropType, SetupContext } from "vue";
import type { FTableSearchFormProps, FTableColumn, FTableBreakPoint } from "../../interface";
import { Refresh, Delete, ArrowDown, ArrowUp } from "@element-plus/icons-vue";
import Grid from "../Grid";
import GridItem from "../GridItem";
import SearchFormItem from "../SearchFormItem";
import { useI18n } from "vue-i18n";

export default defineComponent({
    name: "SearchForm",
    props: {
        loading: Boolean,
        columns: {
            type: Array as PropType<FTableColumn[]>,
            default: []
        },
        searchParam: {
            type: Object as PropType<anyObj>,
            default: {}
        },
        searchFormColumns: {
            type: Object as PropType<number | Record<FTableBreakPoint, number>>,
            default: {}
        },
        search: Function as PropType<() => void>,
        reset: Function as PropType<() => void>,
    },
    setup(props: FTableSearchFormProps, { slots }: SetupContext) {
        const { t } = useI18n();

        // 获取响应式设置
        const getResponsive = (item: FTableColumn) => {
            return {
                span: item.search?.span,
                offset: item.search?.offset ?? 0,
                xs: item.search?.xs,
                sm: item.search?.sm,
                md: item.search?.md,
                lg: item.search?.lg,
                xl: item.search?.xl
            };
        };

        // 是否默认折叠搜索项
        const collapsed = ref(true);

        // 获取响应式断点
        const gridRef = ref();
        const breakPoint = computed<FTableBreakPoint>(() => gridRef.value?.breakPoint);

        // 判断是否显示 展开/合并 按钮
        const showCollapse = computed(() => {
            let show = false;
            props.columns.reduce((prev, current) => {
                prev +=
                    (current.search![breakPoint.value]?.span ?? current.search?.span ?? 1) +
                    (current.search![breakPoint.value]?.offset ?? current.search?.offset ?? 0);
                if (typeof props.searchFormColumns !== "number") {
                    if (prev >= props.searchFormColumns[breakPoint.value]) show = true;
                } else {
                    if (prev >= props.searchFormColumns) show = true;
                }
                return prev;
            }, 0);
            return show;
        });

        return () => (
            <>
                {props.columns.length ? (
                    <div class="el-card table-search">
                        <el-form model={props.searchParam} nativeOnSubmit>
                            <Grid ref={gridRef} collapsed={false} gap={[20, 0]} cols={{ xs: 1, sm: 2, md: 3, lg: 4, xl: 5 }}>
                                {props.columns.slice(0, 4).map((item, index) => (
                                    <GridItem key={item.prop} {...getResponsive(item)} index={index}>
                                        <el-form-item label={`${item.search.label ?? item.label} :`}>
                                            {item.search?.slot ? (
                                                <>
                                                    {slots[item.search.slot] && slots[item.search.slot]({ searchParam: props.searchParam, column: item, search: props.search })}
                                                    {/* <slot name={item.search.slot} searchParam={props.searchParam} column={item} search={props.search} /> */}
                                                </>
                                            ) : (
                                                <SearchFormItem column={item} searchParam={props.searchParam} search={props.search} onChangeInputValue={props.search} />
                                            )}
                                        </el-form-item>
                                    </GridItem>
                                ))}
                                <GridItem suffix>
                                    <div class="operation">
                                        <el-tooltip content={t("components.FTable.modules.SearchForm.刷新")} placement="top">
                                            <el-button loading={props.loading} type="primary" icon={Refresh} onClick={props.search}></el-button>
                                        </el-tooltip>
                                        <el-tooltip content={t("components.FTable.modules.SearchForm.重置")} placement="top">
                                            <el-button loading={props.loading} icon={Delete} onClick={props.reset}></el-button>
                                        </el-tooltip>
                                        {showCollapse && (
                                            <el-button type="primary" link class="search-isOpen" onClick={() => (collapsed.value = !collapsed.value)}>
                                                {collapsed ? t("components.FTable.modules.SearchForm.展开") : t("components.FTable.modules.SearchForm.收起")}
                                                <el-icon class="el-icon--right">
                                                    <component is={collapsed ? ArrowDown : ArrowUp} />
                                                </el-icon>
                                            </el-button>
                                        )}
                                    </div>
                                </GridItem>
                            </Grid>
                        </el-form>
                    </div >

                ) : (null)}
            </>

        )
    },
});
