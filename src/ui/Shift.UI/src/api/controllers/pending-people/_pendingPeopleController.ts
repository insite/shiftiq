import { QueryResult } from "@/models/QueryResult";
import { ApiPendingPersonImportModel } from "./ApiPendingPersonImportModel";
import { fetchHelper } from "@/api/fetchHelper";
import { ApiImportPendingPerson } from "./ApiImportPendingPerson";
import { ApiImportResult } from "./ApiImportResult";

export const _pendingPeopleController = {
    async search(pageIndex: number): Promise<QueryResult<ApiPendingPersonImportModel> | null> {
        return await fetchHelper.getPagedRows<ApiPendingPersonImportModel>(
            "/api/contacts/pending-people/search-import",
            {},
            pageIndex,
            null,
            null,
            null
        );
    },

    async import(imports: ApiImportPendingPerson[]): Promise<ApiImportResult | null> {
        return await fetchHelper.post("/api/contacts/pending-people/import", imports);
    },
}