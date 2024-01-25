import { defineComponent, computed, ref, watch, SetupContext, provide, useSlots, VNodeArrayChildren, VNode, onBeforeMount, onMounted, onActivated, onUnmounted, onDeactivated, PropType } from "vue";
import type { FTableGridProps, FTableBreakPoint } from "../../interface";

export default defineComponent({
    name: "Grid",
    props: {
        cols: {
            type: Object as PropType<number | Record<FTableBreakPoint, number>>,
            default: () => ({ xs: 1, sm: 2, md: 2, lg: 3, xl: 4 })
        },
        collapsed: {
            type: Boolean,
            default: false
        },
        collapsedRows: {
            type: Number,
            default: 1
        },
        gap: {
            type: Object as PropType<[number, number] | number>,
            default: 0
        },
    },
    setup(props: FTableGridProps, { slots }: SetupContext) {
        // 注入 gap 间距
        provide("gap", Array.isArray(props.gap) ? props.gap[0] : props.gap);

        // 注入响应式断点
        const breakPoint = ref<FTableBreakPoint>("xl");
        provide("breakPoint", breakPoint);

        // 注入要开始折叠的 index
        const hiddenIndex = ref(-1);
        provide("shouldHiddenIndex", hiddenIndex);

        // 注入 cols
        const cols = computed(() => {
            if (typeof props.cols === "object") return props.cols[breakPoint.value] ?? props.cols;
            return props.cols;
        });
        provide("cols", cols);

        // 监听屏幕变化
        const resize = (e: UIEvent) => {
            const width = (e.target as Window).innerWidth;
            switch (true) {
                case width < 768:
                    breakPoint.value = "xs";
                    break;
                case width >= 768 && width < 992:
                    breakPoint.value = "sm";
                    break;
                case width >= 992 && width < 1200:
                    breakPoint.value = "md";
                    break;
                case width >= 1200 && width < 1920:
                    breakPoint.value = "lg";
                    break;
                case width >= 1920:
                    breakPoint.value = "xl";
                    break;
            }
        };

        const localSlots = useSlots().default!();

        // 寻找需要开始折叠的字段 index
        const findIndex = () => {
            const fields: VNodeArrayChildren = [];
            let suffix: any = null;
            localSlots.forEach((slot: any) => {
                if (typeof slot.type === "object" && slot.type.name === "GridItem" && slot.props?.suffix !== undefined) suffix = slot;
                if (typeof slot.type === "symbol" && Array.isArray(slot.children)) slot.children.forEach((child: any) => fields.push(child));
            });

            // 计算 suffix 所占用的列
            let suffixCols = 0;
            if (suffix) {
                suffixCols =
                    (suffix.props![breakPoint.value]?.span ?? suffix.props?.span ?? 1) +
                    (suffix.props![breakPoint.value]?.offset ?? suffix.props?.offset ?? 0);
            }
            try {
                let find = false;
                fields.reduce((prev: number = 0, current, index) => {
                    prev +=
                        ((current as VNode)!.props![breakPoint.value]?.span ?? (current as VNode)!.props?.span ?? 1) +
                        ((current as VNode)!.props![breakPoint.value]?.offset ?? (current as VNode)!.props?.offset ?? 0);
                    if ((prev as number) > props.collapsedRows * (cols.value as number) - suffixCols) {
                        hiddenIndex.value = index;
                        find = true;
                        throw "find it";
                    }
                    return prev;
                }, 0);
                if (!find) hiddenIndex.value = -1;
            } catch (e) { }
        };

        onBeforeMount(() => props.collapsed && findIndex());
        onMounted(() => {
            resize({ target: { innerWidth: window.innerWidth } } as any);
            window.addEventListener("resize", resize);
        });
        onActivated(() => {
            resize({ target: { innerWidth: window.innerWidth } } as any);
            window.addEventListener("resize", resize);
        });
        onUnmounted(() => {
            window.removeEventListener("resize", resize);
        });
        onDeactivated(() => {
            window.removeEventListener("resize", resize);
        });

        // 断点变化时 执行 findIndex
        watch(
            () => breakPoint.value,
            () => {
                if (props.collapsed) findIndex();
            }
        );

        // 监听 collapsed
        watch(
            () => props.collapsed,
            (value) => {
                if (value) return findIndex();
                hiddenIndex.value = -1;
            }
        );

        // 设置间距
        const gap = computed(() => {
            if (typeof props.gap === "number") return `${props.gap}px`;
            if (Array.isArray(props.gap)) return `${props.gap[1]}px ${props.gap[0]}px`;
            return "unset";
        });

        // 设置 style
        const style = computed(() => {
            return {
                display: "grid",
                gridGap: gap.value,
                gridTemplateColumns: `repeat(${cols.value}, minmax(0, 1fr))`,
            };
        });

        defineExpose({ breakPoint });

        return () => (
            <div style={style.value}>
                <slot />
            </div>
        );
    },
});
