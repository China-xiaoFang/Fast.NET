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
