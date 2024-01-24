import { isArray } from "lodash-es";

/**
 * @description 处理无数据情况
 * @param {String} callValue 需要处理的值
 * @return string
 * */
export function formatValue(callValue: any) {
    // 如果当前值为数组,使用 / 拼接（根据需求自定义）
    if (isArray(callValue)) return callValue.length ? callValue.join(" / ") : "--";
    return callValue ?? "--";
}

/**
 * @description 处理 prop 为多级嵌套的情况(列如: prop:user.name)
 * @param {Object} row 当前行数据
 * @param {String} prop 当前 prop
 * @return any
 * */
export function handleRowAccordingToProp(row: { [key: string]: any }, prop: string) {
    if (!prop.includes(".")) return row[prop] ?? "--";
    prop.split(".").forEach((item) => (row = row[item] ?? "--"));
    return row;
}

/**
 * @description 处理 prop，当 prop 为多级嵌套时 ==> 返回最后一级 prop
 * @param {String} prop 当前 prop
 * @return string
 * */
export function handleProp(prop: string) {
    const propArr = prop.split(".");
    if (propArr.length == 1) return prop;
    return propArr[propArr.length - 1];
}

/**
 * @description 根据枚举列表查询当需要的数据（如果指定了 label 和 value 的 key值，会自动识别格式化）
 * @param {String} callValue 当前单元格值
 * @param {Array} enumData 字典列表
 * @param {Array} fieldNames 指定 label && value 的 key 值
 * @param {String} type 过滤类型（目前只有 tag）
 * @return string
 * */
export function filterEnum(callValue: any, enumData: any[] | undefined, fieldNames?: { label: string; value: string }, type?: "tag"): string {
    const value = fieldNames?.value ?? "value";
    const label = fieldNames?.label ?? "label";
    let filterData: { [key: string]: any } = {};
    if (Array.isArray(enumData)) filterData = enumData.find((item: any) => item[value] === callValue);
    if (type == "tag") return filterData?.tagType ? filterData.tagType : "";
    return filterData ? filterData[label] : "--";
}

/**
 * 时间处理翻译
 * @param date
 * @returns
 */
export function dateTimeFix(date: string | Date | null | undefined) {
    if (date !== null && date !== undefined && date) {
        if (typeof date === "string") {
            date = new Date(date);
        }

        // 获取时间戳
        let timestamp = date.getTime();
        if (timestamp.toString().length < 13) {
            const arrTimestamp = timestamp.toString().split("");
            for (let start = 0; start < 13; start++) {
                if (!arrTimestamp[start]) {
                    arrTimestamp[start] = "0";
                }
            }
            timestamp = parseInt(arrTimestamp.join(""));
        }
        const minute = 1000 * 60;
        const hour = minute * 60;
        const day = hour * 24;
        const month = day * 30;
        // 获取当前时间
        const curTime = new Date().getTime();
        // 比较
        const diffValue = curTime - timestamp;

        // 计算差异时间的量级
        const monthC = diffValue / month;
        const weekC = diffValue / (7 * day);
        const dayC = diffValue / day;
        const hourC = diffValue / hour;
        const minC = diffValue / minute;

        // 如果本地时间反而小于变量时间
        if (diffValue < 0) {
            const monthC1 = Math.abs(monthC);
            const weekC1 = Math.abs(weekC);
            const dayC1 = Math.abs(dayC);
            const hourC1 = Math.abs(hourC);
            const minC1 = Math.abs(minC);

            if (monthC1 > 12) {
                // 超过1年，直接显示 几 年前
                return parseInt(`${monthC1 / 12}`) + "年后";
            } else if (monthC1 >= 6) {
                return "半年后";
            } else if (monthC1 >= 1) {
                return parseInt(`${monthC1}`) + "月后";
            } else if (weekC1 > 2) {
                return "半月后";
            } else if (weekC1 >= 1) {
                return parseInt(`${weekC1}`) + "周后";
            } else if (dayC1 >= 1) {
                return parseInt(`${dayC1}`) + "天后";
            } else if (hourC1 >= 1) {
                return parseInt(`${hourC1}`) + "小时后";
            } else if (minC1 >= 1) {
                return parseInt(`${minC1}`) + "分钟后";
            }
            return "刚刚";
            // return "不久前";
        }

        // 使用
        if (monthC > 12) {
            // 超过1年，直接显示 几 年前
            return parseInt(`${monthC / 12}`) + "年前";
        } else if (monthC >= 6) {
            return "半年前";
        } else if (monthC >= 1) {
            return parseInt(`${monthC}`) + "月前";
        } else if (weekC > 2) {
            return "半月前";
        } else if (weekC >= 1) {
            return parseInt(`${weekC}`) + "周前";
        } else if (dayC >= 1) {
            return parseInt(`${dayC}`) + "天前";
        } else if (hourC >= 1) {
            return parseInt(`${hourC}`) + "小时前";
        } else if (minC >= 1) {
            return parseInt(`${minC}`) + "分钟前";
        }
        return "刚刚";
    } else {
        return "";
    }
}

/**
 * 数组动态排序
 * @param property
 * @param order
 * @returns
 */
export function arrayDynamicSort(sortList: PageSortInput[]): (a: any, b: any) => number {
    return function (a: any, b: any) {
        if (sortList && sortList.length > 0) {
            for (let condition of sortList) {
                const property = condition.enField;
                const order = condition.mode;

                const aValue = a[property];
                const bValue = b[property];

                if (typeof aValue === "string" && typeof bValue === "string") {
                    if (order === "ascending") {
                        const comparison = aValue.localeCompare(bValue, "zh-CN");
                        if (comparison !== 0) {
                            return comparison;
                        }
                    } else if (order === "descending") {
                        const comparison = bValue.localeCompare(aValue, "zh-CN");
                        if (comparison !== 0) {
                            return comparison;
                        }
                    }
                } else {
                    if (order === "ascending") {
                        if (aValue < bValue) return -1;
                        if (aValue > bValue) return 1;
                    } else if (order === "descending") {
                        if (aValue > bValue) return -1;
                        if (aValue < bValue) return 1;
                    }
                }
            }
        }

        return 0;
    };
}
