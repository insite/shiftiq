import { ForwardedRef, ReactNode, useEffect, useImperativeHandle, useState } from "react";
import Grid from "./Grid";
import { translate } from "@/helpers/translate";
import { QueryResult } from "@/models/QueryResult";
import { useStatusProvider } from "@/contexts/status/StatusProviderContext";

export interface SearchGridFunctions {
    refresh(): Promise<void>;
}

interface SearchResultColumn<Row> {
    key: string;
    className?: string;
    titleClassName?: string;
    itemClassName?: string;
    title?: ReactNode;
    item?: (row: Row, isLoading: boolean) => ReactNode | null;
}

interface Props<Row extends object> {
    ref?: ForwardedRef<SearchGridFunctions>,
    className?: string;
    columns: SearchResultColumn<Row>[];
    onLoad(pageIndex: number): Promise<QueryResult<Row> | null>;
}

export default function SearchGrid<Row extends object>({
    ref,
    className,
    columns,
    onLoad,
}: Props<Row>) {
    const [queryResult, setQueryResult] = useState<QueryResult<Row> | null>(null);
    const [isLoading, setIsLoading] = useState(false);

    const { addError, removeError } = useStatusProvider();

    const [handleLoad] = useState(() => async (pageIndex: number) => {
        setIsLoading(true);

        try {
            const newQueryResult = await onLoad(pageIndex);
            if (newQueryResult) {
                setQueryResult(newQueryResult);
            }
            removeError();
        } catch (err) {
            addError(err, "Error while loading grid data");
        } finally {
            setIsLoading(false);
        }
    });

    useImperativeHandle(ref, () => {
        return {
            async refresh() {
                await handleLoad(0);
            }
        }
    }, [handleLoad]);

    useEffect(() => {
        handleLoad(0);
    }, [handleLoad]);

    return (
        <Grid
            className={className}
            columns={columns.map(({ key, className, titleClassName, title }) => ({
                key,
                className: `${className ?? ""} ${titleClassName ?? ""}`,
                title
            }))}
            pageIndex={queryResult ? queryResult.pageIndex : undefined}
            totalRowCount={queryResult?.totalRowCount ?? 0}
            rowsPerPage={queryResult?.rowsPerPage ?? 0}
            isLoading={isLoading}
            onGoToPage={handleLoad}
        >
            {queryResult && (
                queryResult.rows.length > 0
                    ? queryResult.rows.map((row, index) => (
                        <tr key={index}>
                            {
                                columns
                                    .map(({ className, itemClassName, item }, columnIndex) => ({
                                        key: columns[columnIndex].key,
                                        className: `${className ?? ""} ${itemClassName ?? ""}`,
                                        item
                                    }))
                                    .map(({ key, className, item }) => (
                                        <td key={key} className={className}>
                                            {item?.(row, isLoading)}
                                        </td>
                                    ))
                            }
                        </tr>
                    ))
                    : (
                        <tr>
                            <td colSpan={columns.length}>
                                <em>{translate("No Data")}</em>
                            </td>
                        </tr>
                    )
            )}
            {!queryResult && (
                <tr>
                    <td colSpan={columns.length}>
                        {translate("Data is loading...")}
                    </td>
                </tr>
            )}
        </Grid>
    );
}