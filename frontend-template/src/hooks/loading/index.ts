import { nextTick } from "vue";
import "./index.scss";

export const loading = {
    show: () => {
        const bodyEl: Element = document.body;
        const div = document.createElement("div");
        div.className = "block-loading";
        div.innerHTML = `
            <div class="block-loading-box">
                <div class="block-loading-box-warp">
                    <div class="block-loading-box-item"></div>
                </div>
            </div>
        `;
        bodyEl.insertBefore(div, bodyEl.childNodes[0]);
    },
    hide: () => {
        nextTick(() => {
            setTimeout(() => {
                const el = document.querySelector(".block-loading");
                el && el.parentNode?.removeChild(el);
            }, 1000);
        });
    },
};
