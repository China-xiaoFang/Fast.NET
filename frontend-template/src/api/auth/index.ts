import axios from "@/utils/axios";
import { GetLoginUserInfoOutput } from "@/api/modules";

/**
 * 获取登录用户信息
 * @returns
 */
export const getLoginUserInfo = () => {
    return axios<GetLoginUserInfoOutput>({
        url: "getLoginUserInfo",
        method: "get",
    });
};
