import { fetchHelper } from "@/api/fetchHelper";
import { ApiPersonMatch } from "./ApiPersonMatch";
import { ApiSearchPeople } from "./ApiSearchPeople";
import { ApiImportReport } from "./ApiImportReport";
import { QueryResult } from "@/models/QueryResult";

export const _peopleController = {
    async search(query: ApiSearchPeople, pageIndex: number, pageSize: number, visibleColumns: string[] | null): Promise<QueryResult<ApiPersonMatch> | null> {
        return await fetchHelper.getPagedRows<ApiPersonMatch>(
            "/api/contacts/people/search",
            query,
            pageIndex,
            pageSize,
            null,
            visibleColumns
        );
    },

    async searchImportReport(pageIndex: number): Promise<QueryResult<ApiImportReport> | null> {
        return await fetchHelper.getPagedRows<ApiImportReport>(
            "/api/contacts/people/search-import-report",
            {},
            pageIndex,
            null,
            null,
            null
        );
    },
}
