import { SetupContext, defineComponent, ref, reactive, PropType, watch } from "vue";
import type { Props, Emits, State } from "./interface";
import { ElDialog, ElLoading, ElMessageBox } from "element-plus";
import FIcon from "@/components/FIcon";
import { useI18n } from "vue-i18n";

export default defineComponent({
    name: "FDialog",
    props: {
        width: {
            type: String,
            required: false,
        },
        height: {
            type: String,
            required: false,
        },
        showFullscreen: {
            type: Boolean,
            default: true,
        },
        showClose: {
            type: Boolean,
            default: true,
        },
        showCloseButton: {
            type: Boolean,
            default: true,
        },
        showConfirmButton: {
            type: Boolean,
            default: true,
        },
        disabledConfirmButton: {
            type: Boolean,
            default: false,
        },
        closeButtonText: {
            type: String,
            required: false,
        },
        confirmButtonText: {
            type: String,
            required: false,
        },
        showBeforeClose: {
            type: Boolean,
            default: false,
        },
        fullscreen: {
            type: Boolean,
            default: false,
        },
        appendToBody: {
            type: Boolean,
            default: true,
        },
        scrollbar: {
            type: Boolean,
            default: true,
        },
        fillHeight: {
            type: Boolean,
            default: true,
        },
        closeCallBack: {
            type: Function as PropType<() => boolean>,
            required: false,
        }
    },
    setup(props: Props, { attrs, slots, emit, expose }: SetupContext<Emits>) {
        const { t } = useI18n();

        const state: State = reactive({
            loading: false,
            visible: false,
            fullscreen: props.fullscreen
        })

        const dialogRef = ref<InstanceType<typeof ElDialog>>();

        let loadingService: any = null;

        watch(() => state.loading, () => {
            if (state.loading) {
                loadingService = ElLoading.service({
                    target: dialogRef.value.dialogRef
                })
            } else {
                loadingService?.close();
            }
        })

        /**
         * 打开
         */
        const open = () => {
            emit("onOpen")
            state.visible = true;
        }

        /**
         * 关闭
         */
        const close = () => {
            emit("onClose");
            state.visible = false;
        }

        /**
         * 关闭回调
         * @param done 
         */
        const closeCallBack = (done: () => void) => {
            if (props.closeCallBack) {
                props.closeCallBack() && done()
            } else {
                done();
            }
        }

        /**
         * 关闭前回调
         * @param done 
         */
        const handleBeforeClose = (done: () => void) => {
            if (props.showBeforeClose) {
                ElMessageBox.confirm(t("components.FDialog.确定关闭？"), { type: "warning" }).then(() => {
                    closeCallBack(done);
                });
            } else {
                closeCallBack(done);
            }
        }

        /**
         * 确认按钮点击
         */
        const handleConfirmClick = () => {
            emit("onConfirmClick", state)
        }

        expose({
            element: dialogRef.value,
            ...state,
            open,
            close,
        })

        return () => (
            <>
                <el-dialog
                    ref={dialogRef}
                    {...attrs}
                    class={["f-dialog", state.fullscreen ? "f-dialog-fill-height" : ""]}
                    style={{ maxHeight: props.height ? props.height : "", maxWidth: props.width ? props.width : "" }}
                    v-model={state.visible}
                    appendToBody={props.appendToBody}
                    fullscreen={state.fullscreen ? true : false}
                    beforeClose={handleBeforeClose}
                    draggable
                    destroy-on-close
                    showClose={false}
                >
                    {{
                        header: () => (
                            <>
                                {attrs.title}
                                {slots.header && slots.header()}
                                {
                                    props.showFullscreen ? (
                                        <el-button
                                            class="f-dialog-header-fullscreen"
                                            link
                                            onClick={state.fullscreen = !state.fullscreen}
                                        >
                                            <el-tooltip
                                                content={state.fullscreen ? t("components.FDialog.关闭全屏显示") : t("components.FDialog.全屏显示")}
                                                placement="bottom"
                                            >
                                                <FIcon name={state.fullscreen ? "local-fullscreen-exit" : "local-fullscreen"} />
                                            </el-tooltip>
                                        </el-button>
                                    ) : (null)
                                }
                                {
                                    props.showClose ? (
                                        <el-button
                                            class="f-dialog-header-close"
                                            link
                                            onClick={close}
                                        >
                                            <el-tooltip
                                                content={t("components.FDialog.关闭")}
                                                placement="bottom"
                                            >
                                                <FIcon name="el-icon-Close" />
                                            </el-tooltip>
                                        </el-button>
                                    ) : (null)
                                }
                            </>
                        )
                    }}
                    {
                        props.scrollbar ? (
                            <el-scrollbar>
                                {slots.default && slots.default(state)}
                            </el-scrollbar>
                        ) : (
                            <>
                                {slots.default && slots.default(state)}
                            </>
                        )
                    }
                    {
                        props.showFooterOperator ? (
                            <>
                                {{
                                    footer: () => (
                                        <>
                                            {slots.footer(state)}
                                            {
                                                props.showCloseButton ? (
                                                    <el-button
                                                        onClick={close}
                                                    >
                                                        {t("components.FDialog.取消")}
                                                    </el-button>
                                                ) : (null)
                                            }
                                            {
                                                props.showConfirmButton ? (
                                                    <el-button
                                                        disabled={props.disabledConfirmButton}
                                                        type="primary"
                                                        onClick={handleConfirmClick}
                                                    >
                                                        {t("components.FDialog.确认")}
                                                    </el-button>
                                                ) : (null)
                                            }
                                        </>
                                    )
                                }}
                            </>
                        ) : (null)
                    }
                </el-dialog>
            </>
        );
    },
});
