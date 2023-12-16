<script lang="tsx">
import { createVNode, resolveComponent, defineComponent, computed, type CSSProperties } from "vue";
import type { Props } from "./interface";
import FSvg from "@/components/FIcon/modules/FSvg.vue";
import { isExternal } from "@/utils/validate";

export default defineComponent({
    name: "FIcon",
    props: {
        name: {
            type: String,
            required: true,
        },
        size: {
            type: String,
            default: "18px",
        },
        color: {
            type: String,
            default: "#000000",
        },
    },
    setup(props: Props, { attrs }) {
        const iconStyle = computed((): CSSProperties => {
            const { size, color } = props;
            let s = `${size.replace("px", "")}px`;
            return {
                fontSize: s,
                color: color,
            };
        });

        return () => (
            <>
                {props.name.indexOf("el-icon-") === 0 ? (
                    <el-icon {...attrs} class="icon el-icon" style={iconStyle.value}>
                        {createVNode(resolveComponent(props.name))}
                    </el-icon>
                ) : props.name.indexOf("local-") === 0 || isExternal(props.name) ? (
                    <FSvg {...attrs} name={props.name} size={props.size} color={props.color} />
                ) : (
                    <i {...attrs} class={[props.name, "icon"]} style={iconStyle.value} />
                )}
            </>
        );
    },
});
</script>
