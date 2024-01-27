import { defineComponent, computed, ref, watch, Ref, inject, useAttrs, PropType, SetupContext, reactive } from "vue";
import type { FTableGridItemProps, FTableResponsive, FTableBreakPoint } from "../../interface";

export default defineComponent({
    name: "GridItem",
    props: {
        offset: { type: Number, default: 0 },
        span: { type: Number, default: 1 },
        suffix: { type: Boolean, default: false },
        xs: {
            type: Object as PropType<FTableResponsive>,
            require: true,
        },
        sm: {
            type: Object as PropType<FTableResponsive>,
            require: true,
        },
        md: {
            type: Object as PropType<FTableResponsive>,
            require: true,
        },
        lg: {
            type: Object as PropType<FTableResponsive>,
            require: true,
        },
        xl: {
            type: Object as PropType<FTableResponsive>,
            require: true,
        },
    },
    setup(props: FTableGridItemProps, { slots }: SetupContext) {
        const attrs = useAttrs() as any;
        const state = reactive({
            show: true
        })

        // 注入断点
        const breakPoint = inject<Ref<FTableBreakPoint>>("breakPoint", ref("xl"));
        const shouldHiddenIndex = inject<Ref<number>>("shouldHiddenIndex", ref(-1));

        watch(
            () => [shouldHiddenIndex.value, breakPoint.value],
            (n) => {
                if (~~attrs.index) {
                    // 这里的 -1 是为了将按钮显示在上面
                    // isShow.value = !(n[0] !== -1 && parseInt(attrs.index) >= (n[0] as number) - 1);
                    state.show = !(n[0] !== -1 && parseInt(attrs.index) >= Number(n[0]));
                }
            },
            { immediate: true }
        );

        const gap = inject("gap", 0);
        const cols = inject<Ref<number>>("cols", ref(4));

        const style = computed(() => {
            let span = props[breakPoint.value]?.span ?? props.span;
            let offset = props[breakPoint.value]?.offset ?? props.offset;
            if (props.suffix) {
                return {
                    gridColumnStart: cols.value - span - offset + 1,
                    gridColumnEnd: `span ${span + offset}`,
                    marginLeft: offset !== 0 ? `calc(((100% + ${gap}px) / ${span + offset}) * ${offset})` : "unset",
                };
            } else {
                return {
                    gridColumn: `span ${span + offset > cols.value ? cols.value : span + offset}/span ${span + offset > cols.value ? cols.value : span + offset
                        }`,
                    marginLeft: offset !== 0 ? `calc(((100% + ${gap}px) / ${span + offset}) * ${offset})` : "unset",
                };
            }
        });

        return () => (
            <div style={style.value} v-show={state.show}>
                {slots.default && slots.default()}
            </div>
        );
    },
});
