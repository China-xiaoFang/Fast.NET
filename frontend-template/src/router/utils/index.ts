import router from "@/router";
import { isNavigationFailure, NavigationFailureType } from "vue-router";
import type { RouteLocationRaw, RouteRecordRaw } from "vue-router";
import { ElNotification } from "element-plus";
import { i18n } from "@/lang";

/**
 * 处理 iframe 的url进行编码
 * @param data
 * @returns
 */
export const encodeRoutesURI = (data: RouteRecordRaw[]): RouteRecordRaw[] => {
    data.forEach((item) => {
        if (item.meta?.type == "iframe") {
            item.path = "/iframe/" + encodeURIComponent(item.path);
        }

        if (item.children && item.children.length) {
            item.children = encodeRoutesURI(item.children);
        }
    });
    return data;
};

/**
 * 路由跳转，带错误检查
 * @param to 导航位置，同 router.push
 */
export const routePush = async (to: RouteLocationRaw) => {
    try {
        const failure = await router.push(to);
        if (isNavigationFailure(failure, NavigationFailureType.aborted)) {
            ElNotification({
                message: i18n.global.t("router.utils.导航失败，导航守卫拦截！"),
                type: "error",
            });
        } else if (isNavigationFailure(failure, NavigationFailureType.duplicated)) {
            ElNotification({
                message: i18n.global.t("router.utils.导航失败，已在导航目标位置！"),
                type: "warning",
            });
        }
    } catch (error) {
        ElNotification({
            message: i18n.global.t("router.utils.导航失败，路由无效！"),
            type: "error",
        });
        console.error(error);
    }
};

/**
 * 获取第一个路由
 * @param routes
 * @param type
 * @returns
 */
export const getFirstRoute = (routes: RouteRecordRaw[], type: string = "tab"): false | RouteRecordRaw => {
    const routerPaths: string[] = [];
    const routers = router.getRoutes();
    routers.forEach((item) => {
        if (item.path) routerPaths.push(item.path);
    });
    let find: boolean | RouteRecordRaw = false;
    for (const key in routes) {
        if (routes[key].meta?.type == "menu" && routes[key].meta?.type == type && routerPaths.indexOf(routes[key].path) !== -1) {
            return routes[key];
        } else if (routes[key].children && routes[key].children?.length) {
            find = getFirstRoute(routes[key].children!);
            if (find) return find;
        }
    }
    return find;
};

/**
 * 路由点击
 * @param route
 */
export const onClickRoute = (route: RouteRecordRaw) => {
    switch (route.meta?.menu_type) {
        case "iframe":
        case "tab":
            routePush({ path: route.path });
            break;
        case "link":
            window.open(route.path, "_blank");
            break;

        default:
            ElNotification({
                message: i18n.global.t("router.utils.导航失败，菜单类型无法识别！"),
                type: "error",
            });
            break;
    }

    // 待定....
};
