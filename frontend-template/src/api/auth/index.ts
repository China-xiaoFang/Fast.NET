import axios from "@/utils/axios";
import { GetLoginUserInfoOutput } from "@/api/modules";

/**
 * 获取登录用户信息
 * @returns
 */
const getLoginUserInfo = () => {
    return axios<GetLoginUserInfoOutput>({
        url: "getLoginUserInfo",
        method: "get",
    });
};

export default {
    getLoginUserInfo,
};
