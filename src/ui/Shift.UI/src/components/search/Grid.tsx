import { ReactNode } from "react";
import LoadingOverlay from "../LoadingOverlay";
import Pagination from "../Pagination";
import { shiftConfig } from "@/helpers/shiftConfig";

const _defaultPagesPerView = shiftConfig.pagesPerView;

export interface GridColumn {
    key: string;
    className?: string;
    title?: ReactNode;
}

interface Props {
    className?: string;
    columns: GridColumn[];
    children?: ReactNode;
    pageIndex?: number;
    totalRowCount: number;
    rowsPerPage: number;
    isLoading: boolean;
    onGoToPage: (pageIndex: number) => void;
}

export default function Grid({
    className,
    columns,
    children,
    pageIndex,
    totalRowCount,
    rowsPerPage,
    isLoading,
    onGoToPage,
}: Props) {
    return (
        <table className={"table table-striped " + (className ?? "")}>
            <thead>
                <tr>
                    {columns.map(({ key, className, title }) => (
                        <th key={key} className={className}>{title}</th>
                    ))}
                </tr>
            </thead>
            <tbody>
                {children}
            </tbody>
            {pageIndex !== undefined ? (
                <tfoot>
                    <tr>
                        <td colSpan={columns.length}>
                            <LoadingOverlay isLoading={isLoading}>
                                <Pagination
                                    pageIndex={pageIndex}
                                    rowsPerPage={rowsPerPage}
                                    pagesPerView={_defaultPagesPerView}
                                    totalRowCount={totalRowCount}
                                    onGoToPage={onGoToPage}
                                />
                            </LoadingOverlay>
                        </td>
                    </tr>
                </tfoot>
            ) : null}
        </table>
    );
}