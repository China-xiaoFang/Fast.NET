import { createApp } from "vue";
import App from "./App.vue";
import store from "@/stores";
import { loadLang } from "@/lang";
import router from "@/router";
import "@/router/permission";
import mitt from "mitt";

import { loadPlugins } from "@/plugins";
import { loadDirectives } from "@/directives";

import "element-plus/dist/index.css";
import "element-plus/theme-chalk/display.css";
import "font-awesome/css/font-awesome.min.css";
import "@/styles/index.scss";

async function start() {
    const app = createApp(App);

    // 注册持久化存储
    app.use(store);

    // 全局语言包加载
    loadLang(app);

    app.use(router);

    /** 加载插件 */
    loadPlugins(app);
    /** 加载自定义指令 */
    loadDirectives(app);

    // 挂载全局事件总线
    app.config.globalProperties.eventBus = mitt();

    app.mount("#app");
}

await start();
