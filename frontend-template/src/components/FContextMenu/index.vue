<script lang="tsx" name="FContextMenu">
import { defineComponent, onMounted, reactive, toRaw } from "vue";
import { useEventListener } from "@vueuse/core";
import FIcon from "@/components/FIcon/index.vue";
import type { Axis, ContextMenuItem, ContextMenuItemClickEmitArg, Props } from "./interface";
import { RouteLocationNormalized } from "vue-router";

export default defineComponent({
    name: "FContextMenu",
    props: {
        width: {
            type: Number,
            default: 150,
        },
        items: {
            type: Array<ContextMenuItem>,
            require: true,
        },
    },
    emits: {
        /**
         * 上下文菜单点击
         * @param item 点击项
         */
        onClick: (item: ContextMenuItemClickEmitArg) => null,
    },
    setup(props: Props, { expose, emit }) {
        const state: {
            /**
             * 是否显示
             */
            show: boolean;
            axis: Axis;
            menu: RouteLocationNormalized | undefined;
        } = reactive({
            show: false,
            axis: {
                x: 0,
                y: 0,
            },
            menu: undefined,
        });

        /**
         * 显示
         * @param menu 当前路由菜单
         * @param axis 轴位置
         */
        const show = (menu: RouteLocationNormalized, axis: Axis): void => {
            state.menu = menu;
            (state.axis = axis), (state.show = true);
        };

        /**
         * 隐藏
         */
        const hide = (): void => {
            state.show = false;
        };

        /**
         * 上下文菜单点击
         * @param item 点击项
         */
        const onClickContextMenuItemHandle = (item: ContextMenuItemClickEmitArg) => {
            if (item.disabled) return;
            item.menu = toRaw(state.menu);
            emit("onClick", item);
        };

        onMounted(() => {
            useEventListener(document, "click", hide);
        });

        expose({ show, hide });

        return () => (
            <transition name="el-zoom-in-center">
                <div
                    class="el-popper is-pure is-light el-dropdown__popper fast-contextMenu"
                    style={`top: ${state.axis.y + 5}px;left: ${state.axis.x - 14}px;width:${props.width}px`}
                    key={Math.random()}
                    v-show={state.show}
                    aria-hidden="false"
                    data-popper-placement="bottom"
                >
                    <ul class="el-dropdown-menu">
                        {props.items.map((item: any, index: number) => (
                            <li
                                class={`el-dropdown-menu__item ${item.disabled ? "is-disabled" : ""}`}
                                tabindex="-1"
                                onClick={() => onClickContextMenuItemHandle(item)}
                                key={index}
                            >
                                <FIcon size="12" name={item.icon} />
                                <span>{item.label}</span>
                            </li>
                        ))}
                    </ul>
                </div>
            </transition>
        );
    },
});
</script>

<style scoped lang="scss">
.fast-contextMenu {
    z-index: 9999;
}

.el-popper,
.el-popper.is-light .el-popper__arrow::before {
    box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
    border: none;
}

.el-dropdown-menu__item {
    padding: 8px 20px;
    user-select: none;
}

.el-dropdown-menu__item .icon {
    margin-right: 5px;
}

.el-dropdown-menu__item:not(.is-disabled) {
    &:hover {
        background-color: var(--el-dropdown-menuItem-hover-fill);
        color: var(--el-dropdown-menuItem-hover-color);
        .fa {
            color: var(--el-dropdown-menuItem-hover-color) !important;
        }
    }
}
</style>
