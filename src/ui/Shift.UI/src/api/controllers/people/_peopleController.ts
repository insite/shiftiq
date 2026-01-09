import { fetchHelper } from "@/api/fetchHelper";
import { ApiPersonMatch } from "./ApiPersonMatch";
import { ApiSearchPeople } from "./ApiSearchPeople";

const _baseUrl = "/directory/people";

export const _peopleController = {
    async search(query: ApiSearchPeople, pageIndex: number, pageSize: number, visibleColumns: string[] | null) {
        return await fetchHelper.getPagedRows<ApiPersonMatch>(
            `${_baseUrl}/search`,
            query,
            pageIndex,
            pageSize,
            null,
            visibleColumns
        );
    },
}