import { defineComponent, ref, computed, withDefaults, defineProps } from "vue";
import { Refresh, Delete, ArrowDown, ArrowUp } from "@element-plus/icons-vue";
import SearchFormItem from "../SearchFormItem/index.vue";
import Grid from "../Grid/index.vue";
import GridItem from "../GridItem/index.vue";

interface GTableSearchFormProps {
    loading?: boolean;
    columns?: GTableColumnProps[];
    searchParam?: { [key: string]: any };
    searchCol: number | Record<GTableBreakPoint, number>;
    search: (params: any, key: any) => void;
    reset: (params: any) => void;
}

// 默认值
const props = withDefaults(defineProps<GTableSearchFormProps>(), {
    showSearchItem: false,
    columns: () => [],
    searchParam: () => ({}),
    enterSearch: true
});

// 获取响应式设置
const getResponsive = (item: GTableColumnProps) => {
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
const breakPoint = computed<GTableBreakPoint>(() => gridRef.value?.breakPoint);

// 判断是否显示 展开/合并 按钮
const showCollapse = computed(() => {
    let show = false;
    props.columns.reduce((prev, current) => {
        prev +=
            (current.search![breakPoint.value]?.span ?? current.search?.span ?? 1) +
            (current.search![breakPoint.value]?.offset ?? current.search?.offset ?? 0);
        if (typeof props.searchCol !== "number") {
            if (prev >= props.searchCol[breakPoint.value]) show = true;
        } else {
            if (prev >= props.searchCol) show = true;
        }
        return prev;
    }, 0);
    return show;
});

export default defineComponent({
    name: "SearchForm",
    props,
    setup(props) {
        return () => (
            <div class="el-card table-search" v-if={props.columns.length}>
                <el-form ref="formRef" model={props.searchParam} onSubmit.native.prevent>
                <Grid ref={gridRef} collapsed={false} gap={[20, 0]} cols={{ xs: 1, sm: 2, md: 3, lg: 4, xl: 5 }}>
                    {props.columns.slice(0, 4).map((item, index) => (
                        <GridItem key={item.prop} {...getResponsive(item)} index={index}>
                            <el-form-item label={`${item.search.label ?? item.label} :`}>
                                {item.search?.slot ? (
                                    <slot name={item.search.slot} v-bind={{ searchParam: props.searchParam, column: item, search: props.search }} />
                                ) : (
                                    <SearchFormItem column={item} searchParam={props.searchParam} search={props.search} onChangeInputValue={props.search} />
                                )}
                            </el-form-item>
                        </GridItem>
                    ))}
                    <GridItem suffix>
                        <div class="operation">
                            <el-tooltip content="刷新" placement="top">
                                <el-button loading={props.loading} type="primary" icon={Refresh} onClick={props.search}></el-button>
                            </el-tooltip>
                            <el-tooltip content="重置" placement="top">
                                <el-button loading={props.loading} icon={Delete} onClick={props.reset}></el-button>
                            </el-tooltip>
                            {showCollapse && (
                                <el-button type="primary" link class="search-isOpen" onClick={() => (collapsed.value = !collapsed.value)}>
                                    {collapsed.value ? "展开" : "收起"}
                                    <el-icon class="el-icon--right">
                                        <component is={collapsed.value ? ArrowDown : ArrowUp} />
                                    </el-icon>
                                </el-button>
                            )}
                        </div>
                    </GridItem>
                </Grid>
            </el-form>
      </div >
    );
  }
});
