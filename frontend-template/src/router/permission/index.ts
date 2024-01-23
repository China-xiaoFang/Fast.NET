import router from "@/router";
import { type RouteRecordRaw } from "vue-router";
import { ElNotification, ElMessage } from "element-plus";
import NProgress from "nprogress";
import "nprogress/nprogress.css";
import { whiteRoutes } from "@/router/modules/whiteRoute";
import { loading } from "@/hooks/loading";
import langAutoLoadMap from "@/lang/autoLoad";
import { mergeMessage } from "@/lang/index";
import { useConfig } from "@/stores/config";
import { useUserInfo } from "@/stores/userInfo";
import { genNonceStr, getUrlParams, getGreet } from "@/utils";
import { i18n } from "@/lang";

const modules = import.meta.glob("/src/views/**/**.vue");

/** 导出白名单路由 */
const exportWhiteListFromRouter = (router: RouteRecordRaw[]): string[] => {
    let result: string[] = [];
    router.forEach((item) => {
        result.push(item.path);
        if (item.children && item.children.length > 0) {
            result = result.concat(exportWhiteListFromRouter(item.children));
        }
    });
    return result;
};

/** 加载组件 */
const loadComponent = (component: string) => {
    if (component) {
        if (component.includes("/")) {
            return modules[`/src/views/${component}.vue`];
        }
        return modules[`/src/views/${component}/index.vue`];
    } else {
        return () => import(/* @vite-ignore */ `/src/views/common/empty/index.vue`);
    }
};

/** 加载组件名称 */
const loadComponentName = (name: string) => {
    if (name) {
        if (name.includes("/")) {
            let cArr = name.split("/");
            let result = "";
            cArr.forEach((item) => {
                result += item.slice(0, 1).toUpperCase() + item.slice(1);
            });
            return result;
        }
        return name;
    } else {
        return genNonceStr(8);
    }
};

/** 白名单路由Path集合 */
const whiteList = exportWhiteListFromRouter(whiteRoutes);

/**
 * 配置 NProgress
 */
NProgress.configure({
    // 动画方式
    easing: "ease",
    // 递增进度条的速度
    speed: 500,
    // 是否显示加载ico
    showSpinner: false,
    // 自动递增间隔
    trickleSpeed: 200,
    // 初始化时的最小百分比
    minimum: 0.3,
});

/**
 * 路由加载前
 */
router.beforeEach(async (to, from, next) => {
    // 开启进度条
    NProgress.start();

    // 判断是否已经存在加载动画
    if (!window.existLoading) {
        // 显示加载动画
        loading.show();
        window.existLoading = true;
    }

    // 按需动态加载页面的语言包
    let loadPath: string[] = [];

    // 判断当前路由是否存在按需加载的语言包
    if (to.path in langAutoLoadMap) {
        loadPath.push(...langAutoLoadMap[to.path as keyof typeof langAutoLoadMap]);
    }

    const configStore = useConfig();

    const prefix = `./${configStore.lang.defaultLang}/`;

    for (const key in loadPath) {
        // 替换语言
        loadPath[key] = loadPath[key].replaceAll("${lang}", configStore.lang.defaultLang);
        // 判断是否存在语言包句柄中
        if (loadPath[key] in window.loadLangHandle) {
            window.loadLangHandle[loadPath[key]]().then((res: { default: anyObj }) => {
                // 这里默认删除 ./${lang}/ 前缀
                const pathName = loadPath[key].slice(prefix.length, loadPath[key].lastIndexOf("."));
                /**
                 * 这里处理 index.ts 文件后缀的问题
                 * 原：utils.axios.index.
                 * 处理后：utils.axios.
                 */
                const prefixName = pathName.endsWith("index") ? pathName.substring(0, pathName.lastIndexOf("/")) : pathName;
                mergeMessage(res.default, prefixName);
            });
        }
    }

    const userInfoStore = useUserInfo();

    // 判断是否存在Token
    if (userInfoStore.token) {
        if (to.meta.authForbidView) {
            // 重定向到首页
            next({ path: "/" });
            NProgress.done();
        } else {
            // 判断 pinia 中的动态路由生成的状态
            if (!userInfoStore.asyncRouterGen) {
                try {
                    // 刷新用户信息
                    await userInfoStore.refreshUserInfo();

                    // TODO：动态添加路由

                    // 确保路由添加完成
                    userInfoStore.asyncRouterGen = true;

                    // 延迟 1 秒显示欢迎信息
                    setTimeout(() => {
                        ElNotification({
                            title: "欢迎",
                            message: `${getGreet()}，${userInfoStore.nickName ?? userInfoStore.userName}，${i18n.global.t("router.欢迎回来")}`,
                            type: "success",
                            duration: 1000,
                        });
                    }, 1000);

                    const redirect = decodeURIComponent((from.query.redirect as string) || "");
                    if (redirect) {
                        const _query = getUrlParams(redirect);
                        // 设置 replace: true, 因此导航将不会留下历史记录
                        next({ path: redirect, replace: true, query: _query });
                        // next({ ...to, replace: true })
                    } else {
                        next({ ...to, replace: true });
                    }
                } catch (error: any) {
                    NProgress.done();
                    userInfoStore.logout();
                }
            }
        }
    } else {
        // 如果不存在Token，则判断页面是否存在免登录的白名单路由中
        if (whiteList.indexOf(to.path) !== -1) {
            // 免登录白名单路由，直接进入
            next();
        } else {
            ElMessage.warning(i18n.global.t("router.请登录"));
            // 非免登录白名单路由，重定向到登录页面
            next({ path: "/login", query: { redirect: encodeURIComponent(to.fullPath) } });
            NProgress.done();
        }
    }
});

/**
 * 路由加载后
 */
router.afterEach(() => {
    if (window.existLoading) {
        loading.hide();
    }
    NProgress.done();
});
