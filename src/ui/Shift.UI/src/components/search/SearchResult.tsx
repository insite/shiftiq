import { ReactNode, useEffect } from "react";
import Grid from "./Grid";
import { useSearch } from "./Search";
import { translate } from "@/helpers/translate";
import { BaseCriteria } from "./BaseCriteria";

interface SearchResultColumn<Row> {
    key: string;
    className?: string;
    titleClassName?: string;
    itemClassName?: string;
    title?: ReactNode;
    item?: (row: Row) => ReactNode | null;
}

interface Props<Row> {
    columns: SearchResultColumn<Row>[];
}

export default function SearchResult<Row extends object>({ columns }: Props<Row>) {
    const {
        pageIndex,
        criteria,
        totalRowCount,
        rows,
        rowsPerPage,
        isLoading,
        setPageIndex,
        setColumns,
    } = useSearch<BaseCriteria, Row>();

    const visibleColumns = criteria.visibleColumns.length
        ? columns.filter(({ key }) => criteria.visibleColumns.includes(key))
        : columns;

    const isColumnVisible = columns.map(({ key }) => !criteria.visibleColumns.length || criteria.visibleColumns.includes(key));

    useEffect(() => {
        setColumns(columns);
    }, [setColumns, columns]);

    return (
        <Grid
            className="mt-3"
            columns={visibleColumns.map(({ key, className, titleClassName, title }) => ({
                key,
                className: `${className ?? ""} ${titleClassName ?? ""}`,
                title
            }))}
            pageIndex={rows ? pageIndex : undefined}
            totalRowCount={totalRowCount}
            rowsPerPage={rowsPerPage}
            isLoading={isLoading}
            onGoToPage={pageIndex => setPageIndex(pageIndex)}
        >
            {rows && (
                rows.length > 0 && columns
                    ? rows.map((row, index) => (
                        <tr key={index}>
                            {
                                columns
                                    .map(({ className, itemClassName, item }, columnIndex) => ({
                                        key: columns[columnIndex].key,
                                        className: `${className ?? ""} ${itemClassName ?? ""}`,
                                        item
                                    }))
                                    .filter((_, index) => isColumnVisible[index])
                                    .map(({ key, className, item }) => (
                                        <td key={key} className={className}>
                                            {item?.(row)}
                                        </td>
                                    ))
                            }
                        </tr>
                    ))
                    : (
                        <tr>
                            <td colSpan={visibleColumns.length}>
                                <em>{translate("No Data")}</em>
                            </td>
                        </tr>
                    )
            )}
            {!rows && (
                <tr>
                    <td colSpan={visibleColumns.length}>
                        {translate("Data is loading...")}
                    </td>
                </tr>
            )}
        </Grid>
    );
}