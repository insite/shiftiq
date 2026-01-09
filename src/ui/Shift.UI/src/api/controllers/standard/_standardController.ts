import { fetchHelper } from "@/api/fetchHelper";
import { ApiSearchStandards } from "./ApiSearchStandards";
import { ApiStandardMatch } from "./ApiStandardMatch";
import { ApiStandardModel } from "./ApiStandardModel";

const _baseUrl = "/competency/standards";

export const _standardController = {
    async search(query: ApiSearchStandards, pageIndex: number, pageSize: number) {
        return await fetchHelper.getPagedRows<ApiStandardMatch>(`${_baseUrl}/search`, query, pageIndex, pageSize, null, null);
    },

    async retrieve(standardId: string) {
        return await fetchHelper.get<ApiStandardModel>(`${_baseUrl}/${standardId}`);
    }
}