/// <reference types="vite/client" />

declare module "*.vue" {
    import { DefineComponent } from "vue";
    // eslint-disable-next-line @typescript-eslint/no-explicit-any, @typescript-eslint/ban-types
    const component: DefineComponent<{}, {}, any>;
    export default component;
}

declare module "*.scss" {
    const scss: Record<string, string>;
    export default scss;
}

/**
 * 解决 Element-Plus 国际化引用报错的问题
 * https://github.com/element-plus/element-plus/issues/13614
 */
declare module "element-plus/dist/locale/zh-cn.mjs" {
    const zhLocale: any;
    export default zhLocale;
}
declare module "element-plus/dist/locale/zh-tw.mjs" {
    const zhLocale: any;
    export default zhLocale;
}
declare module "element-plus/dist/locale/en.mjs" {
    const enLocale: any;
    export default enLocale;
}

/**
 * 解决 Vue-router meta. 没有提示的情况
 */
declare module "vue-router" {
    const RouteMeta: {
        /**
         * 页面标题
         */
        title?: string;
        /**
         * 是否添加到 Tab
         */
        addTab?: boolean;
        /**
         * 路由类型
         * @description 可选值 "module" | "tab" | "iframe"
         * @description "module" 模块
         * @description "tab" 标签，也就是实际的路由页面
         * @description "iframe" iframe 页面
         */
        type?: string;
    };
    export default RouteMeta;
}

export {};
