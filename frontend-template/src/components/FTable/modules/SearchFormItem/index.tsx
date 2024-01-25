import { defineComponent, SetupContext, computed, inject, ref, PropType } from "vue";
import { handleProp } from "../../utils";
import type { FTableSearchFormItemProps, FTableColumn } from '../../interface';
import { i18n } from "@/lang";

export default defineComponent({
    name: "SearchFormItem",
    props: {
        column: {
            type: Object as PropType<FTableColumn>,
            require: true
        },
        searchParam: {
            type: Object as PropType<anyObj>,
            require: true
        },
        search: {
            type: Function as PropType<() => void>,
            require: true
        }
    },
    setup(props: FTableSearchFormItemProps, { slots }: SetupContext) {
        // 判断 fieldNames 设置 label && value 的 key 值
        const fieldNames = computed(() => {
            return {
                label: props.column.fieldNames?.label ?? "label",
                value: props.column.fieldNames?.value ?? "value",
            };
        });

        // 默认时间
        const defaultTime = ref<[Date, Date]>([new Date(1970, 1, 1, 0, 0, 0), new Date(2099, 12, 31, 23, 59, 59)]);
        const simpleTime = ref<Date>(new Date(1970, 1, 1, 0, 0, 0));

        // 接收 enumMap
        const enumMap = inject("enumMap", ref(new Map()));
        const columnEnum = computed(() => {
            let enumData = enumMap.value.get(props.column.prop).filter((f: any) => f.isShow == undefined || f.isShow == null || f.isShow);
            if (!enumData) return [];
            if (props.column.search?.el === "select-v2" && props.column.fieldNames) {
                enumData = enumData.map((item: { [key: string]: any }) => {
                    return { ...item, label: item[fieldNames.value.label], value: item[fieldNames.value.value] };
                });
            }
            return enumData;
        });

        // 处理透传的 searchProps(el 为 tree-select、cascader 的时候需要给下默认 label 和 value)
        const handleSearchProps = computed(() => {
            const label = fieldNames.value.label;
            const value = fieldNames.value.value;
            const searchEl = props.column.search?.el;
            const searchProps = props.column.search?.props ?? {};
            let handleProps = searchProps;
            if (searchEl === "tree-select") handleProps = { ...searchProps, props: { label, ...searchProps.props }, nodeKey: value };
            if (searchEl === "cascader") handleProps = { ...searchProps, props: { label, value, ...searchProps.props } };
            return handleProps;
        });

        // 处理默认 placeholder
        const placeholder = computed(() => {
            const search = props.column.search;
            if (["datetimerange", "daterange", "monthrange"].includes(search?.props?.type) || search?.props?.isRange) {
                return {
                    rangeSeparator: i18n.global.t("components.FTable.modules.SearchFormItem.至"), startPlaceholder:
                        i18n.global.t("components.FTable.modules.SearchFormItem.开始时间")
                    , endPlaceholder:
                        i18n.global.t("components.FTable.modules.SearchFormItem.结束时间")
                };
            }
            const placeholder = search?.props?.placeholder ?? (search?.el.includes("input") ? i18n.global.t("components.FTable.modules.SearchFormItem.请输入") :
                i18n.global.t("components.FTable.modules.SearchFormItem.请选择"));

            return { placeholder };
        });

        // 是否有清除按钮
        const clearable = computed(() => {
            const search = props.column.search;
            return search?.props?.clearable;
        });

        const simpleShortcuts = [
            {
                text: i18n.global.t("components.FTable.modules.SearchFormItem.今天"),
                value: new Date(),
            },
            {
                text: i18n.global.t("components.FTable.modules.SearchFormItem.昨天"),
                value: () => {
                    const date = new Date();
                    date.setTime(date.getTime() - 3600 * 1000 * 24);
                    return date;
                },
            },
            {
                text: i18n.global.t("components.FTable.modules.SearchFormItem.一周前"),
                value: () => {
                    const date = new Date();
                    date.setTime(date.getTime() - 3600 * 1000 * 24 * 7);
                    return date;
                },
            },
            {
                text: i18n.global.t("components.FTable.modules.SearchFormItem.一月前"),
                value: () => {
                    const date = new Date();
                    date.setTime(date.getTime() - 3600 * 1000 * 24 * 30);
                    return date;
                },
            },
            {
                text: i18n.global.t("components.FTable.modules.SearchFormItem.一年前"),
                value: () => {
                    const date = new Date();
                    date.setTime(date.getTime() - 3600 * 1000 * 24 * 365);
                    return date;
                },
            },
        ];
        const shortcuts = [
            {
                text: i18n.global.t("components.FTable.modules.SearchFormItem.最近1天"),
                value: () => {
                    const end = new Date();
                    const start = new Date();
                    start.setDate(start.getDate() - 1);
                    return [start, end];
                },
            },
            {
                text: i18n.global.t("components.FTable.modules.SearchFormItem.最近3天"),
                value: () => {
                    const end = new Date();
                    const start = new Date();
                    start.setDate(start.getDate() - 3);
                    return [start, end];
                },
            },
            {
                text: i18n.global.t("components.FTable.modules.SearchFormItem.最近1周"),
                value: () => {
                    const end = new Date();
                    const start = new Date();
                    start.setDate(start.getDate() - 7);
                    return [start, end];
                },
            },
            {
                text: i18n.global.t("components.FTable.modules.SearchFormItem.最近1个月"),
                value: () => {
                    const end = new Date();
                    const start = new Date();
                    start.setMonth(start.getMonth() - 1);
                    return [start, end];
                },
            },
            {
                text: i18n.global.t("components.FTable.modules.SearchFormItem.最近3个月"),
                value: () => {
                    const end = new Date();
                    const start = new Date();
                    start.setMonth(start.getMonth() - 3);
                    return [start, end];
                },
            },
            {
                text: i18n.global.t("components.FTable.modules.SearchFormItem.最近6个月"),
                value: () => {
                    const end = new Date();
                    const start = new Date();
                    start.setMonth(start.getMonth() - 6);
                    return [start, end];
                },
            },
            {
                text: i18n.global.t("components.FTable.modules.SearchFormItem.最近1年"),
                value: () => {
                    const end = new Date();
                    const start = new Date();
                    start.setMonth(start.getMonth() - 12);
                    return [start, end];
                },
            },
        ];

        return () => (
            <>
                {props.column.search?.el ?
                    (
                        <component
                            v-if={props.column.search?.el}
                            is={`el-${props.column.search.el}`}
                            {...{ ...handleSearchProps, ...placeholder }}
                            v-model_trim={props.searchParam[props.column.search.key ?? handleProp(props.column.prop!)]}
                            data={props.column.search?.el === "tree-select" ? columnEnum : []}
                            options={["cascader", "select-v2"].includes(props.column.search?.el) ? columnEnum : []}
                            clearable={clearable}
                            filterable
                            range-separator={i18n.global.t("components.FTable.modules.SearchFormItem.至")}
                            start-placeholder={i18n.global.t("components.FTable.modules.SearchFormItem.开始时间")}
                            end-placeholder={i18n.global.t("components.FTable.modules.SearchFormItem.结束时间")}
                            default-time={props.column.search?.props?.type === "date" ? simpleTime : defaultTime}
                            shortcuts={props.column.search?.props?.type === "date" ? simpleShortcuts : shortcuts}
                            onChange={() => props.search()}
                        >
                            {
                                props.column.search?.el == "cascader" ? (
                                    <>
                                        {{
                                            default: ({ data }) => <span>{data[fieldNames.value.label]}</span>
                                        }}
                                    </>
                                ) : props.column.search?.el == "select" ? (
                                    <>
                                        {columnEnum.value.map((col: any, index: number) => (
                                            <el-option
                                                key={index}
                                                label={col[fieldNames.value.label]}
                                                value={col[fieldNames.value.value]}
                                            />
                                        ))}
                                    </>
                                ) : (
                                    <>
                                        {slots.default()}
                                    </>
                                )
                            }
                        </component>
                    ) : (null)
                }
            </>
        )
    }
});
