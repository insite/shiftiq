import { useState } from "react";
import { ListItem } from "@/models/listItem";
import Pagination from "@/components/Pagination";

const _defaultPagesPerView = 7;

let _groupCounter = 1;

interface Props {
    isLoading: boolean;
    value: string | null;
    items: ListItem[];
    columnHeaderTitle: string;
    pageIndex: number;
    totalItemCount: number;
    itemsPerPage: number;
    onGoToPage: (pageIndex: number) => void;
    onSelect: (item: ListItem) => void;
}

export default function FinderSearchGrid({
    isLoading,
    value,
    items,
    columnHeaderTitle,
    pageIndex,
    totalItemCount,
    itemsPerPage,
    onGoToPage,
    onSelect,
}: Props) {
    const [groupName] = useState(() => "search_group_" + (_groupCounter++));
    
    function handleOptionKeyDown(key: string, item: ListItem) {
        if (!isLoading && key === "Enter") {
            onSelect(item);
        }
    }

    return (
        <table className="table table-hover table-sm mt-3 fe-table">
            <thead>
                <tr>
                    <th className="fe-input"></th>
                    <th>{columnHeaderTitle}</th>
                </tr>
            </thead>
            <tbody>
                {items.length === 0 && (
                    <tr>
                        <td></td>
                        <td>
                            No Items
                        </td>
                    </tr>
                )}
                {items.length > 0 && items.map(item => (
                    <tr key={item.value} onMouseUp={() => {
                        if (!isLoading) {
                            onSelect(item);
                        }
                    }}>
                        <td>
                            <input
                                type="radio"
                                name={groupName}
                                value={item.value}
                                disabled={isLoading}
                                defaultChecked={item.value === value}
                                onKeyDown={e => handleOptionKeyDown(e.key, item)}
                            />
                        </td>
                        <td>
                            {item.text}
                        </td>
                    </tr>
                ))}
            </tbody>
            {totalItemCount > itemsPerPage ? (
                <tfoot>
                    <tr>
                        <td colSpan={2}>
                            <Pagination
                                pageIndex={pageIndex}
                                rowsPerPage={itemsPerPage}
                                pagesPerView={_defaultPagesPerView}
                                totalRowCount={totalItemCount}
                                onGoToPage={onGoToPage}
                                hideDetails={true}
                            />
                        </td>
                    </tr>
                </tfoot>
            ) : null}
        </table>
    );
}