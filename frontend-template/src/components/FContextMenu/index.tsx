import { defineComponent, SetupContext, onMounted, reactive, toRaw } from "vue";
import { useEventListener } from "@vueuse/core";
import styles from "./style/index.module.scss"
import { FIcon } from "@/components";
import type { Axis, ContextMenuItem, ContextMenuItemClickEmitArg, Props, Emits } from "./interface";
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
        onClick: (item: ContextMenuItemClickEmitArg) => null,
    },
    setup(props: Props, { attrs, emit, expose }: SetupContext<Emits>) {
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
            <transition name="el-zoom-in-center" {...attrs}>
                <div
                    className={[styles["fast-contextMenu"], styles["el-popper"], styles["is-pure"], styles["is-light"], styles["el-dropdown__popper"]]}
                    class="el-popper is-pure is-light el-dropdown__popper"
                    style={`top: ${state.axis.y + 5}px;left: ${state.axis.x - 14}px;width:${props.width}px`}
                    key={Math.random()}
                    v-show={state.show}
                    aria-hidden="false"
                    data-popper-placement="bottom"
                >
                    <ul class="el-dropdown-menu">
                        {props.items.map((item: any, index: number) => (
                            <li
                                className={[styles["el-dropdown-menu__item"], item.disabled ? "is-disabled" : ""]}
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
            </transition >
        );
    },
});
