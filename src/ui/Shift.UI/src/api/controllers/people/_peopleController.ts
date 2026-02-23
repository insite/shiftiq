import { fetchHelper } from "@/api/fetchHelper";
import { ApiPersonMatch } from "./ApiPersonMatch";
import { ApiSearchPeople } from "./ApiSearchPeople";

export const _peopleController = {
    async search(query: ApiSearchPeople, pageIndex: number, pageSize: number, visibleColumns: string[] | null) {
        return await fetchHelper.getPagedRows<ApiPersonMatch>(
            "/api/directory/people/search",
            query,
            pageIndex,
            pageSize,
            null,
            visibleColumns
        );
    },
}