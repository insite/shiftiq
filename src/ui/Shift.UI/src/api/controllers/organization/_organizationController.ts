import { ApiDownloadFormat } from "@/api/models/ApiDownload";
import { ApiCountResult } from "@/api/models/ApiCountResult";
import { fetchHelper } from "@/api/fetchHelper";
import { ApiSearchOrganizations } from "./ApiSearchOrganizations";
import { ApiOrganizationMatch } from "./ApiOrganizationMatch";
import { ApiOrganizationModel } from "./ApiOrganizationModel";

const _baseUrl = "/security/organizations";

export const _organizationController = {
    async count(query: ApiSearchOrganizations) {
        const result = await fetchHelper.get<ApiCountResult>(`${_baseUrl}/count`, fetchHelper.toParams(query));
        return result.Count;
    },

    async search(query: ApiSearchOrganizations, pageIndex: number, sortByColumn: string | null, pageSize: number) {
        return await fetchHelper.getPagedRows<ApiOrganizationMatch>(`${_baseUrl}/search`, query, pageIndex, sortByColumn, pageSize);
    },

    async download(query: ApiSearchOrganizations, format: ApiDownloadFormat, visibleColumns: string[]) {
        return await fetchHelper.download(`${_baseUrl}/download`, query, format, visibleColumns);
    },

    async retrieve(organizationId: string) {
        return await fetchHelper.get<ApiOrganizationModel>(`${_baseUrl}/${organizationId}`);
    }
}