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
         * 是否固定在 Tab 中
         */
        affix?: boolean;
        /**
         * 登录后是否禁止查看该页面
         * 默认是 false，为 true 是代表登录后此页面不能再进入，否则跳转到首页
         */
        authForbidView?: boolean;
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
