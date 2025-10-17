export interface QueryResult<Row extends object> {
    pageIndex: number;
    rows: Row[];
    totalRowCount: number;
    rowsPerPage: number;
}

export function mapQueryResult<Row1 extends object, Row2 extends object>(
    queryResult: QueryResult<Row1> | null,
    mapper: (row: Row1) => Row2
): QueryResult<Row2> {
    if (!queryResult) {
        return {
            pageIndex: 0,
            rows: [],
            totalRowCount: 0,
            rowsPerPage: 0,
        };
    }

    return {
        pageIndex: queryResult.pageIndex,
        rows: queryResult.rows.map(row => mapper(row)),
        totalRowCount: queryResult.totalRowCount,
        rowsPerPage: queryResult.rowsPerPage,
    }
}