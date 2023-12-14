<script lang="ts" name="FIcon">
import { createVNode, resolveComponent, defineComponent, computed, type CSSProperties } from "vue";
import svg from "@/components/FIcon/svg/index.vue";
import { isExternal } from "@/utils/validate";
export default defineComponent({
    name: "FIcon",
    props: {
        /**
         * 名称
         * el-icon- 使用 El-icon 的图标
         * local- 使用本地 svg 图标
         */
        name: {
            type: String,
            required: true,
        },
        /**
         * 大小
         */
        size: {
            type: String,
            default: "18px",
        },
        /**
         * 颜色
         */
        color: {
            type: String,
            default: "#000000",
        },
    },
    setup(props) {
        const iconStyle = computed((): CSSProperties => {
            const { size, color } = props;
            let s = `${size.replace("px", "")}px`;
            return {
                fontSize: s,
                color: color,
            };
        });

        if (props.name.indexOf("el-icon-") === 0) {
            return () => createVNode("el-icon", { class: "icon el-icon", style: iconStyle.value }, [createVNode(resolveComponent(props.name))]);
        } else if (props.name.indexOf("local-") === 0 || isExternal(props.name)) {
            return () => createVNode(svg, { name: props.name, size: props.size, color: props.color });
        } else {
            return () => createVNode("i", { class: [props.name, "icon"], style: iconStyle.value });
        }
    },
});
</script>
