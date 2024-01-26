import { SetupContext, defineComponent, watch, reactive } from "vue";
import type { Props } from "./interface";
import styles from "./style/index.module.scss"
import FIcon from "@/components/FIcon";

export default defineComponent({
    name: "FImage",
    props: {
        src: {
            type: String,
            default: "",
        },
        lazy: {
            type: Boolean,
            default: true,
        },
        original: {
            type: Boolean,
            default: false,
        },
        normal: {
            type: Boolean,
            default: false,
        },
        small: {
            type: Boolean,
            default: false,
        },
        thumb: {
            type: Boolean,
            default: false,
        },
        preview: {
            type: Boolean,
            default: true,
        },
    },
    setup(props: Props, { attrs, slots }: SetupContext) {
        const state: {
            src: string,
            previewList: string[]
        } = reactive({
            src: "",
            previewList: []
        })

        watch(() => props.src, () => {
            if (props.src) {
                if (props.original) {
                    state.src = props.src;
                } else if (props.normal) {
                    state.src = `${props.src}@!normal`;
                } else if (props.small) {
                    state.src = `${props.src}@!small`;
                } else if (props.thumb) {
                    state.src = `${props.src}@!thumb`;
                } else {
                    // 默认使用缩略图
                    state.src = `${props.src}@!thumb`;
                }
                if (props.preview) {
                    // 预览图片使用原图
                    state.previewList = [props.src];
                }
            } else {
                state.src = undefined;
            }
        })

        return () => (
            <el-image
                {...attrs}
                className={[styles["el-image-preview"]]}
                src={state.src}
                preview-src-list={state.previewList}
                hide-on-click-modal
                preview-teleported
            >
                {
                    slots.error ? (
                        <>
                            {slots.error()}
                        </>
                    ) : (
                        <div class="error-image">
                            {{
                                error: () => <FIcon name="el-icon-Picture" />
                            }}
                        </div>
                    )
                }
            </el-image>
        );
    },
});
