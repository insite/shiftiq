import { fetchHelper } from "@/api/fetchHelper";
import { ApiSearchViewModel } from "./ApiSearchViewModel";
import { QueryResult } from "@/models/QueryResult";

export const _searchController = {
    async search(searchText: string, pageIndex: number): Promise<QueryResult<ApiSearchViewModel> | null> {
        return await fetchHelper.getQueryResult<ApiSearchViewModel>(`/api/assets/searches`, [
            { value: searchText, name: "searchText" },
            { value: pageIndex + 1, name: "page" },
        ]);
    },

    async refresh(): Promise<void> {
        await fetchHelper.post(`/api/assets/searches`, {});
    },
}