import { ApiDownloadFormat } from "@/api/models/ApiDownload";
import { fetchHelper } from "@/api/fetchHelper";
import { ApiSearchCaseStatuses } from "./ApiSearchCaseStatuses";
import { ApiCaseStatusMatch } from "./ApiCaseStatusMatch";
import { ApiCaseStatusModel } from "./ApiCaseStatusModel";
import { ApiCreateCaseStatus } from "./ApiCreateCaseStatus";
import { ApiUpdateCaseStatus } from "./ApiUpdateCaseStatus";

export const _caseStatusController = {
    async search(query: ApiSearchCaseStatuses, pageIndex: number, sortByColumn: string | null) {
        return await fetchHelper.getPagedRows<ApiCaseStatusMatch>(`/api/workflows/cases-statuses/search`, query, pageIndex, null, sortByColumn, null);
    },

    async download(query: ApiSearchCaseStatuses, format: ApiDownloadFormat, visibleColumns: string[]) {
        return await fetchHelper.download(`/api/workflows/cases-statuses/download`, query, format, visibleColumns);
    },

    async retrieve(statusId: string) {
        return await fetchHelper.get<ApiCaseStatusModel>(`/api/workflows/cases-statuses/${statusId}`);
    },

    async create(query: ApiCreateCaseStatus) {
        return await fetchHelper.post<ApiCaseStatusModel>(`/api/workflows/cases-statuses`, query);
    },

    async update(statusId: string, query: ApiUpdateCaseStatus) {
        return await fetchHelper.put<ApiCaseStatusModel>(`/api/workflows/cases-statuses/${statusId}`, query);
    },

    async delete(statusId: string) {
        return await fetchHelper.delete(`/api/workflows/cases-statuses/${statusId}`, null);
    }
}
