import { ApiDownloadFormat } from "@/api/models/ApiDownload";
import { ApiCountResult } from "@/api/models/ApiCountResult";
import { fetchHelper } from "@/api/fetchHelper";
import { ApiSearchUsers } from "./ApiSearchUsers";
import { ApiUserMatch } from "./ApiUserMatch";
import { ApiUserModel } from "./ApiUserModel";

const _baseUrl = "/security/users";

export const _userController = {
    async count(query: ApiSearchUsers) {
        const result = await fetchHelper.get<ApiCountResult>(`${_baseUrl}/count`, fetchHelper.toParams(query));
        return result.Count;
    },

    async search(query: ApiSearchUsers, pageIndex: number, sortByColumn: string | null, pageSize: number) {
        return await fetchHelper.getPagedRows<ApiUserMatch>(`${_baseUrl}/search`, query, pageIndex, sortByColumn, pageSize);
    },

    async download(query: ApiSearchUsers, format: ApiDownloadFormat, visibleColumns: string[]) {
        return await fetchHelper.download(`${_baseUrl}/download`, query, format, visibleColumns);
    },

    async retrieve(userId: string) {
        return await fetchHelper.get<ApiUserModel>(`${_baseUrl}/${userId}`);
    }
}