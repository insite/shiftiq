import { ListItem } from "@/models/listItem";
import { QueryResult } from "@/models/QueryResult";

export interface FinderSearchWindowData {
    pageIndex: number;
    items: ListItem[];
    totalItemCount: number;
    itemsPerPage: number; 
}

export function toFinderSearchWindowData<Row extends object>(queryResult: QueryResult<Row> | null, mapper: (row: Row) => ListItem): FinderSearchWindowData {
    if (!queryResult) {
        return {
            pageIndex: 0,
            items: [],
            totalItemCount: 0,
            itemsPerPage: 0,
        };
    }

    return {
        pageIndex: queryResult.pageIndex,
        items: queryResult.rows.map(row => mapper(row)),
        totalItemCount: queryResult.totalRowCount,
        itemsPerPage: queryResult.rowsPerPage,
    }
}