import { fetchHelper } from "@/api/fetchHelper";
import { ApiSearchUsers } from "./ApiSearchUsers";
import { ApiUserMatch } from "./ApiUserMatch";
import { ApiUserModel } from "./ApiUserModel";

const _baseUrl = "/security/users";

export const _userController = {
    async search(query: ApiSearchUsers, pageIndex: number, pageSize: number, sortByColumn: string | null) {
        return await fetchHelper.getPagedRows<ApiUserMatch>(`${_baseUrl}/search`, query, pageIndex, pageSize, sortByColumn, null);
    },

    async retrieve(userId: string): Promise<ApiUserModel | null> {
        return await fetchHelper.get<ApiUserModel>(`${_baseUrl}/${userId}`, null, true);
    }
}