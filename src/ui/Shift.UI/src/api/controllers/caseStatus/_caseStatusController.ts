import { ApiDownloadFormat } from "@/api/models/ApiDownload";
import { fetchHelper } from "@/api/fetchHelper";
import { ApiSearchCaseStatuses } from "./ApiSearchCaseStatuses";
import { ApiCaseStatusMatch } from "./ApiCaseStatusMatch";
import { ApiCaseStatusModel } from "./ApiCaseStatusModel";
import { ApiCreateCaseStatus } from "./ApiCreateCaseStatus";
import { ApiUpdateCaseStatus } from "./ApiUpdateCaseStatus";

const _baseUrl = "/workflow/case-statuses";

export const _caseStatusController = {
    async search(query: ApiSearchCaseStatuses, pageIndex: number, sortByColumn: string | null) {
        return await fetchHelper.getPagedRows<ApiCaseStatusMatch>(`${_baseUrl}/search`, query, pageIndex, null, sortByColumn, null);
    },

    async download(query: ApiSearchCaseStatuses, format: ApiDownloadFormat, visibleColumns: string[]) {
        return await fetchHelper.download(`${_baseUrl}/download`, query, format, visibleColumns);
    },

    async retrieve(statusIdentifier: string) {
        return await fetchHelper.get<ApiCaseStatusModel>(`${_baseUrl}/${statusIdentifier}`);
    },

    async create(query: ApiCreateCaseStatus) {
        return await fetchHelper.post<ApiCaseStatusModel>(`${_baseUrl}`, query);
    },

    async update(statusId: string, query: ApiUpdateCaseStatus) {
        return await fetchHelper.put<ApiCaseStatusModel>(`${_baseUrl}/${statusId}`, query);
    },

    async delete(statusId: string) {
        return await fetchHelper.delete(`${_baseUrl}/${statusId}`, null);
    }
}