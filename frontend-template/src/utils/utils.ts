/**
 * 工具类
 */

import { AxiosResponse } from "axios";

/**
 * 下载文件
 * @param res
 */
export function downloadFile(res: AxiosResponse<any, any>) {
    var blob = new Blob([res.data], { type: "application/octet-stream;charset=UTF-8" });
    var contentDisposition = res.headers["content-disposition"];
    var pat = new RegExp("filename=([^;]+\\.[^\\.;]+);*");
    var result = pat.exec(contentDisposition);
    var filename = result[1];
    var downloadElement = document.createElement("a");
    var href = window.URL.createObjectURL(blob); // 创建下载的链接
    var reg = /^["](.*)["]$/g;
    downloadElement.style.display = "none";
    downloadElement.href = href;
    downloadElement.download = decodeURI(filename.replace(reg, "$1")); // 下载后文件名
    document.body.appendChild(downloadElement);
    downloadElement.click(); // 点击下载
    document.body.removeChild(downloadElement); // 下载完成移除元素
    window.URL.revokeObjectURL(href);
}
