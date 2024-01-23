<script lang="tsx">
import { defineComponent, computed, type CSSProperties } from "vue";
import type { Props } from "../interface";
import { isExternal } from "@/utils/validate";

export default defineComponent({
    name: "FSvg",
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
        const s = `${props.size.replace("px", "")}px`;
        const iconName = computed(() => `#${props.name}`);
        const iconStyle = computed((): CSSProperties => {
            return {
                color: props.color,
                fontSize: s,
            };
        });
        const isUrl = computed(() => isExternal(props.name));
        const urlIconStyle = computed(() => {
            return {
                width: s,
                height: s,
                mask: `url(${props.name}) no-repeat 50% 50%`,
                "-webkit-mask": `url(${props.name}) no-repeat 50% 50%`,
            };
        });

        return () => (
            <>
                {isUrl.value ? (
                    <div {...attrs} style={urlIconStyle.value} class="url-svg svg-icon icon" />
                ) : (
                    <svg {...attrs} class="svg-icon icon" style={iconStyle.value}>
                        <use href={iconName.value} />
                    </svg>
                )}
            </>
        );
    },
});
</script>

<style scoped>
.svg-icon {
    width: 1em;
    height: 1em;
    fill: currentColor;
    overflow: hidden;
}
</style>
