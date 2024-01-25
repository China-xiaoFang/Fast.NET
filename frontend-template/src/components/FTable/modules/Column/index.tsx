import { defineComponent, ref, inject, PropType, SetupContext } from "vue";
import type { FTableColumnProps, FTableColumn } from "../../interface";
import { dateTimeFix, filterEnum, formatValue, handleRowAccordingToProp } from "../../utils";
import dayjs from "dayjs";
import FImage from "@/components/FImage/index"
import TableColumn from "./index"

export default defineComponent({
    name: "TableColumn",
    props: {
        column: {
            type: Object as PropType<FTableColumn>,
            require: true,
        },
    },
    setup(props: FTableColumnProps, { slots }: SetupContext) {
        const enumMap = inject("enumMap", ref(new Map()));

        /**
         * 渲染表格数据
         */
        const renderCellData = (item: FTableColumn, scope: { [key: string]: any }) => {
            return enumMap.value.get(item.prop) && item.filterEnum
                ? filterEnum(handleRowAccordingToProp(scope.row, item.prop!), enumMap.value.get(item.prop)!, item.fieldNames)
                : formatValue(handleRowAccordingToProp(scope.row, item.prop!));
        };

        /**
         * 获取 tag 类型
         */
        const getTagType = (item: FTableColumn, scope: { [key: string]: any }) => {
            return filterEnum(handleRowAccordingToProp(scope.row, item.prop!), enumMap.value.get(item.prop), item.fieldNames, "tag") as any;
        };

        const getAlign = () => {
            if (props.column.prop === "operation") {
                return "center";
            }
            switch (props.column.type) {
                case "index":
                case "image":
                    return "center";
                default:
                    return "left";
            }
        };

        /**
         * 获取排序
         */
        const getSortable = () => {
            if (props.column.prop === "operation") {
                return false;
            } else if (props.column.sortable == undefined) {
                return "custom";
            } else {
                return props.column.sortable;
            }
        };

        return () => (
            <>
                {
                    // 如果有配置多级表头的数据，则递归该组件
                    props.column._children?.length ?
                        (
                            <el-table-column
                                v-bind={props.column}
                                width="auto"
                                min-width={(props.column.width || props.column.minWidth) ?? "auto"}
                                header-align="left"
                                align={props.column.align ?? "left"}
                                sortable={getSortable()}
                                sortOrders={props.column.sortOrders ?? ['descending', 'ascending', null]}
                                show-overflow-tooltip={props.column.showOverflowTooltip ?? props.column.type != "operation"}
                            >
                                <>
                                    {
                                        props.column._children.map((col: FTableColumn) => (
                                            <TableColumn column={col}>
                                                <>
                                                    {
                                                        Object.keys(slots).map((slot: string) => (
                                                            <template key={slots} v-slots={{
                                                                [slot]: (scope) => (
                                                                    <slot name={slots} v-bind={scope} />
                                                                )
                                                            }}>
                                                            </template>
                                                        ))
                                                    }
                                                </>
                                            </TableColumn>
                                        ))
                                    }
                                </>
                                {{
                                    header: ({ column, $index }: { column: FTableColumn; $index: number }) =>
                                        [
                                            <>
                                                {
                                                    props.column.headerRender ?
                                                        (
                                                            <component is={props.column.headerRender} v-bind={{ column, index: $index }} />
                                                        )
                                                        : props.column.headerRender ?
                                                            (
                                                                <slot name={props.column.headerSlot} column={column} index={$index} />
                                                            )
                                                            :
                                                            (
                                                                <span>{column.label}</span>
                                                            )
                                                }
                                            </>
                                        ]
                                }}
                            </el-table-column>
                        ) :
                        // 其他正常的列
                        (
                            <el-table-column
                                v-bind={props.column}
                                width="auto"
                                min-width={(props.column.width || props.column.minWidth) ?? "auto"}
                                header-align="left"
                                align={getAlign()}
                                sortable={getSortable()}
                                sortOrders={props.column.sortOrders ?? ['descending', 'ascending', null]}
                                show-overflow-tooltip={props.column.showOverflowTooltip ?? props.column.type != "operation"}
                            >
                                {{
                                    header: ({ column, index }: { column: FTableColumn; index: number }) =>
                                        [
                                            <>
                                                {
                                                    props.column.headerRender ?
                                                        (
                                                            <component is={props.column.headerRender} v-bind={{ column, index }} />
                                                        )
                                                        : props.column.headerRender ?
                                                            (
                                                                <slot name={props.column.headerSlot} column={column} index={index} />
                                                            )
                                                            :
                                                            (
                                                                <span>{column.label}</span>
                                                            )
                                                }
                                            </>
                                        ]
                                }}
                                {{
                                    default: ((row: any, index: number) => (
                                        <>
                                            {
                                                props.column.tag ? (
                                                    // Tag
                                                    <el-tag type={getTagType(props.column, { row })}>
                                                        {renderCellData(props.column, { row })}
                                                    </el-tag>
                                                ) : (null)
                                            }
                                            {
                                                props.column.type == "image" ? (
                                                    <FImage
                                                        loading="lazy"
                                                        src={row[props.column.prop]}
                                                        fit="cover"
                                                        style="width: 32px; height:32px; border-radius: 4px;"
                                                    ></FImage>
                                                ) : (null)
                                            }
                                            {
                                                props.column.type == "date" && row[props.column.prop] ? (
                                                    <>
                                                        {dayjs(row[props.column.prop]).format(props.column.dateFormat ?? "YYYY-MM-DD")}
                                                        {
                                                            props.column.dateFix ? (
                                                                <>
                                                                    <br />
                                                                    <el-tag type="info" round effect="light">
                                                                        {dateTimeFix(row[props.column.prop])}
                                                                    </el-tag>
                                                                </>
                                                            ) : (null)
                                                        }
                                                    </>
                                                ) : (null)
                                            }
                                            {
                                                props.column.link ? (
                                                    <el-button link type="primary" onClick={props.column.click(row)}>{row[props.column.prop]}</el-button>
                                                ) : (null)
                                            }
                                            {
                                                // render函数 使用内置的component组件可以支持h函数渲染和txs语法
                                                props.column.render ? (
                                                    <component is={props.column.render} row={row} index={index} />
                                                ) : (null)
                                            }
                                            {
                                                props.column.slot ? (
                                                    <slot name={props.column.slot} row={row} index={index} />
                                                ) : (null)
                                            }
                                        </>
                                    ))
                                }}
                            </el-table-column>
                        )
                }
            </>
        )
    },
});
