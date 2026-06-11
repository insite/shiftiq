import { ApiCountResult } from "@/api/models/ApiCountResult";
import { fetchHelper } from "@/api/fetchHelper";
import { ApiSearchOrganizations } from "./ApiSearchOrganizations";
import { ApiOrganizationMatch } from "./ApiOrganizationMatch";
import { ApiOrganizationModel } from "./ApiOrganizationModel";

export const _organizationController = {
    async count(query: ApiSearchOrganizations) {
        const result = await fetchHelper.get<ApiCountResult>(`/api/accounts/organizations/count`, fetchHelper.toParams(query));
        return result.Count;
    },

    async search(query: ApiSearchOrganizations, pageIndex: number, pageSize: number, sortByColumn: string | null) {
        return await fetchHelper.getPagedRows<ApiOrganizationMatch>(`/api/accounts/organizations/search`, query, pageIndex, pageSize, sortByColumn, null);
    },

    async retrieve(organizationId: string) {
        return await fetchHelper.get<ApiOrganizationModel>(`/api/accounts/organizations/${organizationId}`);
    }
}
